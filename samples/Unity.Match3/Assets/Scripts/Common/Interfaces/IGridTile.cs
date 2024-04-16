using System;
using UnityEngine;

namespace Match3
{
    public interface IGridTile : IGridSlotState, IDisposable
    {
        void SetActive(bool value);
        void SetWorldPosition(Vector3 worldPosition);
    }
}