using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomEditor(typeof(MapPatternDefinition)), CanEditMultipleObjects]
public class MapShapeEditor : Editor
{

    private static readonly string[] tileTypes = {
        "normal_tile", "block_tile", "swamp_tile"
    };


    public override void OnInspectorGUI()
    {
        if (Event.current.type == EventType.Layout)
        {
            return;
        }

        Rect position = new Rect(0, 50, Screen.width, Screen.height - 50);


        foreach (var item in targets)
        {
            if (position.height < EditorGUIUtility.singleLineHeight * 2)
            {
                continue;
            }

            MapPatternDefinition mapPattern = item as MapPatternDefinition;
            float usedHeight = InspectMapPattern(position, mapPattern);
            position.y += usedHeight;
        }
    }

    public static float InspectMapPattern(Rect position, MapPatternDefinition mapPattern)
    {
        float usedHeight = 0f;
        float quarterPositionX = position.width * 0.5f;

        int newWidth = EditorGUI.IntField(
            new Rect(position.x + quarterPositionX * 0, position.y, quarterPositionX, EditorGUIUtility.singleLineHeight),
            "Width:",
            mapPattern.Width
        );

        int newHeight = EditorGUI.IntField(
            new Rect(position.x + quarterPositionX * 1, position.y, quarterPositionX, EditorGUIUtility.singleLineHeight),
            "Height:",
            mapPattern.Height
        );

        position.y += EditorGUIUtility.singleLineHeight;
        usedHeight += EditorGUIUtility.singleLineHeight;

        mapPattern.offset.x = EditorGUI.IntField(
            new Rect(position.x + quarterPositionX * 0, position.y, quarterPositionX, EditorGUIUtility.singleLineHeight),
            "OffsetX:",
            mapPattern.offset.x
        );

        mapPattern.offset.y = EditorGUI.IntField(
            new Rect(position.x + quarterPositionX * 1, position.y, quarterPositionX, EditorGUIUtility.singleLineHeight),
            "OffsetY:",
            mapPattern.offset.y
        );

        position.y += EditorGUIUtility.singleLineHeight;
        usedHeight += EditorGUIUtility.singleLineHeight;

        if (newWidth != mapPattern.Width || newHeight != mapPattern.Height)
        {
            GUI.changed = true;
            mapPattern.Resize(newWidth, newHeight);
        }
        else
        {
            GUI.changed = false;
        }

        float xWidth = Mathf.Min(
            position.width / Mathf.Max(1, mapPattern.Width),
            position.width / Mathf.Max(1, mapPattern.Height)
        );
        xWidth = Mathf.Min(xWidth, 40);

        GUIStyle fontStyle = new GUIStyle(EditorStyles.textField);
        fontStyle.fontSize = Mathf.FloorToInt(xWidth * 0.7f);

        // Draw map offset
        EditorGUI.DrawRect(new Rect(
            position.x + mapPattern.offset.x * xWidth,
            position.y + mapPattern.offset.y * xWidth,
            (mapPattern.Width - mapPattern.offset.x * 2) * xWidth,
            (mapPattern.Height - mapPattern.offset.y * 2) * xWidth
        ), Color.red);


        for (uint x = 0; x < mapPattern.Width; x++)
        {
            for (uint y = 0; y < mapPattern.Height; y++)
            {
                uint mapY = (uint)mapPattern.Height - 1 - y;
                int res = EditorGUI.IntField(
                    new Rect(
                        position.x + xWidth * x,
                        position.y + xWidth * y,
                        xWidth,
                        xWidth
                    ),
                    IndexOfTile(mapPattern.GetData(x, mapY)),
                    fontStyle
                );

                mapPattern.SetData(x, mapY, tileTypes[res]);
            }
        }

        usedHeight += mapPattern.Height * xWidth;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(mapPattern);
        }

        return usedHeight;
    }

    private static int IndexOfTile(string value)
    {
        var indexOf = Array.IndexOf(tileTypes, value);
        if (indexOf == 3)
        {
            indexOf = 1;
        }
        return Math.Max(0, indexOf);
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect targetRect, GUIStyle background)
    {
        MapPatternDefinition mapPattern = target as MapPatternDefinition;
        if (mapPattern == null)
        {
            return;
        }

        float blockSize = Mathf.Min(
            targetRect.width / mapPattern.Width,
            targetRect.height / mapPattern.Height
        );

        // Get size
        float offX = (targetRect.width - blockSize * mapPattern.Width) * 0.5f + targetRect.x;
        float offY = (targetRect.height - blockSize * mapPattern.Height) * 0.5f + targetRect.y;

        // Get max
        int maxStrength = tileTypes.Length;
        float divMax = 1.0f / maxStrength;

        for (uint x = 0; x < mapPattern.Width; x++)
        {
            for (uint y = 0; y < mapPattern.Height; y++)
            {
                uint mapY = (uint)mapPattern.Height - 1 - y;
                if (mapPattern.GetData(x, mapY) != "")
                {
                    EditorGUI.DrawRect(
                        new Rect(
                            offX + (int)x * blockSize + 1,
                            offY + (int)y * blockSize + 1,
                            blockSize - 2,
                            blockSize - 2
                        ),
                        new Color(0, 0, 0, IndexOfTile(mapPattern.GetData(x, mapY)) * divMax)
                    );
                }
            }
        }
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int texWidth, int texHeight)
    {
        MapPatternDefinition mapPattern = target as MapPatternDefinition;
        if (mapPattern == null)
        {
            return null;
        }

        Texture2D staticPreview = new Texture2D(texWidth, texHeight);
        float blockSize = Mathf.Min(
            texWidth / mapPattern.Width,
            texHeight / mapPattern.Height
        );

        // Get size
        float offX = (texWidth - blockSize * mapPattern.Width) * 0.5f;
        float offY = (texHeight - blockSize * mapPattern.Height) * 0.5f;

        // Get max
        int maxStrength = tileTypes.Length;
        float divMax = 1.0f / maxStrength;

        // Set blank
        Color32 blankColor = new Color32(255,255,255,0);
        Color32[] colBlock = new Color32[texWidth * texHeight];

        for (int i = 0; i < colBlock.Length; i++)
        {
            colBlock[i] = blankColor;
        }
        staticPreview.SetPixels32(colBlock);

        for (uint x = 0; x < mapPattern.Width; x++)
        {
            for (uint y = 0; y < mapPattern.Height; y++)
            {
                uint mapY = (uint)mapPattern.Height - 1 - y;
                if (mapPattern.GetData(x, mapY) != "")
                {
                    int sx = (int)(offX + (int) x * blockSize + 1);
                    int sy = (int)(texHeight - (offY + (int) y * blockSize + 1) - blockSize);

                    Color color = new Color(0, 0, 0, IndexOfTile(mapPattern.GetData(x, mapY)) * divMax);

                    for (int px = 0; px < blockSize - 2; px++)
                    {
                        for (int py = 0; py < blockSize - 2; py++)
                        {
                            staticPreview.SetPixel(sx + px, sy + py, color);
                        }
                    }
                }
            }
        }
        staticPreview.Apply();
        return staticPreview;

    }
}
