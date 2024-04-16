using System;
using UnityEngine;

public interface IGridTile : IGridSlotState, IDisposable
{
    void SetActive(bool value);
    void SetWorldPosition(Vector3 worldPosition);
}