using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using ModLoader;
using ModLoader.Helpers;
using SFS.Input;
using SFS.IO;
using SFS.UI;
using UITools;

namespace SFSBuildBGUtils
{
    [UsedImplicitly]
    public class Main : Mod, IUpdatable
    {
        public override string ModNameID => "BuildSettings";
        public override string DisplayName => "Build Settings";
        public override string Author => "StarMods";
        public override string MinimumGameVersionNecessary => "1.5.10.2";
        public override string ModVersion => "v2.1.5.1";
        public override string Description => "Build settings window and various changes to build mode. See the GitHub repository for a full list of features.";
        public override Dictionary<string, string> Dependencies { get; } = new() { { "UITools", "1.0" } };

        public override Action LoadKeybindings => BGU_Keybindings.LoadKeybindings;
        public Dictionary<string, FilePath> UpdatableFiles => new() { { "https://github.com/Neptune-Sky/SFSBuildSettings/releases/latest/download/BuildSettings.dll", new FolderPath(ModFolder).ExtendToFile("BuildSettings.dll") } };

        private static Harmony patcher;

        public static Main main;
        public static FolderPath modFolder;

        public override void Early_Load()
        {
            modFolder = new FolderPath(ModFolder);

            main = this;

            patcher = new Harmony("Neptune.BuildBGUtils.Mod");
            patcher.PatchAll();
        }

        public override void Load()
        {
            Config.Setup();
            SceneHelper.OnBuildSceneLoaded += () =>
            {
                GUI.Setup();
                SkinUnlocker.Initialize();
            };
        }
    }
}
