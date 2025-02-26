using System.Globalization;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SFSBuildGridUtils
{
    public static class Functionality
    {
        private static Camera camera;
        private static GameObject gridTexture;
        private static GameObject centerMarker;
        
        public static void Setup()
        {
            camera = GameObject.Find("Camera").GetComponent<Camera>();
            Transform gridSize = GameObject.Find("Grid Size").transform;
            gridTexture = gridSize.Find("Grid Texture").gameObject;
            centerMarker = gridSize.Find("Center Marker").gameObject;

            camera.backgroundColor = Config.settings.backgroundColor;

            if (!Config.settings.hideGrid) return;
            gridTexture.SetActive(false);
            centerMarker.SetActive(false);
        }

        public static void OpenMenu()
        {
            var menuElement = new MenuElement(delegate(GameObject root)
            {
                var containerObject = new GameObject("ModGUI Container");
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);
                
                Window scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 450, 430, 0, 0, false, false, 1, "Build Background");

                scroll.Position = new Vector2(0, scroll.Size.y / 2);
                
                HorizontalOrVerticalLayoutGroup layout = scroll.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 15;
                layout.childAlignment = TextAnchor.MiddleCenter;
                scroll.EnableScrolling(Type.Vertical);

                Builder.CreateSpace(scroll, 0, 0);

                ToggleWithLabel hideGrid = Builder.CreateToggleWithLabel(scroll, 360, 35,
                    () => Config.settings.hideGrid, () => Config.settings.hideGrid = !Config.settings.hideGrid, labelText: "Hide Grid Texture");

                Builder.CreateLabel(scroll, 400, 35, text: "Color");
                
                Container red = Builder.CreateContainer(scroll);
                red.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(red, 200, 35, text: "R:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput redInput = CustomUI.CreateNumberInput(red, 200, 50, Config.settings.backgroundColor.r, 0, 1, 8);
                redInput.textInput.OnChange += _ => Config.settings.backgroundColor.r = (float)redInput.currentVal;
                
                Container green = Builder.CreateContainer(scroll);
                green.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(green, 200, 35, text: "G:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput greenInput = CustomUI.CreateNumberInput(green, 200, 50, Config.settings.backgroundColor.g, 0, 1, 8);
                greenInput.textInput.OnChange += _ => Config.settings.backgroundColor.g = (float)greenInput.currentVal;
                
                Container blue = Builder.CreateContainer(scroll);
                blue.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(blue, 200, 35, text: "B:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput blueInput = CustomUI.CreateNumberInput(blue, 200, 50, Config.settings.backgroundColor.b, 0, 1, 8);
                blueInput.textInput.OnChange += _ => Config.settings.backgroundColor.b = (float)blueInput.currentVal;
                
                Container cancelContinueButtons = Builder.CreateContainer(scroll);
                cancelContinueButtons.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateButton(cancelContinueButtons, 100, 50, onClick: ScreenManager.main.CloseCurrent, text: "Cancel");

                void Apply()
                {
                    camera.backgroundColor = Config.settings.backgroundColor;
                    gridTexture.SetActive(!Config.settings.hideGrid);
                    centerMarker.SetActive(!Config.settings.hideGrid);
                    Config.Save();
                    ScreenManager.main.CloseCurrent();
                }
                
                Builder.CreateButton(cancelContinueButtons, 120, 50, onClick: () =>
                {
                    Config.Defaults();
                    Apply();
                }, text: "Defaults");
                Builder.CreateButton(cancelContinueButtons, 100, 50, onClick: Apply, text: "Apply");
                
                
                containerObject.transform.SetParent(root.transform);
            });

            MenuElement[] elements = { menuElement };
            MenuGenerator.OpenMenu(CancelButton.Cancel, CloseMode.Current, elements);
        }
    }
}