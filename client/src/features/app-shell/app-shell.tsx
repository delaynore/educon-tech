"use client";

import { useEffect, type ReactNode } from "react";
import Header from "@/features/header/header";
import { SidebarInset, SidebarProvider } from "@/shared/components/ui/sidebar";
import { AppSidebar } from "./components/app-sidebar";
import { useUIBoolean } from "@/shared/hooks/use-ui-preference";
import { sidebarPreferenceKey } from "@/shared/lib/sidebar-preference";
import { readUIPreference, subscribeUIPreferences } from "@/shared/lib/ui-preference-store";
import { CourseSearch } from "@/features/learning/components/course-search";
import { demoCourses } from "@/entities/courses/demo-data";

export function AppShell({ children }: { children: ReactNode }) {
  const [sidebarOpen, setSidebarOpen] = useUIBoolean(sidebarPreferenceKey, true);

  useEffect(() => {
    function syncSidebarAppearance() {
      // Read the actual preference, never the server's temporary hydration fallback.
      document.documentElement.dataset.sidebarOpen = readUIPreference(sidebarPreferenceKey) === "false" ? "false" : "true";
    }
    syncSidebarAppearance();
    return subscribeUIPreferences(syncSidebarAppearance);
  }, []);

  return <div className="min-h-dvh bg-muted/30 font-sans text-sm text-foreground">
    <a href="#page-content" className="sr-only z-50 rounded-md bg-background p-3 focus:not-sr-only focus:fixed focus:top-2 focus:left-2">Перейти к содержимому</a>
    <SidebarProvider open={sidebarOpen} onOpenChange={setSidebarOpen} style={{ "--sidebar-width": "15rem", "--sidebar-width-icon": "3.75rem" } as React.CSSProperties}>
      <AppSidebar />
      <SidebarInset className="min-w-0 bg-transparent">
        <Header search={<CourseSearch courses={demoCourses} />} />
        <div id="page-content" tabIndex={-1} className="mx-auto w-full max-w-7xl px-4 py-6 outline-none sm:px-6 lg:px-8 lg:py-8">{children}</div>
      </SidebarInset>
    </SidebarProvider>
  </div>;
}
