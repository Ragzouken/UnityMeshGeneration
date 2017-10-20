using UnityEngine;

public partial struct IntVector3
{
    public static IntVector3 zero = new IntVector3(0, 0, 0);
    public static IntVector3 one  = new IntVector3(1, 1, 1);

    public static IntVector3 left    = new IntVector3(-1,  0,  0);
    public static IntVector3 right   = new IntVector3( 1,  0,  0);
    public static IntVector3 up      = new IntVector3( 0,  1,  0);
    public static IntVector3 down    = new IntVector3( 0, -1,  0);
    public static IntVector3 forward = new IntVector3( 0,  0,  1);
    public static IntVector3 back    = new IntVector3( 0,  0, -1);

    public static IntVector3[] ortho = new IntVector3[]
    {
        right, down, left, up, forward, back
    };

    public static IntVector3[] adjacent26 = new IntVector3[]
    {
        right, right + down, down, down + left, left, left + up, up, up + right,
        right + forward, right + down + forward, down + forward, down + left + forward, left + forward, left + up + forward, up + forward, up + right + forward,
        right + back, right + down + back, down + back, down + left + back, left + back, left + up + back, up + back, up + right + back,
    };
}

[System.Serializable]
public partial struct IntVector3 : System.IEquatable<IntVector3>
{
    public int x;
    public int y;
    public int z;

    public IntVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public IntVector3(float x, float y, float z) : this((int) x, (int) y, (int) z) { }

    public void GridCoords(int cellSize,
                           out IntVector3 cell,
                           out IntVector3 local)
    {
        cell = CellCoords(cellSize);
        local = OffsetCoords(cellSize);
    }

    public IntVector3 CellCoords(int cellSize)
    {
        return new IntVector3(Mathf.FloorToInt(x / (float) cellSize),
                              Mathf.FloorToInt(y / (float) cellSize),
                              Mathf.FloorToInt(z / (float) cellSize));
    }

    public IntVector3 OffsetCoords(int cellSize)
    {
        float ox = x % cellSize;
        float oy = y % cellSize;
        float oz = z % cellSize;

        return new IntVector3(ox >= 0 ? ox : cellSize + ox,
                              oy >= 0 ? oy : cellSize + oy,
                              oz >= 0 ? oz : cellSize + oz);
    }

    public IntVector3 Moved(int dx, int dy, int dz)
    {
        x += dx;
        y += dy;
        z += dz;

        return this;
    }

    public static implicit operator Vector3(IntVector3 point)
    {
        return new Vector3(point.x, point.y, point.z);
    }

    public static implicit operator IntVector3(Vector3 vector)
    {
        return new IntVector3(vector.x, vector.y, vector.z);
    }

    public override bool Equals (object obj)
    {
        if (obj is IntVector3)
        {
            return Equals((IntVector3) obj);
        }

        return false;
    }

    public bool Equals(IntVector3 other)
    {
        return other.x == x
            && other.y == y
            && other.z == z;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + x.GetHashCode();
        hash = hash * 23 + y.GetHashCode();
        hash = hash * 23 + z.GetHashCode();
        return hash;
    }

    public override string ToString ()
    {
        return string.Format("(x: {0}, y: {1}, z: {2})", x, y, z);
    }

    public static bool operator ==(IntVector3 a, IntVector3 b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(IntVector3 a, IntVector3 b)
    {
        return a.x != b.x || a.y != b.y || a.z != b.z;
    }

    public static IntVector3 operator +(IntVector3 a, IntVector3 b)
    {
        a.x += b.x;
        a.y += b.y;
        a.z += b.z;

        return a;
    }

    public static IntVector3 operator -(IntVector3 a, IntVector3 b)
    {
        a.x -= b.x;
        a.y -= b.y;
        a.z -= b.z;

        return a;
    }
    
    public static IntVector3 operator *(IntVector3 a, int scale)
    {
        a.x *= scale;
        a.y *= scale;
        a.z *= scale;

        return a;
    }
}
