using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AmazzoliLogo : MonoBehaviour
    {
        DrawerSprite drawer;
        Color c1 = new Color(0, 0, 0, 0);
        Color c2 = ColorUtils.ColorHEX("#fdbb2dff");
        Color c3 = ColorUtils.ColorHEX("#b21f1fff");
        Color c4 = ColorUtils.ColorHEX("#1a2a6cff");

        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            drawer = new DrawerSprite(spriteRenderer, true);
            drawer.NewEmptySprite(6, 4, 200, new Color(0, 0, 0, 1));

            Vector2[] pointsM = new Vector2[5]
            {
            new Vector2(0.75f, 1),
            new Vector2(2.825f, 3),
            new Vector2(2.825f, 1),
            new Vector2(5, 3),
            new Vector2(5, 1)
            };

            //BrokenLine m = new(pointsM, false, 0.02f);
            //drawer.Draw(m);

            int nLines = 35;
            DrawBezierInterpolation(pointsM[0], pointsM[1], pointsM[2], nLines, c1, c2);
            DrawBezierInterpolation(pointsM[1], pointsM[2], pointsM[3], nLines, c2, c3);
            DrawBezierInterpolation(pointsM[2], pointsM[3], pointsM[4], nLines, c3, c4);

            drawer.SavePng("amazzoli_logo");
        }

        private void DrawBezierInterpolation(Vector2 p1, Vector2 p2, Vector2 p3, int nSteps, Color c1, Color c2)
        {
            List<Color> colors = new() { c1, c2 };
            for (int i = 0; i < nSteps; i++)
            {
                float f = i / (float)(nSteps - 1);
                Color c = ColorUtils.Gradient(f, colors);
                LineStyle ls = new LineStyle(1.0f / 100f, c);
                Vector2 pA = Vector2.Lerp(p1, p2, f);
                Vector2 pB = Vector2.Lerp(p2, p3, f);
                BrokenLine blIn = new BrokenLine(new Vector2[2] { pA, pB }, false, ls);
                drawer.Draw(blIn);
            }
        }
    }
}
