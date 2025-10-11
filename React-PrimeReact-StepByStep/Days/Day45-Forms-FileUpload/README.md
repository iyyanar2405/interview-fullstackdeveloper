# Day 45 — Forms + File Upload

## Objectives
- Combine complex forms with FileUpload
- Manage uploaded file metadata in form state
- Validate file type/size and show previews

## Form with FileUpload
```tsx
type Product = { name: string; price: number; image?: File | null };

export default function ProductForm() {
  const { control, register, setValue, handleSubmit, formState: { errors } } = useForm<Product>({ defaultValues: { name: '', price: 0, image: null } });

  const onSelect = (e: FileUploadSelectEvent) => {
    const file = (e.files as File[])[0];
    if (file && file.size < 1024 * 1024 && file.type.startsWith('image/')) setValue('image', file);
  };

  const onSubmit = (v: Product) => {
    const fd = new FormData();
    fd.append('name', v.name);
    fd.append('price', String(v.price));
    if (v.image) fd.append('image', v.image);
    // post fd to API
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid gap-3">
      <span className="p-float-label">
        <InputText id="name" {...register('name', { required: 'Required' })} className={errors.name && 'p-invalid'} />
        <label htmlFor="name">Name</label>
      </span>
      {errors.name && <small className="p-error">{errors.name.message}</small>}

      <div>
        <label className="block mb-1">Price</label>
        <Controller name="price" control={control} render={({ field }) => <InputNumber {...field} mode="currency" currency="USD" />} />
      </div>

      <div>
        <label className="block mb-1">Image</label>
        <FileUpload name="image" mode="basic" accept="image/*" chooseLabel="Choose" customUpload onSelect={onSelect} />
      </div>

      <Button type="submit" label="Save" />
    </form>
  );
}
```

## Preview
```tsx
function ImagePreview({ file }: { file?: File | null }) {
  const [src, setSrc] = useState<string | null>(null);
  useEffect(() => {
    if (!file) return setSrc(null);
    const url = URL.createObjectURL(file);
    setSrc(url);
    return () => URL.revokeObjectURL(url);
  }, [file]);
  return src ? <img src={src} alt="preview" className="w-12rem" /> : null;
}
```

## Checklist
- [ ] File added to form state
- [ ] Size/type validation applied
- [ ] Preview shows when selected
- [ ] FormData contains file