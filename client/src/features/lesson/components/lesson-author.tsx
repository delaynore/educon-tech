import type { DemoCourse } from "@/entities/courses/types";
import { ProfileAvatar } from "@/shared/components/profile-avatar";

export function LessonAuthor({ author }: { author: DemoCourse["author"] }) {
  return <div className="flex items-center gap-3">
    <ProfileAvatar initials={author.initials} className="bg-secondary text-secondary-foreground" />
    <div><span className="block text-xs text-muted-foreground">Автор курса</span><strong className="block text-sm font-medium">{author.name}</strong><span className="mt-1 block text-xs text-muted-foreground">{author.role}</span></div>
  </div>;
}
