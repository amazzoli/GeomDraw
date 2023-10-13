using System.Linq;
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


        public DrawableTexture(Color[] pixels, int nPixelsX, Vector2 origin)
        {
            Pixels = pixels;
            Origin = origin;
            NPixelsX = nPixelsX;
            NPixelsY = (int)Mathf.RoundToInt(pixels.Length / (float)nPixelsX);
        }

        public Vector2 Center(float pixelPerUnits)
        {
            float width = NPixelsX / pixelPerUnits;
            float height = NPixelsY / pixelPerUnits;
            return new Vector2(Origin.x + width / 2.0f, Origin.y + height / 2.0f);
        }

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
            return new DrawableTexture(newPixels, NPixelsX, new Vector2(Origin.x, Origin.y));
        }

        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            ////Sizes in pixel units
            //float wPx = texture.NPixelsX;
            //float hPx = texture.NPixelsY;
            ////Pixel sizes of texture in canvas-pixel units
            //dxHalf = wPx / (float)texture.NPixelsX / 2.0f;
            //dyHalf = hPx / (float)texture.NPixelsY / 2.0f;
            ////Pixels per unit of the texture
            //float pxUnitX = texture.NPixelsX / texture.Size.x;
            //float pxUnitY = texture.NPixelsY / texture.Size.y;
            ////Maximum number of canvas pixels the texture covers 
            //nMaxPxX = (int)Mathf.Ceil(wPx) + 1;
            //nMaxPxY = (int)Mathf.Ceil(hPx) + 1;
            ////Pixel of canvas containing the origin
            //minCanvasI = Mathf.RoundToInt(texture.Origin.y * pxUnit - 0.5f);
            //minCanvasJ = Mathf.RoundToInt(texture.Origin.x * pxUnit - 0.5f);
            ////New colors
            //Color[] newPx = Enumerable.Repeat(new Color(0, 0, 0, 0), nMaxPxX * nMaxPxY).ToArray();
            //// Normalization of texture covering the canvas (necessary for borders where the texture
            //// does not overlap completely the canvas and glitches can appear)
            //float[] norm = new float[newPx.Length];

            //for (int i = 0; i < texture.NPixelsY; i++)
            //{
            //    for (int j = 0; j < texture.NPixelsX; j++)
            //    {
            //        // Coordinate of the new pixel in the system of reference of the canvas pixels
            //        canvasJ = ((j + 0.5f) / pxUnitX + texture.Origin.x) * pxUnit - 0.5f;
            //        canvasI = ((i + 0.5f) / pxUnitY + texture.Origin.y) * pxUnit - 0.5f;

            //        if (canvasI + dyHalf >= -0.5f && canvasI - dyHalf < ni + 0.5f &&
            //            canvasJ + dxHalf >= -0.5f && canvasJ - dxHalf < nj + 0.5f)
            //        {
            //            float xr = canvasJ + dxHalf;
            //            float xll = canvasJ - dxHalf;
            //            float xl = Mathf.Max(Mathf.Ceil(xr - 0.5f) - 0.5f, xll);
            //            float fx = xr - xl;
            //            int canvasPxJ = Mathf.RoundToInt(xl + 0.1f) - minCanvasJ;
            //            ScanOverYAndRender(canvasPxJ, fx, newPx, texture.Pixels[i * texture.NPixelsX + j], norm);

            //            while (xl > xll)
            //            {
            //                xr = xl;
            //                xl = Mathf.Max(xll, xl - 1);
            //                fx = xr - xl;
            //                canvasPxJ = Mathf.RoundToInt(xr - 0.1f) - minCanvasJ;
            //                ScanOverYAndRender(canvasPxJ, fx, newPx, texture.Pixels[i * texture.NPixelsX + j], norm);
            //            }
            //        }
            //    }
            //}
            return true;
        }

        public void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            throw new System.NotImplementedException();
        }

        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            //if (isRelative) rotCenter += Center;

            //Vector2[] rectVerts = new Vector2[4] {
            //    new Vector2(Origin.x, Origin.y), new Vector2(Origin.x, Origin.y + Size.y),
            //    new Vector2(Origin.x + Size.x, Origin.y + Size.y), new Vector2(Origin.x + Size.x, Origin.y) };
            //Vector2 rotVert0 = Utl.Rotate(rectVerts[0] - rotCenter, radAngle) + rotCenter;
            //float minX = rotVert0.x, maxX = rotVert0.x, minY = rotVert0.y, maxY = rotVert0.y;
            //for (int i = 0; i < 4; i++)
            //{
            //    Vector2 rotVert = Utl.Rotate(rectVerts[i] - rotCenter, radAngle) + rotCenter;
            //    if (rotVert.x < minX) minX = rotVert.x;
            //    if (rotVert.y < minY) minY = rotVert.y;
            //    if (rotVert.x > maxX) maxX = rotVert.x;
            //    if (rotVert.y > maxY) maxY = rotVert.y;
            //}
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