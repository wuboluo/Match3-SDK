public static class GridMath
{
    /// 所选的格子位置是否在棋盘内
    public static bool IsPositionOnGrid(GameBoard board, GridPosition gridPosition)
    {
        return IsPositionOnGrid(gridPosition, board.RowCount, board.ColumnCount);
    }

    /// 所选的格子位置是否在棋盘内
    public static bool IsPositionOnGrid(GridPosition gridPosition, int rowCount, int columnCount)
    {
        return gridPosition.RowIndex >= 0 &&
               gridPosition.RowIndex < rowCount &&
               gridPosition.ColumnIndex >= 0 &&
               gridPosition.ColumnIndex < columnCount;
    }
}