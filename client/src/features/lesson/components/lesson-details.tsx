"use client";

import { useState } from "react";
import { ArrowDownToLine, Check, FileText } from "lucide-react";
import type { CourseLesson } from "@/entities/courses/types";
import { Button } from "@/shared/components/ui/button";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/shared/components/ui/tabs";
import { Textarea } from "@/shared/components/ui/textarea";
import { Label } from "@/shared/components/ui/label";
import { Badge } from "@/shared/components/ui/badge";

export function LessonDetails({ lesson }: { lesson: CourseLesson }) {
  const [answer, setAnswer] = useState("");
  const [saved, setSaved] = useState(false);
  const notes = `${lesson.title}\n\n${lesson.description}\n\nЗадание\n${lesson.assignment}`;

  return <Tabs defaultValue="about" className="gap-0">
    <TabsList variant="line" aria-label="Информация об уроке" className="w-full justify-start gap-4 border-b p-0 group-data-horizontal/tabs:h-auto sm:gap-6">
      {[{ value: "about", label: "Об уроке" }, { value: "assignment", label: "Задание" }, { value: "materials", label: "Материалы" }].map(({ value, label }) => <TabsTrigger key={value} value={value} className="h-11 flex-none rounded-none border-0 px-1 py-3 text-sm data-active:text-primary dark:data-active:text-primary after:bg-primary group-data-horizontal/tabs:after:bottom-0">{label}{value === "assignment" && <Badge variant="secondary" className="rounded px-1.5 text-primary/80">1</Badge>}</TabsTrigger>)}
    </TabsList>
    <TabsContent value="about" className="min-h-42 pt-6">
      <h2 className="mb-3 text-base font-medium">Что разберём в этом уроке</h2>
      <p className="max-w-2xl text-sm leading-6 text-muted-foreground">{lesson.description}</p>
      <div className="mt-4 flex flex-wrap gap-4">{["Разберём принципы", "Посмотрим примеры", "Попробуем на практике"].map((item) => <span key={item} className="flex items-center gap-2 text-xs text-primary/70"><Check size={15} />{item}</span>)}</div>
    </TabsContent>
    <TabsContent value="assignment" className="space-y-4 pt-6">
      <h2 className="text-base font-medium">Время попробовать</h2>
      <p className="text-sm leading-6 text-muted-foreground">{lesson.assignment}</p>
      <Label htmlFor="lesson-answer" className="block text-xs text-primary/80">Ваш ответ</Label>
      <Textarea id="lesson-answer" placeholder="Ваши наблюдения…" value={answer} onChange={(event) => { setAnswer(event.target.value); setSaved(false); }} />
      <div className="flex flex-wrap items-center gap-3"><Button disabled={!answer.trim()} onClick={() => setSaved(true)} className="text-xs">Сохранить черновик</Button><span className="text-xs text-muted-foreground">Пока только на этой странице, без отправки.</span></div>
      {saved && <p role="status" className="text-xs text-primary/80">Черновик готов. При переходе к другому уроку или обновлении страницы он сбросится.</p>}
    </TabsContent>
    <TabsContent value="materials" className="min-h-42 space-y-4 pt-6">
      <h2 className="text-base font-medium">Возьмите с собой</h2>
      <a download={`${lesson.id}-notes.txt`} href={`data:text/plain;charset=utf-8,${encodeURIComponent(notes)}`} className="flex max-w-100 items-center gap-3 rounded-lg border p-4 text-sm text-primary/80 outline-none hover:bg-muted/50 focus-visible:ring-2 focus-visible:ring-ring"><FileText size={22} /><span>Заметки к уроку<small className="mt-1 block text-xs text-muted-foreground">TXT · {lesson.title}</small></span><ArrowDownToLine size={18} className="ml-auto shrink-0" /></a>
    </TabsContent>
  </Tabs>;
}
