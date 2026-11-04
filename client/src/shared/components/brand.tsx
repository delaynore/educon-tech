import Link from "next/link";
import { GraduationCap } from "lucide-react";
export function Brand({ compact = false }: { compact?: boolean }) {
  return <Link href="/" className="flex items-center text-[29px] leading-none font-extrabold tracking-[-1.5px]">
    <span className="mr-2 grid size-9 -rotate-7 place-items-center rounded-xl bg-primary text-primary-foreground"><GraduationCap size={23} /></span>
    <span className={compact ? "hidden md:inline" : ""}>educon<span className="text-primary">.</span></span>
  </Link>;
}
