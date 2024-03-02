using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GeomDraw
{
    /// <summary>
    /// It applys the bucket tool to the spriteRenderer
    /// </summary>
    public class Bucket
    {
        private readonly Color newColor;
        private readonly float sensitivity;
        private readonly int ni, nj;
        private readonly int pointI, pointJ;
        private readonly Color bcgColor;

        private Texture2D canvas;
        private Color[] oldPixels, newPixels;
        private bool[] pixelChecked;

        public Bucket(SpriteRenderer renderer, Vector2 point, Color color, float sensitivity)
        {
            this.newColor = color;
            this.sensitivity = sensitivity;

            canvas = renderer.sprite.texture;
            float pixelsPerUnit = renderer.sprite.pixelsPerUnit;
            ni = canvas.height;
            nj = canvas.width;
            oldPixels = canvas.GetPixels();
            newPixels = new Color[oldPixels.Length];
            pixelChecked = new bool[oldPixels.Length];
            for (int i = 0; i < oldPixels.Length; i++) newPixels[i] = new Color(oldPixels[i].r, oldPixels[i].g, oldPixels[i].b);

            (pointI, pointJ) = PixelIndexes(point, pixelsPerUnit);
            if (pointI < 0 || pointI >= ni || pointJ < 0 || pointJ >= nj)
            {
                Debug.LogError("Point of bucket outside canvas");
                return;
            }
            int flatI = FlatCoord(pointI, pointJ);
            bcgColor = new Color(oldPixels[flatI].r, oldPixels[flatI].g, oldPixels[flatI].b);
        }

        public void Run()
        {
            ApplyBucket(pointI, pointJ);
            canvas.SetPixels(newPixels);
            //canvas.Apply(false);
        }


        private void ApplyBucket(int i, int j)
        {
            int flatI = FlatCoord(i, j);
            List<int> pxToCheck = new List<int>() { flatI };
            int iter = 0;

            while(pxToCheck.Count > 0 && iter < oldPixels.Length + 1)
            {
                int k = pxToCheck[0];
                (i, j) = CoordFromFlat(k);
                pxToCheck.RemoveAt(0);
                pixelChecked[k] = true;
                //Debug.Log(k);
                if (ColorUtils.ColorDist(oldPixels[k], bcgColor) <= sensitivity)
                {
                    newPixels[k] = newColor;

                    if (i + 1 < ni)
                    {
                        int k1 = FlatCoord(i + 1, j);
                        if (!pixelChecked[k1] && !pxToCheck.Contains(k1)) pxToCheck.Add(k1);
                    }
                    if (i - 1 >= 0)
                    {
                        int k1 = FlatCoord(i - 1, j);
                        if (!pixelChecked[k1] && !pxToCheck.Contains(k1)) pxToCheck.Add(k1);
                    }
                    if (j + 1 < nj)
                    {
                        int k1 = FlatCoord(i, j + 1);
                        if (!pixelChecked[k1] && !pxToCheck.Contains(k1)) pxToCheck.Add(k1);
                    }
                    if (j - 1 >= 0)
                    {
                        int k1 = FlatCoord(i, j - 1);
                        if (!pixelChecked[k1] && !pxToCheck.Contains(k1)) pxToCheck.Add(k1);
                    }
                }
                iter++;
            }
        }

        //private void BucketRecursive(int i, int j)
        //{
        //    int flatI = FlatCoord(i, j);
        //    if (pixelChecked[flatI]) return;

        //    iter++;
        //    if (iter >= pixels.Length + 1)
        //    {
        //        Debug.LogError("Wrong recursivity in bucket tool");
        //        forcedAlt = true;
        //    }
        //    if (forcedAlt) return;

        //    pixelChecked[flatI] = true;
        //    if (ColorUtils.ColorDist(pixels[flatI], bcgColor) < sensitivity)
        //    {
        //        pixels[flatI] = newColor;

        //        if (i + 1 < ni)
        //        {
        //            int k = FlatCoord(i + 1, j);
        //            if (!pixelChecked[k]) BucketRecursive(i + 1, j);
        //        }
        //        if (i - 1 >= 0)
        //        {
        //            int k = FlatCoord(i - 1, j);
        //            if (!pixelChecked[k]) BucketRecursive(i - 1, j);
        //        }
        //        if (j + 1 < nj)
        //        {
        //            int k = FlatCoord(i, j + 1);
        //            if (!pixelChecked[k]) BucketRecursive(i, j + 1);
        //        }
        //        if (j - 1 >= 0)
        //        {
        //            int k = FlatCoord(i, j - 1);
        //            if (!pixelChecked[k]) BucketRecursive(i, j - 1);
        //        }
        //    }
        //}


        private int FlatCoord(int i, int j)
        {
            return i * nj + j;
        }

        private (int, int) CoordFromFlat(int flatI)
        {
            int j = flatI % nj;
            int i = (int)((flatI - j) / (float)nj);
            return (i, j);
        }

        private (int, int) PixelIndexes(Vector2 point, float pixelsPerUnit)
        {
            int j = Mathf.RoundToInt(point.x * pixelsPerUnit - 0.5f);
            int i = Mathf.RoundToInt(point.y * pixelsPerUnit - 0.5f);
            return (i, j);
        }
    }
}

