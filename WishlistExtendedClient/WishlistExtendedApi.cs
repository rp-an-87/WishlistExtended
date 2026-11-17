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
        /// Hideout upgrade requirements for one item, as set by 'Hideout upgrades wishlist mode':
        /// remaining upgrades only (an upgrade already in progress is paid for). Parallel arrays, one entry per upgrade.
        /// Returns false when hideout is disabled or the item is not needed for any upgrade.
        /// </summary>
        public static bool TryGetHideoutNeeds(string templateId, out int[] quantity, out bool[] fir, out bool[] future, out string[] area, out int[] level)
        {
            quantity = [];
            fir = [];
            future = [];
            area = [];
            level = [];

            if (Settings.HideoutMode!.Value == Settings.EHideoutMode.Disabled)
            {
                return false;
            }

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
    }
}
