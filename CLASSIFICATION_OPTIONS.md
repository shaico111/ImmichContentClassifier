# Classification Options

This document explains classification behavior and tuning.

## Table of Contents

- [Decision logic](#decision-logic)
- [Category filters](#category-filters)
- [Examples](#examples)
- [Notes](#notes)


## Decision logic

An image is flagged when the highest relevant score meets or exceeds the threshold.

Default threshold is 0.70

## Category filters

--ignore-porn  
--ignore-sexy  
--ignore-hentai  
--ignore-neutral  

Ignored categories are excluded from evaluation.

## Examples

Ignore animated content:

dotnet run -- --ignore-hentai --date 2023-01-01

Stricter filtering:

dotnet run -- --threshold 0.85 --date 2023-01-01

## Notes

Threshold tuning is expected  
Always validate with --dry-run  