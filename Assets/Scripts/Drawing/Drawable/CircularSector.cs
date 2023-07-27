using Drawing;
using UnityEditor;
using UnityEngine;

namespace Drawing
{
    public class CircularSector : Circle
    {
        public float angle1 { get; protected set; }
        public float angle2 { get; protected set; }


        /// <summary> Circular sector between angle1 and angle2 in degree </summary>
        public CircularSector(Vector2 center, float radius, float angle1, float angle2, Color color, LineStyle borderStyle) : 
            base(center, radius, color, borderStyle)
        {
            angle1 %= 360;
            angle2 %= 360;
            if (angle2 <= angle1) angle2 += 360;
            this.angle1 = angle1;
            this.angle2 = angle2;
        }

        public override bool CheckDrawability(float pixelsPerUnit)
        {
            if (!base.CheckDrawability(pixelsPerUnit))
                return false;

            float angle = (angle2 - angle1) * Mathf.Deg2Rad;

            if (angle < borderStyle.thickness / (radius - borderStyle.thickness / 2.0f))
            {
                Debug.LogError("The angle spanned by the circular sector is too small");
                return false;
            }
            if (angle2 - angle1 == 360)
            {
                Debug.LogError("This is a circle");
                return false;
            }

            return true;
        }

        protected override Vector2[] ComputeBorder(float pixelsPerUnit)
        {
            int pixelDiameter = Mathf.CeilToInt((2 * radius + borderStyle.thickness) * pixelsPerUnit);
            int nSteps = Mathf.Min(Mathf.CeilToInt((angle2 - angle1) * Mathf.Deg2Rad * angleResolution), pixelDiameter * 3);
            borderCache = new Vector2[nSteps + 2];

            borderCache[0] = center;
            float step = (angle2 - angle1) / nSteps;
            for (int k = 0; k <= nSteps; k++)
            {
                float angle = angle1 + k * step;
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius + center.x;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius + center.y;
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