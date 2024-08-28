using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LinesCubicEx : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            DrawerSprite drawer = new DrawerSprite(spriteRenderer);
            drawer.NewEmptySprite(4, 3, 150, Color.white);

            // First Bezier curve
            Vector2 p1 = new Vector2(0.2f, 0.5f);
            Vector2 p2 = new Vector2(0.4f, 2.8f);
            Vector2 p3 = new Vector2(1.7f, 1.8f);
            Vector2 p4 = new Vector2(1.5f, 0.7f);
            BezierCurve bc = new BezierCurve(p1, p2, p3, p4, 2.0f / 100f);
            drawer.Draw(bc);

            // Same Bezier curve translated
            Vector2 translation = new Vector2(2, 0);
            bc.Translate(translation);
            drawer.Draw(bc);

            // Envelop
            p1 += translation;
            p2 += translation;
            p3 += translation;
            p4 += translation;
            LineStyle blOutStyle = new LineStyle(2.0f / 100f, Color.gray);
            BrokenLine blOut = new BrokenLine(new Vector2[4] { p1, p2, p3, p4 }, false, blOutStyle);
            drawer.Draw(blOut);

            int nSteps = 12;
            LineStyle blInStyle = new LineStyle(1.0f / 100f, Color.white * 0.85f);
            LineStyle blIn1Style = new LineStyle(1.0f / 100f, Color.gray);
            for (int i = 0; i < nSteps; i++)
            {
                Vector2 pA = Vector2.Lerp(p1, p2, (i + 1) / (float)(nSteps + 1));
                Vector2 pB = Vector2.Lerp(p2, p3, (i + 1) / (float)(nSteps + 1));
                Vector2 pC = Vector2.Lerp(p3, p4, (i + 1) / (float)(nSteps + 1));
                BrokenLine blIn = new BrokenLine(new Vector2[3] { pA, pB, pC }, false, blInStyle);
                Vector2 pA1 = Vector2.Lerp(pA, pB, (i + 1) / (float)(nSteps + 1));
                Vector2 pB1 = Vector2.Lerp(pB, pC, (i + 1) / (float)(nSteps + 1));
                BrokenLine blIn1 = new BrokenLine(new Vector2[2] { pA1, pB1 }, false, blIn1Style);
                drawer.Draw(blIn);
                drawer.Draw(blIn1);
            }

            // Highlighting the Bezier curve points
            Circle point1Circ = new Circle(p1, 0.04f, Color.blue);
            drawer.Draw(point1Circ);
            Circle point2Circ = new Circle(p2, 0.04f, Color.red);
            drawer.Draw(point2Circ);
            Circle point3Circ = new Circle(p3, 0.04f, Color.red);
            drawer.Draw(point3Circ);
            Circle point4Circ = new Circle(p4, 0.04f, Color.blue);
            drawer.Draw(point4Circ);

            drawer.SavePng("BezierCubic");
        }
    }
}