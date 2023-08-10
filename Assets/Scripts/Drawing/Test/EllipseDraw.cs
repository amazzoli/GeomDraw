using Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EllipseDraw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer(spriteRenderer);
        drawer.NewEmptySprite(10, 10, 100, Color.white);

        Vector2 center = new Vector2(2.5f, 2.5f);
        DrawEllipses1(center, spriteRenderer);

        center = new Vector2(7.5f, 7.5f);
        DrawEllipses1(center, spriteRenderer);

        center = new Vector2(2.5f, 7.5f);
        DrawEllipses1(center, spriteRenderer);

        center = new Vector2(7.5f, 2.5f);
        DrawEllipses1(center, spriteRenderer);

        center = new Vector2(5f, 5f);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(5f, 10f);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(10f, 5f);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(5f, 0);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(0, 5f);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(10f, 10f);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(0, 10);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(10, 0);
        DrawEllipses2(center, spriteRenderer);

        center = new Vector2(0, 0);
        DrawEllipses2(center, spriteRenderer);
    }

    private void DrawEllipses1(Vector2 center, SpriteRenderer renderer)
    {
        Drawer drawer = new Drawer(renderer);
        List<Color> gradColors = new List<Color>()
        {
            ColorUtils.ColorHEX("#1a2a6cff"),
            ColorUtils.ColorHEX("#b21f1f88"),
            ColorUtils.ColorHEX("#fdbb2dff")
        };
        List<float> gradSpacing = new List<float>() { 0, 0.5f, 1 };

        int nSteps = 8;
        float axisX0 = 0.25f, axisX1 = 2.5f;
        float axisY0 = 2.5f, axisY1 = 0.25f;

        for (int i = 0; i < nSteps + 1; i++)
        {
            float x = i / (float)nSteps;
            float axisX = Mathf.Lerp(axisX0, axisX1, x);
            float axisY = Mathf.Lerp(axisY0, axisY1, x);
            Color c = ColorUtils.Gradient(x, gradColors, gradSpacing);
            Ellipse el = new Ellipse(center, axisX, axisY, 0, c);
            drawer.Draw(el);
        }
    }

    private void DrawEllipses2(Vector2 center, SpriteRenderer renderer)
    {
        Drawer drawer = new Drawer(renderer);
        List<Color> gradColors = new List<Color>()
        {
            ColorUtils.ColorHEX("#fdbb2dff"),
            ColorUtils.ColorHEX("#b21f1f88"),
            ColorUtils.ColorHEX("#1a2a6cff"),
        };
        List<float> gradSpacing = new List<float>() { 0, 0.5f, 1 };

        int nSteps = 8;
        float axisX0 = 0.25f, axisX1 = 2.5f;
        float axisY0 = 2.5f, axisY1 = 0.25f;

        for (int i = 0; i < nSteps + 1; i++)
        {
            float x = i / (float)nSteps;
            float axisX = Mathf.Lerp(axisX0, axisX1, x);
            float axisY = Mathf.Lerp(axisY0, axisY1, x);
            Color c = ColorUtils.Gradient(x, gradColors, gradSpacing);
            Ellipse el = new Ellipse(center, axisX, axisY, 0, c);
            drawer.Draw(el);
        }
    }
}
