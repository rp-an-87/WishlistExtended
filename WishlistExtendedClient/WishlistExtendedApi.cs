using EFT;
using WishlistExtended.Config;
using WishlistExtended.Features;

namespace WishlistExtended
{
    /// <summary>
    /// Stable entry point for other mods (looked up by reflection, so keep names and signatures unchanged).
    /// </summary>
    public static class WishlistExtendedApi
    {
        public const int ApiVersion = 1;

        /// <summary>
        /// Hideout upgrade requirements for one item, every remaining upgrade (an upgrade already in progress is paid for),
        /// whatever the wishlist settings are. Parallel arrays, one entry per upgrade.
        /// Returns false when the item is not needed for any upgrade.
        /// </summary>
        public static bool TryGetHideoutNeeds(string templateId, out int[] quantity, out bool[] fir, out bool[] future, out string[] area, out int[] level)
        {
            quantity = [];
            fir = [];
            future = [];
            area = [];
            level = [];

            // independent of the wishlist settings: this is data, not wishlist
            if (!HideoutDataService.HideoutRequirements.TryGetValue(new MongoID(templateId), out var needs) || needs.Count == 0)
            {
                return false;
            }

            int n = needs.Count;
            quantity = new int[n];
            fir = new bool[n];
            future = new bool[n];
            area = new string[n];
            level = new int[n];

            for (int i = 0; i < n; i++)
            {
                quantity[i] = needs[i].Quantity;
                fir[i] = needs[i].Fir;
                future[i] = needs[i].IsFuture;
                area[i] = needs[i].AreaType.LocalizeAreaName();
                level[i] = needs[i].Stage;
            }

            return true;
        }

        /// <summary>
        /// Every remaining hideout item requirement (parallel arrays): item id, quantity, found in raid,
        /// future (not the next upgrade of its area, or locked). Independent of the wishlist settings.
        /// </summary>
        public static bool TryGetAllHideoutNeeds(out string[] itemId, out int[] quantity, out bool[] fir, out bool[] future)
        {
            var ids = new System.Collections.Generic.List<string>();
            var qty = new System.Collections.Generic.List<int>();
            var firs = new System.Collections.Generic.List<bool>();
            var futures = new System.Collections.Generic.List<bool>();

            foreach (var entry in HideoutDataService.HideoutRequirements)
            {
                foreach (var need in entry.Value)
                {
                    ids.Add(entry.Key.ToString());
                    qty.Add(need.Quantity);
                    firs.Add(need.Fir);
                    futures.Add(need.IsFuture);
                }
            }

            itemId = [.. ids];
            quantity = [.. qty];
            fir = [.. firs];
            future = [.. futures];

            return ids.Count > 0;
        }
    }
}
