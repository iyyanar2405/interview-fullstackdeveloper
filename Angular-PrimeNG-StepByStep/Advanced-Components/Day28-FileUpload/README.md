# Day 28: FileUpload and Images

Handle files like a pro using PrimeNG FileUpload. You’ll implement single/multiple uploads, drag & drop, auto/manual modes, file type/size restrictions, custom upload handlers, and progress UX.

Note: You need an API endpoint for real uploads. This lesson includes a mock handler pattern you can adapt.

## What you’ll build
- Drag & drop multi-upload with preview and progress
- Restricted uploads (type/size)
- Custom HTTP upload with interceptor

## Setup

```ts
import { FileUploadModule } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { ImageModule } from 'primeng/image';
```

## 1) Basic Upload (auto)

```html
<p-fileUpload mode="advanced"
             name="files[]"
             url="/api/upload"
             [auto]="true"
             accept="image/*"
             maxFileSize="1000000">
</p-fileUpload>
```

- auto: starts upload immediately
- accept: only images
- maxFileSize: 1MB

## 2) Advanced with Templates

```ts
// image-uploader.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FileUploadModule, FileUpload } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ImageModule } from 'primeng/image';

@Component({
  selector: 'app-image-uploader',
  standalone: true,
  imports: [CommonModule, FileUploadModule, ToastModule, ButtonModule, ImageModule],
  providers: [MessageService],
  templateUrl: './image-uploader.component.html',
  styleUrls: ['./image-uploader.component.scss']
})
export class ImageUploaderComponent {
  uploadedFiles: any[] = [];

  onUpload(event: any) {
    for (const file of event.files) {
      this.uploadedFiles.push({ name: file.name, objectURL: URL.createObjectURL(file) });
    }
  }

  onClear(uploader: FileUpload) {
    uploader.clear();
    this.uploadedFiles = [];
  }
}
```

```html
<!-- image-uploader.component.html -->
<div class="card">
  <h3>Drag & Drop Images</h3>
  <p-fileUpload #uploader
                mode="advanced"
                name="images[]"
                url="/api/upload"
                [multiple]="true"
                accept="image/*"
                [maxFileSize]="2000000"
                chooseLabel="Select"
                uploadLabel="Upload"
                cancelLabel="Cancel"
                dragDropSupport="true"
                [showUploadButton]="true"
                [showCancelButton]="true"
                (onUpload)="onUpload($event)">

    <ng-template pTemplate="header">
      <div class="p-d-flex p-ai-center p-jc-between" style="width:100%">
        <div>Drop files here or click to browse</div>
        <p-button label="Clear" icon="pi pi-trash" class="p-button-text" (onClick)="onClear(uploader)"></p-button>
      </div>
    </ng-template>

    <ng-template pTemplate="content">
      <div class="previews" *ngIf="uploadedFiles.length">
        <div class="preview" *ngFor="let f of uploadedFiles">
          <p-image [src]="f.objectURL" alt="{{f.name}}" [preview]="true" styleClass="thumb"></p-image>
          <div class="name">{{f.name}}</div>
        </div>
      </div>
    </ng-template>

    <ng-template pTemplate="empty">
      <div class="empty">No files selected.</div>
    </ng-template>
  </p-fileUpload>
</div>
```

```scss
/* image-uploader.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.previews { display:grid; grid-template-columns: repeat(auto-fill, minmax(160px, 1fr)); gap:.75rem; margin-top:.75rem; }
.preview { display:flex; flex-direction:column; align-items:center; gap:.25rem; }
.thumb { width: 100%; height: 120px; object-fit:cover; }
.empty { color:#64748b; padding:.5rem; }
```

## 3) Custom Upload Handler (manual)

Use manual mode to integrate your own HTTP logic.

```html
<p-fileUpload #uploader mode="basic" chooseLabel="Select" [auto]="false" [customUpload]="true" (uploadHandler)="myUploader($event)"></p-fileUpload>
```

```ts
myUploader(event: { files: File[] }) {
  const form = new FormData();
  for (const file of event.files) { form.append('files', file, file.name); }
  // Use HttpClient to post to your server
  // this.http.post('/api/upload', form).subscribe(...)
}
```

## 4) Validation: Type & Size

```html
<p-fileUpload accept=".pdf,.docx" maxFileSize="5000000"></p-fileUpload>
```

## 5) Server Notes

- Expect multipart/form-data
- For large files, support chunking and resume where possible
- Return metadata (URL, size, type) for UI

## Exercises
- Add progress indicator using HttpClient reportProgress
- Restrict total number of files to 5 and show error toast if exceeded
- Add image compression (client-side) before upload

## Summary
You can now build robust upload flows with PrimeNG. Next: Day 29 - VirtualScroller for high-performance long lists.