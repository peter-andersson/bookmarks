export interface Tag {
    id: number;
    name: string;
}

export interface BookmarkModel {
    id: number;
    url: string;
    title: string | null;
    description: string | null;
    tags: Tag[]
}