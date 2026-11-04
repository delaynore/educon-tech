"use client";

import { useCallback, useEffect, useRef, useState, type CSSProperties } from "react";
import { useRouter } from "next/navigation";
import { ArrowUpRight, BookOpen, Search, X } from "lucide-react";
import type { DemoCourse } from "@/entities/courses/types";
import { getContinueLesson, getCourseLessons } from "@/entities/courses/demo-data";
import { Button } from "@/shared/components/ui/button";
import { ButtonLink } from "@/shared/components/ui/button-link";
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogTitle, DialogTrigger } from "@/shared/components/ui/dialog";
import { Input } from "@/shared/components/ui/input";
import { routes } from "@/shared/routes";

import { Kbd } from "@/shared/components/ui/kbd";
import { Empty, EmptyHeader, EmptyTitle, EmptyDescription, EmptyContent, EmptyMedia } from "@/shared/components/ui/empty";
import { ToggleGroup, ToggleGroupItem } from "@/shared/components/ui/toggle-group";

export function CourseSearch({ courses }: { courses: DemoCourse[] }) {
  const [open, setOpen] = useState(false);
  const [query, setQuery] = useState("");
  const [category, setCategory] = useState<string | null>(null);
  const [reveal, setReveal] = useState({
    from: "inset(16px 20% calc(100% - 64px) 20%)",
    to: "inset(0px 0px 0px 0px)",
  });
  const triggerRef = useRef<HTMLButtonElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const router = useRouter();
  const words = query.trim().toLocaleLowerCase("ru").split(/\s+/).filter(Boolean);
  const categories = [...new Set(courses.map((course) => course.category))];
  const results = courses.filter((course) => {
    const text = `${course.title} ${course.description} ${course.author.name} ${course.category}`.toLocaleLowerCase("ru");
    return (!category || course.category === category) && words.every((word) => text.includes(word));
  });

  const changeOpen = useCallback((nextOpen: boolean) => {
    if (nextOpen) {
      const bounds = triggerRef.current?.getBoundingClientRect();
      const visible = bounds && bounds.bottom > 0 && bounds.top < window.innerHeight;
      const top = visible ? bounds.top : 16;
      const left = visible ? bounds.left : 24;
      const width = visible ? bounds.width : window.innerWidth - 48;
      const height = visible ? bounds.height : 48;
      setReveal({
        from: `inset(${Math.max(0, top)}px ${Math.max(0, window.innerWidth - left - width)}px ${Math.max(0, window.innerHeight - top - height)}px ${Math.max(0, left)}px)`,
        to: "inset(0px 0px 0px 0px)",
      });
    }
    setOpen(nextOpen);
  }, []);

  useEffect(() => {
    function onKeyDown(event: KeyboardEvent) {
      const slash = event.key === "/" && !event.ctrlKey && !event.metaKey && !event.altKey;
      const commandK = (event.ctrlKey || event.metaKey) && !event.altKey && event.key.toLowerCase() === "k";
      if (event.defaultPrevented || (!slash && !commandK)) return;
      if (event.target instanceof HTMLElement && event.target.closest("input, textarea, select, [contenteditable]")) return;
      event.preventDefault();
      changeOpen(true);
    }
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [changeOpen]);

  function clearSearch() {
    setQuery("");
    setCategory(null);
    inputRef.current?.focus();
  }

  return (
    <Dialog open={open} onOpenChange={changeOpen}>
      <DialogTrigger render={<Button ref={triggerRef} variant="link" className="h-12 w-full min-w-0 justify-start gap-3 rounded-none border-0 border-b border-border bg-transparent px-0 text-base font-normal text-muted-foreground shadow-none transition-colors duration-200 hover:border-foreground/50 hover:bg-transparent hover:text-foreground hover:no-underline aria-expanded:bg-transparent motion-reduce:transition-none" />}>
        <Search className="size-5! shrink-0" />
        <span className="min-w-0 flex-1 truncate text-left">Найти курс…</span>
        <Kbd className="grid size-7 shrink-0 place-items-center rounded-md border border-border font-mono text-sm text-muted-foreground">/</Kbd>
      </DialogTrigger>
      <DialogContent
        initialFocus={inputRef}
        showCloseButton={false}
        overlayProps={{ className: "bg-transparent supports-backdrop-filter:backdrop-filter-none animate-none data-open:animate-none data-closed:animate-none" }}
        style={{ "--search-reveal-from": reveal.from, "--search-reveal-to": reveal.to } as CSSProperties}
        className="inset-0 flex flex-col bg-background text-foreground sm:max-w-none data-open:animate-none data-closed:animate-none ring-0 h-dvh max-h-dvh w-full max-w-none translate-none gap-0 overflow-y-auto rounded-none border-0 p-0 shadow-none [clip-path:var(--search-reveal-to)] transition-[clip-path,opacity] duration-300 ease-[cubic-bezier(0.22,1,0.36,1)] data-starting-style:[clip-path:var(--search-reveal-from)] data-starting-style:opacity-0 data-ending-style:[clip-path:var(--search-reveal-from)] data-ending-style:opacity-0 motion-reduce:transition-none"
      >
        <header className="flex w-full shrink-0 items-center justify-between gap-4 border-b px-5 py-4 sm:px-8">
          <span className="flex items-center gap-2 text-sm font-medium"><BookOpen className="size-4" />Поиск курсов</span>
          <DialogClose render={<Button variant="ghost" className="h-9 gap-2 px-3 text-muted-foreground" />}>
            <span className="hidden text-xs sm:inline">Закрыть</span><Kbd className="hidden rounded border px-1.5 py-0.5 text-[10px] sm:inline">Esc</Kbd><X className="size-4" /><span className="sr-only sm:hidden">Закрыть поиск</span>
          </DialogClose>
        </header>
        <div className="min-w-0 flex-1">
          <div className="mx-auto w-full max-w-5xl px-5 pt-8 pb-12 sm:px-8 sm:pt-14">
            <DialogTitle className="pr-0 text-3xl font-semibold tracking-tight sm:text-5xl">Чему научимся дальше?</DialogTitle>
            <DialogDescription className="mt-3">Найдите курс по названию, теме или имени преподавателя.</DialogDescription>
            <form
              id="course-search-form"
              role="search"
              className="mt-8 flex items-center gap-3 border-b-2 pb-3 transition-colors focus-within:border-primary sm:mt-10 sm:gap-5"
              onSubmit={(event) => {
                event.preventDefault();
                if (!results[0]) return;
                const course = results[0];
                changeOpen(false);
                router.push(routes.lesson(course.id, getContinueLesson(course).id));
              }}
            >
              <Search className="size-6 shrink-0 text-muted-foreground sm:size-8" />
              <Input ref={inputRef} type="search" aria-label="Поиск курсов" aria-controls="course-search-results" autoComplete="off" placeholder="Что вам интересно?" value={query} onChange={(event) => setQuery(event.target.value)} className="h-14 rounded-none border-0 bg-transparent px-0 text-xl shadow-none outline-none focus-visible:ring-0 sm:text-3xl md:text-3xl dark:bg-transparent [&::-webkit-search-cancel-button]:appearance-none" />
              {query && <Button type="button" variant="ghost" size="icon" aria-label="Очистить запрос" onClick={() => { setQuery(""); inputRef.current?.focus(); }}><X /></Button>}
            </form>
            <ToggleGroup value={[category ?? "all"]} onValueChange={(values) => setCategory(!values[0] || values[0] === "all" ? null : values[0])} className="mt-5 w-full flex-wrap justify-start" aria-label="Темы курсов">
              <ToggleGroupItem value="all" className="rounded-full px-4 text-xs">Все темы</ToggleGroupItem>
              {categories.map((item) => <ToggleGroupItem key={item} value={item} className="rounded-full px-4 text-xs">{item}</ToggleGroupItem>)}
            </ToggleGroup>
            <div className="mt-10 mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div className="flex items-center justify-between gap-3 sm:justify-start">
                <h3 className="text-sm font-medium">{query.trim() || category ? "Результаты поиска" : "Ваши курсы"}</h3>
                <span role="status" aria-live="polite" className="text-xs text-muted-foreground">Найдено: {results.length}</span>
              </div>
              <Button type="submit" form="course-search-form" variant="outline" disabled={!results.length} className="h-10 w-full gap-2 px-4 text-sm sm:w-auto">Открыть первый курс<ArrowUpRight /></Button>
            </div>
            <ul id="course-search-results" className="space-y-3">
              {results.map((course) => (
                <li key={course.id} className="animate-in fade-in duration-200 motion-reduce:animate-none">
                  <ButtonLink href={routes.lesson(course.id, getContinueLesson(course).id)} variant="ghost" onClick={() => changeOpen(false)} className="group h-auto w-full justify-start gap-4 rounded-xl border bg-card p-4 text-left whitespace-normal transition-[background-color,border-color] hover:border-primary/30 hover:bg-accent sm:gap-6 sm:p-5">
                    <span aria-hidden="true" className="grid size-14 shrink-0 place-items-center rounded-lg bg-muted font-serif text-2xl text-muted-foreground sm:size-20 sm:text-4xl">{course.mark}</span>
                    <span className="min-w-0 flex-1">
                      <span className="mb-1 block text-[10px] tracking-wide text-muted-foreground">{course.category} · {getCourseLessons(course).length} уроков</span>
                      <span className="block text-base font-semibold sm:text-xl">{course.title}</span>
                      <span className="mt-1 hidden text-sm font-normal text-muted-foreground sm:block">{course.description}</span>
                      <span className="mt-2 block text-xs font-normal text-muted-foreground">{course.author.name}</span>
                    </span>
                    <ArrowUpRight className="size-5! shrink-0 text-muted-foreground" />
                  </ButtonLink>
                </li>
              ))}
            </ul>
            {!results.length && <Empty className="border py-12"><EmptyHeader><EmptyMedia variant="icon"><Search /></EmptyMedia><EmptyTitle>Пока ничего не нашлось</EmptyTitle><EmptyDescription>Попробуйте другую тему или более короткий запрос.</EmptyDescription></EmptyHeader><EmptyContent><Button variant="outline" onClick={clearSearch}>Сбросить поиск</Button></EmptyContent></Empty>}
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
