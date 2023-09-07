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
        private float dxHalf, dyHalf;
        private int nMaxPxX, nMaxPxY;
        private int minCanvasI, minCanvasJ;
        public void DrawTexture(DrawableTexture texture)
        {
            //Sizes in pixel units
            float wPx = texture.Size.x * pxUnit;
            float hPx = texture.Size.y * pxUnit;
            //Pixel sizes of texture in canvas-pixel units
            dxHalf = wPx / (float)texture.NPixelsX / 2.0f;
            dyHalf = hPx / (float)texture.NPixelsY / 2.0f;
            //Pixels per unit of the texture
            float pxUnitX = texture.NPixelsX / texture.Size.x;
            float pxUnitY = texture.NPixelsY / texture.Size.y;
            //Maximum number of canvas pixels the texture covers 
            nMaxPxX = (int)Mathf.Ceil(wPx) + 1;
            nMaxPxY = (int)Mathf.Ceil(hPx) + 1;
            //Pixel of canvas containing the origin
            minCanvasI = Mathf.RoundToInt(texture.Origin.y * pxUnit - 0.5f);
            minCanvasJ = Mathf.RoundToInt(texture.Origin.x * pxUnit - 0.5f); 
            //New colors
            Color[] newPx = Enumerable.Repeat(new Color(0, 0, 0, 0), nMaxPxX * nMaxPxY).ToArray();
            // Normalization of texture covering the canvas (necessary for borders where the texture
            // does not overlap completely the canvas and glitches can appear)
            float[] norm = new float[newPx.Length];

            for (int i = 0; i < texture.NPixelsY; i++)
            {
                for (int j = 0; j < texture.NPixelsX; j++)
                {
                    // Coordinate of the new pixel in the system of reference of the canvas pixels
                    canvasJ = ((j + 0.5f) / pxUnitX + texture.Origin.x) * pxUnit - 0.5f;
                    canvasI = ((i + 0.5f) / pxUnitY + texture.Origin.y) * pxUnit - 0.5f;

                    if (canvasI + dyHalf >= -0.5f && canvasI - dyHalf < ni + 0.5f && 
                        canvasJ + dxHalf >= -0.5f && canvasJ - dxHalf < nj + 0.5f)
                    {
                        float xr = canvasJ + dxHalf;
                        float xll = canvasJ - dxHalf;
                        float xl = Mathf.Max(Mathf.Ceil(xr - 0.5f) - 0.5f, xll); 
                        float fx = xr - xl;
                        int canvasPxJ = Mathf.RoundToInt(xl + 0.1f) - minCanvasJ;
                        ScanOverYAndRender(canvasPxJ, fx, newPx, texture.Pixels[i * texture.NPixelsX + j], norm);

                        while (xl > xll)
                        {
                            xr = xl;
                            xl = Mathf.Max(xll, xl - 1);
                            fx = xr - xl;
                            canvasPxJ = Mathf.RoundToInt(xr - 0.1f) - minCanvasJ;
                            ScanOverYAndRender(canvasPxJ, fx, newPx, texture.Pixels[i * texture.NPixelsX + j], norm);
                        }
                    }
                }
            }

            MergeAntialiasing(newPx, norm);
        }


        private void ScanOverYAndRender(int canvasPxJ, float fx, Color[] newPx, Color newColor, float[] norm)
        {
            float yu = canvasI + dyHalf;
            float ydd = canvasI - dyHalf;
            float yd = Mathf.Max(Mathf.Ceil(yu - 0.5f) - 0.5f, ydd);
            float fy = yu - yd;
            int canvasPxI = Mathf.RoundToInt(yd + 0.1f) - minCanvasI;
            newPx[canvasPxI * nMaxPxX + canvasPxJ] += newColor * fx * fy;
            norm[canvasPxI * nMaxPxX + canvasPxJ] += fx * fy;

            while (yd > ydd)
            {
                yu = yd;
                yd = Mathf.Max(ydd, yd - 1);
                fy = yu - yd;
                canvasPxI = Mathf.RoundToInt(yu - 0.1f) - minCanvasI;
                newPx[canvasPxI * nMaxPxX + canvasPxJ] += newColor * fx * fy;
                norm[canvasPxI * nMaxPxX + canvasPxJ] += fx * fy;
            }
        }

        private void MergeAntialiasing(Color[] newPx, float[] norm)
        {
            Color[] canvasPx = canvas.GetPixels();

            for (int i = 0; i < nMaxPxY; i++)
            {
                for (int j = 0; j < nMaxPxX; j++)
                {
                    int canvasI = i + minCanvasI, canvasJ = j + minCanvasJ;
                    float n = norm[i * nMaxPxX + j];
                    if (canvasI >= 0 && canvasI < ni && canvasJ >= 0 && canvasJ < nj && n > 0)
                    {
                        Color bgColor = canvasPx[canvasI * nj + canvasJ];
                        Color newColor = newPx[i * nMaxPxX + j] / n;
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