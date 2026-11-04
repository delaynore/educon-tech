import type { ComponentProps } from "react";
import { cn } from "@/shared/lib/utils";

export function Eyebrow({ className, ...props }: ComponentProps<"span">) {
  return <span className={cn("text-[9px] font-semibold tracking-[0.16em] text-muted-foreground", className)} {...props} />;
}
