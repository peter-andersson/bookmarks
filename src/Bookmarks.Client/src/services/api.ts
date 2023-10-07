import type {BookmarkModel, Website, Tag} from "@/models";
import type {RouteParamValue} from "vue-router";

class BookmarkApi {
    async addBookmark(bookmark : BookmarkModel) : Promise<boolean> {
        try {
            await new Promise(r => setTimeout(r, 5000));

            const response = await fetch('/api/bookmark/', {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(bookmark)
            });
            if (response.ok) {
                return true;
            } else {
                this.handleErrorResponse("Add bookmark failed.", response);
                return false;
            }
        } catch (err : any) {
            this.handleErrors(err);
            throw "Failed to add bookmark";
        }
    }

    async deleteBookmark(id: string | RouteParamValue[]) : Promise<boolean> {
        try {
            const response = await fetch('/api/bookmark/' + id, {
                method: "DELETE"
            });
            if (response.ok) {
                return true;
            } else {
                this.handleErrorResponse("Delete bookmark failed.", response);
                return false;
            }
        } catch (err : any) {
            this.handleErrors(err);
            throw "Failed to delete bookmark";
        }
    }

    async getBookmarks() : Promise<BookmarkModel[]> {
        try {
            const response = await fetch('/api/bookmark');

            return await response.json();
        } catch (err : any) {
            this.handleErrors(err);
            throw "Failed to get bookmarks";
        }
    }

    async getBookmark(id: string | RouteParamValue[]) : Promise<BookmarkModel | null> {
        try {
            const response = await fetch('/api/bookmark/' + id);
            if (response.ok) {
                return await response.json();
            } else {
                this.handleErrorResponse("Get bookmark failed.", response);
                return null;
            }
        } catch (err : any) {
            this.handleErrors(err);
            throw "Failed to get bookmark";
        }
    }

    async getTags() : Promise<Tag[]> {
        try {
            const response = await fetch('/api/bookmark/tags');

            return await response.json();
        } catch (err : any) {
            this.handleErrors(err);
            return [];
        }
    }

    async loadInfo(url : string): Promise<Website | null> {
        try {
            await new Promise(r => setTimeout(r, 5000));

            const response = await fetch('/api/bookmark/info', {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: url
            });
            if (response.ok) {
                return await response.json();
            } else {
                this.handleErrorResponse("Get info failed.", response);
                return null;
            }
        } catch (err : any) {
            this.handleErrors(err);
            throw "Failed to get info";
        }
    }

    async updateBookmark(bookmark : BookmarkModel) : Promise<boolean> {
        try {
            const response = await fetch('/api/bookmark/', {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(bookmark)
            });
            if (response.ok) {
                return true;
            } else {
                this.handleErrorResponse("Add bookmark failed.", response);
                return false;
            }
        } catch (err : any) {
            this.handleErrors(err);
            throw "Failed to update bookmark";
        }
    }

    handleErrors(err: any) {
        console.error(err);
    }

    handleErrorResponse(message: string, response: Response) {
        console.error(message + " Status " + response.statusText);
    }
}

export const api = new BookmarkApi();