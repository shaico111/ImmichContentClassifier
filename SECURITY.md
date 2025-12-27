# Security and Privacy

ImmichContentClassifier is designed to keep your data private.

---

## Table of Contents

- [Privacy guarantees](#privacy-guarantees)
- [API key safety](#api-key-safety)
- [Safe operation](#safe-operation)
- [Threat model](#threat-model)


## Privacy guarantees

- All image analysis is local
- No images are uploaded anywhere
- No telemetry or tracking

---

## API key safety

- Never commit API keys to Git
- Prefer environment variables
- Rotate keys if logs are shared

---

## Safe operation

- Always test with `--dry-run`
- Start with small date ranges
- Monitor system load

---

## Threat model

This tool assumes:
- A trusted local environment
- Authorized Immich access
- Non-malicious media content

If you need stronger isolation, consider Docker or sandboxing.
