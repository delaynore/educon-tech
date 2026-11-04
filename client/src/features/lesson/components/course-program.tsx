"use client";

import { Accordion, AccordionItem, AccordionTrigger, AccordionContent } from "@/shared/components/ui/accordion";
import { Collapsible, CollapsibleTrigger, CollapsibleContent } from "@/shared/components/ui/collapsible";
import { AudioLines, Check, ChevronDown, ListVideo } from "lucide-react";
import type { DemoCourse } from "@/entities/courses/types";
import { getCourseLessons } from "@/entities/courses/demo-data";
import { Button } from "@/shared/components/ui/button";
import { ButtonLink } from "@/shared/components/ui/button-link";
import { Progress } from "@/shared/components/ui/progress";
import { useUIBoolean, useUIStringList } from "@/shared/hooks/use-ui-preference";
import { cn } from "@/shared/lib/utils";
import { routes } from "@/shared/routes";

export function CourseProgram({ course, lessonId }: { course: DemoCourse; lessonId: string }) {
  const currentModule = course.modules.find((item) => item.lessons.some((lesson) => lesson.id === lessonId));
  const [expanded, setExpanded] = useUIStringList(
    `educon:course:${course.id}:expanded-modules`,
    currentModule ? [currentModule.id] : [],
  );
  const [mobileOpen, setMobileOpen] = useUIBoolean(`educon:course:${course.id}:program-open`, false);
  const lessons = getCourseLessons(course);
  const completed = lessons.filter((lesson) => lesson.completed).length;

  return (
    <Collapsible open={mobileOpen} onOpenChange={setMobileOpen} render={<aside />} aria-label="Программа курса" className="overflow-hidden rounded-xl border bg-card text-card-foreground xl:sticky xl:top-22">
      <CollapsibleTrigger
        render={<Button variant="ghost" />}
        className="h-auto w-full justify-between rounded-none px-5 py-4 text-sm xl:hidden"
      >
        <span className="flex items-center gap-2"><ListVideo />Программа курса</span>
        <ChevronDown className={cn("transition-transform", mobileOpen && "rotate-180")} />
      </CollapsibleTrigger>
      <CollapsibleContent keepMounted hidden={false} className="max-h-[70dvh] overflow-y-auto data-closed:hidden xl:max-h-[calc(100dvh-7rem)] xl:data-closed:block">
        <div className="border-b p-5">
          <span className="text-xs text-muted-foreground">Программа курса</span>
          <h2 className="mt-2 mb-5 text-lg leading-tight font-semibold tracking-tight">{course.title}</h2>
          <div className="mb-2 flex justify-between text-xs text-muted-foreground">
            <span>{completed} из {lessons.length} уроков</span>
            <span>{Math.round(completed / lessons.length * 100)}%</span>
          </div>
          <Progress value={completed} max={lessons.length} aria-label="Прогресс курса" />
        </div>
        <Accordion multiple value={expanded} onValueChange={setExpanded} className="divide-y">
          {course.modules.map((courseModule, moduleIndex) => {
            const activeModule = courseModule.id === currentModule?.id;
            return (
              <AccordionItem key={courseModule.id} value={courseModule.id}>
                  <AccordionTrigger
                    className={cn("h-auto w-full justify-start gap-3 rounded-none border-0 px-4 py-4 text-left whitespace-normal hover:bg-muted/60 aria-expanded:bg-muted/40", activeModule && "text-foreground")}
                  >
                    <span className="min-w-0 flex-1">
                      <span className="mb-1 block text-xs font-normal text-muted-foreground">Модуль {moduleIndex + 1}</span>
                      <span className="block text-sm leading-5 font-medium">{courseModule.title}</span>
                    </span>
                    <span className="text-xs font-normal text-muted-foreground">{courseModule.lessons.length}</span>
                  </AccordionTrigger>
                <AccordionContent className="pb-0 [&_a]:no-underline">
                  {courseModule.lessons.map((lesson, index) => {
                    const active = lesson.id === lessonId;
                    return (
                      <ButtonLink
                        key={lesson.id}
                        href={routes.lesson(course.id, lesson.id)}
                        variant="ghost"
                        aria-current={active ? "page" : undefined}
                        className={cn("h-auto w-full justify-start gap-3 rounded-none border-0 border-l-2 border-transparent py-3 pr-4 pl-7 text-left whitespace-normal text-muted-foreground hover:bg-muted/60", active && "border-l-primary bg-accent text-accent-foreground")}
                      >
                        <span className="grid w-4 shrink-0 place-items-center text-xs">
                          {active ? <AudioLines size={16} /> : lesson.completed ? <Check size={15} /> : String(index + 1).padStart(2, "0")}
                        </span>
                        <span className="min-w-0 flex-1">
                          <span className={cn("block text-sm leading-5", active && "font-medium")}>{lesson.title}</span>
                          <span className="mt-1 block text-xs font-normal text-muted-foreground">{lesson.duration} мин · Видеоурок</span>
                        </span>
                      </ButtonLink>
                    );
                  })}
                </AccordionContent>
              </AccordionItem>
            );
          })}
        </Accordion>
      </CollapsibleContent>
    </Collapsible>
  );
}
