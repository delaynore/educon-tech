import { Card, CardContent } from "@/shared/components/ui/card";
import { Skeleton } from "@/shared/components/ui/skeleton";

export default function LessonLoading() {
  return <div role="status" aria-label="Загрузка урока" className="space-y-6">
    <span className="sr-only">Загрузка урока…</span>
    <Skeleton className="h-4 w-64 max-w-full" />
    <div aria-hidden="true" className="grid items-start gap-6 xl:grid-cols-[minmax(0,1fr)_280px]">
      <Card className="shadow-none"><CardContent className="space-y-6">
        <Skeleton className="h-8 w-2/3" />
        <Skeleton className="aspect-video w-full" />
        <Skeleton className="h-10 w-48" />
        <Skeleton className="h-32 w-full" />
      </CardContent></Card>
      <Card className="hidden shadow-none xl:flex"><CardContent className="space-y-4">
        <Skeleton className="h-6 w-3/4" />
        <Skeleton className="h-48 w-full" />
      </CardContent></Card>
    </div>
  </div>;
}
