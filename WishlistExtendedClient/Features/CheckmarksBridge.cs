using BepInEx.Bootstrap;
using System;
using System.Reflection;
using WishlistExtended.Config;

namespace WishlistExtended.Features
{
    /// <summary>
    /// Asks the AllQuestsCheckmarks fork (by reflection, optional) whether its tooltip already lists hideout and quests.
    /// </summary>
    internal static class CheckmarksBridge
    {
        private const string PluginGuid = "com.zgfuedkx.allquestscheckmarks";

        private delegate bool FutureQuestsDelegate(out string[] itemId, out string[] questName, out int[] quantity, out bool[] fir);

        private static bool _resolved;
        private static Func<bool>? _showsHideout;
        private static FutureQuestsDelegate? _futureQuests;

        /// <summary>Remaining quest needs of quests not started yet, from the AllQuestsCheckmarks fork (parallel arrays).</summary>
        public static bool TryGetFutureQuestNeeds(out string[] itemId, out string[] questName, out int[] quantity, out bool[] fir)
        {
            itemId = [];
            questName = [];
            quantity = [];
            fir = [];

            Resolve();

            try
            {
                return _futureQuests is not null && _futureQuests(out itemId, out questName, out quantity, out fir);
            }
            catch (Exception e)
            {
                Plugin.LogSource?.LogError($"AllQuestsCheckmarks future quests lookup failed: {e}");
                _futureQuests = null;
                return false;
            }
        }

        public static bool CoversHideoutAndQuests
        {
            get
            {
                if (!Settings.DeferToCheckmarks!.Value)
                {
                    return false;
                }

                Resolve();

                try
                {
                    return _showsHideout is not null && _showsHideout();
                }
                catch (Exception e)
                {
                    Plugin.LogSource?.LogError($"AllQuestsCheckmarks lookup failed, showing own lines: {e}");
                    _showsHideout = null;
                    return false;
                }
            }
        }

        private static void Resolve()
        {
            if (_resolved)
            {
                return;
            }

            _resolved = true;

            if (!Chainloader.PluginInfos.TryGetValue(PluginGuid, out var info) || info.Instance is null)
            {
                return;
            }

            Type? api = info.Instance.GetType().Assembly.GetType("AllQuestsCheckmarks.AllQuestsCheckmarksApi");
            MethodInfo? method = api?.GetMethod("ShowsHideoutInTooltip", BindingFlags.Public | BindingFlags.Static);

            if (method is not null)
            {
                _showsHideout = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method);
            }

            MethodInfo? future = api?.GetMethod("TryGetFutureQuestNeeds", BindingFlags.Public | BindingFlags.Static);
            if (future is not null)
            {
                _futureQuests = (FutureQuestsDelegate)Delegate.CreateDelegate(typeof(FutureQuestsDelegate), future);
            }
        }
    }
}
