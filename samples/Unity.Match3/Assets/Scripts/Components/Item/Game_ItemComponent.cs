using UnityEngine;

namespace Match3
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Game_ItemComponent : MonoBehaviour
    {
        public int ContentId { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public Transform Transform => transform;

        private System.Random _random;

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            _random = new System.Random();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            SetSprite();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SetSprite()
        {
            var sprites = World.Instance.artAtlas.GetSprites();
            
            ContentId = _random.Next(0, sprites.Length);
            SpriteRenderer.sprite = sprites[ContentId];
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