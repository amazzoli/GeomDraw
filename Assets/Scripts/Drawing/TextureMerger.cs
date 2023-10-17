using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

namespace GeomDraw
{
    public class TextureMerger
    {
        private float pxUnit;
        Texture2D canvas;
        int ni, nj;


        public TextureMerger(SpriteRenderer spriteRenderer)
        {
            //this.spriteRenderer = spriteRenderer;
            pxUnit = spriteRenderer.sprite.pixelsPerUnit;
            canvas = spriteRenderer.sprite.texture;
            ni = canvas.height;
            nj = canvas.width;
        }


        private float canvasI, canvasJ;
        private int nMaxPxX, nMaxPxY;
        private int minCanvasI, minCanvasJ;
        float fx1, fx2, fy1, fy2;
        public void DrawTexture(DrawableTexture texture)
        {
            if (pxUnit != texture.PxPerUnit)
            {
                Debug.Log("Px-per-units don't match, texture will be resized");
                texture.Deform(pxUnit);
            }

            //Sizes in pixel units
            float wPx = texture.NPixelsX;
            float hPx = texture.NPixelsY;
            //Maximum number of canvas pixels the texture covers 
            nMaxPxX = Mathf.FloorToInt(wPx) + 1;
            nMaxPxY = Mathf.FloorToInt(hPx) + 1;
            //Pixel of canvas containing the origin
            minCanvasI = Mathf.FloorToInt(texture.Origin.y * pxUnit + 0.0001f);
            minCanvasJ = Mathf.FloorToInt(texture.Origin.x * pxUnit + 0.0001f); 
            //New colors
            Color[] newPx = Enumerable.Repeat(new Color(0, 0, 0, 0), nMaxPxX * nMaxPxY).ToArray();

            // Computing the fraction of overlap of the texture pixel over the canvas pixel
            float canvasJ0 = texture.Origin.x * pxUnit + 0.0001f;
            int canvasPxJ0 = Mathf.FloorToInt(canvasJ0);
            fx1 = canvasPxJ0 - canvasJ0 + 1; fx2 = canvasJ0 - canvasPxJ0;
            float canvasI0 = texture.Origin.y * pxUnit + 0.0001f;
            int canvasPxI0 = Mathf.FloorToInt(canvasI0);
            fy1 = canvasPxI0 - canvasI0 + 1; fy2 = canvasI0 - canvasPxI0;
            float fDownLeft = fx1 * fy1, fDownRight = fx2 * fy1, fUpRight = fx2 * fy2, fUpLeft = fx1 * fy2;

            for (int i = 0; i < texture.NPixelsY; i++)
            {
                for (int j = 0; j < texture.NPixelsX; j++)
                {
                    // Coordinate of the new pixel in the system of reference of the canvas pixels
                    canvasJ = j + canvasJ0; canvasI = i + canvasI0;

                    if (canvasI >= -1 && canvasI < ni + 1 && canvasJ >= -1 && canvasJ < nj + 1)
                    {
                        Color newColor = texture.Pixels[i * texture.NPixelsX + j];

                        // Bottom left pixel of the canvas
                        int canvasPxJ = Mathf.FloorToInt(canvasJ) - minCanvasJ;
                        int canvasPxI = Mathf.FloorToInt(canvasI) - minCanvasI;

                        // Each texture pixel can overlap 4 canvas pixels
                        newPx[canvasPxI * nMaxPxX + canvasPxJ] += newColor * fDownLeft;
                        newPx[(canvasPxI + 1) * nMaxPxX + canvasPxJ] += newColor * fUpLeft;
                        newPx[(canvasPxI + 1) * nMaxPxX + canvasPxJ + 1] += newColor * fUpRight;
                        newPx[canvasPxI * nMaxPxX + canvasPxJ + 1] += newColor * fDownRight;
                    }
                }
            }

            MergeAntialiasing(newPx);
        }


        //private void ScanOverYAndRender(int canvasPxJ, float fx, Color[] newPx, Color newColor, float[] norm)
        //{
        //    float yu = canvasI + 0.5f;
        //    float ydd = canvasI - 0.5f;
        //    float yd = Mathf.Max(Mathf.Ceil(yu - 0.5f) - 0.5f, ydd);
        //    float fy = yu - yd;
        //    int canvasPxI = Mathf.RoundToInt(yd + 0.1f) - minCanvasI;
        //    newPx[canvasPxI * nMaxPxX + canvasPxJ] += newColor * fx * fy;
        //    norm[canvasPxI * nMaxPxX + canvasPxJ] += fx * fy;

        //    while (yd > ydd)
        //    {
        //        yu = yd;
        //        yd = Mathf.Max(ydd, yd - 1);
        //        fy = yu - yd;
        //        canvasPxI = Mathf.RoundToInt(yu - 0.1f) - minCanvasI;
        //        newPx[canvasPxI * nMaxPxX + canvasPxJ] += newColor * fx * fy;
        //        norm[canvasPxI * nMaxPxX + canvasPxJ] += fx * fy;
        //    }
        //}

        private void MergeAntialiasing(Color[] newPx)
        {
            Color[] canvasPx = canvas.GetPixels();

            // This two normalizations correct for pixels at the border that are partially overlapped
            // by the texture. Without correction the border gets darker
            float nx, ny;

            for (int i = 0; i < nMaxPxY; i++)
            {
                if (i == 0) ny = fy1;
                else if (i == nMaxPxY - 1) ny = fy2;
                else ny = 1;

                for (int j = 0; j < nMaxPxX; j++)
                {
                    if (j == 0) nx = fx1;
                    else if (j == nMaxPxX - 1) nx = fx2;
                    else nx = 1;

                    int canvasI = i + minCanvasI, canvasJ = j + minCanvasJ;
                    if (canvasI >= 0 && canvasI < ni && canvasJ >= 0 && canvasJ < nj && nx > 0 && ny > 0)
                    {
                        Color bgColor = canvasPx[canvasI * nj + canvasJ];
                        Color newColor = newPx[i * nMaxPxX + j] / nx / ny;
                        newColor.a = newPx[i * nMaxPxX + j].a;
                        canvasPx[canvasI * nj + canvasJ] = ColorUtils.ColorBlend(newColor, bgColor);
                    }
                }
            }
            canvas.SetPixels(canvasPx);
            canvas.Apply(false);
        }
    }
}