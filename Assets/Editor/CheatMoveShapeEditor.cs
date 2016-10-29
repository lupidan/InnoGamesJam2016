using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheatMovePatternDefinition)), CanEditMultipleObjects]
public class CheatMoveShapeEditor : Editor
{

    private const int RED_COLOR = 0;
    private const int GREEN_COLOR = 200;
    private const int BLUE_COLOR = 150;

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

            CheatMovePatternDefinition movePattern = item as CheatMovePatternDefinition;
            float usedHeight = InspectCheatMovePattern(position, movePattern);
            position.y += usedHeight;
        }
    }

    public static float InspectCheatMovePattern(Rect position, CheatMovePatternDefinition cheatMovePatternDefinition)
    {
        float usedHeight = 0f;
        float halfPositionX = position.width * 0.5f;

        int newWidth = EditorGUI.IntField(
            new Rect(position.x, position.y, halfPositionX, EditorGUIUtility.singleLineHeight),
            "Width:",
            cheatMovePatternDefinition.Width
        );

        int newHeight = EditorGUI.IntField(
            new Rect(position.x + halfPositionX, position.y, halfPositionX, EditorGUIUtility.singleLineHeight),
            "Height:",
            cheatMovePatternDefinition.Height
        );

        position.y += EditorGUIUtility.singleLineHeight;
        usedHeight += EditorGUIUtility.singleLineHeight;

        if (newWidth != cheatMovePatternDefinition.Width || newHeight != cheatMovePatternDefinition.Height)
        {
            GUI.changed = true;
            cheatMovePatternDefinition.Resize(newWidth, newHeight);
        }
        else
        {
            GUI.changed = false;
        }

        float xWidth = Mathf.Min(
            position.width / Mathf.Max(1, cheatMovePatternDefinition.Width),
            position.width / Mathf.Max(1, cheatMovePatternDefinition.Height)
        );
        xWidth = Mathf.Min(xWidth, 40);

        for (uint x = 0; x < cheatMovePatternDefinition.Width; x++)
        {
            for (uint y = 0; y < cheatMovePatternDefinition.Height; y++)
            {
                if (!cheatMovePatternDefinition.IsCenter(x,y))
                {
                    int res = EditorGUI.Toggle(
                        new Rect(
                            position.x + xWidth * x,
                            position.y + xWidth * y,
                            xWidth,
                            xWidth
                        ),
                        cheatMovePatternDefinition.GetData(x, y) != 0
                    ) ? 1 : 0;

                    cheatMovePatternDefinition.SetData(x, y, res);
                }
            }
        }

        usedHeight += cheatMovePatternDefinition.Height * xWidth;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cheatMovePatternDefinition);
        }

        return usedHeight;
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect targetRect, GUIStyle background)
    {
        CheatMovePatternDefinition movePattern = target as CheatMovePatternDefinition;
        if (movePattern == null)
        {
            return;
        }

        float blockSize = Mathf.Min(
            targetRect.width / movePattern.Width,
            targetRect.height / movePattern.Height
        );

        // Get size
        float offX = (targetRect.width - blockSize * movePattern.Width) * 0.5f + targetRect.x;
        float offY = (targetRect.height - blockSize * movePattern.Height) * 0.5f + targetRect.y;

        // Get max
        int maxStrength = movePattern.GetMax();
        float divMax = 1.0f / maxStrength;

        for (uint x = 0; x < movePattern.Width; x++)
        {
            for (uint y = 0; y < movePattern.Height; y++)
            {
                if (movePattern.GetData(x, y) > 0)
                {
                    EditorGUI.DrawRect(
                        new Rect(
                            offX + (int) x * blockSize + 1,
                            offY + (int) y * blockSize + 1,
                            blockSize - 2,
                            blockSize - 2
                        ),
                        new Color(RED_COLOR, GREEN_COLOR, BLUE_COLOR, movePattern.GetData(x, y) * divMax)
                    );
                }
                else if (movePattern.IsCenter(x, y))
                {
                    EditorGUI.DrawRect(
                        new Rect(
                            offX + (int) x * blockSize + 1,
                            offY + (int) y * blockSize + 1,
                            blockSize - 2,
                            blockSize - 2
                        ),
                        new Color(0, 0, 0, 1f)
                    );
                }
            }
        }
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int texWidth, int texHeight)
    {
        CheatMovePatternDefinition movePattern = target as CheatMovePatternDefinition;
        if (movePattern == null)
        {
            return null;
        }

        Texture2D staticPreview = new Texture2D(texWidth, texHeight);
        float blockSize = Mathf.Min(
            texWidth / movePattern.Width,
            texHeight / movePattern.Height
        );

        // Get size
        float offX = (texWidth - blockSize * movePattern.Width) * 0.5f;
        float offY = (texHeight - blockSize * movePattern.Height) * 0.5f;

        // Get max
        int maxStrength = movePattern.GetMax();
        float divMax = 1.0f / maxStrength;

        // Set blank
        Color32 blankColor = new Color32(255,255,255,0);
        Color32[] colBlock = new Color32[texWidth * texHeight];

        for (int i = 0; i < colBlock.Length; i++)
        {
            colBlock[i] = blankColor;
        }
        staticPreview.SetPixels32(colBlock);

        for (uint x = 0; x < movePattern.Width; x++)
        {
            for (uint y = 0; y < movePattern.Height; y++)
            {
                if (movePattern.GetData(x, y) > 0)
                {
                    int sx = (int) (offX + (int) x * blockSize + 1);
                    int sy = (int) (texHeight - (offY + (int) y * blockSize + 1) - blockSize);

                    Color color = new Color(RED_COLOR, GREEN_COLOR, BLUE_COLOR, movePattern.GetData(x, y) * divMax);

                    for (int px = 0; px < blockSize - 2; px++)
                    {
                        for (int py = 0; py < blockSize - 2; py++)
                        {
                            staticPreview.SetPixel(sx + px, sy + py, color);
                        }
                    }
                }
                else if(movePattern.IsCenter(x, y))
                {
                    int sx = (int) (offX + (int) x * blockSize + 1);
                    int sy = (int) (texHeight - (offY + (int) y * blockSize + 1) - blockSize);

                    Color color = new Color(0, 0, 0, 1f);

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
