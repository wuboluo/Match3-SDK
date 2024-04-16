using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class UnityItemGenerator
{
    private readonly Transform _container;
    private readonly GameObject _itemPrefab;
    private readonly Random _random;

    private Queue<UnityItem> _itemsPool;

    private Sprite[] _sprites;

    public UnityItemGenerator(GameObject itemPrefab, Transform container)
    {
        _random = new Random();
        _container = container;
        _itemPrefab = itemPrefab;
    }

    public void SetSprites(Sprite[] sprites)
    {
        _sprites = sprites;
    }

    public UnityItem CreateItem()
    {
        var item = _itemPrefab.CreateNew<UnityItem>(parent: _container);
        item.Hide();

        return item;
    }

    public UnityItem ConfigureItem(UnityItem item)
    {
        var index = _random.Next(0, _sprites.Length);
        item.SetSprite(index, _sprites[index]);

        return item;
    }

    public void CreateItems(int capacity)
    {
        if (_itemsPool != null) throw new InvalidOperationException("Items have already been created.");

        _itemsPool = new Queue<UnityItem>(capacity);

        for (var i = 0; i < capacity; i++) _itemsPool.Enqueue(CreateItem());
    }

    public UnityItem FetchItem()
    {
        return ConfigureItem(_itemsPool.Dequeue());
    }

    public void RecycleItem(UnityItem item)
    {
        _itemsPool.Enqueue(item);
    }

    public virtual void Dispose()
    {
        if (_itemsPool == null) return;

        foreach (var item in _itemsPool)
            if (item is IDisposable disposable)
                disposable.Dispose();
            else
                break;

        _itemsPool.Clear();
        _itemsPool = null;
    }
}