import { Component, OnInit } from '@angular/core';
import { BookmarkService } from "../bookmark.service";

import { Bookmark } from "../bookmark";

@Component({
  selector: 'app-bookmarks',
  templateUrl: './bookmarks.component.html',
  styleUrls: ['./bookmarks.component.css']
})

export class BookmarksComponent implements OnInit {
  bookmarks: Bookmark[] = [];
  tags: string[] = [];

  constructor(private bookmarkService: BookmarkService) {}

  ngOnInit(): void {
    this.bookmarkService.getBookmarks()
      .subscribe(bookmarks => this.bookmarks = bookmarks);

    this.bookmarkService.getTags()
      .subscribe(tags => this.tags = tags);
  }

  search(text: string) {
    // TODO: Search
  }

  filterTag(tag : string) {
    // TODO: Sök på tag
  }
}
