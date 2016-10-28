using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using UnityEditor;
using UnityEngine.Assertions.Comparers;

[CustomEditor(typeof(AttackPatternDefinition)), CanEditMultipleObjects]
public class AttackShapeEditor : Editor {

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

            AttackPatternDefinition attackPattern = item as AttackPatternDefinition;
            float usedHeight = InspectAttackPattern(position, attackPattern);
            position.y += usedHeight;
        }
    }

    public static float InspectAttackPattern(Rect position, AttackPatternDefinition attackPatternDefinition)
    {
        float usedHeight = 0f;
        float halfPositionX = position.width * 0.5f;

        int newWidth = EditorGUI.IntField(
            new Rect(position.x, position.y, halfPositionX, EditorGUIUtility.singleLineHeight),
            "Width:",
            attackPatternDefinition.Width
        );

        int newHeight = EditorGUI.IntField(
            new Rect(position.x + halfPositionX, position.y, halfPositionX, EditorGUIUtility.singleLineHeight),
            "Height:",
            attackPatternDefinition.Height
        );

        position.y += EditorGUIUtility.singleLineHeight;
        usedHeight += EditorGUIUtility.singleLineHeight;

        if (newWidth != attackPatternDefinition.Width || newHeight != attackPatternDefinition.Height)
        {
            GUI.changed = true;
            attackPatternDefinition.Resize(newWidth, newHeight);
        }
        else
        {
            GUI.changed = false;
        }

        float xWidth = Mathf.Min(
            position.width / Mathf.Max(1, attackPatternDefinition.Width),
            position.width / Mathf.Max(1, attackPatternDefinition.Height)
        );
        xWidth = Mathf.Min(xWidth, 40);

        GUIStyle fontStyle = new GUIStyle(EditorStyles.textField);
        fontStyle.fontSize = Mathf.FloorToInt(xWidth * 0.7f);

        for (uint x = 0; x < attackPatternDefinition.Width; x++)
        {
            for (uint y = 0; y < attackPatternDefinition.Height; y++)
            {
                int res = EditorGUI.IntField(
                    new Rect(
                        position.x + xWidth * x,
                        position.y + xWidth * y,
                        xWidth,
                        xWidth
                    ),
                    attackPatternDefinition.GetData(x, y),
                    fontStyle
                );

                attackPatternDefinition.SetData(x, y, res);
            }
        }

        usedHeight += attackPatternDefinition.Height * xWidth;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(attackPatternDefinition);
        }

        return usedHeight;
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect targetRect, GUIStyle background)
    {
        AttackPatternDefinition attackPattern = target as AttackPatternDefinition;
        if (attackPattern == null)
        {
            return;
        }

        float blockSize = Mathf.Min(
            targetRect.width / attackPattern.Width,
            targetRect.height / attackPattern.Height
        );

        // Get size
        float offX = (targetRect.width - blockSize * attackPattern.Width) * 0.5f + targetRect.x;
        float offY = (targetRect.height - blockSize * attackPattern.Height) * 0.5f + targetRect.y;

        // Get max
        int maxStrength = attackPattern.GetMax();
        float divMax = 1.0f / maxStrength;

        for (uint x = 0; x < attackPattern.Width; x++)
        {
            for (uint y = 0; y < attackPattern.Height; y++)
            {
                if (attackPattern.GetData(x, y) > 0)
                {
                    EditorGUI.DrawRect(
                        new Rect(
                            offX + (int)x * blockSize + 1,
                            offY + (int)y * blockSize + 1,
                            blockSize - 2,
                            blockSize - 2
                        ),
                        new Color(0, 0, 0, attackPattern.GetData(x, y) * divMax)
                    );
                }
            }
        }
    }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int texWidth, int texHeight)
    {
        AttackPatternDefinition attackPattern = target as AttackPatternDefinition;
        if (attackPattern == null)
        {
            return null;
        }

        Texture2D staticPreview = new Texture2D(texWidth, texHeight);
        float blockSize = Mathf.Min(
            texWidth / attackPattern.Width,
            texHeight / attackPattern.Height
        );

        // Get size
        float offX = (texWidth - blockSize * attackPattern.Width) * 0.5f;
        float offY = (texHeight - blockSize * attackPattern.Height) * 0.5f;

        // Get max
        int maxStrength = attackPattern.GetMax();
        float divMax = 1.0f / maxStrength;

        // Set blank
        Color32 blankColor = new Color32(255,255,255,0);
        Color32[] colBlock = new Color32[texWidth * texHeight];

        for (int i = 0; i < colBlock.Length; i++)
        {
            colBlock[i] = blankColor;
        }
        staticPreview.SetPixels32(colBlock);

        for (uint x = 0; x < attackPattern.Width; x++)
        {
            for (uint y = 0; y < attackPattern.Height; y++)
            {
                if (attackPattern.GetData(x, y) > 0)
                {
                    int sx = (int)(offX + (int) x * blockSize + 1);
                    int sy = (int)(texHeight - (offY + (int) y * blockSize + 1) - blockSize);

                    Color color = new Color(0, 0, 0, attackPattern.GetData(x, y) * divMax);

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
