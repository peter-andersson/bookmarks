import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { Bookmark } from '../models/bookmark';
import { BookmarkService } from '../services/bookmark.service';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit {
  public bookmark: Bookmark | undefined;
  public saving: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private bookmarkService: BookmarkService,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.getBookmark();
  }

  getBookmark(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.bookmarkService.getBookmark(id)
      .subscribe(bookmark => this.bookmark = bookmark);
  }

  save() {
    if (!this.bookmark) {
      return;
    }

    // TODO: Validate model...

    this.saving = true;

    this.bookmarkService.updateBookmark(this.bookmark)
      .subscribe(bookmark => {
        if (bookmark) {
          this.location.back();
        }

        this.saving = false;
      });
  }

  cancel() {
    this.location.back();
  }
}
