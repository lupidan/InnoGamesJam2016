using System;

[System.Serializable]
public struct Position {
	public int x;
	public int y;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

	public override bool Equals(Object obj) 
	{
		return obj is Position && this == (Position)obj;
	}
	public override int GetHashCode() 
	{
		return x.GetHashCode() ^ y.GetHashCode();
	}
	public static bool operator ==(Position a, Position b) 
	{
		return a.x == b.x && a.y == b.y;
	}
	public static bool operator !=(Position a, Position b) 
	{
		return !(a == b);
	}
		
}
