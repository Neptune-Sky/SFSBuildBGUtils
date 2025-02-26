using System;
using ModLoader.Helpers;
using SFS.IO;
using UITools;
using UnityEngine;

namespace SFSBuildBGUtils
{
    public class Config : ModSettings<Config.SettingsData>
    {
        protected override FilePath SettingsFile => Main.modFolder.ExtendToFile("Config.txt");

        private static Config main;

        private static Action saveAction;

        public static void Setup()
        {
            main = new Config();
            main.Initialize();
        }

        public class SettingsData
        {
            public bool hideGrid;
            public Color backgroundColor = new(0.2229f, 0.3487f, 0.55f, 1f);
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            saveAction = onChange;
        }

        public static void Save() => saveAction.Invoke();

        public static void Defaults()
        {
            settings = new SettingsData();
            Save();
        }
    }
}
