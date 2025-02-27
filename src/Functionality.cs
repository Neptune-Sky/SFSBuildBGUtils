using System.Globalization;
using HarmonyLib;
using SFS.Builds;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SFSBuildGridUtils
{
    [HarmonyPatch(typeof(GridSize))]
    internal static class CustomGridSizePatch
    {
        [HarmonyPatch(nameof(GridSize.GetOwnedGridSize))]
        [HarmonyPostfix]
        private static void SetGridSize(ref Vector2 __result)
        {
            if (__result == Vector2.zero) return;
            __result = Config.settings.gridSize;
        }
    }
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
                
                Window scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), 450, 600, 0, 0, false, false, 1, "Edit Build Grid");

                scroll.Position = new Vector2(0, scroll.Size.y / 2);
                
                HorizontalOrVerticalLayoutGroup layout = scroll.CreateLayoutGroup(Type.Vertical);
                layout.spacing = 15;
                layout.childAlignment = TextAnchor.MiddleCenter;
                scroll.EnableScrolling(Type.Vertical);

                Builder.CreateSpace(scroll, 0, 0);

                bool hideGridValue = Config.settings.hideGrid;
                ToggleWithLabel hideGrid = Builder.CreateToggleWithLabel(scroll, 360, 35,
                    () => hideGridValue, () => hideGridValue = !hideGridValue, labelText: "Hide Grid Texture");

                Builder.CreateLabel(scroll, 400, 35, text: "Color");
                
                Container red = Builder.CreateContainer(scroll);
                red.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(red, 200, 35, text: "R:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput redInput = CustomUI.CreateNumberInput(red, 200, 50, Config.settings.backgroundColor.r, 0, 1, 8);
                
                Container green = Builder.CreateContainer(scroll);
                green.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(green, 200, 35, text: "G:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput greenInput = CustomUI.CreateNumberInput(green, 200, 50, Config.settings.backgroundColor.g, 0, 1, 8);
                
                Container blue = Builder.CreateContainer(scroll);
                blue.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(blue, 200, 35, text: "B:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput blueInput = CustomUI.CreateNumberInput(blue, 200, 50, Config.settings.backgroundColor.b, 0, 1, 8);
                
                Builder.CreateLabel(scroll, 400, 35, text: "Grid Size");
                
                Container sizeX = Builder.CreateContainer(scroll);
                sizeX.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(sizeX, 200, 35, text: "X:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput sizeXInput = CustomUI.CreateNumberInput(sizeX, 200, 50, Config.settings.gridSize.x, 1, 500, 3);
                
                Container sizeY = Builder.CreateContainer(scroll);
                sizeY.CreateLayoutGroup(Type.Horizontal, spacing: 10f);
                Builder.CreateLabel(sizeY, 200, 35, text: "Y:").TextAlignment = TextAlignmentOptions.Left;
                NumberInput sizeYInput = CustomUI.CreateNumberInput(sizeY, 200, 50, Config.settings.gridSize.y, 1, 500, 3);
                
                Container cancelContinueButtons = Builder.CreateContainer(scroll);
                cancelContinueButtons.CreateLayoutGroup(Type.Horizontal, spacing: 10f);

                void Cancel()
                {
                    hideGridValue = Config.settings.hideGrid;
                    
                    redInput.textInput.Text = Config.settings.backgroundColor.r.ToString(CultureInfo.InvariantCulture);
                    greenInput.textInput.Text = Config.settings.backgroundColor.g.ToString(CultureInfo.InvariantCulture);
                    blueInput.textInput.Text = Config.settings.backgroundColor.b.ToString(CultureInfo.InvariantCulture);

                    sizeXInput.textInput.Text = Config.settings.gridSize.x.ToString(CultureInfo.InvariantCulture);
                    sizeYInput.textInput.Text = Config.settings.gridSize.y.ToString(CultureInfo.InvariantCulture);
                }
                
                Builder.CreateButton(cancelContinueButtons, 100, 50, onClick: () =>
                {
                    Cancel();
                    ScreenManager.main.CloseCurrent();
                }, text: "Cancel");

                void Apply()
                {
                    Config.settings.hideGrid = hideGridValue;
                    Config.settings.backgroundColor = new((float)redInput.currentVal, (float)greenInput.currentVal,
                        (float)blueInput.currentVal, 1);
                    
                    camera.backgroundColor = Config.settings.backgroundColor;
                    
                    gridTexture.SetActive(!Config.settings.hideGrid);
                    centerMarker.SetActive(!Config.settings.hideGrid);

                    Config.settings.gridSize = new Vector2Int((int)sizeXInput.currentVal, (int)sizeYInput.currentVal);
                    
                    BuildManager.main.buildGridSize.UpdateBuildSpaceSize();
                    Config.Save();
                    ScreenManager.main.CloseCurrent();
                }
                
                Builder.CreateButton(cancelContinueButtons, 120, 50, onClick: () =>
                {
                    Config.Defaults();
                    Cancel();
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