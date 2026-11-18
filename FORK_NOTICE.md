# Fork notice

Personal fork of **WishlistExtended** 1.2.0 by ZGFueDkx (danx91).

- Upstream: https://github.com/danx91/WishlistExtended (tag v1.2.0)
- License: GPL-3.0 (see `LICENSE`); this fork stays GPL-3.0 with its source available.
- Build dependency: https://github.com/danx91/SPT-ZGFueDkxCommonLibrary (no license file upstream), cloned as a sibling folder and compiled in, as upstream's README describes. It is not modified or redistributed here.
- Branch `master` mirrors upstream; work lives on `fir-hideout`.
- AI use: code in this fork is written with Claude (Claude Code) assistance. Do not send it upstream as-is.

## What this fork adds

- **Only found in raid items wishlisted** for hideout upgrades and for quests (useful with Hideout Backport, which drops FiR from many requirements), also future quests (from the AllQuestsCheckmarks fork). The wishlist then marks the item type, so any copy gets marked.
- Hideout and quest items go to the Other wishlist group.
- **Hideout data independent of the wishlist settings:** every upgrade stage is cached, the wishlist filters by its own mode. A small public API (`WishlistExtendedApi`) gives other mods the hideout needs; the AllQuestsCheckmarks fork builds its overlays and tooltip from it.
- Tooltip: hideout needs for items that are not wishlisted, labelled FiR / non-FiR, and an option to leave hideout and quest lines to the AllQuestsCheckmarks fork when it shows them.
- The quest totals were not cleared when the quest cache was rebuilt (upstream); they are now.

## Where the changes are (for merging upstream)

New files: `WishlistExtendedApi`, `CheckmarksBridge`, `SettingsFork`. Upstream files with edits: `WishlistResolver.cs`, `TooltipManager.cs`, `QuestDataService.cs`, `HideoutDataService.cs`, `Plugin.cs`, locales, and `Settings.cs` (three lines: `partial` and a call to `InitFork`).
Settings: upstream's sections are unchanged; ours are "5. Hideout and quests" and "6. Tooltip additions".

Build changes: csproj uses `TarkovDir` (from an uncommitted `.csproj.user`) instead of assuming the repo sits in the game folder; deploys the DLL and the locales, keeping `.orig`/`.bak`.

See `MERGING.md` for the step by step update from upstream, the conflict table and the settings ownership.

**Archived 2026-11-18.** Superseded for hideout and quest needs by the AllQuestsCheckmarks fork (https://github.com/rp-an-87/AllQuestsCheckmarks), which reads the hideout itself. This repo is kept read-only for reference; upstream is https://github.com/danx91/WishlistExtended.
