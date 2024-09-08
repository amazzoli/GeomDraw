using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Analytics;


/*
 * TODO:
 * 
 * - Finish to extend GeomDraw to Mesh. Option to convert renderer to tex2d
 * - Inheritance structure for Drawer classes and MyRenderer classes
 * 
 * LATER TODO:
 * 
 * - Improving the Antialiasing alg with CS: unpair the raycast (to assign in or out pixel) from side aa.
 *   In such a way we can use shaders for raycast computing and application.
 * - Check vertex antialiasing for non convex vertices.
 * 
 */
namespace GeomDraw
{
    /// <summary>
    /// It draws Drawbles on a mesh renderer
    /// </summary>
    public class MyRendererMesh
    {
        private readonly DrawerMesh drawer;
        public readonly float pxUnit;
        private readonly int ni, nj;
        private RenderTexture tex;
        [SerializeField] private ComputeShader setPixelsCS;
        [SerializeField] private ComputeBuffer raysXBuff;
        [SerializeField] private ComputeBuffer raysYBuff;
        [SerializeField] private ComputeBuffer shapeXBuff;
        [SerializeField] private ComputeBuffer shapeYBuff;
        [SerializeField] private ComputeBuffer shadesBuff;

        //[SerializeField] private ComputeShader pixelShadeCS;
        //private readonly MeshRenderer renderer;

        public MyRendererMesh(MeshRenderer texRenderer, DrawerMesh drawer)
        {
            Texture tex = texRenderer.sharedMaterial.mainTexture;
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

            // Loading the computeShader that makes all the operations for shade computing and texture writing
            setPixelsCS = Resources.Load<ComputeShader>("ComputeShaders/SetPixels");
            setPixelsCS.SetInt("niTex", ni);
            setPixelsCS.SetInt("njTex", nj);
            setPixelsCS.SetTexture(2, "tex", tex);
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
            (float[] aa, int[] rect) = ComputePixelShades(PixelCoords(shape.Border(pxUnit), pxUnit));
            Debug.Log("AntialiaseShape " + stopWatch.ElapsedMilliseconds);
            WriteTexture(aa, rect, shape.Color);
            Debug.Log("Merge anitaliasing" + stopWatch.ElapsedMilliseconds);

            //if (shape.BorderStyle.thickness > 0)
            //{
            //    Vector2[] border = shape.Border(pxUnit);
            //    IDrawableLine AAborder = new BrokenLine(border, true, shape.BorderStyle);
            //    drawer.Draw(AAborder, false);
            //}
        }


        // AUXILIARY FUNCTIONS

        private void WriteTexture(float[] shades, int[] rect, Color aaColor)
        {
            // For AA the shade array contains the shade info and has to be loaded into the shade
            // From non AA the shade array is already in the memory of the shader
            if (drawer.Antialiase) { }
                shadesBuff = ComputeUtils.LoadArrayOnCS(shades, setPixelsCS, "shades", 2);

            setPixelsCS.SetFloats("color", new float[4] { aaColor.r, aaColor.g, aaColor.b, aaColor.a });
            ComputeUtils.Dispatch(setPixelsCS, rect[1], rect[0], 1, 2);

            shadesBuff.Release();
            if (!drawer.Antialiase)
            {
                raysXBuff.Release();
                raysYBuff.Release();
                shapeXBuff.Release();
                shapeYBuff.Release();
            }
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
        public (float[], int[]) ComputePixelShades(Vector2[] shape)
        {
            // Key starting funcion for drawing shapes and lines

            // It starts by setting the extremes of the shape
            (int minRoundX, int minRoundY, int maxRoundX, int maxRoundY) = ShapeRoundExtremes(shape);

            // The pixel shade array is then created
            int nj = maxRoundX - minRoundX + 1, ni = maxRoundY - minRoundY + 1;
            float[] shades = Enumerable.Repeat(-1.0f, ni * nj).ToArray();

            // The array storing the size and the origin of the rect containing the shape is created
            int[] rect = new int[4] { nj, ni, minRoundX, minRoundY };
            setPixelsCS.SetInt("width", rect[0]);
            setPixelsCS.SetInt("height", rect[1]);
            setPixelsCS.SetInt("offsetX", rect[2]);
            setPixelsCS.SetInt("offsetY", rect[3]);

            // Each consecutive vertex generates a line needed for next computation
            (Line[] lines, bool[] orients) = ComputeLines(shape);

            // The pixel shade is first compute for vertices 
            shades = ComputeVertexShade(shades, rect, shape, lines);

            // And then for all the other pixels
            // In case of antialiaing, the computation is not parallelized and the array shade is updated
            if (drawer.Antialiase)
                shades = ComputeOtherPixelShade(shades, rect, shape, lines, orients);
            // Otherwise the shade array is moved on GPU memory and updated there
            else
                ComputeOtherPixelShadeCS(shades, rect, shape);
            shades = ComputeOtherPixelShade(shades, rect, shape, lines, orients);

            return (shades, rect);
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
                    int asd = FlatCoord(px.y, px.x, rect);
                    if (drawer.Antialiase)
                    {
                        Vector2 pIn = shape[(ui - 1 + L) % L], pOut = shape[(ui + counts[i]) % L];
                        Line lineIn = lines[(ui - 1 + L) % L], lineOut = lines[(ui + counts[i] - 1) % L];
                        Vector2[] innerPoints = new Vector2[counts[i]];
                        for (int k = 0; k < counts[i]; k++) innerPoints[k] = shape[(ui + k) % L];
                        float area = Utl.PixelAreaBelowBrokenLine(px, pIn, lineIn, pOut, lineOut, innerPoints);
                        pixels[asd] = area;
                    }
                    else
                        pixels[asd] = 1;
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

        private void ComputeOtherPixelShadeCS(float[] shades, int[] rect, Vector2[] shape)
        {
            // Initialize raycasts to identify in or out pixels
            int ni = rect[1] + 1, nj = rect[0] + 1;
            uint[] nCrossXRays = new uint[ni * nj], nCrossYRays = new uint[ni * nj];
            raysXBuff = ComputeUtils.LoadArrayOnCS(nCrossXRays, setPixelsCS, "nCrossXRays", new int[2] { 0, 1 });
            raysYBuff = ComputeUtils.LoadArrayOnCS(nCrossYRays, setPixelsCS, "nCrossYRays", new int[2] { 0, 1 });
            setPixelsCS.SetInt("L", shape.Length);

            // Computing the number each pixel raycast crosses shape sides. Parallel over the sides
            float[] sx = new float[shape.Length], sy = new float[shape.Length];
            for (int i = 0; i < shape.Length; i++)
            {
                sx[i] = -1.0f;
                sy[i] = shape[i].y;
            }
            shapeXBuff = ComputeUtils.LoadArrayOnCS<float>(sx, setPixelsCS, "shapeX", 0);
            shapeYBuff = ComputeUtils.LoadArrayOnCS<float>(sy, setPixelsCS, "shapeY", 0);
            ComputeUtils.Dispatch(setPixelsCS, shape.Length, 1, 1, 0);

            raysXBuff.GetData(nCrossXRays);
            raysYBuff.GetData(nCrossYRays);
            Debug.Log(nCrossYRays[nj + 10]);

            // Setting if the pixel is inside or outside the shape
            shadesBuff = ComputeUtils.LoadArrayOnCS(shades, setPixelsCS, "shades", new int[2] { 1, 2 });
            ComputeUtils.Dispatch(setPixelsCS, rect[1], rect[0], 1, 1);
        }

        private float[] ComputeOtherPixelShade(float[] pixels, int[] rect, Vector2[] shape, Line[] lines, bool[] orient)
        {
            // Initialize raycasts to identify in or out pixels
            int ni = rect[1] + 1, nj = rect[0] + 1;
            ushort[] nCrossXRays = new ushort[ni * nj], nCrossYRays = new ushort[ni * nj];
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
                    float xSide = lines[k].X(pvi - 0.5f + rect[3]);
                    for (pvj = 0; pvj < minPxVertX; pvj++) nCrossXRays[pvi * nj + pvj] += 1;
                    for (pvj = minPxVertX; pvj <= maxPxVertX; pvj++)
                    {
                        if (xSide >= pvj - 0.5f + rect[2]) nCrossXRays[pvi * nj + pvj] += 1;
                        else break;
                    }
                    if (minPxVertX > 0 || maxPxVertX - minPxVertX >= 0) pvj = Mathf.Max(0, pvj - 1);
                    sideInfoX[pvi * nj + pvj, k] = true;
                }

                for (int pvj = minPxVertX; pvj <= maxPxVertX; ++pvj)
                {
                    int pvi;
                    float ySide = lines[k].Y(pvj - 0.5f + rect[2]);
                    for (pvi = 0; pvi < minPxVertY; pvi++) nCrossYRays[pvi * nj + pvj] += 1;
                    for (pvi = minPxVertY; pvi <= maxPxVertY; ++pvi)
                    {
                        if (ySide >= pvi - 0.5f + rect[3]) nCrossYRays[pvi * nj + pvj] += 1;
                        else break;
                    }
                    if (minPxVertY > 0 || maxPxVertY - minPxVertY >= 0) pvi = Mathf.Max(0, pvi - 1);
                    sideInfoY[pvj * ni + pvi, k] = true;
                }
            }

            Debug.Log(nCrossYRays[nj + 10]);

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

