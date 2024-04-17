namespace Match3
{
    public class Game_SlotComponent : Component
    {
        public Game_SlotComponent(Game_ItemComponent itemComponent, GridPosition gridPosition)
        {
            ItemComponent = itemComponent;
            GridPosition = gridPosition;
        }

        public Game_ItemComponent ItemComponent { get; private set; }
        public GridPosition GridPosition { get; }
        public int ItemId => ItemComponent.ContentId;
        public bool HasItem => ItemComponent != null;

        public void SetItem(Game_ItemComponent itemComponent)
        {
            ItemComponent = itemComponent;
        }

        public void Dispose()
        {
            ItemComponent = null;
        }
    }
}