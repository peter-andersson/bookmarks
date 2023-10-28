import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { BookmarksComponent } from "./bookmarks/bookmarks.component";
import { DeleteComponent } from "./delete/delete.component";
import { AddComponent } from "./add/add.component";
import { EditComponent } from "./edit/edit.component";
import { BookmarkEditorComponent } from "./bookmark-editor/bookmark-editor.component";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    BookmarksComponent,
    DeleteComponent,
    EditComponent,
    AddComponent,
    BookmarkEditorComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: BookmarksComponent, pathMatch: 'full' },
      { path: 'add', component: AddComponent },
      { path: 'delete/:id', component: DeleteComponent },
      { path: 'edit/:id', component: EditComponent },
    ]),
    NgbModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
