import { ArrowUpRight } from "lucide-react";
import { demoCourses } from "@/entities/courses/demo-data";
import { CourseList } from "@/features/learning/components/course-list";
import { WelcomeBanner } from "@/features/learning/components/welcome-banner";
import { ButtonLink } from "@/shared/components/ui/button-link";
import { Eyebrow } from "@/shared/components/eyebrow";
import { routes } from "@/shared/routes";

type DashboardSection = "home" | "courses";

export function Dashboard({ section = "home" }: { section?: DashboardSection }) {
  return <div className="space-y-6">
        <div className="mb-7 flex items-center justify-between gap-3">
          <div><Eyebrow>ЛИЧНЫЙ КАБИНЕТ</Eyebrow><h1 className="my-2 text-2xl font-semibold tracking-tight sm:text-3xl">{section === "home" ? "Привет, Александр" : "Мои курсы"}</h1><p className="text-sm text-muted-foreground">{section === "home" ? "Продолжите обучение или выберите курс." : "Ваши курсы и прогресс обучения."}</p></div>
          {section === "home" && <ButtonLink href={routes.courses} variant="outline" className="text-[11px]"><span className="hidden sm:inline">Мои курсы</span><ArrowUpRight /></ButtonLink>}
        </div>
        {section === "home" && <WelcomeBanner course={demoCourses[0]} />}
        <CourseList courses={demoCourses} />
  </div>;
}
