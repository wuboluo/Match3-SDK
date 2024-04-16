using System;
using UnityEngine;

// 棋子模型
[Serializable]
public class TileModel
{
    [SerializeField] private TileType _type;
    [SerializeField] private GameObject _prefab;

    public TileType Type => _type;
    public GameObject Prefab => _prefab;
}