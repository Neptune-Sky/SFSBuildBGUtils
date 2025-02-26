using ModLoader;
using ModLoader.Helpers;
using UnityEngine;
using static SFS.Input.KeybindingsPC;
// ReSharper disable MemberCanBePrivate.Global

namespace SFSBuildGridUtils
{
    public class DefaultKeys
    {
        public Key ShowMenu = KeyCode.Slash;
    }

    public class BGU_Keybindings : ModKeybindings
    {
        private static readonly DefaultKeys defaultKeys = new();

        #region Keys
        public Key ShowMenu = defaultKeys.ShowMenu;
        #endregion

        public static BGU_Keybindings main;

        public static void LoadKeybindings()
        {
            main = SetupKeybindings<BGU_Keybindings>(Main.main);
            SceneHelper.OnBuildSceneLoaded += OnBuildLoad;
        }

        private static void OnBuildLoad()
        {
            AddOnKeyDown_Build(main.ShowMenu, Functionality.OpenMenu);
        }

        public override void CreateUI()
        {
            CreateUI_Text("Build BG Keybindings");
            CreateUI_Keybinding(ShowMenu, defaultKeys.ShowMenu, "Show Options Menu");
            CreateUI_Space();
        }
    }
}