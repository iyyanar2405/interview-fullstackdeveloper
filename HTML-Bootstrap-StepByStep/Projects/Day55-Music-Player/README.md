# Day 55: Music Player Interface 🎵
Music player with playlists, controls, album art.
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>MusicPlay</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.2/font/bootstrap-icons.css" rel="stylesheet">
</head>
<body class="bg-dark text-white">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-3 bg-black p-3">
                <h5>Playlists</h5>
                <div class="list-group"><a href="#" class="list-group-item list-group-item-dark">Favorites</a></div>
            </div>
            <div class="col-md-9 p-4">
                <div class="text-center mb-4">
                    <img src="https://via.placeholder.com/300" class="rounded" alt="Album">
                    <h3 class="mt-3">Song Title</h3>
                    <p class="text-muted">Artist Name</p>
                </div>
                <div class="text-center">
                    <button class="btn btn-outline-light btn-lg rounded-circle"><i class="bi bi-skip-backward"></i></button>
                    <button class="btn btn-light btn-lg rounded-circle mx-3"><i class="bi bi-play-fill"></i></button>
                    <button class="btn btn-outline-light btn-lg rounded-circle"><i class="bi bi-skip-forward"></i></button>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
```
**Next: Day 56 - Final Capstone** 🎓
