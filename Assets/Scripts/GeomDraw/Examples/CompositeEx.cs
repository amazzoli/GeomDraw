using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CompositeEx : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer(spriteRenderer);
        drawer.NewEmptySprite(4, 2.5f, 150, Color.white);

        BezierCurve c1 = new BezierCurve(
            new Vector2(1.0f, 0.5f),
            new Vector2(0, 1.5f),
            new Vector2(0.75f, 2),
            new Vector2(1, 1.5f),
            0.02f
        );
        drawer.Draw(c1);

        BezierCurve c2 = new BezierCurve(
            new Vector2(1.0f, 0.5f),
            new Vector2(2, 1.5f),
            new Vector2(1.25f, 2),
            new Vector2(1, 1.5f),
            0.02f
        );

        CompositeShape sh = new CompositeShape(new IDrawableLine[2]{c1, c2}, Color.red);
        sh.Translate(new Vector2(2, 0));
        drawer.Draw(sh);

        drawer.SavePng("Composite_exe");
    }
}