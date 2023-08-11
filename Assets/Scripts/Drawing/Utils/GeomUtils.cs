using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;


namespace Drawing
{
    public class Utl
    {
        // CLOCKWISE COUNTERCLOCKWISE

        public static bool IsClockwisePath(Vector2[] path)
        {
            // Algorithm taken from
            // https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
            float result = 0;
            for (int k = 0; k < path.Length; k++)
            {
                int kNext = (k + 1) % path.Length;
                result += (path[kNext].y + path[k].y) * (path[kNext].x - path[k].x);
            }
            return result > 0;
        }

        public static Vector2[] InvertClockwise(Vector2[] path)
        {
            int L = path.Length;
            Vector2[] newPath = new Vector2[L];
            for (int k = 0; k < L; k++)
                newPath[k] = path[L - k - 1];
            return newPath;
        }


        // INTERSECTION OF SEGMENTS

        /// <summary> Check whether the segments p1q1 intersect the segment p2q2 </summary>
        public static bool SegmentIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            float o1 = Orientation(p1, q1, p2);
            float o2 = Orientation(p1, q1, q2);
            float o3 = Orientation(p2, q2, p1);
            float o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4) return true;

            // Special Cases
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 OnSegment collinear and q2 lies on segment p1q1
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        private static float Orientation(Vector2 p, Vector2 q, Vector2 r)
        {
            float val = (q.y - p.y) * (r.x - q.x) -  (q.x - p.x) * (r.y - q.y);
            if (val == 0) return 0;  // collinear
            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        private static bool OnSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
                q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
                return true;
            return false;
        }

        public static Vector2 SegmentsIntersections(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
        {
            if (!SegmentIntersect(p1, q1, p2, q2))
            {
                Debug.LogError("Segments do not intersect");
                return Vector2.zero;
            }

            Line l1 = new Line(p1, q1), l2 = new Line(p2, q2);
            float x = (l1.Intercept - l2.Intercept) / (l2.AngCoef - l1.AngCoef);
            float y = l1.AngCoef * x + l1.Intercept;
            return new Vector2(x, y);
        }


        // AREA WITHIN PIXELS

        /// <summary>
        /// It computes the area within the pixel below the line. On the right of the line in the case of 
        /// vertical line.
        /// </summary>
        public static float PixelAreaBelowLine(PixelCoord px, Line line, bool complement = false)
        {
            float x1 = px.x1, x2 = px.x2, y1 = px.y1, y2 = px.y2;
            float area = 1;

            if (line.yParallel)
                area = x2 - line.InterceptInverse;
            else if (line.xParallel)
                area = line.Intercept - y1;
            else
            {
                float l_x1 = line.Y(x1), l_x2 = line.Y(x2), l_y1 = line.X(y1), l_y2 = line.X(y2);
                if (line.AngCoef >= 1)
                {
                    if (l_x2 < y1) area = 0;
                    else if (l_x2 < y2) area = (l_x2 - y1) * (x2 - l_y1) * 0.5f;
                    else if (l_x1 < y1) area = x2 - l_y2 + (l_y2 - l_y1) * 0.5f;
                    else if (l_x1 < y2) area = 1 - (y2 - l_x1) * (l_y2 - x1) * 0.5f;
                }
                else if (line.AngCoef > 0)
                {
                    if (l_x2 < y1) area = 0;
                    else if (l_x1 < y1) area = (l_x2 - y1) * (x2 - l_y1) * 0.5f;
                    else if (l_x2 < y2) area = l_x1 - y1 + (l_x2 - l_x1) * 0.5f;
                    else if (l_x1 < y2) area = 1 - (y2 - l_x1) * (l_y2 - x1) * 0.5f;
                }
                else if (line.AngCoef >= -1)
                {
                    if (l_x1 < y1) area = 0;
                    else if (l_x2 < y1) area = (l_x1 - y1) * (l_y1 - x1) * 0.5f;
                    else if (l_x1 < y2) area = l_x2 - y1 + (l_x1 - l_x2) * 0.5f;
                    else if (l_x2 < y2) area = 1 - (y2 - l_x2) * (x2 - l_y2) * 0.5f;
                }
                else
                {
                    if (l_x1 < y1) area = 0;
                    else if (l_x1 < y2) area = (l_x1 - y1) * (l_y1 - x1) * 0.5f;
                    else if (l_x2 < y1) area = l_y2 - x1 + (l_y1 - l_y2) * 0.5f;
                    else if (l_x2 < y2) area = 1 - (y2 - l_x2) * (x2 - l_y2) * 0.5f;
                }
            }

            if (complement)
                return Mathf.Clamp(area, 0, 1);
            else
                return 1 - Mathf.Clamp(area, 0, 1);
        }

        /// <summary>
        /// It computes the area within the pixel on the right of the broken line that starts from line_in,
        /// connects all the innerPoints and exit though line_out.
        /// It assumes that less than 3 inner points are present.
        /// </summary>
        public static float PixelAreaBelowBrokenLine(PixelCoord px, Vector2 pointIn, Line lineIn, Vector2 pointOut, Line lineOut, Vector2[] innerPoints)
        {
            int L = innerPoints.Length;
            (Dir sideIn, float coordIn) = PixelIntersection(px, innerPoints[0], pointIn, lineIn);
            (Dir sideOut, float coordOut) = PixelIntersection(px, innerPoints[L-1], pointOut, lineOut);

            float area = 0;
            if (sideIn == sideOut)
                area += Mathf.Abs(coordIn - coordOut) * PointSideDist(px, sideIn, innerPoints[0]);
            else
            {
                float bs = Mathf.Abs(coordIn - CountClokCoord(px, sideIn));
                area += bs * PointSideDist(px, sideIn, innerPoints[0]);

                bs = 1 - Mathf.Abs(coordOut - CountClokCoord(px, sideOut));
                area += bs * PointSideDist(px, sideOut, innerPoints[0]);

                for (int k=0; k<4; k++)
                {
                    sideIn = DirUtils.RightDir(sideIn);
                    if (sideIn == sideOut) break;
                    area += PointSideDist(px, sideIn, innerPoints[0]);
                }
            }

            if (innerPoints.Length == 2)
            {
                Vector2 borderPoint = CoordOnPixelBorder(px, sideOut, coordOut);
                area += AreaTriangle(innerPoints[0], innerPoints[1], borderPoint);
            }
            else if (innerPoints.Length >= 3) // Not correct for > 3, but it should be unlikely
            {
                Vector2 borderPoint = CoordOnPixelBorder(px, sideOut, coordOut);
                area += AreaQuadrilateral(innerPoints[0], innerPoints[1], innerPoints[2], borderPoint);
            }

            return area / 2.0f;
        }

        /// <summary>
        /// Aux for PixelAreaBelowBrokenLine. Given the side of the pixel specified by dir, the 
        /// counterclockwise point is considered and the coordinate perpendicular to the side is
        /// returned.
        /// </summary>
        private static float CountClokCoord(PixelCoord px, Dir dir)
        {
            if (dir == Dir.north) return px.x1;
            else if (dir == Dir.west) return px.y1;
            else if (dir == Dir.south) return px.x2;
            else /*(dir == Dir.east)*/ return px.y2;
        }

        /// <summary>
        /// Aux for PixelAreaBelowBrokenLine. Given the side of the pixel specified by dir, the 
        /// distance with a point internal to the pixel point is returned.
        /// </summary>
        private static float PointSideDist(PixelCoord px, Dir dir, Vector2 point)
        {
            if (dir == Dir.north) return px.y2 - point.y;
            else if (dir == Dir.west) return point.x - px.x1;
            else if (dir == Dir.south) return point.y - px.y1;
            else /*(dir == Dir.east)*/ return px.x2 - point.x;
        }

        /// <summary>
        /// Aux for PixelAreaBelowBrokenLine. Given the side of the pixel specified by dir, it
        /// returns the point with coordinates the one of the side and the one passed as arg.
        /// </summary>
        private static Vector2 CoordOnPixelBorder(PixelCoord px, Dir dir, float coord)
        {
            if (dir == Dir.north) return new Vector2(px.y2, coord);
            else if (dir == Dir.west) return new Vector2(px.x1, coord);
            else if (dir == Dir.south) return new Vector2(coord, px.y1);
            else /*(dir == Dir.east)*/ return new Vector2(px.x2, coord);
        }

        /// <summary>
        /// Aux for PixelAreaBelowBrokenLine. It finds the intersection of the line connecting 
        /// the point inside the pixel to the point outside with a border of the pixel.
        /// It return the direction specifying the side of the pixel intersecting the line
        /// and the point of the intersection (only the non trivial coordinate).
        /// </summary>
        /// <returns></returns>
        private static (Dir, float) PixelIntersection(PixelCoord px, Vector2 pInside, Vector2 pOutside, Line line)
        {
            float dx = pInside.x - pOutside.x;
            float dy = pInside.y - pOutside.y;
            float th = (float)1e-8;

            if (dx >= th)
            {
                float l_x1 = line.Y(px.x1);
                if (dy >= 1e-8)
                {
                    if (l_x1 < px.y1) return (Dir.south, line.X(px.y1));
                    else return (Dir.west, l_x1);
                }
                else if (dy < th && dy > -th)  
                    return (Dir.west, pInside.y);
                else
                {
                    if (l_x1 > px.y2) return (Dir.north, line.X(px.y2));
                    else return (Dir.west, l_x1);
                }
            }
            else if (dx < th && dx > -th)
            {
                if (dy > 0) return (Dir.south, pInside.x);
                else return (Dir.north, pInside.x);
            }
            else
            {
                float l_x2 = line.Y(px.x2);
                if (dy > th)
                {
                    if (l_x2 < px.y1) return (Dir.south, line.X(px.y1));
                    else return (Dir.east, l_x2);
                }
                else if (dy < th && dy > -th)
                    return (Dir.east, pInside.y);
                else
                {
                    if (l_x2 > px.y2)
                        return (Dir.north, line.X(px.y2));
                    else
                        return (Dir.east, l_x2);
                }
            }
        }

        private static float AreaTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float a = (p1 - p2).magnitude, b = (p2 - p3).magnitude, c = (p3 - p1).magnitude;
            float smp = (a + b + c) / 2.0f;
            return Mathf.Sqrt(smp * (smp - a) * (smp - b) * (smp - c));
        }

        private static float AreaQuadrilateral(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            float diag = (p1 - p3).magnitude;
            float a = (p1 - p2).magnitude, b = (p2 - p3).magnitude;
            float smp = (a + b + diag) / 2.0f;
            float a1 = Mathf.Sqrt(smp * (smp - a) * (smp - b) * (smp - diag));
            a = (p3 - p4).magnitude; b = (p4 - p1).magnitude;
            smp = (a + b + diag) / 2.0f;
            return a1 + Mathf.Sqrt(smp * (smp - a) * (smp - b) * (smp - diag));
        }

        public static float Dist(Vector2 v1, Vector2 v2)
        {
            return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));
        }

        public static float Angle(Vector2 p1, Vector2 p2, Vector2 origin)
        {
            float x1 = p1[0] - origin[0];
            float x2 = p2[0] - origin[0];
            float y1 = p1[1] - origin[1];
            float y2 = p2[1] - origin[1];
            float t1 = Mathf.Atan2(x1, y1), t2 = Mathf.Atan2(x2, y2);
            return (t2 - t1 + 4 * Mathf.PI) % (2 * Mathf.PI);
        }

        public static float Angle(Vector2 p1, Vector2 p2)
        {
            return Angle(p1, p2, Vector2.zero);
        }

        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static Vector2 Rotate(Vector2 point, float radAngle)
        {
            float x = point.x * Mathf.Cos(radAngle) - point.y * Mathf.Sin(radAngle);
            float y = point.x * Mathf.Sin(radAngle) + point.y * Mathf.Cos(radAngle);
            return new Vector2(x, y);
        }

        /// <summary>
        /// Center of mass of the points as the average of x and y coordinates 
        /// </summary>
        public static Vector2 MassCenter(Vector2[] points)
        {
            Vector2 center = new Vector2(0, 0);
            foreach(Vector2 point in points)
            {
                center.x += point.x;
                center.y += point.y;
            }
            center.x /= (float)points.Length;
            center.y /= (float)points.Length;

            return center;
        }

        /// <summary>
        /// Rectangle in which the points are inscribed
        /// </summary>
        public static Rect ContainingRect(Vector2[] points)
        {
            return ContainingRect(new List<Vector2>(points));
        }

        /// <summary>
        /// Center of the rectangle in which the points are inscribed
        /// </summary>
        public static Vector2 RectCenter(Vector2[] points)
        {
            return RectCenter(new List<Vector2>(points));
        }

        /// <summary>
        /// Rectangle in which the points are inscribed
        /// </summary>
        public static Rect ContainingRect(List<Vector2> points)
        {
            var xSorted = points.Select((x, i) => new KeyValuePair<float, int>(x.x, i)).OrderBy(x => x.Key).ToList();
            float xMin = xSorted[0].Key, xMax = xSorted[xSorted.Count - 1].Key;
            var ySorted = points.Select((x, i) => new KeyValuePair<float, int>(x.y, i)).OrderBy(x => x.Key).ToList();
            float yMin = ySorted[0].Key, yMax = ySorted[ySorted.Count - 1].Key;
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }


        /// <summary>
        /// Center of the rectangle in which the points are inscribed
        /// </summary>
        public static Vector2 RectCenter(List<Vector2> points)
        {
            Rect rect = ContainingRect(points);
            return rect.center;
        }
    }
}