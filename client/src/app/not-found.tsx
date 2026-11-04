import { ArrowLeft, BookOpen } from "lucide-react";
import { ButtonLink } from "@/shared/components/ui/button-link";
import { routes } from "@/shared/routes";
import { Card } from "@/shared/components/ui/card";

export default function NotFound() {
  return (
    <section className="grid min-h-[60dvh] place-items-center py-8">
      <Card className="w-full max-w-lg gap-0 p-6 text-center shadow-none sm:p-10">
        <p className="mb-4 text-sm font-medium tracking-widest text-muted-foreground">404</p>
        <h1 className="text-2xl font-semibold tracking-tight sm:text-3xl">Страница не найдена</h1>
        <p className="mt-3 text-sm leading-6 text-muted-foreground">Возможно, адрес изменился или в нём есть опечатка. Вернитесь на главную или продолжите обучение.</p>
        <div className="mt-7 flex flex-col justify-center gap-3 sm:flex-row">
          <ButtonLink href={routes.home} className="h-10 gap-2 px-4"><ArrowLeft />На главную</ButtonLink>
          <ButtonLink href={routes.courses} variant="outline" className="h-10 gap-2 px-4"><BookOpen />Мои курсы</ButtonLink>
        </div>
      </Card>
    </section>
  );
}
