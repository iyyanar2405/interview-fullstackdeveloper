# Day 25 — Carousel & Galleria

## Objectives
- Image carousels and galleries
- Media display components
- Navigation and thumbnails

## Carousel Component
```tsx
function CarouselDemo() {
  const [products, setProducts] = useState([]);
  const responsiveOptions = [
    {
      breakpoint: '1199px',
      numVisible: 1,
      numScroll: 1
    },
    {
      breakpoint: '991px',
      numVisible: 2,
      numScroll: 1
    },
    {
      breakpoint: '767px',
      numVisible: 1,
      numScroll: 1
    }
  ];

  const productTemplate = (product) => {
    return (
      <div className="border-1 surface-border border-round m-2 text-center py-5 px-3">
        <div className="mb-3">
          <img src={`images/product/${product.image}`} alt={product.name} className="w-6 shadow-2" />
        </div>
        <div>
          <h4 className="mb-1">{product.name}</h4>
          <h6 className="mt-0 mb-3">${product.price}</h6>
          <Tag value={product.inventoryStatus} severity={getSeverity(product)} />
          <div className="mt-5 flex flex-wrap gap-2 justify-content-center">
            <Button icon="pi pi-search" className="p-button p-button-rounded" />
            <Button icon="pi pi-star-fill" className="p-button-success p-button-rounded" />
          </div>
        </div>
      </div>
    );
  };

  return (
    <div className="card">
      <Carousel 
        value={products} 
        numVisible={3} 
        numScroll={3} 
        responsiveOptions={responsiveOptions} 
        itemTemplate={productTemplate} 
        header={<h5>Basic</h5>}
      />
    </div>
  );
}
```

## Galleria Component
```tsx
function GalleriaDemo() {
  const [images, setImages] = useState([]);

  useEffect(() => {
    const galleriaImages = [
      {
        itemImageSrc: 'demo/images/galleria/galleria1.jpg',
        thumbnailImageSrc: 'demo/images/galleria/galleria1s.jpg',
        alt: 'Description for Image 1',
        title: 'Title 1'
      },
      {
        itemImageSrc: 'demo/images/galleria/galleria2.jpg',
        thumbnailImageSrc: 'demo/images/galleria/galleria2s.jpg',
        alt: 'Description for Image 2',
        title: 'Title 2'
      }
      // ... more images
    ];
    setImages(galleriaImages);
  }, []);

  const itemTemplate = (item) => {
    return <img src={item.itemImageSrc} alt={item.alt} style={{ width: '100%' }} />;
  };

  const thumbnailTemplate = (item) => {
    return <img src={item.thumbnailImageSrc} alt={item.alt} />;
  };

  return (
    <div className="card">
      <Galleria 
        value={images} 
        responsiveOptions={responsiveOptions} 
        numVisible={5} 
        style={{ maxWidth: '640px' }}
        item={itemTemplate} 
        thumbnail={thumbnailTemplate} 
      />
    </div>
  );
}
```

## Advanced Galleria Features
```tsx
function AdvancedGalleria() {
  const [images, setImages] = useState([]);
  const [activeIndex, setActiveIndex] = useState(0);
  const [showThumbnails, setShowThumbnails] = useState(true);

  const galleriaService = new PhotoService();

  useEffect(() => {
    galleriaService.getImages().then(data => setImages(data));
  }, []);

  const itemTemplate = (item) => {
    return (
      <img 
        src={item.itemImageSrc} 
        alt={item.alt} 
        style={{ width: '100%', display: 'block' }} 
      />
    );
  };

  const thumbnailTemplate = (item) => {
    return (
      <div className="grid grid-nogutter justify-content-center">
        <img src={item.thumbnailImageSrc} alt={item.alt} style={{ display: 'block' }} />
      </div>
    );
  };

  const caption = (item) => {
    return (
      <React.Fragment>
        <h4 style={{ marginBottom: '.5rem', color: '#ffffff' }}>{item.title}</h4>
        <p style={{ color: '#ffffff' }}>{item.alt}</p>
      </React.Fragment>
    );
  };

  return (
    <div className="card">
      <div className="flex align-items-center justify-content-around mb-5 gap-2">
        <InputSwitch 
          checked={showThumbnails} 
          onChange={(e) => setShowThumbnails(e.value)} 
        />
        <span>Show Thumbnails</span>
      </div>
      
      <Galleria 
        value={images} 
        activeIndex={activeIndex} 
        onItemChange={(e) => setActiveIndex(e.index)}
        responsiveOptions={responsiveOptions} 
        numVisible={7} 
        style={{ maxWidth: '800px' }} 
        circular 
        autoPlay 
        transitionInterval={3000}
        showThumbnails={showThumbnails}
        showItemNavigators 
        showItemNavigatorsOnHover
        item={itemTemplate} 
        thumbnail={thumbnailTemplate}
        caption={caption}
      />
    </div>
  );
}
```

## Image Component
```tsx
function ImageDemo() {
  return (
    <div className="card">
      <div className="grid">
        <div className="col-12 md:col-4">
          <h5>Basic</h5>
          <Image src="demo/images/galleria/galleria1.jpg" alt="Image" width="250" />
        </div>
        <div className="col-12 md:col-4">
          <h5>Preview</h5>
          <Image src="demo/images/galleria/galleria1.jpg" alt="Image" width="250" preview />
        </div>
        <div className="col-12 md:col-4">
          <h5>Custom Preview Icon</h5>
          <Image 
            src="demo/images/galleria/galleria1.jpg" 
            alt="Image" 
            width="250" 
            preview
            indicatorIcon="pi pi-search"
          />
        </div>
      </div>
    </div>
  );
}
```

## Exercise
- Create a product showcase carousel
- Build an image gallery with preview functionality
- Add custom navigation and indicators

## Checklist
- [ ] Carousel displays multiple items
- [ ] Galleria shows images with thumbnails
- [ ] Image preview functionality works
- [ ] Responsive options applied correctly