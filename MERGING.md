# Merging upstream into this fork

Fork of WishlistExtended 1.2.0. `master` mirrors upstream, our work is on `fir-hideout`.

## Update steps

```
git fetch upstream
git checkout master && git merge --ff-only upstream/master
git checkout fir-hideout && git rebase master      # or: git merge master
dotnet build WishlistExtendedClient/WishlistExtendedClient.csproj -c Release
```

Push (if there is ever an origin) with `--force-with-lease` after a rebase. Always ask the owner first.

## Where conflicts will happen

New files (ours): `WishlistExtendedApi.cs`, `Features/CheckmarksBridge.cs`, `Config/SettingsFork.cs`. They never conflict.

Upstream files we edited:

| File | What we changed | How to resolve |
|---|---|---|
| `Features/HideoutDataService.cs` | Caches every stage regardless of wishlist settings; the "not wishlistable" filter became a flag (`Wishlistable`) stored on `WishlistHideoutData` | Re-apply: remove the filter, pass `WishlistManager.IsAvailableForWishlist(...)` as the last constructor argument |
| `Models/WishlistData.cs` | `Wishlistable` property | Small |
| `Features/WishlistResolver.cs` | Hideout and quests only wishlist found in raid items (type level); hideout and quests go to the Other group; `Wishlistable` filter | Re-apply our branches around `ResolveHideout` / `ResolveQuests` |
| `Features/TooltipManager.cs` | Defers hideout and quest lines to the AllQuestsCheckmarks fork, small FiR labels | Re-apply |
| `Features/QuestDataService.cs` | Future quests from the checkmarks fork; totals were not cleared on rebuild (upstream bug) | Re-apply |
| `Config/Settings.cs` | `partial` and one `InitFork(config)` call | Re-add |
| `Plugin.cs`, `locales/en.json`, `locales/pl.json` | Small | Re-add |
| `WishlistExtendedClient.csproj` | `TarkovDir` from `.csproj.user`, deploy keeps `.orig`/`.bak` | Keep ours |

## What to check after a merge

1. It builds.
2. `WishlistExtendedApi` signatures are unchanged: the checkmarks fork finds them by reflection (`TryGetHideoutNeeds`, `TryGetAllHideoutNeeds`). Change them only together with `WishlistBridge.cs` in that fork, and bump `ApiVersion`.
3. In game: with the checkmarks fork installed, the log says "Loaded hideout requirements" with the full count; the tooltip of a hideout item shows the hideout section.

## Settings

| Setting | Owner | Note |
|---|---|---|
| 1. General (crafts, hideout, barters, quests, priorities, locked as future, stash count, remove if enough, in-raid only) | upstream | all still work. Crafts and barters are not covered by the checkmarks fork |
| 1. General: Show in checkmark tooltip | upstream | with the checkmarks fork the hover tooltip is the checkmarks one; see `DeferToCheckmarks` |
| 2. Tooltip, 3. Colors, 4. Loose Loot, 9. Debug | upstream | used |
| 5. Hideout and quests, 6. Tooltip additions | fork | `HideoutFirOnly`, `QuestsFirOnly`, `QuestsIncludeFuture`, `BothAsOther`, `TooltipHideoutAlways`, `TooltipFirLabels`, `DeferToCheckmarks` |

The wishlist itself is no longer needed for the overlays (they are drawn by the checkmarks fork). This fork is still needed for the hideout data (API), and for the in-game wishlist and flea wishlist tab if used.

Config migration: when a fork key is renamed or moved, edit the owner's `BepInEx\config\com.zgfuedkx.wishlistextended.cfg` with a backup first.

Full settings cross reference of both forks: `dev-own\docs\checkmarks-settings-reference.md` (workspace, not in this repo). Fork settings start their F12 description with `[Fork]`.
