import { Component, OnInit } from '@angular/core';
import { BookmarkService } from "../services/bookmark.service";

import { Bookmark } from "../models/bookmark";

@Component({
  selector: 'app-bookmarks',
  templateUrl: './bookmarks.component.html',
  styleUrls: ['./bookmarks.component.css']
})

export class BookmarksComponent implements OnInit {
  bookmarks: Bookmark[] = [];
  filteredBookmarks: Bookmark[] = [];
  tags: string[] = [];
  searchText: string = "";

  constructor(private bookmarkService: BookmarkService) {}

  ngOnInit(): void {
    this.bookmarkService.getBookmarks()
      .subscribe(bookmarks => {
        this.bookmarks = bookmarks;
        this.searchText = "";
        this.search();
      });

    this.bookmarkService.getTags()
      .subscribe(tags => this.tags = tags);
  }

  searchKeyup(event:any) {
    if (event.keyCode == 13) {
      this.search();
    }
  }

  search() {
    if (!this.searchText) {
      this.filteredBookmarks = this.bookmarks;
      return;
    }

    if (this.searchText.startsWith("#")) {
      // Search by tag
      const name = this.searchText.substring(1);

      this.filteredBookmarks = this.bookmarks.filter((bookmark) => {
        let foundTag = false;
        bookmark.tags.every((tag) => {
          if (tag == name) {
            foundTag = true;
            return false;
          }

          return true;
        });

        return foundTag;
      });

      return;
    }

    // Search by url/title/description
    const searchValue = this.searchText.toLowerCase();
    this.filteredBookmarks = this.bookmarks.filter((bookmark) => {
      return bookmark.url.toLowerCase().includes(searchValue) ||
        bookmark.title?.toLowerCase().includes(searchValue) ||
        bookmark.description?.toLowerCase().includes(searchValue);
    });
  }

  filterTag(tag : string) {
    this.searchText = "#" + tag;
    this.search();
  }
}
