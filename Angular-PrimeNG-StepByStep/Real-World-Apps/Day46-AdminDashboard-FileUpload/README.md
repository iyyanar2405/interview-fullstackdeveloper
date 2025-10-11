# Day 46 — Admin Dashboard: File Uploads

Objectives
- Integrate FileUpload for product images, CSV imports, and avatar updates.
- Use custom upload handler to send to backend and validate file types/sizes.
- Show previews and progress; handle errors gracefully.

Components
- FileUpload, Image, Toast

Product image upload (template mode)
```ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FileUploadModule } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-product-image-upload',
  standalone: true,
  imports: [CommonModule, FileUploadModule, ToastModule],
  providers: [MessageService],
  templateUrl: './product-image-upload.component.html'
})
export class ProductImageUploadComponent {
  uploadedFiles: any[] = [];
  onUpload(event: any) {
    for (let file of event.files) { this.uploadedFiles.push(file); }
  }
  customUpload(event: any) {
    const file: File = event.files[0];
    if (!file.type.startsWith('image/')) { event.options.clear(); return; }
    setTimeout(() => { event.options.clear(); }, 600); // simulate success
  }
}
```

Template
```html
<p-toast></p-toast>
<p-fileUpload name="images[]" url="/api/upload" mode="advanced" [auto]="false" [customUpload]="true" (uploadHandler)="customUpload($event)" accept="image/*" maxFileSize="2000000"></p-fileUpload>
```

CSV import
- Accept .csv and post to `/api/users/import`.
- Parse server response; show toasts for inserted/failed rows.

Exercises
1) Restrict to 3 images per product and show thumbnails.
2) Add drag-and-drop area with image preview grid.
3) Show progress bar per file and retry on failure.
