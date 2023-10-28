import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Location} from "@angular/common";

import {BookmarkService} from "../services/bookmark.service";
import {Bookmark} from "../models/bookmark";

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.css']
})
export class AddComponent {
  public bookmark: Bookmark = {
    "id": 0,
    "title": "",
    "description": "",
    "url": "",
    "tags": []
  };

  public adding: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private bookmarkService: BookmarkService,
    private location: Location
  ) {}

  add() {
    // TODO: Validate model...
    this.adding = true;

    this.bookmarkService.addBookmark(this.bookmark)
      .subscribe(bookmark => {
        if (bookmark) {
          this.location.back();
        }

        this.adding = false;
      });
  }

  cancel() {
    this.location.back();
  }
}
