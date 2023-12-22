using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;


namespace GeomDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DrawnSprite : MonoBehaviour
    {
        // public SpriteRenderer prova;
        Sprite oldSprite = null;
        [HideInInspector] public SpriteRenderer spriteRenderer;

        public IDrawable lastDrawable { get; private set; }

        public bool Undoable { get; private set; }

        public void Init()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            oldSprite = null;
            Undoable = false;
        }

        public void NewDraw(IDrawable drawable)
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            oldSprite = DeepCopySprite(spriteRenderer.sprite);
            lastDrawable = drawable;
            Undoable = true;
        }

        public void NewDraw()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            oldSprite = DeepCopySprite(spriteRenderer.sprite);
            Undoable = true;
        }

        public void Undo()
        {
            if (Undoable)
            {
                spriteRenderer.sprite = oldSprite;
                Undoable = false;
                lastDrawable = null;
            }
        }

        public void SavePng(string name)
        {
            SavePng(name, Application.dataPath + "/../SaveImages/");
        }

        public void SavePng(string name, string dirPath)
        {
            byte[] bytes = spriteRenderer.sprite.texture.EncodeToPNG();
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            File.WriteAllBytes(dirPath + name + ".png", bytes);
            Debug.Log("Image saved at " + dirPath + name + ".png");
        }

        private Sprite DeepCopySprite(Sprite sprite)
        {
            Texture2D oldTexture = sprite.texture;
            Texture2D newTexture = new Texture2D(oldTexture.width, oldTexture.height);
            newTexture.SetPixels(oldTexture.GetPixels());
            newTexture.Apply();
            Rect newRect = new Rect(sprite.rect);
            Vector2 pivot = new Vector2(sprite.pivot.x / newRect.width, sprite.pivot.y / newRect.height);
            return Sprite.Create(newTexture, newRect, pivot, sprite.pixelsPerUnit);
        }
    }
}

