using Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenLineAngleTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer();
        drawer.NewEmptySprite(spriteRenderer, 10, 4, 100, Color.white);

        float nAngles = 15;
        float a = 90 * Mathf.Deg2Rad;
        float da = 2 * Mathf.PI / (nAngles - 1);
        float dx = 9 / (nAngles - 1);
        float x = 0.5f;
        for (int i = 0; i < nAngles; i++)
        {
            Vector2 p2 = new Vector2(x, 1);
            Vector2 p1 = new Vector2(x, 0);
            Vector2 p3 = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) + p2;
            BrokenLine line = new BrokenLine(new Vector2[3] { p1, p2, p3 }, false, 1 / spriteRenderer.sprite.pixelsPerUnit);
            drawer.Draw(spriteRenderer, line);
            x += dx;
            a += da;
        }

        a = -90 * Mathf.Deg2Rad;
        x = 0.5f;
        LineStyle style = new LineStyle(20 / spriteRenderer.sprite.pixelsPerUnit, new Color(0, 0, 0, 0.5f));
        for (int i = 0; i < nAngles; i++)
        {
            Vector2 p2 = new Vector2(x, 3);
            Vector2 p1 = new Vector2(x, 4);
            Vector2 p3 = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) + p2;
            BrokenLine line = new BrokenLine(new Vector2[3] { p1, p2, p3 }, false, style);
            drawer.Draw(spriteRenderer, line);
            x += dx;
            a -= da;
        }
    }
}
