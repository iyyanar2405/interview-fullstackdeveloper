# Day 26: Carousel and Galleria - Rich Media Browsing

Today we’ll build immersive image/content sliders using Carousel and Galleria. You’ll implement responsive carousels, custom templates, thumbnails, full-screen lightbox, and lazy loading.

## What you’ll build
- Product promo carousel with custom item templates
- Responsive carousel with autoplay, circular, and indicators
- Galleria with thumbnails, fullscreen, and filmstrip modes

## Prerequisites
Install modules: `CarouselModule`, `GalleriaModule`, `ButtonModule`, `TagModule`, `ImageModule`.

## 1) Carousel: Product promos

```ts
// product-carousel.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CarouselModule } from 'primeng/carousel';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';

interface PromoItem {
  title: string;
  subtitle: string;
  image: string;
  price: number;
  badge?: string;
}

@Component({
  selector: 'app-product-carousel',
  standalone: true,
  imports: [CommonModule, CarouselModule, ButtonModule, TagModule],
  templateUrl: './product-carousel.component.html',
  styleUrls: ['./product-carousel.component.scss']
})
export class ProductCarouselComponent implements OnInit {
  promos: PromoItem[] = [];
  responsiveOptions = [
    { breakpoint: '1199px', numVisible: 3, numScroll: 3 },
    { breakpoint: '991px', numVisible: 2, numScroll: 2 },
    { breakpoint: '767px', numVisible: 1, numScroll: 1 }
  ];

  ngOnInit() {
    this.promos = Array.from({ length: 8 }, (_, i) => ({
      title: `Super Gadget ${i+1}`,
      subtitle: 'Limited time offer',
      image: `https://picsum.photos/seed/car${i}/800/500`,
      price: Math.floor(Math.random()*900)+99,
      badge: i % 3 === 0 ? 'Hot' : undefined
    }));
  }
}
```

```html
<!-- product-carousel.component.html -->
<div class="card">
  <h3>Featured Deals</h3>
  <p-carousel 
    [value]="promos"
    [numVisible]="3"
    [numScroll]="3"
    [circular]="true"
    [autoplayInterval]="4000"
    [responsiveOptions]="responsiveOptions"
    [showIndicators]="true"
    [showNavigators]="true">

    <ng-template pTemplate="item" let-item>
      <div class="promo">
        <img [src]="item.image" alt="{{item.title}}" />
        <div class="content">
          <div class="top">
            <div class="title">{{item.title}}</div>
            <p-tag *ngIf="item.badge" [value]="item.badge" severity="warning"></p-tag>
          </div>
          <div class="subtitle">{{item.subtitle}}</div>
          <div class="bottom">
            <div class="price">{{item.price | currency:'USD'}}</div>
            <div class="actions">
              <button pButton icon="pi pi-shopping-cart" label="Buy"></button>
              <button pButton icon="pi pi-info" class="p-button-text"></button>
            </div>
          </div>
        </div>
      </div>
    </ng-template>

    <ng-template pTemplate="header">
      <div class="header">Swipe to explore</div>
    </ng-template>

    <ng-template pTemplate="footer">
      <div class="footer">Best prices this week</div>
    </ng-template>
  </p-carousel>
</div>
```

```scss
/* product-carousel.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.promo { display:flex; flex-direction: column; border:1px solid #e2e8f0; border-radius:8px; overflow:hidden; }
.promo img { width:100%; height:220px; object-fit:cover; }
.content { padding:.75rem; display:flex; flex-direction:column; gap:.5rem; }
.top { display:flex; align-items:center; justify-content:space-between; }
.title { font-weight:600; }
.subtitle { color:#64748b; font-size:.9rem; }
.bottom { display:flex; align-items:center; justify-content:space-between; }
.price { font-weight:700; }
.actions .p-button { margin-left:.5rem; }
.header,.footer { text-align:center; color:#64748b; padding:.5rem 0; }
```

## 2) Galleria: Thumbnails & fullscreen

```ts
// gallery.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GalleriaModule } from 'primeng/galleria';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [CommonModule, GalleriaModule, ButtonModule],
  templateUrl: './gallery.component.html',
  styleUrls: ['./gallery.component.scss']
})
export class GalleryComponent implements OnInit {
  images: any[] = [];
  displayCustom = false;
  activeIndex = 0;

  ngOnInit() {
    this.images = Array.from({ length: 9 }, (_, i) => ({
      itemImageSrc: `https://picsum.photos/seed/g${i}/1200/800`,
      thumbnailImageSrc: `https://picsum.photos/seed/g${i}/200/120`,
      alt: `Image ${i+1}`,
      title: `Beautiful Shot #${i+1}`
    }));
  }

  openGallery(index: number) {
    this.activeIndex = index;
    this.displayCustom = true;
  }
}
```

```html
<!-- gallery.component.html -->
<div class="card">
  <h3>Travel Gallery</h3>

  <div class="thumbs">
    <div class="thumb" *ngFor="let img of images; let i = index" (click)="openGallery(i)">
      <img [src]="img.thumbnailImageSrc" [alt]="img.alt" />
      <div class="title">{{img.title}}</div>
    </div>
  </div>

  <p-galleria 
    [(visible)]="displayCustom"
    [value]="images"
    [responsiveOptions]="[{breakpoint:'991px', numVisible:5}, {breakpoint:'767px', numVisible:3}, {breakpoint:'575px', numVisible:1}]"
    [numVisible]="7"
    [circular]="true"
    [fullScreen]="true"
    [showThumbnails]="true"
    [showItemNavigators]="true"
    [showIndicators]="false"
    [showItemNavigatorsOnHover]="true"
    [containerStyle]="{ 'max-width': '1000px' }"
    [activeIndex]="activeIndex">

    <ng-template pTemplate="item" let-item>
      <img [src]="item.itemImageSrc" [alt]="item.alt" style="width: 100%; display: block;" />
    </ng-template>

    <ng-template pTemplate="thumbnail" let-item>
      <div class="thumb-cell">
        <img [src]="item.thumbnailImageSrc" [alt]="item.alt" />
      </div>
    </ng-template>
  </p-galleria>
</div>
```

```scss
/* gallery.component.scss */
.card { padding:1rem; background:#fff; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,.06); }
.thumbs { display:grid; grid-template-columns: repeat(auto-fill, minmax(160px, 1fr)); gap:.75rem; }
.thumb { border:1px solid #e2e8f0; border-radius:8px; overflow:hidden; cursor:pointer; background:#f8fafc; }
.thumb img { width:100%; height:120px; object-fit:cover; display:block; }
.thumb .title { padding:.5rem; font-size:.9rem; color:#475569; }
.thumb-cell img { width:100%; height:100%; object-fit:cover; }
```

## Tips
- Use `lazyLoad` on images for performance
- Keep `numVisible` lower on mobile for better UX
- Prefer Galleria for image-first experiences; Carousel for mixed content

## Exercises
- Add autoplay to Galleria and a custom caption
- Add sale badge overlay to carousel items on discount
- Implement keyboard navigation and loop boundaries

## Summary
You now can create rich, responsive media browsers with Carousel and Galleria. Next: Day 27 - Charts integration for interactive analytics.