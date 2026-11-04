import { ButtonLink } from "@/shared/components/ui/button-link";
import { routes } from "@/shared/routes";
import { Empty, EmptyHeader, EmptyTitle, EmptyDescription, EmptyContent } from "@/shared/components/ui/empty";

export default function LessonNotFound() {
  return <Empty className="min-h-[70dvh]">
    <EmptyHeader><EmptyTitle><h1 className="text-3xl font-semibold">Урок не найден</h1></EmptyTitle>
    <EmptyDescription>Проверьте адрес или выберите урок из ваших курсов.</EmptyDescription></EmptyHeader>
    <EmptyContent><ButtonLink href={routes.courses}>К моим курсам</ButtonLink></EmptyContent>
  </Empty>;
}
