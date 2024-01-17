using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Profiling;

namespace GeomDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EllipseDraw : MonoBehaviour
    {
        Drawer drawer;
        Color c1 = ColorUtils.ColorHEX("#1a2a6cff");
        Color c2 = ColorUtils.ColorHEX("#b21f1f88");
        Color c3 = ColorUtils.ColorHEX("#fdbb2dff");

        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            drawer = new Drawer(spriteRenderer);
            drawer.NewEmptySprite(12, 8, 100, Color.white);

            List<Color> gradColors1 = new List<Color>() { c1, c2, c3 };
            List<Color> gradColors2 = new List<Color>() { c3, c2, c1 };

            foreach (float x in new float[3] { 2, 6, 10 })
            {
                foreach (float y in new float[2] { 2, 6 })
                {
                    DrawEllipses(new Vector2(x, y), spriteRenderer, gradColors1);
                }
            }

            foreach (float x in new float[4] { 0, 4, 8, 12 })
            {
                foreach (float y in new float[3] { 0, 4, 8 })
                {
                    DrawEllipses(new Vector2(x, y), spriteRenderer, gradColors2);
                }
            }

            drawer.SavePng("Ellipses_draw");
        }

        private void DrawEllipses(Vector2 center, SpriteRenderer renderer, List<Color> gradient)
        {
            int nSteps = 8;
            float axisX0 = 0.2f, axisX1 = 2f;
            float axisY0 = 2f, axisY1 = 0.2f;

            for (int i = 0; i < nSteps + 1; i++)
            {
                float x = i / (float)nSteps;
                float axisX = Mathf.Lerp(axisX0, axisX1, x);
                float axisY = Mathf.Lerp(axisY0, axisY1, x);
                Color c = ColorUtils.Gradient(x, gradient);
                Ellipse el = new Ellipse(center, axisX, axisY, 0, c);
                drawer.Draw(el, false);
            }
        }
    }
}
