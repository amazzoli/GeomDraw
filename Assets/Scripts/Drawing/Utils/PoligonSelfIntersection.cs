using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Drawing;
using UnityEngine.UIElements;

namespace Drawing
{
    public class PoligonSelfIntersection
    {
        Vector2[] verts;
        List<int> xSortedIndexes;
        int nVerts;
        public float Tollerance { get; private set; }


        /// <summary> List of pairs of segment indexes that intersect </summary>
        public List<int[]> Inters { get; private set; }
        /// <summary> List of intersection coordinates </summary>
        public List<Vector2> IntersCoords { get; private set; }
        /// <summary> Poligon without intersections that follows the external path of the vertices </summary>
        public List<Vector2> ExternalPath { get; private set; }
        /// <summary>
        /// Segment index to the list of intersections: index of the other intersecting segment,
        /// Coordinate of the intersection, index of the intersection in Intersections
        /// </summary>
        private Dictionary<int, List<(int, Vector2, int)>> SegmToInters;


        public PoligonSelfIntersection(Poligon poligon)
        {
            verts = poligon.Border(0.0f);
            nVerts = verts.Length;
            //Random.InitState(0);
        }


        public void FindIntersections(float pixelPerUnits)
        {
            // Randomizing the xs to avoid y parallel segments and colinear segments
            Tollerance = 1 / pixelPerUnits * 0.1f;
            verts = MicroRandomizeXs(verts, Tollerance);

            // Ordering the vertices according to xs
            List<float> xVerts = new List<float>(new float[verts.Length]);
            for (int i = 0; i < verts.Length; i++) xVerts[i] = verts[i].x;
            xSortedIndexes = Argsort(xVerts);

            // Intersections are stored as pairs of segment indexes ..
            Inters = new List<int[]>();
            // .. and coordinates of the intersections
            IntersCoords = new List<Vector2>();

            // Segments intersected by an imaginary line parallel to y
            // Segments defined by The segment index i: [vertices[i], vertices[i+1]]
            // Those segments are always ordered by y coordinate
            List<int> yLineSegmentIntersections = new List<int>();
            // y coordinate of the segments intersected
            List<float> yLineYCoordsIntersections = new List<float>();


            // The line runs through the x-sorted vertices
            foreach (int vi in xSortedIndexes)
            {
                //string s = "";
                //for (int i = 0; i < yLineYCoordsIntersections.Count; i++)
                //    s += yLineYCoordsIntersections[i].ToString() + " ";
                //Debug.Log(vi.ToString() + " " + s);

                // Vertices through which the y-line passes
                float vx = verts[vi].x, vy = verts[vi].y;

                // Updating the intersection values at the new vx
                for (int i = 0; i < yLineYCoordsIntersections.Count; i++)
                    yLineYCoordsIntersections[i] = YSegmentFromX(vx, yLineSegmentIntersections[i]);

                // Check if the order is scrambled -> intersection
                for (int i = 0; i < yLineYCoordsIntersections.Count - 1; i++)
                    for (int j = i + 1; j < yLineYCoordsIntersections.Count; j++)
                        if (yLineYCoordsIntersections[i] > yLineYCoordsIntersections[j] + Tollerance * 0.01f)
                        {
                            int[] newInters = new int[2] { yLineSegmentIntersections[i], yLineSegmentIntersections[j] };
                            Inters.Add(newInters);
                            Vector2 p1 = verts[newInters[0]], q1 = verts[ModV(newInters[0] + 1)];
                            Vector2 p2 = verts[newInters[1]], q2 = verts[ModV(newInters[1] + 1)];
                            IntersCoords.Add(Utl.SegmentsIntersections(p1, q1, p2, q2));
                        }

                // Reordering the yLineSegmentIntersections
                List<int> newOrdering = Argsort(yLineYCoordsIntersections);
                yLineYCoordsIntersections = ReorderList<float>(yLineYCoordsIntersections, newOrdering);
                yLineSegmentIntersections = ReorderList<int>(yLineSegmentIntersections, newOrdering);


                // Updating the intersections of the line depending on new segments appearing 
                // or disappearing after vi
                float vxNext = verts[ModV(vi + 1)].x, vyNext = verts[ModV(vi + 1)].y;
                float vxPrev = verts[ModV(vi - 1)].x, vyPrev = verts[ModV(vi - 1)].y;

                // Two new segments will be present after the vertex
                if (vxNext > vx && vxPrev > vx)
                {
                    // Angular coefficients of the segments
                    float mPrev = (vyPrev - vy) / (vxPrev - vx);
                    float mNext = (vyNext - vy) / (vxNext - vx);

                    // Indexes of the two new segments ordered by angular coef
                    List<int> newSegms = new List<int>();
                    if (mNext > mPrev)
                        newSegms = new List<int>() { ModV(vi - 1), vi };
                    else if (mNext < mPrev)
                        newSegms = new List<int>() { vi, ModV(vi - 1) };
                    else
                        Debug.LogError("Error, overlapping segments");

                    // Inserting the new segments in a way that the intersections with the line
                    // remain ordered by y coordinate
                    int newSegmI = yLineSegmentIntersections.Count;
                    for (int i = 0; i < yLineSegmentIntersections.Count; i++)
                        if (yLineYCoordsIntersections[i] > vy)
                        {
                            newSegmI = i;
                            break;
                        }

                    yLineSegmentIntersections.Insert(newSegmI, newSegms[0]);
                    yLineSegmentIntersections.Insert(newSegmI + 1, newSegms[1]);
                    yLineYCoordsIntersections.Insert(newSegmI, vy);
                    yLineYCoordsIntersections.Insert(newSegmI + 1, vy);
                }
                // The two coming segments disappears in the current vertex
                else if (vxNext < vx && vxPrev < vx)
                {
                    int segm1 = yLineSegmentIntersections.IndexOf(ModV(vi - 1));
                    int segm2 = yLineSegmentIntersections.IndexOf(vi);
                    yLineSegmentIntersections.RemoveAt(Mathf.Max(segm1, segm2));
                    yLineSegmentIntersections.RemoveAt(Mathf.Min(segm1, segm2));
                    yLineYCoordsIntersections.RemoveAt(Mathf.Max(segm1, segm2));
                    yLineYCoordsIntersections.RemoveAt(Mathf.Min(segm1, segm2));
                }
                // Continuation to a new segment
                else
                {
                    if (vxPrev < vx)
                    {
                        int comingSegmI = yLineSegmentIntersections.IndexOf(ModV(vi - 1));
                        yLineSegmentIntersections[comingSegmI] = vi;
                    }
                    else if (vxNext < vx)
                    {
                        int comingSegmI = yLineSegmentIntersections.IndexOf(vi);
                        yLineSegmentIntersections[comingSegmI] = ModV(vi - 1);
                    }
                    else
                    {
                        Debug.LogError("Vertical segment");
                    }
                }
            }

            BuildSegmToInters();
        }

        public List<Vector2> FindExternalPath(float pixelPerUnits)
        {
            if (Inters == null)
                FindIntersections(pixelPerUnits);

            if (Inters.Count == 0)
                return new List<Vector2>(verts);
            Debug.Log("The poligon is self intersecting, the external path will be drawn");

            // Indexes of vertices (false) or intersections (true) associated to the external path
            // Init from the leftmost vertex
            //List<(int, bool)> indexes = new List<(int, bool)>() { (xSortedIndexes[0], false) };
            int iPrev = xSortedIndexes[0];
            bool iPrevIsInters = false;
            // External path to be returned
            ExternalPath = new List<Vector2>() { verts[iPrev] };

            // Going towards the connected vertex with smallest angle with the vertical
            Vector2 vertP = new Vector2(ExternalPath[0].x, ExternalPath[0].y + 1);
            float a1 = Utl.Angle(vertP, verts[ModV(iPrev - 1)], verts[iPrev]);
            float a2 = Utl.Angle(vertP, verts[ModV(iPrev + 1)], verts[iPrev]);
            int viNext;
            if (a1 < a2) viNext = ModV(iPrev - 1);
            else viNext = ModV(iPrev + 1);

            for (int k = 1; k < verts.Length * (Inters.Count + 1); k++)
            {
                Vector2 pPrev = ExternalPath[ExternalPath.Count - 1];

                int ii, segmI;
                if (iPrevIsInters)
                    (ii, segmI) = FindClosestConnectedPointFromInter(iPrev, viNext);
                else
                    (ii, segmI) = FindClosestConnectedPointFromVertex(iPrev, viNext);

                // The current point is a vertex
                if (ii < 0)
                {
                    // The current vertex is added to the external path
                    ExternalPath.Add(verts[viNext]);

                    // The previous point is not an intersection 
                    if (!iPrevIsInters)
                    {
                        // The next vertex to point is adjacent to the current point
                        if (ModV(viNext + 1) == iPrev)
                        {
                            iPrev = viNext;
                            viNext = ModV(viNext - 1);
                        }
                        else
                        {
                            iPrev = viNext;
                            viNext = ModV(viNext + 1);
                        }
                    }
                    // The previous point is an intersection
                    else
                    {
                        iPrev = viNext;
                        // The vertex with a zero angle from the previous point is chosen for the next vertex  
                        int viAux = ModV(viNext - 1);
                        if (viAux != segmI && viAux != ModV(segmI + 1))
                            viNext = viAux;
                        else
                            viNext = ModV(viNext + 1);
                    }

                    iPrevIsInters = false;
                }
                // The current point is an intersection
                else
                {
                    // The current intersection is added to the external path
                    ExternalPath.Add(IntersCoords[ii]);
                    iPrev = ii; iPrevIsInters = true;

                    // The next vertex to point is the one with the minimal (non zero) angle with pPrev
                    Vector2 p = IntersCoords[ii];
                    List<int> nextVis = new List<int>() { };
                    List<float> angles = new List<float>() { };
                    for (int j1 = 0; j1 < 2; j1++)
                        for(int j2 = 0; j2 < 2; j2++)
                        {
                            int viAux = ModV(Inters[ii][j1] + j2);
                            if (viAux != segmI && viAux != ModV(segmI + 1))
                            {
                                nextVis.Add(viAux);
                                angles.Add(Utl.Angle(pPrev, verts[viAux], p));
                            }
                        }
                    int iMin = Argsort(angles)[0];
                    viNext = nextVis[iMin];
                }
                //Debug.Log(viNext.ToString() + " " + ii.ToString());

                // Break when the current point is the leftmost vertex
                if (xSortedIndexes[0] == iPrev && !iPrevIsInters)
                    break;
            }

            return ExternalPath;
        }

        private void BuildSegmToInters()
        {
            SegmToInters = new Dictionary<int, List<(int, Vector2, int)>>();

            for (int i = 0; i < Inters.Count; i++)
            {
                int[] pair = Inters[i];
                Vector2 point = IntersCoords[i];
                for (int j = 0; j < 2; j++)
                {
                    if (!SegmToInters.ContainsKey(pair[j]))
                        SegmToInters.Add(pair[j], new List<(int, Vector2, int)>() { (pair[1 - j], point, i) });
                    else
                        SegmToInters[pair[j]].Add((pair[1 - j], point, i));
                }
            }
        }

        /// <summary>
        /// Starting from the vertex with index vi and going towards viNext it returns  the
        /// index of the Intersections list of the first intersection along the way.
        /// It returns -1 if there are no intersection between vi and viNext. 
        /// As a second argument it returns also the segment index.
        /// </summary>
        private (int, int) FindClosestConnectedPointFromVertex(int vi, int viNext)
        {
            if (vi != ModV(viNext + 1) && vi != ModV(viNext - 1))
            {
                Debug.LogError("Vertices " + vi.ToString() + " and " + viNext.ToString() + " are not connected");
                return (-2, 0);
            }

            int segmI = viNext;
            if (viNext == ModV((vi + 1)))
                segmI = vi;

            if (!SegmToInters.ContainsKey(segmI))
                return (-1, segmI);
            else
            {
                List<float> dists = new List<float>();
                foreach ((int, Vector2, int) inter in SegmToInters[segmI])
                    dists.Add(Utl.Dist(verts[vi], inter.Item2));
                int closestI = Argsort(dists)[0];
                return (SegmToInters[segmI][closestI].Item3, segmI);
            }        
        }

        /// <summary>
        /// Starting from the intersection with index ii and going towards the vertex viNext 
        /// it returns the index of the Intersections list of the first intersection along the way.
        /// It returns -1 if there are no intersection between vi and viNext. 
        /// As a second argument it returns also the segment index.
        /// </summary>
        private (int, int) FindClosestConnectedPointFromInter(int ii, int viNext)
        {
            int segmI = 0;
            if (Inters[ii][0] == viNext || Inters[ii][0] == ModV(viNext - 1))
                segmI = Inters[ii][0];
            else if (Inters[ii][1] == viNext || Inters[ii][1] == ModV(viNext - 1))
                segmI = Inters[ii][1];
            else
            {
                Debug.LogError("Intersection " + ii.ToString() + " and vertex " + viNext.ToString() + " are not connected");
                return (-2, segmI);
            }

            List<float> dists = new List<float>();
            List<int> middleInters = new List<int>();
            Vector2 iiPoint = IntersCoords[ii];
            float dxSign = Mathf.Sign(verts[viNext].x - iiPoint.x);
            foreach ((int, Vector2, int) inter in SegmToInters[segmI])
                if (inter.Item3 != ii && dxSign * Mathf.Sign(inter.Item2.x - iiPoint.x) > 0)
                {
                    middleInters.Add(inter.Item3);
                    dists.Add(Utl.Dist(iiPoint, inter.Item2));
                }

            if (middleInters.Count == 0)
                return (-1, segmI);
            else
                return (middleInters[Argsort(dists)[0]], segmI);
        }


        /// <summary>
        /// Get y coordinate on a segment given x. The segment index i corresponds to 
        /// [vertices[i], vertices[i+1]].
        /// </summary>
        private float YSegmentFromX(float x, int segmIndex)
        {
            float x1 = verts[segmIndex][0], y1 = verts[segmIndex][1];
            float x2 = verts[ModV(segmIndex + 1)][0], y2 = verts[ModV(segmIndex + 1)][1];

            if (x >= x1 && x <= x2)
                return (y2 - y1) / (x2 - x1) * (x - x1) + y1;
            else if (x >= x2 && x <= x1)
                return (y2 - y1) / (x2 - x1) * (x - x2) + y2;
            else
            {
                Debug.LogError("X outside segment");
                return 0;
            }
        }

        private Vector2[] MicroRandomizeXs(Vector2[] vertices, float delta=1e-5f)
        {
            Vector2[] newVerts = new Vector2[(int)vertices.Length];
            for (int i=0; i< vertices.Length; i++)
                newVerts[i] = new Vector2(Random.Range(vertices[i].x, vertices[i].x + delta), vertices[i].y);
            return newVerts;
        }

        private List<int> Argsort(List<float> list)
        {
            var sorted = list
                .Select((x, i) => new KeyValuePair<float, int>(x, i))
                .OrderBy(x => x.Key)
                .ToList();

            return sorted.Select(x => x.Value).ToList();
        }

        private List<T> ReorderList<T>(List<T> list, List<int> order)
        {
            if (list.Count != order.Count)
            {
                Debug.LogError("Invalid reordering list");
                return null;
            }

            List<T> auxList = new List<T>( new T[list.Count] );
            for (int i = 0; i < list.Count; i++) auxList[i] = list[order[i]];
            return auxList;

        } 

        private int ModV(int x)
        {
            return Utl.Mod(x, nVerts);
        }
    }
}