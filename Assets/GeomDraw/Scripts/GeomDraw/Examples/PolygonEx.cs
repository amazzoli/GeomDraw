using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PolygonEx : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            DrawerSprite drawer = new DrawerSprite(spriteRenderer);
            drawer.NewEmptySprite(4, 2.5f, 150, Color.white);

            Vector2[] points = new Vector2[5]{
            new Vector2(0.5f, 0.5f),
            new Vector2(1.0f, 2.1f),
            new Vector2(1.5f, 0.5f),
            new Vector2(0.1f, 1.4f),
            new Vector2(1.9f, 1.4f),
        };
            BrokenLine starLines = new BrokenLine(points, true, new LineStyle(0.02f, Color.black));
            drawer.Draw(starLines);

            starLines.Translate(new Vector2(2, 0));
            Polygon star = new Polygon(starLines.Points, Color.blue);
            drawer.Draw(star);

            drawer.SavePng("Poli_exe");
        }
    }
}