import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BookmarkEditorComponent } from './bookmark-editor.component';

describe('BookmarkEditorComponent', () => {
  let component: BookmarkEditorComponent;
  let fixture: ComponentFixture<BookmarkEditorComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BookmarkEditorComponent]
    });
    fixture = TestBed.createComponent(BookmarkEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
