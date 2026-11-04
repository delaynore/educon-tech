"use client";

import { useCallback, useEffect, useSyncExternalStore, type SetStateAction } from "react";
import { readUIPreference, writeUIPreference, subscribeUIPreferences } from "@/shared/lib/ui-preference-store";

function serverSnapshot() { return null; }
function isBoolean(value: unknown): value is boolean { return typeof value === "boolean"; }
function isStringList(value: unknown): value is string[] {
  return Array.isArray(value) && value.every((item) => typeof item === "string");
}

function decode<T>(raw: string | null, fallback: T, validate: (value: unknown) => value is T): T {
  if (raw === null) return fallback;
  try {
    const value: unknown = JSON.parse(raw);
    return validate(value) ? value : fallback;
  } catch {
    return fallback;
  }
}

function useUIPreference<T>(key: string, fallback: T, validate: (value: unknown) => value is T) {
  const fallbackSnapshot = JSON.stringify(fallback);
  const getSnapshot = useCallback(() => readUIPreference(key), [key]);
  const raw = useSyncExternalStore(subscribeUIPreferences, getSnapshot, serverSnapshot);
  const value = decode(raw, fallback, validate);

  useEffect(() => {
    // Remember the initially open module before the first lesson navigation.
    if (readUIPreference(key) === null) writeUIPreference(key, fallbackSnapshot);
  }, [key, fallbackSnapshot]);

  const setValue = useCallback((update: SetStateAction<T>) => {
    const previous = decode(readUIPreference(key), JSON.parse(fallbackSnapshot) as T, validate);
    const next = typeof update === "function" ? (update as (value: T) => T)(previous) : update;
    writeUIPreference(key, JSON.stringify(next));
  }, [key, fallbackSnapshot, validate]);

  return [value, setValue] as const;
}

export function useUIBoolean(key: string, fallback: boolean) {
  return useUIPreference(key, fallback, isBoolean);
}

export function useUIStringList(key: string, fallback: string[]) {
  return useUIPreference(key, fallback, isStringList);
}
