import type { CourseModule, DemoCourse } from "./types";

function module(id: string, title: string, titles: string[], completed = 0): CourseModule {
  return {
    id, title,
    lessons: titles.map((lessonTitle, index) => ({
      id: `${id}-${index + 1}`,
      title: lessonTitle,
      duration: [18, 22, 25, 30, 12][index % 5],
      completed: index < completed,
      description: `В уроке «${lessonTitle}» разберём основные принципы, посмотрим на примеры и попробуем применить новые знания на практике. Двигайтесь в своём темпе и возвращайтесь к материалам, когда понадобится.`,
      assignment: `Выберите пример по теме «${lessonTitle}». Опишите три наблюдения и предложите своё решение: что можно улучшить и почему?`,
    })),
  };
}

// Local fixtures only: no requests, uploads, progress writes or persistence.
export const demoCourses: DemoCourse[] = [
  {
    id: "ui-ux", title: "Основы UI/UX-дизайна", category: "ДИЗАЙН",
    author: { name: "Анна Смирнова", initials: "АС", role: "Продуктовый дизайнер · ваш наставник" },
    mark: "Aa",
    description: "От первой идеи до интерфейса, которым хочется пользоваться.",
    modules: [
      module("intro", "Погружение", ["Знакомство с визуальным дизайном", "Как устроен процесс дизайна"], 2),
      module("visual", "Визуальный язык", ["Композиция и визуальная иерархия", "Цвет как инструмент", "Типографика: от буквы к системе", "Собираем первый макет"]),
      module("practice", "Практика", ["Компоненты и состояния", "Адаптивный интерфейс"]),
      module("project", "Ваш проект", ["От идеи до прототипа", "Презентация проекта"]),
    ],
  },
  {
    id: "frontend", title: "Frontend с нуля", category: "РАЗРАБОТКА",
    author: { name: "Михаил Волков", initials: "МВ", role: "Frontend-разработчик · ваш наставник" },
    mark: "</>",
    description: "Превращайте идеи в живые, интерактивные продукты.",
    modules: [
      module("start", "Первые шаги", ["Как работает веб", "Настраиваем рабочее место"], 1),
      module("layout", "Создаём страницу", ["Семантический HTML", "Стили и адаптивная вёрстка"]),
      module("javascript", "Добавляем интерактивность", ["Основы JavaScript", "События и состояние"]),
      module("project", "Ваш проект", ["Собираем приложение", "Публикуем результат"]),
    ],
  },
  {
    id: "creative", title: "Креативное мышление", category: "SOFT SKILLS",
    author: { name: "Дарья Белова", initials: "ДБ", role: "Креативный стратег · ваш наставник" },
    mark: "✳",
    description: "Находите неожиданные решения привычных задач.",
    modules: [
      module("ideas", "Учимся замечать", ["Откуда берутся идеи", "Наблюдаем и задаём вопросы"]),
      module("methods", "Инструменты креативности", ["Ассоциации и аналогии", "Меняем точку зрения"]),
      module("practice", "Проверяем идеи", ["Быстрый эксперимент", "Обратная связь"]),
      module("project", "Ваш проект", ["Создаём концепцию", "Делимся результатом"]),
    ],
  },
];

export function getCourseLessons(course: DemoCourse) {
  return course.modules.flatMap((module) => module.lessons);
}

export function getContinueLesson(course: DemoCourse) {
  const lessons = getCourseLessons(course);
  return lessons.find((lesson) => !lesson.completed) ?? lessons[0];
}
