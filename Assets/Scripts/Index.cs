public struct Index
{
    public int row;
    public int col;

    public Index(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public override string ToString() => $"{row}, {col}";

    public static Index operator +(Index a, Index b) => new Index(a.row + b.row, a.col + b.col);
    public static Index operator -(Index a, Index b) => new Index(a.row - b.row, a.col - b.col);
    public static bool operator ==(Index a, Index b) => a.row == b.row && a.col == b.col;
    public static bool operator !=(Index a, Index b) => !(a == b);
    public override bool Equals(object obj) => obj is Index && this == (Index)obj;
    public override int GetHashCode()
    {
        int hashCode = -1720622044;
        hashCode = hashCode * -1521134295 + row.GetHashCode();
        hashCode = hashCode * -1521134295 + col.GetHashCode();
        return hashCode;
    }
}