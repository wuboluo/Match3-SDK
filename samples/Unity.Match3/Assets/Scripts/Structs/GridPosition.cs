using System;
using System.Runtime.CompilerServices;

public readonly struct GridPosition : IEquatable<GridPosition>
{
    public GridPosition(int rowIndex, int columnIndex)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
    }

    public int RowIndex { get; }

    public int ColumnIndex { get; }

    // 行+1
    public static GridPosition Up { get; } = new(1, 0);

    // 行-1
    public static GridPosition Down { get; } = new(-1, 0);

    // 列-1
    public static GridPosition Left { get; } = new(0, -1);

    // 列+1
    public static GridPosition Right { get; } = new(0, 1);

    public static GridPosition Zero { get; } = new(0, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.RowIndex + b.RowIndex, a.ColumnIndex + b.ColumnIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.RowIndex - b.RowIndex, a.ColumnIndex - b.ColumnIndex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.RowIndex == b.RowIndex && a.ColumnIndex == b.ColumnIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return a.RowIndex != b.RowIndex || a.ColumnIndex != b.ColumnIndex;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(GridPosition other)
    {
        return RowIndex == other.RowIndex && ColumnIndex == other.ColumnIndex;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RowIndex.GetHashCode() ^ (ColumnIndex.GetHashCode() << 2);
    }
}