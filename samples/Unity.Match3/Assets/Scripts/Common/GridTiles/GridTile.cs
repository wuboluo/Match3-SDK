using UnityEngine;

namespace Match3
{
    public abstract class GridTile : MonoBehaviour, IGridTile
    {
        private bool _isDestroyed;

        private void OnDestroy()
        {
            _isDestroyed = true;
        }

        public abstract int TypeId { get; }
        public abstract bool IsLocked { get; }
        public abstract bool CanContainItem { get; }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public void Dispose()
        {
            if (_isDestroyed == false) Destroy(gameObject);
        }
    }
}