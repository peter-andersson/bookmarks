import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { Bookmark } from '../bookmark';
import { BookmarkService } from '../bookmark.service';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css']
})
export class DeleteComponent implements OnInit {
  bookmark: Bookmark | undefined;

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
    this.bookmarkService.deleteBookmark(this.bookmark?.id ?? 0)
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
