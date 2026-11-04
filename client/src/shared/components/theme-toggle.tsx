"use client";

import { useEffect } from "react";
import { Moon, Sun } from "lucide-react";
import { Toggle } from "@/shared/components/ui/toggle";
import { useUIBoolean } from "@/shared/hooks/use-ui-preference";
import { themePreferenceKey } from "@/shared/lib/theme-preference";
import { readUIPreference, subscribeUIPreferences } from "@/shared/lib/ui-preference-store";

export function ThemeToggle() {
  const [dark, setDark] = useUIBoolean(themePreferenceKey, true);

  useEffect(() => {
    function syncTheme() {
      document.documentElement.classList.toggle("dark", readUIPreference(themePreferenceKey) !== "false");
    }
    syncTheme();
    return subscribeUIPreferences(syncTheme);
  }, []);

  const label = dark ? "Включить светлую тему" : "Включить тёмную тему";

  return (
    <Toggle pressed={dark} onPressedChange={setDark} aria-label={label} title={label} className="size-8 p-0 data-pressed:bg-transparent">
      <Sun className="hidden dark:block" />
      <Moon className="dark:hidden" />
    </Toggle>
  );
}
