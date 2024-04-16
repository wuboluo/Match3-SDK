namespace Match3
{
    public class AvailableState : SpriteGridTile
    {
        public override int TypeId => (int)TileType.Available;
        public override bool IsLocked => false;
        public override bool CanContainItem => true;
    }
}