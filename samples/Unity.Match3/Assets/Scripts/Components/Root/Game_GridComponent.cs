namespace Match3
{
    public class Game_GridComponent : Component
    {
        private readonly Grid _grid;

        public Game_GridComponent(int row, int column)
        {
            _grid = new Grid(row, column);
        }

        public Grid GetGrid()
        {
            return _grid;
        }
    }
}