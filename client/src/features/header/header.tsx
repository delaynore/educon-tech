"use client";

import { ProfileAvatar } from "@/shared/components/profile-avatar";
import { SidebarTrigger, useSidebar } from "@/shared/components/ui/sidebar";
import type { ReactNode } from "react";
import { ThemeToggle } from "@/shared/components/theme-toggle";

type HeaderProps = {
  search: ReactNode;
};

export default function Header({ search }: HeaderProps) {
  const { openMobile } = useSidebar();
  return <header className="sticky top-0 z-20 grid h-20 grid-cols-[auto_minmax(0,1fr)_auto] md:grid-cols-[minmax(0,1fr)_auto] xl:grid-cols-[auto_minmax(0,1fr)_auto] items-center gap-4 bg-background/90 px-4 backdrop-blur-sm sm:gap-8 sm:px-6 lg:px-8">
    <div className="flex min-w-0 items-center gap-3 md:hidden xl:flex">
      <SidebarTrigger size="icon" aria-expanded={openMobile} aria-label="Открыть меню" className="md:hidden aria-expanded:bg-transparent aria-expanded:hover:bg-muted" />
      <span className="hidden truncate text-sm font-medium xl:inline">Моё обучение</span>
    </div>
    <div className="mx-auto w-full min-w-0 max-w-3xl">
      {search}
    </div>
    <div className="flex items-center gap-2 justify-self-end">
      <ThemeToggle />
      <ProfileAvatar className="size-8 text-[10px]" />
    </div>
  </header>;
}
