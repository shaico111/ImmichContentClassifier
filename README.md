# ImmichContentClassifier

ImmichContentClassifier is a **local image classification tool** for Immich that analyzes media content using machine learning and optionally organizes assets into albums.

The project focuses on **content classification and moderation**, with strong emphasis on:
- Privacy-first, offline processing
- Explicit and safe automation
- Clear user control over side effects

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Safety First](#safety-first)
- [Basic Usage](#basic-usage)
- [Documentation](#documentation)
- [Future Direction](#future-direction)
- [Acknowledgements](#acknowledgements)
- [License](#license)


## Overview

The tool scans image thumbnails from an Immich instance, runs a local machine‑learning model to classify content, and can optionally organize assets into albums based on configurable rules.

It is designed to be:
- Understandable even without deep ML knowledge
- Safe to run in production environments
- Easy to extend in the future

---

## Key Features

- Local-only processing (no cloud, no uploads)
- Machine‑learning–based image classification
- Configurable thresholds and category filters
- Optional album organization
- Clear logging and dry‑run support

---

## Safety First

⚠️ **Important**

By default, the application **can modify Immich albums**.

For first-time use or testing, always start with:

```powershell
dotnet run -- --dry-run --date 2023-01-01
```

This ensures no data is modified.

---

## Basic Usage

Set your Immich API key:

```powershell
$env:IMMICH_API_KEY="YOUR_API_KEY"
```

Run a safe test:

```powershell
dotnet run -- --dry-run --date 2023-01-01
```

Run with album updates enabled:

```powershell
dotnet run -- --date 2023-01-01 --threshold 0.8
```

---

## Documentation

Additional documentation is available in this repository:

- **README_OPERATOR.md** – Practical usage guide
- **ARCHITECTURE.md** – System design overview
- **SECURITY.md** – Privacy and security considerations
- **CONTRIBUTING.md** – Contribution guidelines

---

## Future Direction

A key planned improvement is adding a **locally adaptive model** trained on confirmed correct and incorrect classifications.  
This will improve accuracy while keeping all data private and local.

---

## Acknowledgements

- Immich – self-hosted media management  
  https://github.com/immich-app/immich

- NsfwSpy.NET – image classification library  
  https://github.com/NsfwSpy/NsfwSpy.NET

---

## License

MIT License