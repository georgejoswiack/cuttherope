using System;

namespace CutTheRopeDX.GameMain
{
    /// <summary>
    /// Provides helper properties to determine whether seasonal events
    /// should be active based on the current system date or a manual theme.
    /// </summary>
    internal static class SpecialEvents
    {
        [ThreadStatic]
        internal static DateTime? PinnedDate;

        /// <summary>
        /// Manual seasonal theme:
        /// -1 = automatic
        /// 0 = Classic
        /// 1 = Experiments
        /// 2 = Halloween
        /// 3 = Christmas
        /// </summary>
        internal static int ManualTheme { get; set; } = -1;

        private static DateTime Today => PinnedDate ?? DateTime.Now;

        public static bool IsJanuary => Today.Month == 1;

        public static bool IsXmas =>
            ManualTheme == 3 || (ManualTheme == -1 && Today.Month is 12 or 1);

        public static bool IsHalloween =>
            ManualTheme == 2 || (ManualTheme == -1 && Today.Month is 10);
    }
}
