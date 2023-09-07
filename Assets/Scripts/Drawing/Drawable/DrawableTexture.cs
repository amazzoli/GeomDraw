using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    public class DrawableTexture : IDrawable
    {
        public Color[] Pixels { get; private set; }
        /// <summary> Bottom left corner of the rect spanning the texture in world units </summary>
        public Vector2 Origin { get; private set; }
        /// <summary> Size the rect spanning the texture in world units </summary>
        public Vector2 Size { get; private set; }
        public int NPixelsX { get; private set; }
        public int NPixelsY { get; private set; }


        public DrawableTexture(Color[] pixels, int nPixelsX, Vector2 origin, Vector2 size)
        {
            Pixels = pixels;
            Origin = origin;
            Size = size;
            NPixelsX = nPixelsX;
            NPixelsY = (int)Mathf.RoundToInt(pixels.Length / (float)nPixelsX);
        }

        public DrawableTexture(Color[] pixels, int nPixelsX, Vector2 origin, float pixelPerUnit)
        {
            Pixels = pixels;
            Origin = origin;
            NPixelsX = nPixelsX;
            NPixelsY = (int)Mathf.RoundToInt(pixels.Length / (float)nPixelsX);
            Size = new Vector2(NPixelsX / pixelPerUnit, NPixelsY / pixelPerUnit);
        }

        public Vector2 Center 
        {
            get
            {
                return new Vector2(Origin.x + Size.x / 2.0f, Origin.y + Size.y / 2.0f);
            }
        }

        public bool CheckDrawability(float pixelsPerUnit)
        {
            if (Size.x <= 0 || Size.y <= 0 || pixelsPerUnit <= 0 || NPixelsX <= 0 || NPixelsY <=0)
                return false;

            return true;
        }

        public IDrawable Copy()
        {
            Color[] newPixels = new Color[Pixels.Length];
            for (int i = 0; i < Pixels.Length; i++) newPixels[i] = Pixels[i];
            return new DrawableTexture(newPixels, NPixelsX, new Vector2(Origin.x, Origin.y), new Vector2(Size.x, Size.y));
        }

        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            float cDef = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cDef += Center.x;
                Origin = new Vector2(factor * (Origin.x - cDef) + cDef, Origin.y);
                Size = new Vector2(Size.x * factor, Size.y);
            }
            else
            {
                if (isRelative) cDef += Center.y;
                Origin = new Vector2(Origin.x, factor * (Origin.y - cDef) + cDef);
                Size = new Vector2(Size.x, factor * Size.y);
            }
            return true;
        }

        public void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            throw new System.NotImplementedException();
        }

        public void Translate(Vector2 translation)
        {
            Origin += translation;
        }
    }
}