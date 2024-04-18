namespace Match3
{
    public class Game_SlotComponent : Component
    {
        public Game_SlotComponent(Game_GridComponent gridComponent)
        {
            GridComponent = gridComponent;
        }

        // 由于 Game_ItemComponent 没有继承Component，但其实也是 SlotComponent subComponent 的概念，所以这里用一个属性存一下
        public Game_ItemComponent ItemComponent { get; private set; }
        public Game_GridComponent GridComponent { get; }

        public int ItemId => ItemComponent.ContentId;
        public bool HasItem => ItemComponent != null;

        public void SetItem(Game_ItemComponent itemComponent)
        {
            ItemComponent = itemComponent;
        }
    }
}