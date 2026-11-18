using BepInEx.Configuration;
using WishlistExtended.Features;
using ZGFueDkx.ZGCLib.Config;

namespace WishlistExtended.Config
{
    // The settings added by this fork, in sections of their own (numbers 5 and 6) so upstream's settings stay as they were
    internal static partial class Settings
    {
        // Hideout and quests
        public static ConfigEntry<bool>? HideoutFirOnly;
        public static ConfigEntry<bool>? QuestsFirOnly;
        public static ConfigEntry<bool>? QuestsIncludeFuture;
        public static ConfigEntry<bool>? BothAsOther;

        // Tooltip
        public static ConfigEntry<bool>? TooltipHideoutAlways;
        public static ConfigEntry<bool>? TooltipFirLabels;
        public static ConfigEntry<bool>? DeferToCheckmarks;

        // Every setting of this fork says so in its description, so it can be told apart from upstream's in F12
        private static ConfigEntry<T> ForkBind<T>(this ConfigCategory category, string key, T defaultValue, string description, bool hide = false, AcceptableValueBase? acceptableValue = null)
        {
            return category.BindConfig(key, defaultValue, "[Fork] " + description, hide, acceptableValue);
        }

        private static ConfigEntry<UnityEngine.Color> ForkColor(this ConfigCategory category, string key, string defaultValue, string description, bool hide = false)
        {
            return category.BindColor(key, defaultValue, "[Fork] " + description, hide);
        }

        private static void InitFork(ConfigFile config)
        {
            ConfigCategory hideoutAndQuests = config.MakeCategory(5, "Hideout and quests");
            ConfigCategory tooltip = config.MakeCategory(6, "Tooltip additions");

            /*
             * HIDEOUT AND QUESTS
             */
            HideoutFirOnly = hideoutAndQuests.ForkBind(
                "Wishlist only FiR hideout items",
                true,
                "Whether only hideout requirements that need found in raid items should be wishlisted. " +
                    "Useful when hideout mods (for example Hideout Backport) removed FiR from most requirements and you only care about the ones you have to find. " +
                    "Requirements that do not need FiR can still be listed in the tooltip, see 'Tooltip for non-wishlisted hideout items'. " +
                    "Works on top of 'Hideout upgrades wishlist mode' (section 1): that picks which upgrades, this keeps their found in raid requirements. " +
                    "No effect on the AllQuestsCheckmarks fork, which counts every requirement"
            );

            HideoutFirOnly.SettingChanged += (_, _) => WishlistService.RebuildAll();

            QuestsFirOnly = hideoutAndQuests.ForkBind(
                "Wishlist only FiR quest items",
                true,
                "Whether only quest hand-ins that need found in raid items should be wishlisted. " +
                    "Needs 'Include quests' (section 1): with that off no quest item is wishlisted at all. No effect on the AllQuestsCheckmarks fork"
            );

            QuestsFirOnly.SettingChanged += (_, _) => WishlistService.RebuildAll();

            QuestsIncludeFuture = hideoutAndQuests.ForkBind(
                "Include future quests (needs AllQuestsCheckmarks fork)",
                true,
                "Whether items needed for quests you have not started yet should be wishlisted too (needs 'Include quests' in section 1). " +
                    "Uses the quest data and settings of the AllQuestsCheckmarks fork; has no effect without it"
            );

            QuestsIncludeFuture.SettingChanged += (_, _) => WishlistService.RebuildAll();

            BothAsOther = hideoutAndQuests.ForkBind(
                "Hideout and quest items go to the Other group",
                true,
                "Whether an item needed for both a hideout upgrade and a quest should be listed in the Other wishlist group " +
                    "instead of the group chosen by the priorities in section 1"
            );

            /*
             * TOOLTIP ADDITIONS
             */
            TooltipHideoutAlways = tooltip.ForkBind(
                "Tooltip for non-wishlisted hideout items",
                true,
                "Whether hideout upgrade requirements should be shown in the tooltip even if the item is not wishlisted " +
                    "(for example non-FiR requirements when 'Wishlist only FiR hideout items' is on). Every requirement is labeled FiR or non-FiR"
            );

            TooltipFirLabels = tooltip.ForkBind(
                "Label hideout requirements as FiR / non-FiR",
                true,
                "Whether every hideout line in the tooltip should say if the requirement needs a found in raid item"
            );

            DeferToCheckmarks = tooltip.ForkBind(
                "Leave hideout and quests to AllQuestsCheckmarks fork",
                true,
                "Whether the hideout and quest lines should be left out of this tooltip when the AllQuestsCheckmarks fork shows them " +
                    "(it lists them together with combined totals). Crafts and barters stay here. " +
                    "Because of that 'Include quests' and 'Tooltip for non-wishlisted hideout items' change nothing in the tooltip then, and " +
                    "'Show amount of items in stash' (section 1) only shows on items that have crafts or barters (the checkmarks tooltip prints the stash count itself)"
            );
        }
    }
}
