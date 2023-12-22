using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    public class CircularSector : Circle
    {
        public float angle1 { get; protected set; }
        public float angle2 { get; protected set; }


        /// <summary> Circular sector between angle1 and angle2 in radiants </summary>
        public CircularSector(Vector2 center, float radius, float angle1, float angle2, Color color, LineStyle borderStyle) : 
            base(center, radius, color, borderStyle)
        {
            angle1 %= Mathf.PI * 2;
            angle2 %= Mathf.PI * 2;
            if (angle2 <= angle1) angle2 += Mathf.PI * 2;
            this.angle1 = angle1;
            this.angle2 = angle2;
        }

        // IDRAWABLE

        public override IDrawable Copy()
        {
            Color newColor = new Color(color.r, color.g, color.b, color.a);
            return new CircularSector(new Vector2(center.x, center.y), radius, angle1, angle2, newColor, BorderStyle.Copy());
        }

        public override bool CheckDrawability(float pixelsPerUnit)
        {
            if (!base.CheckDrawability(pixelsPerUnit))
                return false;

            float angle = angle2 - angle1;

            if (angle < borderStyle.thickness / (radius - borderStyle.thickness / 2.0f))
            {
                Debug.LogError("The angle spanned by the circular sector is too small");
                return false;
            }
            if (angle2 - angle1 == Mathf.PI * 2)
            {
                Debug.LogError("This is a circle");
                return false;
            }

            return true;
        }

        // IDRAWABLE TRANSFORMATIONS

        public override void Rotate(float radAngle, Vector2 rotCenter, bool isRelative = true)
        {
            base.Rotate(radAngle, rotCenter, isRelative);   
            angle1 += radAngle;
            angle2 += radAngle;
        }

        public override void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            base.Reflect(axis, coord, isRelative);
            float auxAngle = angle1;
            if (axis == Axis.x)
            {
                angle1 = (-angle2) % (Mathf.PI * 2);
                angle2 = (-auxAngle) % (Mathf.PI * 2);
            }
            else
            {
                angle1 = (Mathf.PI - angle2) % (Mathf.PI * 2);
                angle2 = (Mathf.PI - auxAngle) % (Mathf.PI * 2);
            }
        }

        // BORDER DISCRETIZATION

        protected override Vector2[] ComputeBorder(float pixelsPerUnit)
        {
            int pixelDiameter = Mathf.CeilToInt((2 * radius + borderStyle.thickness) * pixelsPerUnit);
            int nSteps = Mathf.Min(Mathf.CeilToInt((angle2 - angle1) * angleResolution), pixelDiameter * 3);
            borderCache = new Vector2[nSteps + 2];

            borderCache[0] = center;
            float step = (angle2 - angle1) / (nSteps - 1);
            for (int k = 0; k <= nSteps; k++)
            {
                float angle = angle1 + k * step;
                float x = Mathf.Cos(angle) * radius + center.x;
                float y = Mathf.Sin(angle) * radius + center.y;
                borderCache[nSteps - k + 1] = new Vector2(x, y);
            }

            return borderCache;
        }


        // The circular sector implements also the drawable line. This is different 
        // the border because it doesn't draw the connections with the center.

        //public LineStyle Style => borderStyle;

        //public Vector2[] Discretization(float pixelsPerUnit)
        //{
        //    if (borderCache.Length == 0)
        //        borderCache = Border(pixelsPerUnit);

        //    // The center is removed from the border
        //    Vector2[] border = new Vector2[borderCache.Length - 1];
        //    for (int k = 0; k < border.Length; k++) border[k] = borderCache[k + 1];

        //    return border;
        //}

        //public (Vector2[], Vector2[]) LeftRightDiscretization(float pixelsPerUnit)
        //{
        //    if (borderCache.Length == 0)
        //        borderCache = Border(pixelsPerUnit);

        //    Vector2[] border;
        //    if (angle2 - angle1 >= 360)
        //        border = borderCache;
        //    else
        //    {
        //        border = new Vector2[borderCache.Length + 1];
        //        border[0] = center;
        //        for (int k = 0; k < borderCache.Length; k++) border[k + 1] = borderCache[k];
        //    }

        //    BrokenLine aux = new BrokenLine(border, true, Style);
        //    return aux.LeftRightDiscretization(pixelsPerUnit);
        //}
    }
}
