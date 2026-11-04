const changeEvent = "educon:ui-preferences-change";
const memoryFallback = new Map<string, string>();

export function readUIPreference(key: string): string | null {
  if (typeof window === "undefined") return null;
  if (memoryFallback.has(key)) return memoryFallback.get(key) ?? null;
  try {
    return window.localStorage.getItem(key);
  } catch {
    return null;
  }
}

export function writeUIPreference(key: string, value: string) {
  if (typeof window === "undefined" || readUIPreference(key) === value) return;
  try {
    window.localStorage.setItem(key, value);
    memoryFallback.delete(key);
  } catch {
    // Keep controls working for this session when browser storage is unavailable.
    memoryFallback.set(key, value);
  }
  window.dispatchEvent(new Event(changeEvent));
}

export function subscribeUIPreferences(onChange: () => void) {
  window.addEventListener("storage", onChange);
  window.addEventListener(changeEvent, onChange);
  return () => {
    window.removeEventListener("storage", onChange);
    window.removeEventListener(changeEvent, onChange);
  };
}
