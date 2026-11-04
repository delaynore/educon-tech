import assert from "node:assert/strict";
import { test } from "node:test";
import { runInNewContext } from "node:vm";
import { sidebarInitScript, sidebarPreferenceKey } from "./sidebar-preference.ts";

for (const [stored, expected] of [["false", "false"], ["true", "true"], [null, "true"], ["invalid", "true"]]) {
  test(`initial HTML uses ${expected} before React starts when storage contains ${stored}`, () => {
    const document = { documentElement: { dataset: { sidebarOpen: "true" } } };
    runInNewContext(sidebarInitScript, {
      document,
      localStorage: { getItem: (key) => { assert.equal(key, sidebarPreferenceKey); return stored; } },
    });
    assert.equal(document.documentElement.dataset.sidebarOpen, expected);
  });
}

test("blocked storage leaves the default layout usable", () => {
  const document = { documentElement: { dataset: { sidebarOpen: "true" } } };
  assert.doesNotThrow(() => runInNewContext(sidebarInitScript, {
    document,
    localStorage: { getItem: () => { throw new Error("Storage blocked"); } },
  }));
  assert.equal(document.documentElement.dataset.sidebarOpen, "true");
});
