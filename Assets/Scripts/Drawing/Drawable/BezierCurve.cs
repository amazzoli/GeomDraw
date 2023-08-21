using UnityEditor;
using UnityEngine;

namespace Drawing
{
    public class BezierCurve : IDrawableLine
    {
        private readonly float discrSize = 5;

        protected Vector2[] points;
        protected LineStyle style;
        Vector2[] discretizCache;

        /// <summary> Quadratic Bezier curve, starting from p1, ending in p3 </summary>
        public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, LineStyle style)
        {
            points = new Vector2[3] { p1, p2, p3 };
            this.style = style;
            discretizCache = new Vector2[0];
        }

        /// <summary> Quadratic Bezier curve, starting from p1, ending in p3 </summary>
        public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, float thickness)
        {
            points = new Vector2[3] { p1, p2, p3 };
            style = new LineStyle(thickness, Color.black);
            discretizCache = new Vector2[0];
        }

        /// <summary> Cubic Bezier curve, starting from p1, ending in p4 </summary>
        public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, LineStyle style)
        {
            points = new Vector2[4] { p1, p2, p3, p4 };
            this.style = style;
            discretizCache = new Vector2[0];
        }

        public BezierCurve(LineStyle style)
        {
            discretizCache = new Vector2[0];
            this.style = style;
        }

        /// <summary> Cubic Bezier curve, starting from p1, ending in p4 </summary>
        public BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float thickness)
        {
            points = new Vector2[4] { p1, p2, p3, p4 };
            style = new LineStyle(thickness, Color.black);
            discretizCache = new Vector2[0];
        }

        // IDRAWABLE

        public IDrawable Copy()
        {
            Vector2 p0 = new Vector2(points[0].x, points[0].y);
            Vector2 p1 = new Vector2(points[1].x, points[1].y);
            Vector2 p2 = new Vector2(points[2].x, points[2].y);

            if (points.Length == 3)
                return new BezierCurve(p0, p1, p2, Style.Copy());
            else
                return new BezierCurve(p0, p1, p2, new Vector2(points[3].x, points[3].y), Style.Copy());
        }

        public bool CheckDrawability(float pixelsPerUnit)
        {
            if (Utl.Dist(points[0], points[points.Length - 1]) < 1 / pixelsPerUnit)
            {
                Debug.LogError("Distance between end points smaller than a pixel");
                return false;
            }
            return true;
        }

        // IDRAWABLE TRANSFROMATIONS

        public void Translate(Vector2 translation)
        {
            discretizCache = new Vector2[0];
            for (int i = 0; i < points.Length; i++) points[i] += translation;
        }

        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            discretizCache = new Vector2[0];
            
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
            discretizCache = new Vector2[0];

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
            discretizCache = new Vector2[0];

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

        public LineStyle Style => style;

        public Vector2[] Points => points;

        public (Vector2[], Vector2[]) LeftRightDiscretization(float pixelsPerUnit)
        {
            if (discretizCache.Length == 0) Discretization(pixelsPerUnit);
            BrokenLine aux = new BrokenLine(discretizCache, false, style);
            return aux.LeftRightDiscretization(pixelsPerUnit);
        }

        public Vector2[] Discretization(float pixelsPerUnit)
        {
            if (discretizCache.Length == 0)
            {
                float size = (points[0] - points[1]).magnitude + (points[1] - points[2]).magnitude;
                if (points.Length == 3)
                {
                    int nSteps = Mathf.RoundToInt(size * pixelsPerUnit / discrSize);
                    discretizCache = DiscrBezierQuad(points[0], points[1], points[2], nSteps);
                }
                else if (points.Length == 4)
                {
                    size += (points[2] - points[3]).magnitude;
                    int nSteps = Mathf.RoundToInt(size * pixelsPerUnit / discrSize);
                    discretizCache = DiscrBezierCub(points[0], points[1], points[2], points[3], nSteps);
                }
            }

            return discretizCache;
        }

        private Vector2[] DiscrBezierLin(Vector2 p1, Vector2 p2, int nSteps)
        {
            Vector2[] discr = new Vector2[nSteps];
            float step = 1 / (float)(nSteps - 1);
            for(int i = 0; i < nSteps; i++)
            {
                float t = i * step;
                discr[i] = new Vector2(p2.x * t + p1.x * (1 - t), p2.y * t + p1.y * (1 - t));
            }
            return discr;
        }

        private Vector2[] DiscrBezierQuad(Vector2 p1, Vector2 p2, Vector2 p3, int nSteps)
        {
            Vector2[] l1 = DiscrBezierLin(p1, p2, nSteps), l2 = DiscrBezierLin(p2, p3, nSteps);
            Vector2[] discr = new Vector2[nSteps];
            float step = 1 / (float)(nSteps - 1);
            for (int i = 0; i < nSteps; i++)
            {
                float t = i * step;
                discr[i] = new Vector2(l2[i].x * t + l1[i].x * (1 - t), l2[i].y * t + l1[i].y * (1 - t));
            }
            return discr;
        }

        private Vector2[] DiscrBezierCub(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, int nSteps)
        {
            Vector2[] q1 = DiscrBezierQuad(p1, p2, p3, nSteps), q2 = DiscrBezierQuad(p2, p3, p4, nSteps);
            Vector2[] discr = new Vector2[nSteps];
            float step = 1 / (float)(nSteps - 1);
            for (int i = 0; i < nSteps; i++)
            {
                float t = i * step;
                discr[i] = new Vector2(q2[i].x * t + q1[i].x * (1 - t), q2[i].y * t + q1[i].y * (1 - t));
            }
            return discr;
        }
    }
}