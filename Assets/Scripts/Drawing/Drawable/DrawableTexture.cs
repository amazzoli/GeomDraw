using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace GeomDraw
{
    public class DrawableTexture : IDrawable
    {
        public Color[] Pixels { get; private set; }
        /// <summary> Bottom left corner of the rect spanning the texture in world units </summary>
        public Vector2 Origin { get; private set; }
        /// <summary> Size the rect spanning the texture in world units </summary>
        //public Vector2 Size { get; private set; }
        public int NPixelsX { get; private set; }
        public int NPixelsY { get; private set; }
        public float PxPerUnit { get; private set; }


        public DrawableTexture(Color[] pixels, int nPixelsX, Vector2 origin, float pixelPerUnit)
        {
            Pixels = pixels;
            Origin = origin;
            NPixelsX = nPixelsX;
            NPixelsY = (int)Mathf.RoundToInt(pixels.Length / (float)nPixelsX);
            PxPerUnit = pixelPerUnit;
        }

        public Vector2 Size => new Vector2(NPixelsX / PxPerUnit, NPixelsY / PxPerUnit);

        public Vector2 Center => new Vector2(Origin.x + Size.x / 2.0f, Origin.y + Size.y / 2.0f);

        public bool CheckDrawability(float pixelsPerUnit)
        {
            if (pixelsPerUnit <= 0 || NPixelsX <= 0 || NPixelsY <= 0)
                return false;

            return true;
        }

        public IDrawable Copy()
        {
            Color[] newPixels = new Color[Pixels.Length];
            for (int i = 0; i < Pixels.Length; i++) newPixels[i] = Pixels[i];
            return new DrawableTexture(newPixels, NPixelsX, new Vector2(Origin.x, Origin.y), PxPerUnit);
        }

        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            float cDef = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cDef += Center.x;
                Origin = new Vector2(factor * (Origin.x - cDef) + cDef, Origin.y);
                DeformPixels(new Vector2(factor, 1));
            }
            else
            {
                if (isRelative) cDef += Center.y;
                Origin = new Vector2(Origin.x, factor * (Origin.y - cDef) + cDef);
                DeformPixels(new Vector2(1, factor));
            }
            return true;
        }

        /// <summary>
        /// Rescaling the texture with a new pixel-per-unit scale and conserving the old size in 
        /// world units
        /// </summary>
        public void Deform(float newPxUnit)
        {

            //Vector2 oldSize = new Vector2(Size.x, Size.y);
            //int newNPixelsX = Mathf.CeilToInt(oldSize.x * PxPerUnit);
            //int newNPixelsY = Mathf.CeilToInt(oldSize.y * PxPerUnit);
            //Vector2 newSize = new Vector2(newNPixelsX / PxPerUnit, newNPixelsY / PxPerUnit);
            float factor = newPxUnit / PxPerUnit;
            PxPerUnit = newPxUnit;
            DeformPixels(Vector2.one * factor);
        }

        Vector2 halfDeltaSize;
        int newNPxX, newNPxY;
        float fx, fy;
        int newJ, newI;
        private void DeformPixels(Vector2 defFactors)
        {
            newNPxX = Mathf.CeilToInt(NPixelsX * defFactors.x);
            newNPxY = Mathf.CeilToInt(NPixelsY * defFactors.y);

            // Collapsing to zero small deviations of delta size to avoid weird numerical errors
            float dsx = newNPxX - NPixelsX * defFactors.x;
            if (dsx < 0.001) dsx = 0;
            float dsy = newNPxY - NPixelsY * defFactors.y;
            if (dsy < 0.001) dsy = 0;
            halfDeltaSize = new Vector2(dsx, dsy) / 2.0f;
            Origin -= halfDeltaSize / PxPerUnit;

            Color[] newPx = Enumerable.Repeat(new Color(0, 0, 0, 0), newNPxX * newNPxY).ToArray();

            for (int i = 0; i < NPixelsY; i++)
            {
                for (int j = 0; j < NPixelsX; j++)
                {
                    // Boundaries of the old-pixel in the system of reference of the new size
                    float xLeft = j * defFactors.x + halfDeltaSize.x, xRight = xLeft + defFactors.x;
                    newJ = Mathf.FloorToInt(xLeft);
                    while (newJ + 1 < xRight)
                    {
                        fx = Mathf.Min(1, newJ + 1 - xLeft);
                        ScanOverYDeformPixel(i, defFactors, newPx, Pixels[i * NPixelsX + j]);
                        xLeft = newJ + 1;
                        newJ += 1;
                    }
                    fx = xRight - Mathf.Max(newJ, xLeft);
                    ScanOverYDeformPixel(i, defFactors, newPx, Pixels[i * NPixelsX + j]);
                }
            }

            NPixelsX = newNPxX;
            NPixelsY = newNPxY;
            Pixels = newPx;

            // Correcting for partial overlap of the pixels at the border
            for (int i = 0; i < NPixelsY; i++)
            {
                float alpha = Pixels[i * NPixelsX].a;
                Pixels[i * NPixelsX] /= (1 - halfDeltaSize.x);
                Pixels[i * NPixelsX].a = alpha;
                alpha = Pixels[i * NPixelsX + NPixelsX - 1].a;
                Pixels[i * NPixelsX + NPixelsX - 1] /= (1 - halfDeltaSize.x);
                Pixels[i * NPixelsX + NPixelsX - 1].a = alpha;
            }
            for (int j = 0; j < NPixelsX; j++)
            {
                float alpha = Pixels[j].a;
                Pixels[j] /= (1 - halfDeltaSize.y);
                Pixels[j].a = alpha;
                alpha = Pixels[(NPixelsY - 1) * NPixelsX + j].a;
                Pixels[(NPixelsY - 1) * NPixelsX + j] /= (1 - halfDeltaSize.y);
                Pixels[(NPixelsY - 1) * NPixelsX + j].a = alpha;
            }
        }


        private void ScanOverYDeformPixel(int i, Vector2 defFactors, Color[] newPx, Color oldColor)
        {
            float yDown = i * defFactors.y + halfDeltaSize.y, yUp = yDown + defFactors.y;
            newI = Mathf.FloorToInt(yDown);
            while(newI + 1 < yUp)
            {
                fy = Mathf.Min(1, newI + 1 - yDown);
                newPx[newI * newNPxX + newJ] += fx * fy * oldColor;
                yDown = newI + 1;
                newI += 1;
            }
            fy = yUp - Mathf.Max(newI, yDown);
            newPx[newI * newNPxX + newJ] += fx * fy * oldColor;
        }


        public void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            throw new System.NotImplementedException();
        }

        /// <summary> The center has to be in pixel coordinates </summary>
        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            if (isRelative)
            {
                Vector2 pxCenter = new Vector2(NPixelsX * 0.5f, NPixelsY * 0.5f);
                rotCenter += pxCenter;
            }

            Vector2[] rectVerts = new Vector2[4] {
                Vector2.zero, new Vector2(0, NPixelsY), new Vector2(NPixelsX, NPixelsY), new Vector2(NPixelsY, 0) 
            };
            // Finding the dimensions of the rotated sprites
            Vector2 rotVert0 = Utl.Rotate(rectVerts[0] - rotCenter, radAngle) + rotCenter;
            float minX = rotVert0.x, maxX = rotVert0.x, minY = rotVert0.y, maxY = rotVert0.y;
            for (int i = 0; i < 4; i++)
            {
                Vector2 rotVert = Utl.Rotate(rectVerts[i] - rotCenter, radAngle) + rotCenter;
                if (rotVert.x < minX) minX = rotVert.x;
                if (rotVert.y < minY) minY = rotVert.y;
                if (rotVert.x > maxX) maxX = rotVert.x;
                if (rotVert.y > maxY) maxY = rotVert.y;
            }
            //int newNPixelsX = (int)(Mathf.Ceil((maxX - minX) * NPixelsX / Size.x) + 1);
            //int newNPixelsY = (int)(Mathf.Ceil((maxY - minY) * NPixelsY / Size.y) + 1);
            //Origin = new Vector2(minX, minY);
            //Size = new Vector2(newNPixelsX * Size.x / NPixelsX, newNPixelsY * Size.y / NPixelsY);

            //Vector2 versorX = new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
            //Vector2 versorY = new Vector2(-versorX.y, versorX.x);

            //Color[] newPixels = new Color[newNPixelsX * newNPixelsY];
            //float[] norm = new float[newNPixelsX * newNPixelsY];
            //Vector2 pxOrigin = new Vector2(Origin.x * newNPixelsX / Size.x, Origin.y * newNPixelsY / Size.y);
            //rotVert0 = new Vector2(rotVert0.x * newNPixelsX / Size.x, rotVert0.y * newNPixelsY / Size.y);
            //for (int i = 0; i < NPixelsY; i++)
            //{
            //    for (int j = 0; j < NPixelsX; j++)
            //    {
            //        // Center of the old pixel i,j in the new reference with rotation
            //        Vector2 pxCenter = rotVert0 + (j + 0.5f) * versorX + (i + 0.5f) * versorY - pxOrigin;
            //        Color oldColor = Pixels[i * NPixelsX + j];

            //        // Contribution of the bottom left pixel
            //        int newJ = (int)(pxCenter.x - 0.5f), newI = (int)(pxCenter.y - 0.5f);
            //        float fx = newJ + 1.5f - pxCenter.x, fy = newI + 1.5f - pxCenter.y;
            //        SetColorRotation(newPixels, norm, newI, newJ, fx, fy, oldColor, newNPixelsX, newNPixelsY);
            //        // Contribution of the top left pixel
            //        newI = (int)(pxCenter.y + 0.5f);
            //        fy = pxCenter.y + 0.5f - newI;
            //        SetColorRotation(newPixels, norm, newI, newJ, fx, fy, oldColor, newNPixelsX, newNPixelsY);
            //        // Contribution of the top right pixel
            //        newJ = (int)(pxCenter.x + 0.5f);
            //        fx = pxCenter.x + 0.5f - newJ;
            //        SetColorRotation(newPixels, norm, newI, newJ, fx, fy, oldColor, newNPixelsX, newNPixelsY);
            //        // Contribution of the bottom right pixel
            //        newI = (int)(pxCenter.y - 0.5f);
            //        fy = newI + 1.5f - pxCenter.y;
            //        SetColorRotation(newPixels, norm, newI, newJ, fx, fy, oldColor, newNPixelsX, newNPixelsY);
            //    }
            //}

            //NPixelsX = newNPixelsX; NPixelsY = newNPixelsY;
            //Pixels = new Color[NPixelsX * NPixelsY];
            //for (int i = 0; i < Pixels.Length; i++)
            //{
            //    if (norm[i] > 0)
            //    {
            //        Pixels[i] = newPixels[i] / norm[i];
            //        //Pixels[i].a = norm[i];
            //    }
            //}

        }

        public void Translate(Vector2 translation)
        {
            Origin += translation;
        }

        private void SetColorRotation(Color[] newPixels, float[] norm, int newI, int newJ, float fx, float fy, Color oldColor, int newNPixelsX, int newNPixelsY)
        {
            if (fx > 0 && fy > 0)
            {
                int flatNewI = newI * newNPixelsX + newJ;
                if (flatNewI < newNPixelsX * newNPixelsY)
                {
                    norm[flatNewI] += fx * fy;
                    newPixels[flatNewI] += fx * fy * oldColor;
                }
                else
                    Debug.Log(newJ + " " + newI + "\t" + flatNewI);
            }
            else
                Debug.Log(newJ + " " + newI + "\t" + fx + " " + fy);
        }
    }
}