using Drawing;
using System.Drawing;
using Unity.Profiling;
using UnityEngine;
using Color = UnityEngine.Color;

public class RosoneDraw : MonoBehaviour
{
    Drawer drawer;
    float pxUnit = 100;

    Vector2 center;

    readonly Color darkRed = new Color(0.8f, 0.0f, 0.0f);
    readonly Color orangeRed = new Color(0.95f, 0.15f, 0.05f); 
    readonly Color orangeRed2 = new Color(1f, 0.35f, 0.0f);
    readonly Color orangeYellow = new Color(1f, 0.6f, 0.05f);
    readonly Color gold1 = new Color(1f, 0.80f, 0.0f);
    readonly Color gold2 = new Color(1f, 0.6f, 0.1f);
    readonly Color lemonYellow = new Color(0.9f, 1f, 0.05f);
    readonly Color lightYellow1 = new Color(1f, 0.95f, 0.3f);
    readonly Color lightYellow2 = new Color(1f, 1f, 0.15f);


    readonly float radius1 = 0.75f, radius2 = 1.87f, radius3 = 3.5f;

    LineStyle largeLine, lightLine;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        drawer = new Drawer(spriteRenderer);
        pxUnit = 100;
        drawer.NewEmptySprite(7, 7, pxUnit, Color.black);

        largeLine = new LineStyle(3 / pxUnit, Color.black);
        lightLine = new LineStyle(1 / pxUnit, Color.black);
        center = new Vector2(3.5f, 3.5f);

        Random.InitState(5);

        DrawLayer2();
        DrawLayer1();
        DrawCentralArcs();
        DrawCentralFlower();
    }

    void DrawCentralFlower()
    {
        int nArcs = 8;
        float angle = 0, angleStep = 2 * Mathf.PI / (float)nArcs;
        angle += angleStep / 2.0f;
        float radius = 0.4f;
        for (int i = 0; i < ((int)nArcs / 2f); i++)
        {
            Vector2 p1 = new Vector2(center.x + radius * Mathf.Cos(angle), center.y + radius * Mathf.Sin(angle));
            Vector2 p2 = new Vector2(center.x - radius * Mathf.Cos(angle), center.y - radius * Mathf.Sin(angle));
            SimmCurve s1 = new SimmCurve(p1, p2, Mathf.PI / 3f, radius / 2.5f, lightLine);
            SimmCurve s2 = new SimmCurve(p2, p1, Mathf.PI / 3f, radius / 2.5f, lightLine);
            drawer.Draw(new CompositeShape(new IDrawableLine[] { s1, s2 }, darkRed, lightLine));
            angle += angleStep;
        }
        angle = angleStep / 2.0f;
        for (int i = 0; i < ((int)nArcs / 2f); i++)
        {
            Vector2 p1 = new Vector2(center.x + radius * Mathf.Cos(angle), center.y + radius * Mathf.Sin(angle));
            Vector2 p2 = new Vector2(center.x - radius * Mathf.Cos(angle), center.y - radius * Mathf.Sin(angle));
            SimmCurve s1 = new SimmCurve(p1, p2, Mathf.PI / 3f, radius / 2.5f, lightLine);
            SimmCurve s2 = new SimmCurve(p2, p1, Mathf.PI / 3f, radius / 2.5f, lightLine);
            drawer.Draw(s1);
            drawer.Draw(s2);
            angle += angleStep;
        }
    }

    void DrawCentralArcs()
    {
        int nArcs = 8;
        float angle = 0, angleStep = 2 * Mathf.PI / (float)nArcs;
        float effRadius = radius1 - radius1 * angleStep / 2.5f;
        for (int i = 0; i < nArcs; i++)
        {
            Color c = Color.Lerp(orangeRed2, orangeRed, Random.value);
            DrawArc(angleStep, angle, 0.1f, 0.1f, effRadius, c, largeLine);
            angle += angleStep;
        }
    }

    private void DrawLayer1()
    {
        int nArcs = 16;
        float angle = 0, angleStep = 2 * Mathf.PI / (float)nArcs;
        angle += angleStep / 2f;
        float effRadius = radius2 - radius2 * angleStep / 2f;
        float rFromUp = radius1 + 0.05f, rFromDown = radius1 - 0.05f;
        for (int i = 0; i < nArcs; i++)
        {
            Color c = Color.Lerp(gold1, gold2, Random.value);
            if (i%2 == 0)
                DrawArc(angleStep, angle, rFromUp, rFromDown, effRadius, c, largeLine);
            else
                DrawArc(angleStep, angle, rFromDown, rFromUp, effRadius, c, largeLine);
            DrawPattern1(angleStep, angle, rFromDown, effRadius, lightLine);
            angle += angleStep;
        }
    }

    private void DrawLayer2()
    {
        int nArcs = 32;
        float angle = 0, angleStep = 2 * Mathf.PI / (float)nArcs;
        angle += angleStep / 2f;
        float effRadius = radius3 * (1 - angleStep / 2f);
        float rFromUp = radius2 + 0.075f, rFromDown = radius2 - 0.075f;
        for (int i = 0; i < nArcs; i++)
        {
            Color c = Color.Lerp(lightYellow1, lightYellow2, Random.value);
            if (i % 2 != 0)
            {
                DrawArc(angleStep, angle, rFromUp, rFromDown, effRadius, c, largeLine);
                DrawPattern2Right(angleStep, angle, rFromUp, rFromDown, effRadius, lightLine);
            }
            else
            {
                DrawArc(angleStep, angle, rFromDown, rFromUp, effRadius, c, largeLine);
                DrawPattern2Left(angleStep, angle, rFromUp, rFromDown, effRadius, lightLine);
            }
            angle += angleStep;
        }
    }


    private void DrawArc(float angle, float rotation, float radiusFromLeft, float radiusFromRight, float radiusTo, Color color, LineStyle style)
    {
        IDrawableLine[] arc = Arc(angle, 0, radiusTo, style);

        float th = angle / 2.0f;
        Vector2 leftPointDown = new Vector2(center.x - radiusFromLeft * Mathf.Sin(th), center.y + radiusFromLeft * Mathf.Cos(th));
        Vector2 rightPointDown = new Vector2(center.x + radiusFromRight * Mathf.Sin(th), center.y + radiusFromRight * Mathf.Cos(th));
        float arcRadiusDown = (leftPointDown - rightPointDown).magnitude / 2.0f;

        BrokenLine br1 = new BrokenLine(new Vector2[] { leftPointDown, ((BezierCurve)arc[0]).Points[0] }, false, style);
        BrokenLine br2 = new BrokenLine(new Vector2[] { rightPointDown, ((BezierCurve)arc[1]).Points[3] }, false, style);
        SimmCurve arc3 = new SimmCurve(leftPointDown, rightPointDown, 30 * Mathf.Deg2Rad, arcRadiusDown * 0.25f, style);
        IDrawableLine[] csLines = new IDrawableLine[] { br1, arc[0], arc[1], br2, arc3 };
        CompositeShape cs = new CompositeShape(csLines, color, style);
        cs.Rotate(rotation, center, false);
        drawer.Draw(cs);
    }

    private void DrawPattern1(float angle, float rotation, float radiusFrom, float radiusTo, LineStyle style)
    {
        IDrawableLine[] arc1 = Arc(angle / 2.0f, rotation - angle / 4.0f, radiusTo, style, 0.7f);
        IDrawableLine[] arc2 = Arc(angle / 2.0f, rotation + angle / 4.0f, radiusTo, style, 0.7f);
        foreach (IDrawableLine l in arc1) drawer.Draw(l);
        foreach (IDrawableLine l in arc2) drawer.Draw(l);

        Vector2[] points = new Vector2[] { new Vector2(center.x, radiusFrom + center.y), new Vector2(center.x, radiusTo + center.y) };
        BrokenLine br = new BrokenLine(points, false, style);
        br.Rotate(rotation, center, false);
        drawer.Draw(br);
    }

    private void DrawPattern2Right(float angle, float rotation, float radiusFromUp, float radiusFromDown, float radiusTo, LineStyle style)
    {
        float th = angle / 2.0f;
        Vector2 leftPointDown = new Vector2(center.x - radiusFromUp * Mathf.Sin(th), center.y + radiusFromUp * Mathf.Cos(th));
        Vector2 rightPointUp = new Vector2(center.x + radiusTo * Mathf.Sin(th), center.y + radiusTo * Mathf.Cos(th));
        Vector2 rightPointDown = new Vector2(center.x + radiusFromDown * Mathf.Sin(th), center.y + radiusFromDown * Mathf.Cos(th));
        float l = (leftPointDown - rightPointUp).magnitude;
        SimmCurve c1 = new SimmCurve(leftPointDown, rightPointUp, Mathf.PI / 8f, l / 8f, style);
        c1.Rotate(rotation, center, false);
        drawer.Draw(c1);
        SimmCurve c2 = new SimmCurve(rightPointDown, rightPointUp, Mathf.PI / 8f, l / 8f, style);
        c2.Rotate(rotation, center, false);
        drawer.Draw(c2);
    }

    private void DrawPattern2Left(float angle, float rotation, float radiusFromUp, float radiusFromDown, float radiusTo, LineStyle style)
    {
        float th = angle / 2.0f;
        Vector2 leftPointUp = new Vector2(center.x - radiusTo * Mathf.Sin(th), center.y + radiusTo * Mathf.Cos(th));
        Vector2 rightPointDown = new Vector2(center.x + radiusFromUp * Mathf.Sin(th), center.y + radiusFromUp * Mathf.Cos(th));
        Vector2 leftPointDown = new Vector2(center.x - radiusFromDown * Mathf.Sin(th), center.y + radiusFromDown * Mathf.Cos(th));
        float l = (leftPointUp - rightPointDown).magnitude;
        SimmCurve c1 = new SimmCurve(leftPointUp, rightPointDown, Mathf.PI / 8f, l / 8f, style);
        c1.Rotate(rotation, center, false);
        drawer.Draw(c1);
        SimmCurve c2 = new SimmCurve(leftPointUp, leftPointDown, Mathf.PI / 8f, l / 8f, style);
        c2.Rotate(rotation, center, false);
        drawer.Draw(c2);
    }

    private IDrawableLine[] Arc(float angle, float rotation, float radius, LineStyle style, float radiusFaactor = 1)
    {
        float th = angle / 2.0f;
        Vector2 leftPoint = new Vector2(center.x - radius * Mathf.Sin(th), center.y + radius * Mathf.Cos(th));
        Vector2 rightPoint = new Vector2(center.x + radius * Mathf.Sin(th), center.y + radius * Mathf.Cos(th));
        float arcRadius = (leftPoint - rightPoint).magnitude / 2.0f * radiusFaactor;
        Vector2 arcKey = new Vector2(center.x, center.y + radius + arcRadius);
        SimmCurve arc1 = new SimmCurve(leftPoint, arcKey, 30 * Mathf.Deg2Rad, arcRadius * 0.25f, style);
        SimmCurve arc2 = new SimmCurve(arcKey, rightPoint, 30 * Mathf.Deg2Rad, arcRadius * 0.25f, style);
        arc1.Rotate(rotation, center, false);
        arc2.Rotate(rotation, center, false);
        return new IDrawableLine[] { arc1, arc2 };
    }
}
