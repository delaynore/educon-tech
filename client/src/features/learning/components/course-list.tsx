"use client";
import { useState } from "react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/shared/components/ui/tabs";
import { getCourseLessons } from "@/entities/courses/demo-data";
import type { DemoCourse } from "@/entities/courses/types";
import { Empty, EmptyDescription } from "@/shared/components/ui/empty";
import { CourseCard } from "./course-card";

export function CourseList({ courses }: { courses: DemoCourse[] }) {
  const [filter, setFilter] = useState("Все курсы");
  const shown = courses.filter((course) => {
    const lessons = getCourseLessons(course);
    const completed = lessons.filter((lesson) => lesson.completed).length;
    return (filter !== "В процессе" || (completed > 0 && completed < lessons.length))
      && (filter !== "Завершены" || completed === lessons.length);
  });
  return <section className="min-w-0">
    <h2 className="mb-4 text-xl font-semibold tracking-tight">Мои курсы <span className="ml-2 align-middle text-xs font-normal text-muted-foreground">{String(courses.length).padStart(2, "0")}</span></h2>
    <Tabs value={filter} onValueChange={(value) => setFilter(String(value))}>
      <div className="mb-1">
        <TabsList aria-label="Фильтр курсов" className="bg-transparent p-0">{["Все курсы", "В процессе", "Завершены"].map((tab) => <TabsTrigger key={tab} value={tab} className="px-2 text-xs data-active:bg-muted data-active:text-primary data-active:shadow-none">{tab}</TabsTrigger>)}</TabsList>
      </div>
      <TabsContent value={filter} className="grid gap-4 min-[480px]:grid-cols-2 min-[1500px]:grid-cols-3">
        {shown.map((course) => <CourseCard key={course.id} course={course} />)}
        {!shown.length && <Empty className="col-span-full border p-8"><EmptyDescription className="text-xs leading-6">{filter === "Завершены" ? "Первые завершённые курсы скоро появятся здесь. Продолжайте учиться!" : "Для этого фильтра пока нет курсов."}</EmptyDescription></Empty>}
      </TabsContent>
    </Tabs>
  </section>;
}
