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

        private static void InitFork(ConfigFile config)
        {
            ConfigCategory hideoutAndQuests = config.MakeCategory(5, "Hideout and quests");
            ConfigCategory tooltip = config.MakeCategory(6, "Tooltip additions");

            /*
             * HIDEOUT AND QUESTS
             */
            HideoutFirOnly = hideoutAndQuests.BindConfig(
                "Wishlist only FiR hideout items",
                true,
                "Whether only hideout requirements that need found in raid items should be wishlisted. " +
                    "Useful when hideout mods (for example Hideout Backport) removed FiR from most requirements and you only care about the ones you have to find. " +
                    "Requirements that do not need FiR can still be listed in the tooltip, see 'Tooltip for non-wishlisted hideout items'"
            );

            HideoutFirOnly.SettingChanged += (_, _) => WishlistService.RebuildAll();

            QuestsFirOnly = hideoutAndQuests.BindConfig(
                "Wishlist only FiR quest items",
                true,
                "Whether only quest hand-ins that need found in raid items should be wishlisted"
            );

            QuestsFirOnly.SettingChanged += (_, _) => WishlistService.RebuildAll();

            QuestsIncludeFuture = hideoutAndQuests.BindConfig(
                "Include future quests (needs AllQuestsCheckmarks fork)",
                true,
                "Whether items needed for quests you have not started yet should be wishlisted too (needs 'Include quests' in section 1). " +
                    "Uses the quest data and settings of the AllQuestsCheckmarks fork; has no effect without it"
            );

            QuestsIncludeFuture.SettingChanged += (_, _) => WishlistService.RebuildAll();

            BothAsOther = hideoutAndQuests.BindConfig(
                "Hideout and quest items go to the Other group",
                true,
                "Whether an item needed for both a hideout upgrade and a quest should be listed in the Other wishlist group " +
                    "instead of the group chosen by the priorities in section 1"
            );

            /*
             * TOOLTIP ADDITIONS
             */
            TooltipHideoutAlways = tooltip.BindConfig(
                "Tooltip for non-wishlisted hideout items",
                true,
                "Whether hideout upgrade requirements should be shown in the tooltip even if the item is not wishlisted " +
                    "(for example non-FiR requirements when 'Wishlist only FiR hideout items' is on). Every requirement is labeled FiR or non-FiR"
            );

            TooltipFirLabels = tooltip.BindConfig(
                "Label hideout requirements as FiR / non-FiR",
                true,
                "Whether every hideout line in the tooltip should say if the requirement needs a found in raid item"
            );

            DeferToCheckmarks = tooltip.BindConfig(
                "Leave hideout and quests to AllQuestsCheckmarks fork",
                true,
                "Whether the hideout and quest lines should be left out of this tooltip when the AllQuestsCheckmarks fork shows them " +
                    "(it lists them together with combined totals). Crafts and barters stay here"
            );
        }
    }
}
