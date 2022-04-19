using System;
using System.Runtime.CompilerServices;
using Match3.Core.Interfaces;
using Match3.Core.Structs;
using Terminal.Match3.Interfaces;

namespace Terminal.Match3
{
    public sealed class TerminalGridSlot : ITerminalGridSlot
    {
        private readonly IGridSlotState _state;

        public TerminalGridSlot(IGridSlotState state, GridPosition gridPosition)
        {
            _state = state;
            GridPosition = gridPosition;
        }

        public int ItemId => Item.ContentId;

        public bool HasItem => Item != null;
        public bool IsLocked => _state.IsLocked;
        public bool CanContainItem => _state.CanContainItem;
        public bool IsMovable => IsLocked == false && HasItem;
        public bool CanSetItem => CanContainItem && HasItem == false;

        public ITerminalItem Item { get; private set; }
        public GridPosition GridPosition { get; }

        public void SetItem(ITerminalItem item)
        {
            EnsureItemIsNotNull(item);
            Item = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureItemIsNotNull(ITerminalItem item)
        {
            if (item == null)
            {
                throw new NullReferenceException(nameof(item));
            }
        }
    }
}