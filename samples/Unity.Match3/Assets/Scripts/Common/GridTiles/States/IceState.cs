using Common.Enums;

namespace Common.GridTiles.States
{
    public class IceState : StatefulGridTile
    {
        private bool _isLocked = true;
        private int _group = (int) TileType.Ice;

        public override int TypeId => _group;
        public override bool IsLocked => _isLocked;
        public override bool CanContainItem => true;

        protected override void OnComplete()
        {
            _isLocked = false;
            _group = (int) TileType.Available;
        }

        protected override void OnReset()
        {
            _isLocked = true;
            _group = (int) TileType.Ice;
        }
    }
}