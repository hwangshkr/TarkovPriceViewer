# Release flow

This project uses **a single source of truth** for the version: the `<Version>` element in `TarkovPriceViewer.csproj`.
The `.NET Desktop` workflow automatically builds the tag as `v{Version}` and decides whether it is a pre‑release or a stable release.

## Conventions

- Version in `TarkovPriceViewer.csproj` is **without the leading `v`**.
- **Pre‑release** examples: `1.35-beta.1`, `1.35-rc.1`, etc.  
  Generated tag: `v1.35-beta.1`.
- **Stable** examples: `1.35`, `1.35.1`, etc.  
  Generated tag: `v1.35`, `v1.35.1`.
- Workflow rules:
  - On `main` only **stable** versions are allowed (no `-...` suffix).
  - On branches other than `main` only **pre‑release** versions are allowed (with a `-...` suffix).

## Creating a pre‑release (branch != main)

1. Edit `TarkovPriceViewer.csproj` and set a version **with a suffix**:
   - Example: `<Version>1.35-beta.1</Version>`
2. Commit and push the changes on your feature branch (not `main`).
3. On GitHub:
   - Go to **Actions** → workflow **.NET Desktop**.
   - Run the workflow selecting **your branch**.
4. The workflow will:
   - Read the version (`1.35-beta.1`).
   - Ensure the branch is **not** `main`.
   - Compute the tag `v1.35-beta.1`.
   - Build, publish, and create a **GitHub pre‑release** with that tag.

## Creating a stable release (main)

1. On `main`, edit `TarkovPriceViewer.csproj` and set a version **without a suffix**:
   - Example: `<Version>1.35</Version>`
2. Commit and push to `main`.
3. On GitHub:
   - Go to **Actions** → workflow **.NET Desktop**.
   - Run the workflow selecting **main**.
4. The workflow will:
   - Read the version (`1.35`).
   - Ensure the branch is `main`.
   - Compute the tag `v1.35`.
   - Build, publish, and create a **GitHub stable release** with that tag.

## Auto bump on main

There is an additional workflow `auto-promote-version.yml` (**Auto bump pre-release version on main**) that runs on every push to `main`:

- If `<Version>` does **not** contain a pre-release suffix (e.g. `1.35`), it does nothing.
- If `<Version>` **does** contain a pre-release suffix (e.g. `1.35-beta.1`):
  - It derives the base version: `1.35`.
  - It replaces `<Version>1.35-beta.1</Version>` with `<Version>1.35</Version>` in `TarkovPriceViewer.csproj`.
  - It commits and pushes the bump: `chore: bump version 1.35-beta.1 -> 1.35`.

Typical flow for a stable release:

1. PR uses a pre-release version, for example `<Version>1.35-beta.1</Version>`.
2. PR is merged into `main` (now `main` has `1.35-beta.1`).
3. The **Auto bump** workflow runs on push to `main` and bumps it to `<Version>1.35</Version>`.
4. When you decide to publish, run the **.NET Desktop** workflow on `main` to create the `v1.35` stable release.

## Notes

- You do **not** need to create the tag manually: `softprops/action-gh-release` creates the `v{Version}` tag if it does not exist.
- If the version and branch do not respect the rules (pre‑release on `main` or stable outside `main`), the workflow will fail with a clear error message.
