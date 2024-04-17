using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Game_ItemComponent : MonoBehaviour
    {
        public int ContentId { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public Transform Transform => transform;

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
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
            SpriteRenderer.sprite = sprite;
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
            transform.localScale = new Vector3(value, value, 1);
        }
    }
}