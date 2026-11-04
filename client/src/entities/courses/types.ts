// View data for the static design. API DTOs can be mapped to these types later.
export type CourseLesson = {
  id: string;
  title: string;
  duration: number;
  description: string;
  assignment: string;
  completed: boolean;
};

export type CourseModule = {
  id: string;
  title: string;
  lessons: CourseLesson[];
};

export type DemoCourse = {
  id: string;
  title: string;
  category: string;
  author: { name: string; initials: string; role: string };
  description: string;
  mark: string;
  modules: CourseModule[];
};
