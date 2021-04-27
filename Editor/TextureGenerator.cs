using UnityEngine;
using UnityEditor;
using System.IO;

namespace BLBits
{
    public enum TextureType { WhiteNoise, PerlinNoise, NormalNoise, Gradient }

    public class TextureGenerator : EditorWindow
    {
        private Vector2Int textureSize = new Vector2Int(256, 256);
        private TextureType noiseType;
        private bool useColors = false;
        private Color[] colors;
        private Vector2 perlinScale = Vector2.one;
        private Vector2 perlinOrigin;
        private Gradient gradient = new Gradient();

        private Texture2D curGeneratedTexture = null;

        private Rect textureRect;
        private Vector2 scrollPos;

        private string assetName = "New Texture";
        private string assetPath = "Assets/";

        [MenuItem("Window/Texture Generator")]
        static void ShowWindow()
        {
            TextureGenerator window = EditorWindow.GetWindow<TextureGenerator>();
            window.Show();
            window.titleContent = new GUIContent("Texture Generator");
        }

        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            noiseType = (TextureType)EditorGUILayout.EnumPopup("Noise Type", noiseType);
            if (noiseType == TextureType.PerlinNoise)
            {
                perlinScale = EditorGUILayout.Vector2Field("Noise Scale", perlinScale);
                perlinOrigin = EditorGUILayout.Vector2Field("Noise Offset", perlinOrigin);
            }

            if(noiseType == TextureType.Gradient)
            {
                textureSize.x = EditorGUILayout.IntField("Texture Size", textureSize.x);

                gradient = EditorGUILayout.GradientField("Gradient", gradient);
            }
            else
            {
                textureSize = EditorGUILayout.Vector2IntField("Texture Size", textureSize);
            }
            

            if (curGeneratedTexture == null)
            {
                GUILayout.EndScrollView();

                if (GUILayout.Button("Generate Texture"))
                {
                    curGeneratedTexture = GenerateTexture();
                }
            }
            else
            {
                if (Event.current.type == EventType.Repaint)
                {
                    textureRect = GUILayoutUtility.GetLastRect();
                    textureRect.y += textureRect.height + 2;
                    textureRect.height = textureRect.width;
                }

                GUI.DrawTexture(textureRect, curGeneratedTexture);

                GUILayout.EndScrollView();

                GUILayout.Label("Texture Name");
                assetName = GUILayout.TextField(assetName);

                GUILayout.Label("Save Location");
                assetPath = GUILayout.TextField(assetPath);

                if (GUILayout.Button("Select Folder"))
                {
                    assetPath = EditorUtility.OpenFolderPanel("Where To Save This Noise", "Assets/", "");
                }

                if (GUILayout.Button("Save Texture"))
                {
                    byte[] png = ImageConversion.EncodeToPNG(curGeneratedTexture);


                    if (Directory.Exists(assetPath))
                    {
                        Directory.CreateDirectory(assetPath);
                    }

                    if (assetPath.EndsWith("/") == false)
                    {
                        assetPath += "/";
                    }


                    string fullpath = assetPath + assetName;
                    if (fullpath.EndsWith(".png") == false)
                    {
                        fullpath += ".png";
                    }

                    File.WriteAllBytes(fullpath, png);

                    AssetDatabase.Refresh();
                }

                if (GUILayout.Button("Regenerate Texture"))
                {
                    DestroyImmediate(curGeneratedTexture);
                    curGeneratedTexture = GenerateTexture();
                }
            }


        }

        private Texture2D GenerateTexture()
        {
            if(noiseType == TextureType.Gradient)
            {
                return GenerateGradientTexture();
            }
            else
            {
                return GenerateNoiseTexture();
            }
        }

        private Texture2D GenerateNoiseTexture()
        {
            if (textureSize.x <= 0 || textureSize.y <= 0)
            {
                Debug.LogError("Cannot create a texture with 0 or negative size...");
                return null;
            }

            Texture2D newTex = new Texture2D(textureSize.x, textureSize.y);
            Color[] pixels = new Color[textureSize.x * textureSize.y];

            if (noiseType != TextureType.PerlinNoise)
            {
                for (int i = 0; i < pixels.Length; i++)
                {
                    if (noiseType == TextureType.NormalNoise)
                    {
                        pixels[i] = new Color(Random.value, Random.value, Random.value, 1.0f);
                    }
                    else
                    {
                        if (useColors)
                        {
                            pixels[i] = colors[Random.Range(0, colors.Length)] * Random.value;
                        }
                        else
                        {
                            float r = Random.value;
                            pixels[i] = new Color(r, r, r, 1);
                        }
                    }
                }
            }
            else
            {
                for (int y = 0; y < textureSize.y; y++)
                {
                    for (int x = 0; x < textureSize.x; x++)
                    {
                        float xCoord = perlinOrigin.x + ((float)x / (float)textureSize.x) * perlinScale.x;
                        float yCoord = perlinOrigin.y + ((float)y / (float)textureSize.y) * perlinScale.y;

                        float r = Mathf.PerlinNoise(xCoord, yCoord);
                        pixels[(y * textureSize.x) + x] = new Color(r, r, r, 1);
                    }
                }
            }

            newTex.SetPixels(pixels);
            newTex.Apply();
            return newTex;
        }

        private Texture2D GenerateGradientTexture()
        {
            if (textureSize.x <= 0)
            {
                Debug.LogError("Cannot create a texture with 0 or negative size...");
                return null;
            }

            Texture2D newTex = new Texture2D(textureSize.x, 1);
            Color[] pixels = new Color[textureSize.x];

            for(int i = 0; i < textureSize.x; i++)
            {
                float t = (float)i / (float)textureSize.x;
                pixels[i] = gradient.Evaluate(t);
            }

            newTex.SetPixels(pixels);
            newTex.Apply();
            return newTex;
        }
    }
}