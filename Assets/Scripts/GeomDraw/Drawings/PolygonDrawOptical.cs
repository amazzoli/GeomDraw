using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PolygonDrawOptical : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer(spriteRenderer);
        drawer.NewEmptySprite(4, 4, 100, Color.white);

        float maxSide = 4 * 2;
        float sideStep = 0.18f;
        float rotStep = 0.05f;
        float driftStep = 0.015f;

        Color color = Color.black;
        float rot = 0;
        Vector2 center = new Vector2(2, 2);
        for (float r = maxSide; r > 0.1f; r -= sideStep)
        {
            PolygonRegular poli = new PolygonRegular(5, center, new Vector2(r, r), rot, color, new LineStyle());
            if (color == Color.black)
                color = Color.white;
            else
                color = Color.black;
            rot += rotStep;
            center = new Vector2(center.x, center.y + driftStep);

            drawer.Draw(poli);
        }

        drawer.SavePng("OpticalPentagon");
    }
}
