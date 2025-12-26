# ImmichContentClassifier – Operator Guide

This guide is for users who **run the tool**, not developers.

---

## Table of Contents

- [Purpose](#purpose)
- [Safe First Run (Recommended)](#safe-first-run-recommended)
- [⚠️ Operational Warning](#operational-warning)
- [Required Setup](#required-setup)
  - [Immich API Key](#immich-api-key)
- [Common Commands](#common-commands)
  - [Scan one day](#scan-one-day)
  - [Scan a date range](#scan-a-date-range)
  - [Scan a single asset](#scan-a-single-asset)
- [Important Flags](#important-flags)
  - [Safety](#safety)
  - [Classification](#classification)
  - [Performance](#performance)
  - [Logging](#logging)
- [Recommended Workflow](#recommended-workflow)
- [Notes](#notes)


## Purpose

ImmichContentClassifier helps you analyze and organize images in Immich using local machine‑learning classification.

---

## Safe First Run (Recommended)

```powershell
dotnet run -- --dry-run --date 2023-01-01 --log-file logs/test.log
```

No albums will be modified.

---

## ⚠️ Operational Warning

Running without `--dry-run` **will modify Immich albums**.

Always test on a small date range before large scans.

---

## Required Setup

### Immich API Key

```powershell
$env:IMMICH_API_KEY="YOUR_API_KEY"
```

---

## Common Commands

### Scan one day
```powershell
dotnet run -- --date 2023-01-01
```

### Scan a date range
```powershell
dotnet run -- --start-date 2023-01-01 --end-date 2023-01-31
```

### Scan a single asset
```powershell
dotnet run -- --asset-id <ASSET_ID>
```

---

## Important Flags

### Safety
- `--dry-run` – No album changes
- `--no-album` – Disable album updates entirely

### Classification
- `--threshold 0.70` – Classification cutoff
- `--ignore-porn`
- `--ignore-sexy`
- `--ignore-hentai`
- `--ignore-neutral`

### Performance
- `--parallelism N` – Control CPU usage

### Logging
- `--log-file <path>`
- `--log-level Info|Debug|Warn|Error`
- `--no-console`

---

## Recommended Workflow

1. Run with `--dry-run`
2. Review logs
3. Adjust threshold and filters
4. Run without `--dry-run`
5. Review album results

---

## Notes

- All processing is local
- CUDA warnings are normal on CPU-only systems
- Start small before scanning large libraries