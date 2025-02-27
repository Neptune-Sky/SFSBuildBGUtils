using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using ModLoader;
using ModLoader.Helpers;
using SFS.IO;
using UITools;

namespace SFSBuildGridUtils
{
    [UsedImplicitly]
    public class Main : Mod, IUpdatable
    {
        public override string ModNameID => "BuildGridUtils";
        public override string DisplayName => "Build Grid Utils";
        public override string Author => "NeptuneSky";
        public override string MinimumGameVersionNecessary => "1.5.10.2";
        public override string ModVersion => "v1.1";
        public override string Description => "A simple menu for editing the build grid.";
        public override Dictionary<string, string> Dependencies { get; } = new() { { "UITools", "1.0" } };

        public override Action LoadKeybindings => BGU_Keybindings.LoadKeybindings;
        public Dictionary<string, FilePath> UpdatableFiles => new() { { "https://github.com/Neptune-Sky/SFSBuildGridUtils/releases/latest/download/BuildGridUtils.dll", new FolderPath(ModFolder).ExtendToFile("BuildBGUtils.dll") } };

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
                Functionality.Setup();
            };
        }
    }
}
