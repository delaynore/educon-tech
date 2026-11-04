"use client";

import { Suspense } from "react";
import Link from "next/link";
import { BookOpen, LayoutGrid, PanelLeftClose, PanelLeftOpen, X } from "lucide-react";
import { useSelectedLayoutSegment } from "next/navigation";
import { Brand } from "@/shared/components/brand";
import { ProfileAvatar } from "@/shared/components/profile-avatar";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from "@/shared/components/ui/sidebar";
import { routes } from "@/shared/routes";

const navigation = [
  { segment: null, label: "Главная", href: routes.home, icon: LayoutGrid },
  {
    segment: "courses",
    label: "Мои курсы",
    href: routes.courses,
    icon: BookOpen,
  },
];

function SidebarNavigation() {
  const activeSegment = useSelectedLayoutSegment();
  const { setOpenMobile } = useSidebar();
  return (
    <SidebarMenu className="gap-1">
      {navigation.map(({ segment, label, href, icon: Icon }) => (
        <SidebarMenuItem key={href}>
          <SidebarMenuButton
            render={<Link href={href} />}
            tooltip={label}
            aria-label={label}
            isActive={activeSegment === segment}
            aria-current={activeSegment === segment ? "page" : undefined}
            onClick={() => setOpenMobile(false)}
            className="h-11 gap-3 px-3 text-sidebar-foreground/70 data-active:text-sidebar-accent-foreground md:[html[data-sidebar-open=false]_&]:size-8 md:[html[data-sidebar-open=false]_&]:gap-0 md:[html[data-sidebar-open=false]_&]:p-2"
          >
            <Icon />
            <span className="md:[html[data-sidebar-open=false]_&]:hidden">
              {label}
            </span>
          </SidebarMenuButton>
        </SidebarMenuItem>
      ))}
    </SidebarMenu>
  );
}

export function AppSidebar() {
  const { open, openMobile, isMobile, toggleSidebar, setOpenMobile } = useSidebar();
  return (
    <Sidebar collapsible="icon" className="border-sidebar-border">
      <SidebarHeader className="overflow-hidden p-5 pb-0 transition-[padding] duration-300 ease-out motion-reduce:transition-none md:[html[data-sidebar-open=false]_&]:px-3">
        <div
          onClick={() => setOpenMobile(false)}
          className="w-fit md:[html[data-sidebar-open=false]_&]:[&_a>span:last-child]:hidden md:[html[data-sidebar-open=false]_&]:[&_a>span:first-child]:mr-0"
        >
          <Brand />
        </div>
      </SidebarHeader>
      <SidebarContent className="px-5 transition-[padding] duration-300 ease-out motion-reduce:transition-none md:[html[data-sidebar-open=false]_&]:px-3.5">
        <SidebarGroup className="mt-5 p-0">
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton
                onClick={toggleSidebar}
                aria-expanded={isMobile ? openMobile : open}
                aria-label={isMobile ? "Закрыть меню" : open ? "Свернуть меню" : "Развернуть меню"}
                tooltip="Развернуть меню"
                className="h-11 gap-3 px-3 text-sidebar-foreground/70 md:[html[data-sidebar-open=false]_&]:size-8 md:[html[data-sidebar-open=false]_&]:gap-0 md:[html[data-sidebar-open=false]_&]:p-2"
              >
                <X className="md:hidden" />
                <PanelLeftClose className="hidden md:block [html[data-sidebar-open=false]_&]:hidden" />
                <PanelLeftOpen className="hidden md:[html[data-sidebar-open=false]_&]:block" />
                <span className="md:hidden">Закрыть меню</span>
                <span className="hidden md:block [html[data-sidebar-open=false]_&]:hidden">Свернуть</span>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroup>
        <SidebarGroup className="mt-3 p-0">
          <SidebarGroupLabel className="mb-3 px-3 text-[10px] font-bold tracking-[0.2em] whitespace-nowrap text-muted-foreground md:[html[data-sidebar-open=false]_&]:invisible">
            ЛИЧНОЕ ПРОСТРАНСТВО
          </SidebarGroupLabel>
          <nav aria-label="Основная навигация">
            <Suspense
              fallback={
                <div className="p-3 text-muted-foreground">Навигация…</div>
              }
            >
              <SidebarNavigation />
            </Suspense>
          </nav>
        </SidebarGroup>
      </SidebarContent>
      <SidebarFooter className="overflow-hidden p-5 transition-[padding] duration-300 ease-out motion-reduce:transition-none md:[html[data-sidebar-open=false]_&]:px-3">
        <div className="flex items-center gap-3 border-t border-sidebar-border pt-5">
          <ProfileAvatar />
          <div className="whitespace-nowrap md:[html[data-sidebar-open=false]_&]:hidden">
            <strong className="text-sm font-medium">Даниил</strong>
            <small className="block text-xs text-muted-foreground">
              Личный аккаунт
            </small>
          </div>
        </div>
      </SidebarFooter>
    </Sidebar>
  );
}
