using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class SimmCurveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer(spriteRenderer);
        float pxUnit = 100;
        drawer.NewEmptySprite(4, 4, pxUnit, Color.white);
        LineStyle style = new LineStyle(1 / pxUnit, Color.black);
        LineStyle styleC = new LineStyle(1 / pxUnit, Color.red);

        Vector2 p1 = new Vector2(0.1f, 0.1f);
        Vector2 p2 = new Vector2(0.9f, 0.1f);
        SimmCurve c1 = new SimmCurve(p1, p2, 45 * Mathf.Deg2Rad, 0.2f, style);
        drawer.Draw(c1);
        DrawControls(drawer, c1, styleC);

        p1 += Vector2.up;
        p2 += Vector2.up;
        SimmCurve c2 = new SimmCurve(p2, p1, 45 * Mathf.Deg2Rad, 0.2f, style);
        drawer.Draw(c2);
        DrawControls(drawer, c2, styleC);

        p1 += Vector2.up;
        p2 += Vector2.up;
        SimmCurve c3 = new SimmCurve(p1 +0.25f * Vector2.down, p2 + 0.25f * Vector2.up, 45 * Mathf.Deg2Rad, 0.2f, style);
        drawer.Draw(c3);
        DrawControls(drawer, c3, styleC);
    }

    void DrawControls(Drawer drawer, BezierCurve curve, LineStyle style)
    {
        BrokenLine lc1 = new BrokenLine(new Vector2[] { curve.Points[0], curve.Points[1] }, false, style);
        drawer.Draw(lc1);
        BrokenLine lc2 = new BrokenLine(new Vector2[] { curve.Points[2], curve.Points[3] }, false, style);
        drawer.Draw(lc2);
    }
}
