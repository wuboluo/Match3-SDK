public class NotAvailableState : GridTile
{
    public override int TypeId => (int)TileType.Unavailable;
    public override bool IsLocked => true;
    public override bool CanContainItem => false;
}