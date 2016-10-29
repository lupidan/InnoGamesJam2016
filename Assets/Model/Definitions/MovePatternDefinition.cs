using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "Assets/MovePattern", order = 1)]
public class MovePatternDefinition : ScriptableObject
{
    public int Width = 1;
    public int Height = 1;
    [SerializeField]
    private int[] Data = new int[1];

    public void SetData(uint x, uint y, int value)
    {
        if (x < Width && y < Height)
        {
            Data[Index(x,y)] = value;
        }
    }

    public int GetData(uint x, uint y)
    {
        if (x < Width && y < Height)
        {
            return Data[Index(x,y)];
        }
        return 0;
    }

    public bool IsCenter(uint x, uint y)
    {
        return x == Width / 2 && y == Height / 2;
    }

    public void Resize(int width, int height)
    {
        width = Mathf.Max(width, 1);
        height = Mathf.Max(height, 1);

        if ((width & 0x1) == 0)
        {
            width += 1;
        }

        if ((height & 0x1) == 0)
        {
            height += 1;
        }

        int[] newData = new int[width * height];
        for (uint x = 0; x < Width && x < width; x++)
        {
            for (uint y = 0; y < Height && y < height; y++)
            {
                newData[x + y * width] = Data[Index(x, y)];
            }
        }

        Width = width;
        Height = height;
        Data = newData;

        if (Width > 0 && Height > 0)
        {
            Data[Width / 2 + Width * (Height / 2)] = 0;
        }
    }

    public int GetMax()
    {
        return Mathf.Max(Data);
    }

    private int Index(uint x, uint y)
    {
        return (int)x + (int)y * Width;
    }
}
