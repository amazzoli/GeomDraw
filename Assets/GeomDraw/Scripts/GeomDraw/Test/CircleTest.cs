using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    public class CircleTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Drawer drawer = new Drawer(spriteRenderer);
            drawer.NewEmptySprite(10, 5, 100, Color.white);

            // KNOWN BUG: Trying with a thickness smaller than 1px leads to an wrong antialized border, check 0.5.
            LineStyle lineStyle = new LineStyle(1f / spriteRenderer.sprite.pixelsPerUnit, new Color(0, 0, 0, 0.5f));
            float rStart = lineStyle.thickness / 2.0f + 0.5f / spriteRenderer.sprite.pixelsPerUnit, rMax = 2.5f, nSteps = 5;
            float r = rStart, rStep = (rMax - rStart) / nSteps;
            while (r <= rMax)
            {
                Circle c = new Circle(new Vector2(2.5f, 2.5f), r, new Color(0, 0, 0, 0.1f), lineStyle);
                drawer.Draw(c);
                r += rStep;
            }

            lineStyle = new LineStyle(20 / spriteRenderer.sprite.pixelsPerUnit, new Color(0, 0, 0, 0.5f));
            rStart = lineStyle.thickness / 2.0f + 0.5f / spriteRenderer.sprite.pixelsPerUnit;
            rStep = (rMax - rStart) / nSteps;
            r = rStart;
            while (r <= rMax)
            {
                Circle c = new Circle(new Vector2(7.5f, 2.5f), r, new Color(0, 0, 0, 0.1f), lineStyle);
                drawer.Draw(c);
                r += rStep;
            }
        }
    }
}
