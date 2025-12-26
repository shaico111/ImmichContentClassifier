# ImmichContentClassifier Operator Guide

This guide is for running the tool.

## Purpose

Analyze and optionally organize media using local classification.

Assets in the Private area are never archived and are not moved to the Locked folder.

## First run

Always start with dry run:

dotnet run -- --dry-run --date 2023-01-01

## Setup

Set API key:

$env:IMMICH_API_KEY="YOUR_API_KEY"

## Common usage

Single day:

dotnet run -- --date 2023-01-01

Date range:

dotnet run -- --start-date 2023-01-01 --end-date 2023-01-31

Single asset:

dotnet run -- --asset-id <ASSET_ID>

## Controls

--dry-run  
--no-album  
--threshold  
--parallelism  

## Workflow

Dry run  
Review logs  
Adjust options  
Run without dry run  
Review results  
