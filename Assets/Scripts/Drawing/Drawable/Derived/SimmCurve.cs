using Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimmCurve : BezierCurve
{
    /// <summary>
    /// Bezier cubic simmetric curve connecting p1 to p2.
    /// </summary>
    /// <param name="angle"> Angle between the curve an the segment p1-p2 </param>
    /// <param name="depth"> Distance of the control points from the segment p1-p2 </param>
    public SimmCurve(Vector2 p1, Vector2 p2, float angle, float depth, LineStyle style) : 
        base(style)
    {
        float phi = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x);
        float d = depth / Mathf.Sin(angle);
        float c1x = p1.x + d * Mathf.Cos(angle + phi);
        float c1y = p1.y + d * Mathf.Sin(angle + phi);
        float c2x = p2.x - d * Mathf.Cos(angle - phi);
        float c2y = p2.y + d * Mathf.Sin(angle - phi);

        points = new Vector2[] { p1, new Vector2(c1x, c1y), new Vector2(c2x, c2y), p2 };
    }
}
