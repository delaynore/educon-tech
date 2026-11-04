import assert from "node:assert/strict";
import { afterEach, beforeEach, test } from "node:test";
import { readUIPreference, subscribeUIPreferences, writeUIPreference } from "./ui-preference-store.ts";

beforeEach(() => {
  const values = new Map();
  globalThis.window = Object.assign(new EventTarget(), {
    localStorage: {
      getItem: (key) => values.get(key) ?? null,
      setItem: (key, value) => values.set(key, value),
    },
  });
});

afterEach(() => { delete globalThis.window; });

test("closed sidebar survives a fresh store instance, as on reload", async () => {
  writeUIPreference("educon:sidebar-open", "false");
  const reloaded = await import("./ui-preference-store.ts?reload");
  assert.equal(reloaded.readUIPreference("educon:sidebar-open"), "false");
});

test("module expansion is kept separately for each course, including all closed", () => {
  const design = "educon:course:ui-ux:expanded-modules";
  const frontend = "educon:course:frontend:expanded-modules";
  writeUIPreference(design, JSON.stringify(["intro", "visual"]));
  writeUIPreference(frontend, JSON.stringify(["intro"]));
  assert.deepEqual(JSON.parse(readUIPreference(design)), ["intro", "visual"]);
  writeUIPreference(frontend, "[]");
  assert.deepEqual(JSON.parse(readUIPreference(frontend)), []);
  assert.deepEqual(JSON.parse(readUIPreference(design)), ["intro", "visual"]);
});

test("same-page writes and other-tab storage events notify subscribers", () => {
  let changes = 0;
  const unsubscribe = subscribeUIPreferences(() => { changes++; });
  writeUIPreference("notifications", "true");
  window.dispatchEvent(new Event("storage"));
  assert.equal(changes, 2);
  unsubscribe();
  writeUIPreference("notifications", "false");
  window.dispatchEvent(new Event("storage"));
  assert.equal(changes, 2);
});

test("controls still retain state when storage access is blocked", () => {
  window.localStorage.getItem = () => { throw new Error("Storage blocked"); };
  window.localStorage.setItem = () => { throw new Error("Storage blocked"); };
  assert.equal(readUIPreference("blocked-storage"), null);
  writeUIPreference("blocked-storage", "false");
  assert.equal(readUIPreference("blocked-storage"), "false");
});

test("server rendering does not access browser storage", () => {
  delete globalThis.window;
  assert.equal(readUIPreference("educon:sidebar-open"), null);
  assert.doesNotThrow(() => writeUIPreference("educon:sidebar-open", "false"));
});

test("writing an unchanged preference does not notify or touch storage", () => {
  writeUIPreference("unchanged-preference", "true");
  let notifications = 0;
  let writes = 0;
  const setItem = window.localStorage.setItem;
  window.localStorage.setItem = (...args) => { writes++; setItem(...args); };
  const unsubscribe = subscribeUIPreferences(() => { notifications++; });
  writeUIPreference("unchanged-preference", "true");
  assert.equal(writes, 0);
  assert.equal(notifications, 0);
  writeUIPreference("unchanged-preference", "false");
  assert.equal(writes, 1);
  assert.equal(notifications, 1);
  unsubscribe();
});
