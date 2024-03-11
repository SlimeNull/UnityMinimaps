using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SlimeNull.UnityMinimaps
{
    public class MinimapTools : EditorWindow
    {
        [MenuItem("Tools/Minimap")]
        static void Open()
        {
            var window = EditorWindow.GetWindow<MinimapTools>();
            window.Show();
        }

        string _textureSizeStr = "1024";
        string _cameraHeightStr = "5";
        string _minimapAreaSizeStr = "50";
        int _minimapLayers = ~0;

        int _selectedMinimapType = 0;
        string _createMinimapAreaSizeStr = "50";

        Camera _createMinimapRenderCamera;
        Texture _createMinimapStaticTexture;

        private void GUIHeader(string text)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(text);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void SetRectStretch(Transform transform)
        {
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform == null)
                rectTransform = transform.gameObject.AddComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2();
            rectTransform.sizeDelta = new Vector2(0, 0);
        }

        Minimap CreateBasicMinimap(Transform parent)
        {
            GameObject minimapObject = new GameObject("Minimap");
            minimapObject.transform.SetParent(parent);
            minimapObject.transform.localPosition = Vector3.zero;

            var minimap = minimapObject.AddComponent<Minimap>();
            var minimapModifier = minimapObject.AddComponent<MinimapModifier>();
            var minimapRenderer = minimapObject.AddComponent<MinimapRenderer>();
            var minimapNavigator = minimapObject.AddComponent<MinimapNavigator>();
            var minimapIndicator = minimapObject.AddComponent<MinimapIndicator>();

            GameObject navPathRendererObject = new GameObject("NavigationPathRenderer");
            navPathRendererObject.transform.SetParent(minimapObject.transform);
            navPathRendererObject.transform.localPosition = new Vector3();
            SetRectStretch(navPathRendererObject.transform);

            var navPathRenderer = navPathRendererObject.AddComponent<MinimapNavigationPathRenderer>();
            navPathRenderer.Minimap = minimap;
            navPathRenderer.raycastTarget = false;

            GameObject iconsObject = new GameObject("Icons");
            iconsObject.transform.SetParent(minimapObject.transform);
            iconsObject.transform.localPosition = new Vector3();
            SetRectStretch(iconsObject.transform);
            minimapIndicator.IconsSlot = iconsObject.transform;

            return minimap;
        }

        MinimapTextureSource CreateTextureSource(GameObject gameObject, Texture texture, int areaSize)
        {
            var textureSource = gameObject.AddComponent<MinimapStaticTextureSource>();
            textureSource.StaticTexture = texture;
            textureSource.StaticAreaSize = areaSize;

            return textureSource;
        }


        MinimapTextureSource CreateTextureSource(GameObject gameObject, Camera renderCamera)
        {
            var textureSource = gameObject.AddComponent<MinimapRenderTextureSource>();
            textureSource.RenderCamera = renderCamera;

            return textureSource;
        }


        Minimap CreateMinimap(Transform parent, Texture texture, int areaSize)
        {
            var minimap = CreateBasicMinimap(parent);
            minimap.TextureSource = CreateTextureSource(minimap.gameObject, texture, areaSize);

            return minimap;
        }


        Minimap CreateMinimap(Transform parent, Camera renderCamera)
        {
            var minimap = CreateBasicMinimap(parent);
            minimap.TextureSource = CreateTextureSource(minimap.gameObject, renderCamera);

            return minimap;
        }

        void DrawWizardTool()
        {
            GUIHeader("Wizard");

            GUILayout.Label("Minimap type:");
            _selectedMinimapType = EditorGUILayout.Popup(_selectedMinimapType, new string[] { "Static texture", "Render texture" });

            if (_selectedMinimapType == 0)
            {
                GUILayout.Label("Texture:");
                _createMinimapStaticTexture = EditorGUILayout.ObjectField(_createMinimapStaticTexture, typeof(Texture), true) as Texture;

                GUILayout.Label("Texture area size:");
                _createMinimapAreaSizeStr = GUILayout.TextField(_createMinimapAreaSizeStr);

                if (_createMinimapStaticTexture == null)
                {
                    GUILayout.Label("No texture selected");
                    return;
                }

                if (!int.TryParse(_createMinimapAreaSizeStr, out var areaSize))
                {
                    GUILayout.Label("Invalid area size");
                    return;
                }


                if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponentInParent<Canvas>() is not Canvas)
                {
                    GUILayout.Label("Select a game object under Canvas to create minimap");
                    return;
                }

                var parent = Selection.activeGameObject.transform;
                if (GUILayout.Button("Create"))
                    CreateMinimap(parent, _createMinimapStaticTexture, areaSize);
            }
            else
            {
                GUILayout.Label("Render camera");
                _createMinimapRenderCamera = EditorGUILayout.ObjectField(_createMinimapRenderCamera, typeof(Camera), true) as Camera;

                if (Selection.activeGameObject == null || Selection.activeGameObject.GetComponentInParent<Canvas>() is not Canvas)
                {
                    GUILayout.Label("Select a game object under Canvas to create minimap");
                    return;
                }

                var parent = Selection.activeGameObject.transform;
                if (GUILayout.Button("Create"))
                    CreateMinimap(parent, _createMinimapRenderCamera);
            }
        }

        void DrawTextureTool()
        {
            GUIHeader("Texture tool");

            GUILayout.Label("Texture size: ");
            _textureSizeStr = GUILayout.TextField(_textureSizeStr);

            GUILayout.Label("Camera height: ");
            _cameraHeightStr = GUILayout.TextField(_cameraHeightStr);

            GUILayout.Label("Minimap area size: ");
            _minimapAreaSizeStr = GUILayout.TextField(_minimapAreaSizeStr);

            GUILayout.Label("Minimap layers: ");
            LayerMask tempMask = EditorGUILayout.MaskField( InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_minimapLayers), InternalEditorUtility.layers);
            _minimapLayers = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

            GUILayout.Space(10);

            if (!int.TryParse(_textureSizeStr, out var textureSize) ||
                !float.TryParse(_cameraHeightStr, out var cameraHeight) ||
                !float.TryParse(_minimapAreaSizeStr, out var minimapAreaSize))
            {
                GUILayout.Label("Error input!");
                return;
            }

            if (GUILayout.Button("Generate minimap texture"))
            {
                string filename = EditorUtility.SaveFilePanel("Save texture", Application.dataPath, "Minimap", "png");

                RenderTexture rt = new RenderTexture(textureSize, textureSize, 32);

                GameObject cameraGameObject = new GameObject();
                Camera camera = cameraGameObject.AddComponent<Camera>();

                cameraGameObject.transform.eulerAngles = new Vector3(90, 0, 0);
                cameraGameObject.transform.position = new Vector3(0, cameraHeight, 0);

                camera.cullingMask = _minimapLayers;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0, 0, 0, 0);
                camera.orthographic = true;
                camera.orthographicSize = minimapAreaSize / 2;

                camera.targetTexture = rt;
                camera.Render();

                var activeRenderTextureBefore = RenderTexture.active;
                RenderTexture.active = rt;

                Texture2D png = new Texture2D(textureSize, textureSize);
                png.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
                png.Apply();

                RenderTexture.active = activeRenderTextureBefore;

                var bytes = png.EncodeToPNG();
                System.IO.File.WriteAllBytes(filename, bytes);

                DestroyImmediate(cameraGameObject);
                DestroyImmediate(rt);
                DestroyImmediate(png);
            }
        }

        private void OnGUI()
        {
            DrawWizardTool();
            GUILayout.Space(20);

            DrawTextureTool();
        }
    }
}
