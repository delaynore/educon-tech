"use client";
import { useRef, useState } from "react";
import { Maximize, Play, Volume2 } from "lucide-react";
import { Button } from "@/shared/components/ui/button";
import { Progress } from "@/shared/components/ui/progress";
export function LessonVideo({ title, courseTitle, duration }: { title: string; courseTitle: string; duration: number }) {
  const stageRef = useRef<HTMLDivElement>(null);
  const [previewOpen, setPreviewOpen] = useState(false);
  return <div ref={stageRef} className="relative isolate aspect-video overflow-hidden rounded-lg bg-muted text-foreground">
    <div className="absolute inset-0 bg-[linear-gradient(var(--border)_1px,transparent_1px),linear-gradient(90deg,var(--border)_1px,transparent_1px)] bg-size-[40px_40px]" />
    <span className="absolute top-[6%] left-[4%] text-[7px] font-semibold tracking-wide sm:text-[9px]">educon / design school</span>
    <div className="absolute top-[28%] right-0 left-[7%]"><span className="text-[5px] tracking-widest text-muted-foreground sm:text-[7px]">{courseTitle}</span><h2 className="relative z-10 mt-3 font-serif max-w-[70%] text-[clamp(22px,3.2vw,52px)] leading-[1.05] tracking-tight">{title}</h2></div>
    <div aria-hidden="true" className="absolute top-[26%] right-[9%] size-[37%] -rotate-12"><i className="absolute top-0 right-0 size-[74%] rounded-full border-[clamp(12px,2vw,30px)] border-primary/40 shadow-xl" /><b className="absolute top-[37%] right-[24%] size-[68%] rotate-18 rounded bg-secondary shadow-xl" /><span className="absolute top-[57%] -right-[10%] text-[clamp(48px,7vw,115px)] leading-none text-primary">✳</span></div>
    <Button variant="ghost" size="icon-lg" aria-label={"Открыть предпросмотр видео"} onClick={() => setPreviewOpen(true)} className="absolute top-1/2 left-1/2 z-10 size-12 -translate-1/2 rounded-full bg-background/80 text-foreground shadow-xl backdrop-blur-sm hover:bg-background sm:size-15"><Play fill="currentColor" /></Button>
    {previewOpen && <p role="status" className="absolute right-0 bottom-12 left-0 bg-popover/90 p-2 text-center text-[8px] text-popover-foreground sm:text-[10px]">Демонстрационный урок: видео пока не добавлено.</p>}
    <div className="absolute right-0 bottom-0 left-0 flex h-11 items-center gap-3 bg-linear-to-b from-transparent to-background/90 px-3 text-foreground sm:px-5">
      <Button variant="ghost" size="icon-sm" aria-label="Открыть предпросмотр видео" className="text-inherit hover:bg-background/10 hover:text-inherit" onClick={() => setPreviewOpen(true)}><Play /></Button><span className="text-[8px]">00:00 / {duration}:00</span><Progress value={0} className="flex-1 [&_[data-slot=progress-track]]:h-0.5" aria-label="Video progress" /><Volume2 size={16} className="hidden sm:block" />
      <Button variant="ghost" size="icon-sm" aria-label="На весь экран" className="text-inherit hover:bg-background/10 hover:text-inherit" onClick={() => { if (document.fullscreenElement) void document.exitFullscreen(); else void stageRef.current?.requestFullscreen(); }}><Maximize /></Button>
    </div>
  </div>;
}
