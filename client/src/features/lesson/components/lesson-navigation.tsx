import { ArrowRight, ChevronLeft } from "lucide-react";
import type { CourseLesson } from "@/entities/courses/types";
import { ButtonLink } from "@/shared/components/ui/button-link";
import { routes } from "@/shared/routes";

export function LessonNavigation({ courseId, previous, next }: { courseId: string; previous?: CourseLesson; next?: CourseLesson }) {
  return <nav aria-label="Переход между уроками" className="mt-6 flex flex-col gap-3 border-t pt-6 sm:flex-row sm:justify-between">
    {previous ? <ButtonLink href={routes.lesson(courseId, previous.id)} variant="outline" className="h-auto min-h-11 gap-2 px-4 py-3 text-sm whitespace-normal"><ChevronLeft />Предыдущий урок</ButtonLink> : <span className="hidden sm:block" />}
    {next ? <ButtonLink href={routes.lesson(courseId, next.id)} variant="outline" className="h-auto min-h-11 gap-2 px-4 py-3 text-sm whitespace-normal">Следующий урок<ArrowRight /></ButtonLink> : <ButtonLink href={routes.courses} variant="outline" className="h-auto min-h-11 gap-2 px-4 py-3 text-sm whitespace-normal">К моим курсам<ArrowRight /></ButtonLink>}
  </nav>;
}
