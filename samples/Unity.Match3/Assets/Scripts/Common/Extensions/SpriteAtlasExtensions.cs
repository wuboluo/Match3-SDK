using UnityEngine;
using UnityEngine.U2D;

public static class SpriteAtlasExtensions
{
    public static Sprite[] GetSprites(this SpriteAtlas spriteAtlas)
    {
        var sprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(sprites);

        return sprites;
    }
}