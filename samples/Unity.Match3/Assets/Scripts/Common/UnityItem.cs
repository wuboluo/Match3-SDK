using UnityEngine;

namespace Match3
{
    public class UnityItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _isDestroyed;

        public int ContentId { get; private set; }
        public Transform Transform => transform;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private void OnDestroy()
        {
            _isDestroyed = true;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetSprite(int spriteId, Sprite sprite)
        {
            ContentId = spriteId;
            _spriteRenderer.sprite = sprite;
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }

        public void SetScale(float value)
        {
            transform.localScale = new Vector3(value, value, value);
        }

        public void Dispose()
        {
            if (_isDestroyed == false) Destroy(gameObject);
        }
    }
}