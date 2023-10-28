import { Component, Input, OnInit } from '@angular/core';
import { Observable, OperatorFunction } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

import {Bookmark} from "../models/bookmark";
import {BookmarkService} from "../services/bookmark.service";

@Component({
  selector: 'app-bookmark-editor',
  templateUrl: './bookmark-editor.component.html',
  styleUrls: ['./bookmark-editor.component.css']
})
export class BookmarkEditorComponent implements OnInit {
  @Input() bookmark?:Bookmark;

  tags: string[] = [];
  public newTag: any;

  constructor(private bookmarkService: BookmarkService) {}

  ngOnInit(): void {
    this.bookmarkService.getTags()
      .subscribe(tags => this.tags = tags);
  }

  removeTag(tag:string) {
    if (!this.bookmark) {
      return;
    }

    let index = this.bookmark.tags.indexOf(tag);
    this.bookmark.tags.splice(index, 1);
  }

  loadInfo() {
    if (!this.bookmark?.url) {
      return;
    }

    if (!this.bookmark.url.startsWith("http://") && !this.bookmark.url.startsWith("https://")) {
      return;
    }

    this.bookmarkService.loadInfo(this.bookmark.url)
      .subscribe(siteInfo => {
        if (this.bookmark) {
          this.bookmark.title = siteInfo.title;
          this.bookmark.description = siteInfo.description;
        }
      });
  }

  selectTag(event:any) {
    if (event.keyCode == 13) {
      this.bookmark?.tags.push(this.newTag);
      this.newTag = "";
    }
  }

  search: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map((term) =>
        term.length < 2 ? [] : this.tags.filter((v) => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10),
      ),
    );
}
