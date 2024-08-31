using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Analytics;


namespace GeomDraw
{
    /// <summary>
    /// It draws Drawbles on a mesh renderer
    /// </summary>
    public class MyRendererMesh
    {
        private DrawerMesh drawer;
        public float pxUnit;
        private RenderTexture tex;
        private int ni, nj;
        [SerializeField] private ComputeShader setPixelsCS;
        MeshRenderer renderer;

        public MyRendererMesh(MeshRenderer texRenderer, DrawerMesh drawer)
        {
            this.renderer = texRenderer;
            Texture tex = texRenderer.material.mainTexture;
            this.drawer = drawer;
            if (tex is RenderTexture texture)
                this.tex = texture;
            else
            {
                this.tex = ComputeUtils.CreateRenderTexture(tex.width, tex.height);
                Graphics.Blit(tex, this.tex);
                this.tex.enableRandomWrite = true;
                this.tex.autoGenerateMips = drawer.UpdateMipMaps;
                texRenderer.sharedMaterial.mainTexture = this.tex;
            }

            ni = tex.height;
            nj = tex.width;

            // Pixels per units are estimated from the size of the renderer, they don't need to
            // precise but they are used to set scales of approximations 
            float width = texRenderer.bounds.size.x;
            pxUnit = nj / width;
            Debug.Log("Px unit" +  pxUnit);

            setPixelsCS = Resources.Load<ComputeShader>("ComputeShaders/SetPixels");
            setPixelsCS.SetInt("ni", ni);
            setPixelsCS.SetInt("nj", nj);
        }


        /// <summary> Draw a shape over the renderer </summary>
        public void DrawShape(IDrawableShape shape)
        {
            if (drawer == null)
            {
                Debug.LogError("MyRenderer initialized without Drawer");
                return;
            }

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            (float[] aa, int[] rect) = AntialiaseShape(PixelCoords(shape.Border(pxUnit), pxUnit));
            Debug.Log("AntialiaseShape " + stopWatch.ElapsedMilliseconds);
            ComputeBuffer aaReader = new ComputeBuffer(aa.Length, Marshal.SizeOf(typeof(System.Single)), ComputeBufferType.Default);
            aaReader.SetData(aa);
            Debug.Log("Build buffer " + stopWatch.ElapsedMilliseconds);
            setPixelsCS.SetTexture(0, "tex", tex);
            Debug.Log("load texture " + stopWatch.ElapsedMilliseconds);
            setPixelsCS.SetBuffer(0, "aaReader", aaReader);
            Debug.Log("Load buffer " + stopWatch.ElapsedMilliseconds);
            setPixelsCS.SetInt("width", rect[0]);
            setPixelsCS.SetInt("height", rect[1]);
            setPixelsCS.SetInt("offsetX", rect[2]);
            setPixelsCS.SetInt("offsetY", rect[3]);
            setPixelsCS.SetFloats("color", new float[4] { shape.Color.r, shape.Color.g, shape.Color.b, shape.Color.a });
            Debug.Log("Load stuff " + stopWatch.ElapsedMilliseconds);
            ComputeUtils.Dispatch(setPixelsCS, rect[1], rect[1], 1, 0);
            Debug.Log("Dispach " + stopWatch.ElapsedMilliseconds);
            aaReader.Release();
            Debug.Log("Release " + stopWatch.ElapsedMilliseconds);

            //ComputeShader fillTexCS = (ComputeShader)Resources.Load("ComputeShaders/FillTex");
            //fillTexCS.SetInt("width", nj);
            //fillTexCS.SetInt("height", ni);
            //fillTexCS.SetFloats("color", new float[4] { shape.Color.r, shape.Color.g, shape.Color.b, shape.Color.a });
            //fillTexCS.SetTexture(0, "tex", tex);
            //ComputeUtils.Dispatch(fillTexCS, nj, ni, 1, 0);

            //renderer.sharedMaterial.mainTexture = tex;

            //pixels = MergeAntialiasing(pixels, ni, nj, aa, rect, shape.Color, false);
            //canvas.SetPixels(pixels);
            //canvas.Apply(drawer.UpdateMipMaps);

            //if (shape.BorderStyle.thickness > 0)
            //{
            //    Vector2[] border = shape.Border(pxUnit);
            //    IDrawableLine AAborder = new BrokenLine(border, true, shape.BorderStyle);
            //    drawer.Draw(AAborder, false);
            //}
        }


        // AUXILIARY FUNCTIONS

        /// <summary> Apply the antialiasing of given color to the canvas </summary>
        private Color[] MergeAntialiasing(Color[] canvas, int ni, int nj, float[] antialiasing, int[] rect, Color aaColor, bool erase)
        {
            for (int i = 0; i < rect[1]; i++)
            {
                for (int j = 0; j < rect[0]; j++)
                {
                    int canvasI = i + rect[3], canvasJ = j + rect[2];
                    if (canvasI >= 0 && canvasI < ni && canvasJ >= 0 && canvasJ < nj)
                    {
                        float a = aaColor.a * antialiasing[i * rect[0] + j];
                        if (!erase)
                        {
                            Color bgColor = canvas[canvasI * nj + canvasJ];
                            Color newColor = new Color(aaColor.r, aaColor.g, aaColor.b, a);
                            canvas[canvasI * nj + canvasJ] = ColorUtils.ColorBlend(newColor, bgColor);
                        }
                        else
                        {
                            Color bgColor = canvas[canvasI * nj + canvasJ];
                            canvas[canvasI * nj + canvasJ] = ColorUtils.ColorErase(1 - antialiasing[i * rect[0] + j], bgColor);
                        }
                    }
                }
            }
            return canvas;
        }


        private Vector2[] ShapeFromDiscretization(Vector2 dsRight1, Vector2 dsLeft1, Vector2 dsLeft2, Vector2 dsRight2)
        {
            Vector2[] shape;
            if (dsLeft1 == dsLeft2)
                shape = new Vector2[] { dsRight1, dsLeft1, dsRight2 };
            else if (dsRight1 == dsRight2)
                shape = new Vector2[] { dsRight1, dsLeft1, dsLeft2 };
            else
            {
                shape = new Vector2[] { dsRight1, dsLeft1, dsLeft2, dsRight2 };
                if (Utl.SegmentIntersect(shape[0], shape[1], shape[2], shape[3]))
                {
                    shape[0] = dsRight2;
                    shape[3] = dsRight1;
                }
            }
            if (!Utl.IsClockwisePath(shape)) shape = Utl.InvertClockwise(shape);
            return shape;
        }

        /// <summary> Generate an antialiased projection of the shape. </summary>
        /// <param name="shape"> Vertices of the shape in clockwise order in pixel units </param>
        /// <returns> 
        /// First element of the pair containing the antialiased pixel projection as a flatten 
        /// array of floats between 0 and 1 (0 -> empty pixel, 1 -> fully coloured pixel). Second element 
        /// containint four int: canvas width, canvas height, canvas x offset, canvas y offset. 
        /// </returns>
        public (float[], int[]) AntialiaseShape(Vector2[] shape)
        {
            // Key starting funcion for drawing shapes and lines

            // It starts by setting the extremes of the shape
            (int minRoundX, int minRoundY, int maxRoundX, int maxRoundY) = ShapeRoundExtremes(shape);
            // The pixel array is then created
            int nj = maxRoundX - minRoundX + 1, ni = maxRoundY - minRoundY + 1;
            float[] pixels = Enumerable.Repeat(-1.0f, ni * nj).ToArray();

            // The array storing the size and the origin of the rect containing the shape is created
            int[] rect = new int[4] { nj, ni, minRoundX, minRoundY };
            // Each consecutive vertex generates a line needed for next computation
            (Line[] lines, bool[] orients) = ComputeLines(shape);
            // The pixel shade is first compute for vertices 
            pixels = ComputeVertexShade(pixels, rect, shape, lines);
            // And then for all the other pixels
            pixels = ComputeOtherPixelShade(pixels, rect, shape, lines, orients);

            return (pixels, rect);
        }

        /// <summary>
        /// Aux of AntialiaseShape. Computes maximums and minimums rounded to int of the shape.
        /// </summary>
        private (int, int, int, int) ShapeRoundExtremes(Vector2[] shape)
        {
            int minRoundX = Mathf.RoundToInt(shape[0].x), minRoundY = Mathf.RoundToInt(shape[0].y);
            int maxRoundX = minRoundX, maxRoundY = minRoundY;
            for (int i = 1; i < shape.Length; i++)
            {
                int roundX = Mathf.RoundToInt(shape[i][0]), roundY = Mathf.RoundToInt(shape[i][1]);
                if (roundX < minRoundX) minRoundX = roundX;
                if (roundY < minRoundY) minRoundY = roundY;
                if (roundX > maxRoundX) maxRoundX = roundX;
                if (roundY > maxRoundY) maxRoundY = roundY;
            }
            return (minRoundX, minRoundY, maxRoundX, maxRoundY);
        }

        /// <summary>
        /// It returns the lines passing for each couple of points of the shape and whether the
        /// inside of the shape is upper-left of the side.
        /// </summary>
        private (Line[], bool[]) ComputeLines(Vector2[] shape)
        {
            int L = shape.Length;
            Line[] lines = new Line[L];
            bool[] orient = new bool[L];
            for (int k = 0; k < L; k++)
            {
                lines[k] = new Line(shape[k], shape[(k + 1) % L]);
                orient[k] = shape[k].x <= shape[(k + 1) % L].x;
                if (shape[k].x == shape[(k + 1) % L].x)
                    orient[k] = shape[k].y < shape[(k + 1) % L].y;
            }
            return (lines, orient);
        }

        /// <summary> It computes the shade value of pixels at the position of the vertices </summary>
        private float[] ComputeVertexShade(float[] pixels, int[] rect, Vector2[] shape, Line[] lines)
        {
            (List<int> vertUnique, List<int> counts) = FindVerticesInPixels(shape);

            //A shape all contained in a pixel should not happen, but in case, the pixel is set to 1 
            if (vertUnique.Count == 1)
            {
                PixelCoord px = new PixelCoord(shape[vertUnique[0]]);
                pixels[FlatCoord(px.y, px.x, rect)] = 1;
            }
            else
            {
                int L = shape.Length;
                for (int i = 0; i < vertUnique.Count; i++)
                {
                    int ui = vertUnique[i];
                    Vector2 p = shape[vertUnique[i]];
                    PixelCoord px = new PixelCoord(p);
                    Vector2 pIn = shape[(ui - 1 + L) % L], pOut = shape[(ui + counts[i]) % L];
                    Line lineIn = lines[(ui - 1 + L) % L], lineOut = lines[(ui + counts[i] - 1) % L];
                    Vector2[] innerPoints = new Vector2[counts[i]];
                    for (int k = 0; k < counts[i]; k++) innerPoints[k] = shape[(ui + k) % L];
                    float area = Utl.PixelAreaBelowBrokenLine(px, pIn, lineIn, pOut, lineOut, innerPoints);
                    int asd = FlatCoord(px.y, px.x, rect);
                    pixels[asd] = area;
                }
            }
            return pixels;
        }

        /// <summary>
        /// It returns a list of vertices indexes and how many consecutive vertices are found in the same pixel.
        /// The first returned list contains the counterclockwise-most unique indexes of the shape and 
        /// the second list the count of other vertices in the same pixel.
        /// Non-adjacent vertices in the same pixel would generate a wrong list.
        /// </summary>
        private (List<int>, List<int>) FindVerticesInPixels(Vector2[] shape)
        {
            int[,] intShape = new int[shape.Length, 2];
            for (int i = 0; i < shape.Length; i++)
            {
                intShape[i, 0] = Mathf.RoundToInt(shape[i].x);
                intShape[i, 1] = Mathf.RoundToInt(shape[i].y);
            }

            List<int> vertUnique = new List<int>(), counts = new List<int>();
            for (int i = 0; i < shape.Length; i++)
            {
                bool found = false;
                for (int j = 0; j > vertUnique.Count; j++)
                {
                    if (intShape[vertUnique[j], 0] == intShape[i, 0] && intShape[vertUnique[j], 1] == intShape[i, 1])
                    {
                        counts[j] += 1;
                        found = true;
                    }
                }
                if (!found)
                {
                    vertUnique.Add(i);
                    counts.Add(1);
                }
            }

            for (int i = 0; i < vertUnique.Count; i++)
            {
                if (counts[i] > 1 && counts[i] < shape.Length)
                {
                    for (int k = 0; k < shape.Length; k++)
                    {
                        int u = vertUnique[i];
                        int iLeft = (u - 1) % shape.Length;
                        if (intShape[iLeft, 0] == intShape[u, 0] && intShape[iLeft, 1] == intShape[u, 1])
                        {
                            vertUnique[i] = iLeft;
                            u = iLeft;
                        }

                        else
                            break;
                    }
                }
            }
            return (vertUnique, counts);
        }

        private float[] ComputeOtherPixelShade(float[] pixels, int[] rect, Vector2[] shape, Line[] lines, bool[] orient)
        {
            // Initialize raycasts to identify in or out pixels
            int ni = rect[1] + 1, nj = rect[0] + 1;
            int[] nCrossXRays = new int[ni * nj], nCrossYRays = new int[ni * nj];
            bool[,] sideInfoX = new bool[ni * nj, shape.Length];
            bool[,] sideInfoY = new bool[ni * nj, shape.Length];

            for (int k = 0; k < shape.Length; k++)
            {
                Vector2 v1 = shape[k], v2 = shape[(k + 1) % shape.Length];
                int maxPxVertY = (int)(Mathf.Ceil(Mathf.Max(v1[1], v2[1]) - 0.5f) - rect[3]);
                int minPxVertY = (int)(Mathf.Ceil(Mathf.Min(v1[1], v2[1]) + 0.5f) - rect[3]);
                int maxPxVertX = (int)(Mathf.Ceil(Mathf.Max(v1[0], v2[0]) - 0.5f) - rect[2]);
                int minPxVertX = (int)(Mathf.Ceil(Mathf.Min(v1[0], v2[0]) + 0.5f) - rect[2]);

                for (int pvi = minPxVertY; pvi <= maxPxVertY; ++pvi)
                {
                    int pvj;
                    for (pvj = 0; pvj < minPxVertX; pvj++) nCrossXRays[pvi * nj + pvj] += 1;
                    for (pvj = minPxVertX; pvj <= maxPxVertX; pvj++)
                    {
                        float xSide = lines[k].X(pvi - 0.5f + rect[3]);
                        if (xSide >= pvj - 0.5f + rect[2]) nCrossXRays[pvi * nj + pvj] += 1;
                        else break;
                    }
                    if (minPxVertX > 0 || maxPxVertX - minPxVertX >= 0) pvj = Mathf.Max(0, pvj - 1);
                    sideInfoX[pvi * nj + pvj, k] = true;
                }

                for (int pvj = minPxVertX; pvj <= maxPxVertX; ++pvj)
                {
                    int pvi;
                    for (pvi = 0; pvi < minPxVertY; pvi++) nCrossYRays[pvi * nj + pvj] += 1;
                    for (pvi = minPxVertY; pvi <= maxPxVertY; ++pvi)
                    {
                        float ySide = lines[k].Y(pvj - 0.5f + rect[2]);
                        if (ySide >= pvi - 0.5f + rect[3]) nCrossYRays[pvi * nj + pvj] += 1;
                        else break;
                    }
                    if (minPxVertY > 0 || maxPxVertY - minPxVertY >= 0) pvi = Mathf.Max(0, pvi - 1);
                    sideInfoY[pvj * ni + pvi, k] = true;
                }
            }

            for (int i = 0; i < rect[1]; i++)
            {
                for (int j = 0; j < rect[0]; j++)
                {
                    if (nCrossXRays[i * nj + j] == nCrossXRays[i * nj + j + 1] && nCrossXRays[(i + 1) * nj + j] == nCrossXRays[(i + 1) * nj + j + 1] &&
                        nCrossYRays[i * nj + j] == nCrossYRays[(i + 1) * nj + j] && nCrossYRays[i * nj + j + 1] == nCrossYRays[(i + 1) * nj + j + 1])
                    {
                        if (nCrossXRays[i * nj + j] % 2 == 0) pixels[i * rect[0] + j] = 0;
                        else pixels[i * rect[0] + j] = 1;
                    }
                    else if (pixels[i * rect[0] + j] == -1.0f)
                    {
                        HashSet<int> sides = new HashSet<int>();
                        for (int k = 0; k < shape.Length; k++)
                        {
                            if (sideInfoX[i * nj + j, k]) sides.Add(k);
                            if (sideInfoX[(i + 1) * nj + j, k]) sides.Add(k);
                            if (sideInfoY[j * ni + i, k]) sides.Add(k);
                            if (sideInfoY[(j + 1) * ni + i, k]) sides.Add(k);
                        }
                        float area = 0;
                        foreach (int s in sides)
                        {
                            PixelCoord px = new PixelCoord(new Vector2(j + rect[2], i + rect[3]));
                            area += Utl.PixelAreaBelowLine(px, lines[s], orient[s]);
                        }
                        pixels[i * rect[0] + j] = area - sides.Count + 1;
                    }
                }
            }

            return pixels;
        }


        /// <summary>
        /// Flat coordinate of the canvas given the local pixel indexes of a sub-rect.
        /// </summary>
        /// <param name="rect"> {nPixelx, nPixely, pixel origin y, pixel origin x} </param>
        private int FlatCoord(int i, int j, int[] rect)
        {
            return (i - rect[3]) * rect[0] + j - rect[2];
        }

        private Vector2[] PixelCoords(Vector2[] points, float pixelsPerUnit)
        {
            Vector2[] pixelVertices = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
                pixelVertices[i] = PixelCoord(points[i], pixelsPerUnit);
            return pixelVertices;
        }

        private Vector2 PixelCoord(Vector2 point, float pixelsPerUnit)
        {
            return new Vector2(point.x * pixelsPerUnit - 0.5f, point.y * pixelsPerUnit - 0.5f); ;
        }
    }
}

