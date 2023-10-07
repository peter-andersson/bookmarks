import type {BookmarkModel} from "@/models";
import type {RouteParamValue} from "vue-router";
import router from "@/router";

class BookmarkApi {
    async addBookmark(bookmark : BookmarkModel) : Promise<boolean> {
        console.log(bookmark);
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

    handleErrors(err) {
        console.error(err);
    }

    handleErrorResponse(message: string, response: Response) {
        console.error(message + " Status " + response.statusText);
    }
}

export const api = new BookmarkApi();