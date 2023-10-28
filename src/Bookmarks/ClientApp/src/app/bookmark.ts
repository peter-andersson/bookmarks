export interface Bookmark {
  id: number;
  url: string;
  title: string | null;
  description: string | null;
  tags: string[];
}
