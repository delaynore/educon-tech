import Link from "next/link";
import { Check } from "lucide-react";
import { Breadcrumb, BreadcrumbList, BreadcrumbItem, BreadcrumbLink, BreadcrumbPage, BreadcrumbSeparator } from "@/shared/components/ui/breadcrumb";
import { Card } from "@/shared/components/ui/card";
import type { CourseLesson, DemoCourse } from "@/entities/courses/types";
import { getCourseLessons } from "@/entities/courses/demo-data";
import { routes } from "@/shared/routes";
import { CourseProgram } from "./components/course-program";
import { LessonAuthor } from "./components/lesson-author";
import { LessonDetails } from "./components/lesson-details";
import { LessonNavigation } from "./components/lesson-navigation";
import { LessonVideo } from "./components/lesson-video";

export function LessonPage({ course, lesson }: { course: DemoCourse; lesson: CourseLesson }) {
  const lessons = getCourseLessons(course);
  const index = lessons.findIndex((item) => item.id === lesson.id);
  const currentModule = course.modules.find((item) => item.lessons.some((item) => item.id === lesson.id));

  return <div className="space-y-6">
    <Breadcrumb aria-label="Навигация по курсу"><BreadcrumbList className="text-xs"><BreadcrumbItem><BreadcrumbLink render={<Link href={routes.courses} />}>Мои курсы</BreadcrumbLink></BreadcrumbItem><BreadcrumbSeparator /><BreadcrumbItem>{course.title}</BreadcrumbItem><BreadcrumbSeparator /><BreadcrumbItem><BreadcrumbPage>{currentModule?.title}</BreadcrumbPage></BreadcrumbItem></BreadcrumbList></Breadcrumb>
    <div className="grid items-start gap-6 xl:grid-cols-[minmax(0,1fr)_280px]">
      <Card className="min-w-0 gap-0 p-4 shadow-none sm:p-6">
        <div className="flex justify-between gap-4"><span className="text-xs text-muted-foreground">Урок {index + 1} из {lessons.length} · {lesson.duration} минут</span><span className="hidden text-xs text-muted-foreground sm:inline">В вашем темпе</span></div>
        <h1 className="mt-3 mb-2 text-2xl leading-tight font-semibold tracking-tight sm:text-3xl">{lesson.title}</h1>
        <p className="mb-6 text-sm leading-6 text-muted-foreground">Шаг за шагом — от новых знаний к собственным решениям.</p>
        <LessonVideo key={`video:${course.id}:${lesson.id}`} title={lesson.title} courseTitle={course.title} duration={lesson.duration} />
        <div className="my-6 flex flex-wrap items-center justify-between gap-4"><LessonAuthor author={course.author} /><span className="flex items-center gap-2 text-xs text-muted-foreground">{lesson.completed ? <><Check size={16} />Урок пройден</> : "Видеоурок · практика"}</span></div>
        <LessonDetails key={`details:${course.id}:${lesson.id}`} lesson={lesson} />
        <LessonNavigation courseId={course.id} previous={lessons[index - 1]} next={lessons[index + 1]} />
      </Card>
      <CourseProgram key={course.id} course={course} lessonId={lesson.id} />
    </div>
  </div>;
}
