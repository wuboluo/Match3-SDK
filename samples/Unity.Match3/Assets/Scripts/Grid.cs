using System;
using System.Runtime.CompilerServices;

namespace Match3
{
    public readonly struct Grid : IEquatable<Grid>
    {
        public Grid(int row, int column)
        {
            RowIndex = row;
            ColumnIndex = column;
        }

        public int RowIndex { get; }

        public int ColumnIndex { get; }

        public static Grid Up { get; } = new(1, 0);

        public static Grid Down { get; } = new(-1, 0);

        public static Grid Left { get; } = new(0, -1);

        public static Grid Right { get; } = new(0, 1);

        public static Grid Zero { get; } = new(0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Grid operator +(Grid a, Grid b)
        {
            return new Grid(a.RowIndex + b.RowIndex, a.ColumnIndex + b.ColumnIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Grid operator -(Grid a, Grid b)
        {
            return new Grid(a.RowIndex - b.RowIndex, a.ColumnIndex - b.ColumnIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Grid other)
        {
            return RowIndex == other.RowIndex && ColumnIndex == other.ColumnIndex;
        }
    }
}