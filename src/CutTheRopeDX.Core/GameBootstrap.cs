using System;
using CutTheRopeDX.Commons;
using CutTheRopeDX.Framework;
using CutTheRopeDX.Framework.Core;
using CutTheRopeDX.Framework.Media;
using CutTheRopeDX.Framework.Platform;

namespace CutTheRopeDX
{
    /// <summary>
    /// The single device-independent boot sequence. Both the desktop host (<c>Game1</c>)
    /// and the headless host call this, so the two cannot drift â€” if they did, headless tests
    /// would stop reflecting the real game.
    /// </summary>
    internal static class GameBootstrap
    {
        /// <summary>
        /// Installs the asset platform and brings the engine up to the point where the root
        /// controller can be ticked.
        /// </summary>
        /// <param name="platform">Asset platform to install before any asset load.</param>
        /// <param name="audioBackend">Audio backend for sound and music, or <see langword="null"/> for silent runs.</param>
        /// <param name="surfaceWidth">Logical surface width.</param>
        /// <param name="surfaceHeight">Logical surface height.</param>
        /// <param name="language">Language to initialize the engine with.</param>
        /// <param name="devicePixelRatio">Physical pixels per logical pixel on the host surface.</param>
        public static void Initialize(
            IAssetPlatform platform,
            IAudioBackend audioBackend,
            int surfaceWidth,
            int surfaceHeight,
            Language language,
            float devicePixelRatio = 1f)
        {
            AssetPlatform.Current = platform;
            CutTheRopeDX.Helpers.ContentPaths.InitializeProfile();
            SoundMgr.SetBackend(audioBackend);
            Preferences.LoadPreferences();

            int savedTheme = Preferences.GetIntForKey("PREFS_SELECTED_THEME");

            if (savedTheme >= 0 && savedTheme < CutTheRopeDX.GameMain.ThemeRegistry.Count)
            {
                CutTheRopeDX.GameMain.ThemeDefinition theme =
                    CutTheRopeDX.GameMain.ThemeRegistry.Get(savedTheme);

                CutTheRopeDX.GameMain.ThemeRegistry.CurrentIndex = savedTheme;


                CutTheRopeDX.GameMain.MenuTheme.Current = theme.MenuStyle;
                CutTheRopeDX.GameMain.SpecialEvents.ManualTheme = theme.ManualTheme;
            }
            if (CutTheRopeDX.GameMain.MenuTheme.TryParse(CutTheRopeDX.Helpers.ContentPaths.MenuStyle, out CutTheRopeDX.GameMain.MenuStyle profileMenuStyle))
            {
                CutTheRopeDX.GameMain.MenuTheme.Current = profileMenuStyle;
            }

            GameLifecycle.InitRuntime(language);
            GameLifecycle.OnSurfaceCreated();
            GameLifecycle.OnSurfaceChanged(surfaceWidth, surfaceHeight, devicePixelRatio);
        }
    }
}











