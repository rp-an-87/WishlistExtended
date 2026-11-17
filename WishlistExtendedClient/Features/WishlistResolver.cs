using EFT;
using EFT.InventoryLogic;
using System.Collections.Generic;
using System.Linq;
using WishlistExtended.Config;
using WishlistExtended.Models;
using ZGFueDkx.ZGCLib.helpers;

namespace WishlistExtended.Features
{
    internal static class WishlistResolver
    {
        public static bool IsInWishlist(MongoID templateId, out EWishlistGroup group, Item? item = null)
        {
            if (ResolveWhishlist(false, item).TryGetValue(templateId, out WishlistData data))
            {
                group = GetWishlistGroup(data);
                return true;
            }

            group = EWishlistGroup.Hideout;
            return false;
        }

        public static Dictionary<MongoID, EWishlistGroup> GetWishlist()
        {
            Dictionary<MongoID, EWishlistGroup> wishlist = [];

            foreach (var (itemId, data) in ResolveWhishlist())
            {
                wishlist[itemId] = GetWishlistGroup(data);
            }

            return wishlist;
        }

        public static Dictionary<MongoID, WishlistData> ResolveWhishlist(bool ignoreEnough = false, Item? item = null, bool forTooltip = false)
        {
            if (Settings.RaidOnly!.Value && !RaidUtils.IsInRaid())
            {
                return [];
            }

            bool excludeFir = item is not null && !item.MarkedAsSpawnedInSession;
            Dictionary<MongoID, WishlistData> wishlist = [];

            ResolveCrafts(wishlist);
            ResolveHideout(wishlist, !ignoreEnough, excludeFir, forTooltip && Settings.TooltipHideoutAlways!.Value);
            ResolveBarters(wishlist);
            ResolveQuests(wishlist, !ignoreEnough, excludeFir, forTooltip && Settings.TooltipHideoutAlways!.Value);

            return wishlist;
        }

        public static void ResolveCrafts(Dictionary<MongoID, WishlistData> wishlist)
        {
            if (Settings.CraftMode!.Value == Settings.ECraftMode.Disabled)
            {
                return;
            }

            foreach (var (itemId, craft) in CraftDataService.Crafts)
            {
                if (!wishlist.TryGetValue(itemId, out var wishlistData))
                {
                    wishlistData = new();
                    wishlist[itemId] = wishlistData;
                }

                wishlistData.Crafts.AddRange(craft);
            }
        }

        // everything: tooltip that lists every hideout requirement (FiR and non-FiR, any item copy), not only the wishlisted ones
        public static void ResolveHideout(Dictionary<MongoID, WishlistData> wishlist, bool hideEnough, bool excludeFir, bool everything = false)
        {
            if (Settings.HideoutMode!.Value == Settings.EHideoutMode.Disabled)
            {
                return;
            }

            bool onlyCurrent = Settings.HideoutMode!.Value == Settings.EHideoutMode.Current;

            foreach (var (itemId, allStages) in HideoutDataService.HideoutRequirements)
            {
                List<WishlistHideoutData> hideout = onlyCurrent ? [.. allStages.Where(h => !h.IsFuture)] : allStages;
                bool firOnly = Settings.HideoutFirOnly!.Value && !everything;

                if (hideEnough && Settings.RemoveIfEnough!.Value)
                {
                    Models.ItemsCount inStash = StashService.GetItemCount(itemId);
                    Models.ItemsCount needed = new() { Total = hideout.Sum(h => h.Quantity), Fir = hideout.Where(h => h.Fir).Sum(h => h.Quantity) };

                    // FiR only: non-FiR requirements are not wishlisted, so only the FiR amount matters
                    if (firOnly ? inStash.Fir >= needed.Fir : (inStash.Total >= needed.Total && inStash.Fir >= needed.Fir))
                    {
                        continue;
                    }
                }

                foreach (WishlistHideoutData item in hideout)
                {
                    // FiR only: the item is wanted as a type ("find this one"), so a non-FiR copy is marked too, like crafts
                    if (!everything && !firOnly && excludeFir && item.Fir)
                    {
                        continue;
                    }

                    if (firOnly && !item.Fir)
                    {
                        continue;
                    }

                    if (!wishlist.TryGetValue(itemId, out var wishlistData))
                    {
                        wishlistData = new();
                        wishlist[itemId] = wishlistData;
                    }

                    wishlistData.Hideout.Add(item);
                }
            }
        }

        public static void ResolveBarters(Dictionary<MongoID, WishlistData> wishlist)
        {
            if (!Settings.IncludeBarters!.Value)
            {
                return;
            }

            foreach (var (itemId, barters) in BarterDataService.Barters)
            {
                if (!wishlist.TryGetValue(itemId, out var wishlistData))
                {
                    wishlistData = new();
                    wishlist[itemId] = wishlistData;
                }

                wishlistData.Barters.AddRange(barters);
            }
        }

        public static void ResolveQuests(Dictionary<MongoID, WishlistData> wishlist, bool hideEnough, bool excludeFir, bool everything = false)
        {
            if (!Settings.IncludeQuests!.Value)
            {
                return;
            }

            foreach (var (itemId, quests) in QuestDataService.Quests)
            {
                bool firOnly = Settings.QuestsFirOnly!.Value && !everything;

                if (hideEnough && Settings.RemoveIfEnough!.Value)
                {
                    Models.ItemsCount inStash = StashService.GetItemCount(itemId);
                    Models.ItemsCount needed = QuestDataService.TotalNeeded.TryGetValue(itemId, out var total) ? total : new();

                    if (firOnly ? inStash.Fir >= needed.Fir : (inStash.Total >= needed.Total && inStash.Fir >= needed.Fir))
                    {
                        continue;
                    }
                }

                foreach (WishlistQuestData item in quests)
                {
                    if (!everything && !firOnly && excludeFir && item.Fir)
                    {
                        continue;
                    }

                    if (firOnly && !item.Fir)
                    {
                        continue;
                    }

                    if (!wishlist.TryGetValue(itemId, out var wishlistData))
                    {
                        wishlistData = new();
                        wishlist[itemId] = wishlistData;
                    }

                    wishlistData.Quests.Add(item);
                }
            }
        }

        private static EWishlistGroup GetWishlistGroup(WishlistData data)
        {
            // needed for both a hideout upgrade and a quest: its own group, so it stands apart from either
            if (Settings.BothAsOther!.Value && data.HasHideout && data.HasQuests)
            {
                return EWishlistGroup.Other;
            }

            EWishlistGroup group = EWishlistGroup.Hideout;
            int priority = int.MinValue;

            if (data.HasHideout && Settings.HideoutPriority!.Value > priority)
            {
                group = EWishlistGroup.Hideout;
                priority = Settings.HideoutPriority!.Value;
            }

            if (data.HasCrafts && Settings.CraftsPriority!.Value > priority)
            {
                group = EWishlistGroup.Equipment;
                priority = Settings.HideoutPriority!.Value;
            }

            if (data.HasBarters && Settings.BartersPriority!.Value > priority)
            {
                group = EWishlistGroup.Trading;
                priority = Settings.BartersPriority!.Value;
            }

            if (data.HasQuests && Settings.QuestsPriority!.Value > priority)
            {
                group = EWishlistGroup.Quests;
            }

            return group;
        }
    }
}
