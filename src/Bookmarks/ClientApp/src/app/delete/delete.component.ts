import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { Bookmark } from '../models/bookmark';
import { BookmarkService } from '../services/bookmark.service';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css']
})
export class DeleteComponent implements OnInit {
  public bookmark: Bookmark | undefined;
  public deleting: boolean = false;

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

  delete(): void {
    this.deleting = true;

    this.bookmarkService.deleteBookmark(this.bookmark?.id ?? 0)
      .subscribe(bookmark => {
        if (bookmark) {
          this.location.back();
        }

        this.deleting = false;
      });
  }

  cancel(): void {
    this.location.back();
  }
}
