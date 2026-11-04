"use client";

import { Avatar, AvatarFallback } from "@/shared/components/ui/avatar";
import { cn } from "@/shared/lib/utils";
export function ProfileAvatar({ initials = "АК", className }: { initials?: string; className?: string }) {
  return <Avatar className={cn("relative flex size-9 shrink-0 overflow-hidden rounded-full bg-secondary text-xs font-semibold text-secondary-foreground", className)}>
    <AvatarFallback className="bg-inherit text-inherit text-[length:inherit]">{initials}</AvatarFallback>
  </Avatar>;
}
