using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeomDraw
{
    public class BrokenLine : IDrawableLine
    {
        readonly float discrLength = 30; // in pixels
        readonly float discrLengthCusp = 2.5f; // in pixels

        Vector2[] points;
        bool isClosed;
        float thickness;
        float hTick;
        Color color;

        private Vector2[] tangents;
        private Vector2[] normals;
        private float[] lengths;
        private float pixelPerUnits;

        /// <summary> Segment in world units </summary>
        public BrokenLine(Vector2[] points, bool isClosed, LineStyle style)
        {
            Init(points, isClosed, style.color, style.thickness);
        }

        /// <summary> Black segment in world units </summary>
        public BrokenLine(Vector2[] points, bool isClosed, float thickness)
        {
            Init(points, isClosed, Color.black, thickness);
        }

        // IDRAWABLE

        public IDrawable Copy()
        {
            Vector2[] newPoints = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++) newPoints[i] = new Vector2(points[i].x, points[i].y);
            return new BrokenLine(newPoints, isClosed, Style.Copy());
        }

        public bool CheckDrawability(float pixelsPerUnit)
        {
            // Short side check
            List<Vector2> newVert = new List<Vector2>();
            Vector2 oldVert = points[0];

            for (int i = 0; i < points.Length; i++)
            {
                int iNext = (i + 1) % points.Length;
                if ((oldVert - points[iNext]).magnitude * pixelsPerUnit > 1)
                {
                    newVert.Add(points[i]);
                    oldVert = points[iNext];
                }
            }
            points = newVert.ToArray();

            if (points.Length < 2)
            {
                Debug.LogError("Broken line with less than 2 points");
                return false;
            }
                
            if (points.Length == 2 && isClosed)
            {
                Debug.LogError("Closed broken line with less than 3 points");
                return false;
            }

            return true;
        }

        // IDRAWABLE TRANSFORMATIONS

        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < points.Length; i++) points[i] += translation;
        }

        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            if (isRelative)
                rotCenter += Utl.RectCenter(points);
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 auxP = points[i] - rotCenter;
                points[i] = Utl.Rotate(auxP, radAngle) + rotCenter;
            }
        }

        public void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            float cRefl = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cRefl += Utl.RectCenter(points).y;
                for (int i = 0; i < points.Length; i++)
                    points[i].y = 2 * cRefl - points[i].y;
            }
            else
            {
                if (isRelative) cRefl += Utl.RectCenter(points).x;
                for (int i = 0; i < points.Length; i++)
                    points[i].x = 2 * cRefl - points[i].x;
            }
        }

        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            float cDef = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cDef += Utl.RectCenter(points).x;
                for (int i = 0; i < points.Length; i++)
                    points[i].x = factor * (points[i].x - cDef) + cDef;
            }
            else
            {
                if (isRelative) cDef += Utl.RectCenter(points).y;
                for (int i = 0; i < points.Length; i++)
                    points[i].y = factor * (points[i].y - cDef) + cDef;
            }
            return true;
        }

        // IDRAWABLE LINE

        public float Length
        {
            get
            {
                float l = 0;
                for (int i = 0; i < points.Length - 1; i++) l += (points[i + 1] - points[i]).magnitude;
                if (isClosed) l += (points[points.Length - 1] - points[0]).magnitude;
                return l;
            }
        }

        public LineStyle Style => new LineStyle(thickness, color);

        public Vector2[] Points => points;

        public Vector2[] Discretization(float pixelPerUnit) => points;

        public (Vector2[], Vector2[]) LeftRightDiscretization(float pixelsPerUnit)
        {
            this.pixelPerUnits = pixelsPerUnit;
            List<Vector2> discrLeft = new List<Vector2>() {};
            List<Vector2> discrRight = new List<Vector2>() {};

            // Computing normals and tangents
            int nTang = points.Length;
            if (!isClosed) nTang -= 1;
            tangents = new Vector2[nTang];
            normals = new Vector2[nTang];
            lengths = new float[nTang];   
            for (int i=0; i<nTang; i++)
            {
                Vector2 p1 = points[i], p2 = points[Mod(i + 1)];
                float norm = Utl.Dist(p1, p2);
                lengths[i] = norm;
                tangents[i] = new Vector2((p2.x - p1.x) / norm, (p2.y - p1.y) / norm);
                normals[i] = new Vector2(-tangents[i].y, tangents[i].x);
            }

            // First corner discretization is different for close or open lines
            if (!isClosed)
            {
                discrLeft.Add(normals[0] * hTick + points[0]);
                discrRight.Add(-normals[0] * hTick + points[0]);
            }
            else
            {
                (Vector2[] dLeft, Vector2[] dRight) = DiscretizationAtCorner(points.Length - 1);
                discrLeft.AddRange(dLeft);
                discrRight.AddRange(dRight);
            }

            //for (int k = 0; k < discrRight.Count; k++)
            //    Debug.Log(discrLeft[k].x + " " + discrLeft[k].y + "\t" + discrRight[k].x + " " + discrRight[k].y);

            for (int i=0; i<nTang; i++)
            {
                // Finding the discretizations of the next point corner
                Vector2[] leftCorners, rightCorners;
                int lastI = points.Length - 1;
                // Last point of an open line
                if (i == lastI - 1 && !isClosed)
                {
                    leftCorners = new Vector2[1] { normals[lastI - 1] * hTick + points[lastI] };
                    rightCorners = new Vector2[1] { -normals[lastI - 1] * hTick + points[lastI] };
                }
                else
                {
                    // Last point of a closed line
                    if (i == lastI)
                    {
                        leftCorners = new Vector2[1] { discrLeft[0] };
                        rightCorners = new Vector2[1] { discrRight[0] };
                    }
                    // All the other cases
                    else
                    {
                        (leftCorners, rightCorners) = DiscretizationAtCorner(i);
                    }
                }

                //for (int k= 0; k<leftCorners.Length; k++ )
                //    Debug.Log(leftCorners[k].x + " " + leftCorners[k].y + "\t" + rightCorners[k].x + " " + rightCorners[k].y);

                // Finding the discretization of the line between the corners
                float length = (points[Mod(i + 1)] - points[i]).magnitude;
                float nSteps = Mathf.RoundToInt(length / discrLength * pixelsPerUnit);
                float leftLen = (leftCorners[0] - discrLeft[discrLeft.Count - 1]).magnitude;
                float leftStep = leftLen / (nSteps + 1);
                float rightLen = (rightCorners[0] - discrRight[discrRight.Count - 1]).magnitude;
                float rightStep = rightLen / (nSteps + 1);
                for (int k = 0; k < nSteps; k++)
                {
                    discrLeft.Add(tangents[i] * leftStep + discrLeft[discrLeft.Count - 1]);
                    discrRight.Add(tangents[i] * rightStep + discrRight[discrRight.Count - 1]);
                }

                if (i < lastI)
                {
                    discrLeft.AddRange(leftCorners);
                    discrRight.AddRange(rightCorners);
                }
                else
                {
                    discrLeft.Add(discrLeft[0]);
                    discrRight.Add(discrRight[0]);
                }
            }

            return (discrLeft.ToArray(), discrRight.ToArray());
        }


        private void Init(Vector2[] points, bool isClosed, Color color, float thickness)
        {
            this.points = points;
            this.isClosed = isClosed;
            this.thickness = thickness;
            this.color = color;
            hTick = thickness / 2;
        }

        private (Vector2[], Vector2[]) DiscretizationAtCorner(int i)
        {
            float a = (tangents[Mod(i + 1)] + tangents[i]).magnitude / 2.0f;
            // b is the distance between the corner and the intersection of the 
            // lines spanned by the  discretization 
            float b = hTick / a;
            Vector2 avNormal = (normals[Mod(i + 1)] + normals[i]) / 2.0f;
            avNormal /= avNormal.magnitude;

            //Debug.Log("corner" + a + "\t" + b);

            // If b too large (corner with angle smaller than 90°)
            if (b > hTick * Mathf.Sqrt(2) + 0.00001f)
            {
                bool isLeft = Vector3.Cross(tangents[i], tangents[Mod(i + 1)]).z < 0;
                return DiscrteizationAtCusp(isLeft, i, avNormal, b);
            }
            else
            {
                Vector2[] d1 = new Vector2[1] { avNormal * b + points[Mod(i + 1)] };
                Vector2[] d2 = new Vector2[1] { -avNormal * b + points[Mod(i + 1)] };
                return (d1, d2);
            }
        }

        private (Vector2[], Vector2[]) DiscrteizationAtCusp(bool isLeft, int i, Vector2 avNormal, float b)
        {
            int iNext = Utl.Mod(i + 1, normals.Length);
            int sign;
            float angle;
            if (isLeft) {
                sign = 1;
                angle = Utl.Angle(normals[i], normals[iNext]);
            }
            else
            {
                sign = -1;
                angle = Utl.Angle(normals[iNext], normals[i]);
            }

            float thStart = Mathf.Atan2(normals[i].y, normals[i].x);
            float thStep = angle * hTick / discrLengthCusp * pixelPerUnits;
            int nSteps = Mathf.Max(1, Mathf.RoundToInt(thStep));
            thStep = angle / nSteps;

            Vector2[] discr1 = new Vector2[nSteps + 1], discr2 = new Vector2[nSteps + 1];
            discr1[0] = sign * normals[i] * hTick + points[Mod(i + 1)];
            if (b > Mathf.Min(lengths[i], lengths[iNext])) 
                discr2[0] = -sign * tangents[i] * hTick + points[Mod(i + 1)];
            else
                discr2[0] = -sign * avNormal * b + points[Mod(i + 1)];
            //Debug.Log("cusp" + isLeft + "\t" + angle + "\t" + nSteps);

            float th = thStart;
            for (int j = 0; j < nSteps; j++)
            {
                th -= sign * thStep;
                Vector2 vect = new Vector2(Mathf.Cos(th), Mathf.Sin(th)) * hTick;
                discr1[j + 1] = sign * vect + points[Mod(i + 1)];
                if (b > Mathf.Min(lengths[i], lengths[iNext]))
                    discr2[j + 1] = -sign * tangents[i] * hTick + points[Mod(i + 1)];
                else
                    discr2[j + 1] = -sign * avNormal * b + points[Mod(i + 1)];
            }

            if (isLeft) return (discr1, discr2);
            else return (discr2, discr1);
        }


        private int Mod(int n)
        {
            return Utl.Mod(n, points.Length);
        }
    }
}