export type Lesson = {
  id: string;
  title: string;
  description: string;
  video?: MediaDto;
  createdAt: Date;
  updatedAt: Date;
};

export type MediaDto = {
  id: string;
  url: string;
  status: MediaStatus;
};

export type MediaStatus = "uploading" | "uploaded" | "ready" | "failed" | "deleted";
