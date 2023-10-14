export interface BookmarkModel {
    id: number;
    url: string;
    title: string | null;
    description: string | null;
    tags: string[];
}

export interface Website {
    "title": string;
    "description": string;
}