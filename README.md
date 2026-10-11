# Snowdin Tag (beta, untested source)

A Gorilla Tag x Undertale mashup for [Melty](https://melty.gg).

- **Host:** Gorilla Tag (BepInEx plugin)
- **Companion:** Undertale. Its music is read from the player's own install at runtime. Nothing from Undertale is copied or shipped in this repo.

## What players get
Tag plays like an Undertale encounter: the music and on-screen lines change when a round starts, when you're tagged, and when it ends. Beta is audio and text only.

## Status
Source only. Not compiled, not tested, not published. Gorilla Tag type names and Undertale filenames are unverified (see `sheets/`).

## Layout
- `src/` plugin source
- `sheets/` design sheets (source of truth)
- `tools/preflight.py` lists unfilled or unverified cells (`python3 -I tools/preflight.py`)
- `melty.draft.json` draft Melty recipe (not validated; rename to `melty.json` once validated)
- `lib/` put Gorilla Tag's `Assembly-CSharp.dll` and BepInEx/Unity references here to build (not committed)
- `docs/` notes

## Build
Install the .NET SDK, add the references to `lib/`, then `dotnet build -c Release`.

## Notes
- Intended for private modded lobbies only, not public matchmaking.
- License and remix permissions: not chosen yet. Add a LICENSE before publishing.
- Not affiliated with Undertale or Gorilla Tag.
