import { ArrowUpRight, ListVideo } from "lucide-react";
import { Card, CardContent, CardTitle } from "@/shared/components/ui/card";
import { ButtonLink } from "@/shared/components/ui/button-link";
import { Progress } from "@/shared/components/ui/progress";
import { cn } from "@/shared/lib/utils";
import { routes } from "@/shared/routes";
import { getContinueLesson, getCourseLessons } from "@/entities/courses/demo-data";
import type { DemoCourse } from "@/entities/courses/types";

export function CourseCard({ course }: { course: DemoCourse }) {
  const lessons = getCourseLessons(course);
  const completed = lessons.filter((lesson) => lesson.completed).length;
  const href = routes.lesson(course.id, getContinueLesson(course).id);
  return <Card className="gap-0 overflow-hidden py-0 shadow-none">
    <ButtonLink href={href} variant="ghost" aria-label={"Открыть " + course.title} className="relative block h-40 w-full overflow-hidden rounded-none bg-muted p-0 text-left text-muted-foreground hover:bg-accent hover:text-accent-foreground">
      <span className="absolute top-4 left-4 text-[7px] tracking-[0.2em]">{course.category}</span>
      <span className={cn("absolute top-6 left-1/2 -translate-x-1/2 -rotate-8 font-serif text-[94px] leading-tight tracking-tighter", course.mark === "</>" && "top-11 font-mono text-[64px]")}>{course.mark}</span>
      <span className="absolute -right-4 -bottom-18 size-44 rounded-full border border-current opacity-20" /><ArrowUpRight className="absolute right-4 bottom-4 size-5!" />
    </ButtonLink>
    <CardContent className="p-4">
      <span className="flex flex-wrap items-center gap-1 text-xs text-muted-foreground"><ListVideo size={12} />{lessons.length} уроков · {course.author.name}</span>
      <CardTitle className="mt-2 mb-1 text-base leading-normal"><ButtonLink href={href} variant="link" className="h-auto justify-start p-0 text-left text-inherit whitespace-normal">{course.title}</ButtonLink></CardTitle>
      <p className="min-h-9 text-xs leading-5 text-muted-foreground">{course.description}</p>
      <div className="mt-4 mb-2 flex justify-between text-xs text-muted-foreground"><span>Пройдено {completed} из {lessons.length}</span><span className="text-primary/70">{Math.round(completed / lessons.length * 100)}%</span></div>
      <Progress value={completed} max={lessons.length} aria-label={"Прогресс курса " + course.title} />
    </CardContent>
  </Card>;
}
