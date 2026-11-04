import { notFound } from "next/navigation";
import { demoCourses, getCourseLessons } from "@/entities/courses/demo-data";
import { LessonPage } from "@/features/lesson/lesson-page";

export function generateStaticParams() {
  return demoCourses.flatMap((course) => getCourseLessons(course).map((lesson) => ({
    courseId: course.id,
    lessonId: lesson.id,
  })));
}

export default async function CourseLessonPage({ params }: {
  params: Promise<{ courseId: string; lessonId: string }>;
}) {
  const { courseId, lessonId } = await params;
  const course = demoCourses.find((course) => course.id === courseId);
  if (!course) notFound();
  const lesson = getCourseLessons(course).find((lesson) => lesson.id === lessonId);
  if (!lesson) notFound();
  return <LessonPage course={course} lesson={lesson} />;
}
