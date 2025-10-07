# 15 — AWS Deployment (Nx Angular)

Deploy an Angular app built with Nx on AWS using S3 + CloudFront or Amplify.

## 1) S3 + CloudFront (CDN)
- Create an S3 bucket (static website hosting)
- Build the app and upload to S3

```powershell
npx nx build web --configuration=production
aws s3 sync dist/apps/web s3://<bucket-name> --delete
```

- Create a CloudFront distribution with the S3 origin
- Set default root object to `index.html`
- Add a behavior for SPA rewrites: if 404, return `index.html` (Function@Edge/Lambda@Edge or CloudFront default root)

Invalidate cache on deploy:
```powershell
aws cloudfront create-invalidation --distribution-id <ID> --paths "/*"
```

## 2) AWS Amplify Hosting (managed CI/CD)
- Connect your GitHub repo in Amplify console
- Set build command and output dir: `dist/apps/web`
- Amplify handles CDN, SSL, and deploy pipeline

## Tips
- Use hashed filenames for long-term caching (Angular default)
- Keep SPA fallback configured so deep links work
- For multi-app Nx, ensure the correct app output path
