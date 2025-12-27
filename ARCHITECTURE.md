# Architecture Overview

This document explains how ImmichContentClassifier is structured.

---

## Table of Contents

- [Design goals](#design-goals)
- [High-level flow](#high-level-flow)
- [Main components](#main-components)
  - [Program.cs](#program-cs)
  - [Core](#core)
  - [Immich](#immich)
  - [Models](#models)
  - [Utils](#utils)
  - [Logging](#logging)
- [Why reflection is used](#why-reflection-is-used)
- [Extensibility](#extensibility)


## Design goals

- Easy to understand
- Clear separation of responsibilities
- No hidden global state
- Safe side effects

---

## High-level flow

```
CLI Arguments
      ↓
AppOptions
      ↓
Asset Selection
      ↓
Thumbnail Download
      ↓
NSFW Classification
      ↓
Decision
      ↓
Optional Album Update
```

---

## Main components

### Program.cs
Controls the application flow and connects all components.

### Core
Handles:
- CLI parsing
- Date filtering
- Execution control
- Counters

### Immich
Responsible only for communication with the Immich API.

### Models
Simple data objects (no logic).

### Utils
Helper functions (image conversion, normalization).

### Logging
Centralized logging and error handling.

---

## Why reflection is used

NsfwSpy output is accessed via reflection to avoid tight coupling.
This allows easier upgrades if the model output changes.

---

## Extensibility

The architecture allows:
- Adding new classifiers
- Adding a second-stage local model
- Running inside Docker
- Optional GPU support
