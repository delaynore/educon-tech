import { ArrowRight, Clock3 } from "lucide-react";
import { ButtonLink } from "@/shared/components/ui/button-link";
import { routes } from "@/shared/routes";
import { getContinueLesson } from "@/entities/courses/demo-data";
import type { DemoCourse } from "@/entities/courses/types";
export function WelcomeBanner({ course }: { course: DemoCourse }) {
  return <section className="relative isolate min-h-65 overflow-hidden rounded-xl border border-border bg-muted">
    <div className="relative z-10 p-6 sm:p-8">
      <span className="flex items-center gap-2 text-[7px] tracking-widest text-muted-foreground sm:text-[8px]"><span className="size-1 rounded-full bg-primary" />ПРОДОЛЖИМ С ТОГО ЖЕ МЕСТА</span>
      <h2 className="mt-4 mb-3 text-[28px] leading-[1.18] font-semibold tracking-tight text-foreground sm:text-[32px]">Большие идеи<br />начинаются с основ.</h2>
      <p className="text-[11px] text-muted-foreground">{course.title} · Продолжить курс</p>
      <div className="mt-5 flex flex-wrap items-center gap-3"><ButtonLink href={routes.lesson(course.id, getContinueLesson(course).id)} className="h-9 gap-6 text-[11px]">Продолжить обучение <ArrowRight /></ButtonLink><span className="flex items-center gap-1 text-[9px] text-muted-foreground"><Clock3 size={12} /> Следующий урок — {getContinueLesson(course).duration} минут</span></div>
    </div>
    <div aria-hidden="true" className="absolute top-0 right-0 h-full w-[44%] opacity-20 sm:opacity-80">
      <div className="absolute top-0 left-[10%] size-72 rounded-full border border-border" />
      <div className="absolute top-8 left-[52%] size-25 rounded-full border-25 border-border shadow-lg" />
      <div className="absolute top-12 left-[18%] h-42 w-39 -rotate-13 rounded-2xl bg-linear-140 from-primary/60 to-primary text-center font-serif text-[135px] leading-none text-primary-foreground shadow-2xl">a<span className="absolute -right-13 -bottom-4 rotate-15 font-sans text-[113px] text-muted-foreground">✳</span></div>
      <div className="absolute bottom-8 left-0 hidden -rotate-7 rounded-lg border border-border bg-background/80 px-4 py-3 text-[10px] text-muted-foreground shadow-lg backdrop-blur-sm lg:block">✦ Create something great</div>
    </div>
  </section>;
}
