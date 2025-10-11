# Day 27 — FileUpload & Image

## Objectives
- Use PrimeReact FileUpload for single/multiple files
- Implement custom upload handler and progress
- Display images with Image component and preview

## FileUpload Basics
```tsx
function FileUploadBasic() {
  return (
    <div className="card">
      <h5>Basic</h5>
      <FileUpload name="files[]" url="/api/upload" multiple accept="image/*" maxFileSize={1000000} />
    </div>
  );
}
```

## Custom Upload Handler (no server)
```tsx
function FileUploadCustom() {
  const [totalSize, setTotalSize] = useState(0);
  const toast = useRef<Toast>(null);

  const onTemplateSelect = (e: FileUploadSelectEvent) => {
    let _totalSize = totalSize;
    const files = e.files as File[];
    files.forEach((file) => { _totalSize += file.size || 0; });
    setTotalSize(_totalSize);
  };

  const uploadHandler = async (e: FileUploadHandlerEvent) => {
    // simulate upload delay
    await new Promise((r) => setTimeout(r, 1000));
    toast.current?.show({ severity: 'success', summary: 'Uploaded', detail: `${e.files.length} file(s) uploaded`, life: 3000 });
    e.options.clear();
    setTotalSize(0);
  };

  return (
    <div className="card">
      <Toast ref={toast} />
      <FileUpload 
        name="demo[]" 
        customUpload 
        uploadHandler={uploadHandler} 
        onSelect={onTemplateSelect}
        multiple 
        accept="image/*" 
        maxFileSize={2000000}
        emptyTemplate={<p className="m-0">Drag and drop files here to upload.</p>} 
        chooseLabel="Choose" uploadLabel="Upload" cancelLabel="Clear"
      />
      <div className="mt-3">Total Size: {(totalSize/1024).toFixed(1)} KB</div>
    </div>
  );
}
```

## Image Preview
```tsx
function ImagePreview() {
  return (
    <div className="card grid">
      <div className="col-12 md:col-4">
        <h5>Basic</h5>
        <Image src="/images/samples/nature1.jpg" alt="Nature" width="250" />
      </div>
      <div className="col-12 md:col-4">
        <h5>Preview</h5>
        <Image src="/images/samples/nature2.jpg" alt="Nature" width="250" preview />
      </div>
      <div className="col-12 md:col-4">
        <h5>Custom Indicator</h5>
        <Image src="/images/samples/nature3.jpg" alt="Nature" width="250" preview indicatorIcon="pi pi-search" />
      </div>
    </div>
  );
}
```

## Exercises
- Build an avatar uploader with client-side validation (size/type)
- Implement drag & drop multi-upload with thumbnails
- Show upload progress and error handling messages

## Checklist
- [ ] Multiple file selection works
- [ ] Custom upload handler shows Toast
- [ ] Image preview works with zoom
- [ ] Validation for size and type in place