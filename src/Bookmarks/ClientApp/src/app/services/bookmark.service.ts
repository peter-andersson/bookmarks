import {Inject, Injectable} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {Observable, of} from "rxjs";
import { catchError, map, tap } from 'rxjs/operators';

import { Bookmark } from "../models/bookmark";
import {Website} from "../models/website";
import {ToastService} from "./toast.service";

@Injectable({
  providedIn: 'root'
})
export class BookmarkService {
  private bookmarksUrl = 'api/bookmark';
  private httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private http: HttpClient, private toastService: ToastService) {}

  addBookmark(bookmark: Bookmark): Observable<Bookmark> {
    return this.http.post<Bookmark>(this.bookmarksUrl, bookmark, this.httpOptions).pipe(
      tap((newBookmark: Bookmark) => this.log(`Added bookmark w/ id=${newBookmark.id}`, LogLevel.Info)),
      catchError(this.handleError<Bookmark>('addBookmark'))
    );
  }

  deleteBookmark(id: number) : Observable<Bookmark> {
    const url = `${this.bookmarksUrl}/${id}`;

    return this.http.delete<Bookmark>(url, this.httpOptions).pipe(
      tap(_ => this.log(`Deleted bookmark w/ id=${id}`, LogLevel.Info)),
      catchError(this.handleError<Bookmark>('deleteBookmark'))
    );
  }

  getBookmarks(): Observable<Bookmark[]> {
    return this.http.get<Bookmark[]>(this.bookmarksUrl)
      .pipe(
        tap(_ => this.log('Fetched bookmarks', LogLevel.Debug)),
        catchError(this.handleError<Bookmark[]>('getBookmarks', []))
      );
  }

  getBookmark(id: number): Observable<Bookmark> {
    const url = `${this.bookmarksUrl}/${id}`;

    return this.http.get<Bookmark>(url).pipe(
      tap(_ => this.log(`Fetched bookmark id=${id}`, LogLevel.Debug)),
      catchError(this.handleError<Bookmark>(`getBookmark id=${id}`))
    );
  }

  getTags(): Observable<string[]> {
    const url = `${this.bookmarksUrl}/tags`;

    return this.http.get<string[]>(url)
      .pipe(
        tap(_ => this.log('Fetched tags', LogLevel.Debug)),
        catchError(this.handleError<string[]>('getTags', []))
      );
  }

  loadInfo(url: string): Observable<Website> {
    const apiUrl = `${this.bookmarksUrl}/info`;

    return this.http.post<Website>(apiUrl, url, this.httpOptions).pipe(
      tap(_ => this.log(`Fetched info for url ${url}`, LogLevel.Debug)),
      catchError(this.handleError<Website>(`loadInfo url=${url}`))
    );
  }

  updateBookmark(bookmark: Bookmark): Observable<any> {
    return this.http.put(this.bookmarksUrl, bookmark, this.httpOptions).pipe(
      tap(_ => this.log(`Updated bookmark w/ id=${bookmark.id}`, LogLevel.Info)),
      catchError(this.handleError<any>('updateBookmark'))
    );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      this.log(error, LogLevel.Error);
      return of(result as T);
    };
  }

  private log(message: string, level : LogLevel) {
    switch (level) {
      case LogLevel.Debug:
        console.debug(message);
        break;
      case LogLevel.Info:
        this.toastService.show(message, { classname: 'bg-info text-dark', delay: 5000 })
        console.info(message);
        break;
      case LogLevel.Error:
        this.toastService.show(message, { classname: 'bg-danger text-white', delay: 5000 })
        console.error(message);
        break;
      default:
        console.log(message);
        break;
    }
  }
}

enum LogLevel {
  Error = 1,
  Info,
  Debug
}
