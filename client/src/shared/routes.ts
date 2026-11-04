export const routes = {
  home: "/",
  courses: "/courses",
  lesson: (courseId: string, lessonId: string) =>
    `/courses/${encodeURIComponent(courseId)}/lessons/${encodeURIComponent(lessonId)}`,
};
