# Immich Content Classifier

A local tool that analyzes and organizes media in an Immich instance using machine learning.

The tool runs fully on your local machine. It processes thumbnails only and does not upload images or metadata to external services.

Assets located in Immich's Private area are never archived or moved to the Locked folder.

## How it works

The application connects to the Immich API, selects assets based on filters, downloads low resolution thumbnails, and evaluates them using a local model.

Based on the results, it can log findings or optionally organize assets into albums.

## Getting started

Safety note:
This tool can modify albums. Always start with a dry run.

### Example

Set your API key:

$env:IMMICH_API_KEY="YOUR_API_KEY"

Dry run:

dotnet run -- --dry-run --date 2023-01-01

Apply changes:

dotnet run -- --date 2023-01-01 --threshold 0.8

## Documentation

README_OPERATOR.md  
CLASSIFICATION_OPTIONS.md  
ARCHITECTURE.md  
SECURITY.md  
CONTRIBUTING.md  

## License

MIT License
