# Fork notice

Personal fork of **WishlistExtended** 1.2.0 by ZGFueDkx (danx91).

- Upstream: https://github.com/danx91/WishlistExtended (tag v1.2.0)
- License: GPL-3.0 (see `LICENSE`); this fork stays GPL-3.0 with its source available.
- Build dependency: https://github.com/danx91/SPT-ZGFueDkxCommonLibrary (no license file upstream), cloned as a sibling folder and compiled in, as upstream's README describes. It is not modified or redistributed here.
- Branch `master` mirrors upstream; work lives on `fir-hideout`.
- AI use: code in this fork is written with Claude (Claude Code) assistance. Do not send it upstream as-is.

## Changes (planned)

- Option to wishlist only found-in-raid hideout requirements (useful with Hideout Backport, which drops FiR from many).
- Tooltip option to show hideout needs for items that are not wishlisted, labelled FiR / non-FiR with stash counts.
- Small public API for hideout needs so the AllQuestsCheckmarks fork can merge them into its tooltip; its own tooltip drops duplicate hideout/quest lines when that mod is present.

Build changes: csproj uses `TarkovDir` (from an uncommitted `.csproj.user`) instead of assuming the repo sits in the game folder; deploys the DLL only, keeping `.orig`/`.bak`.
