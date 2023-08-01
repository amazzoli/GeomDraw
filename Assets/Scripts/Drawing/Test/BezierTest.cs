using Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer();
        float nSteps = 10;
        float pxUnit = 100;
        drawer.NewEmptySprite(spriteRenderer, nSteps, 4, pxUnit, Color.white);

        float width = 0.9f;
        float x = 0.5f;
        float ctrlSpread = 0;
        float ctrlMaxSread = 4;
        float spreadStep = (ctrlMaxSread - ctrlSpread) / (nSteps - 1);
        for (int i = 0; i < nSteps; i++)
        {
            Vector2 p0 = new Vector2(x - width / 2.0f, 0.1f);
            Vector2 pf = new Vector2(x + width / 2.0f, 0.1f);
            Vector2 c1 = new Vector2(x - width / 2.0f, 2f);
            Vector2 c2 = new Vector2(x + width / 2.0f - ctrlSpread, 2f);
            BezierCurve line = new BezierCurve(p0, c1, c2, pf, 1 / pxUnit);
            drawer.Draw(spriteRenderer, line);
            x += 1;
            ctrlSpread += spreadStep;
        }

        ctrlSpread = 0;
        x = 0.5f;
        for (int i = 0; i < nSteps; i++)
        {
            Vector2 p0 = new Vector2(x - width / 2.0f, 2.1f);
            Vector2 pf = new Vector2(x + width / 2.0f, 2.1f);
            Vector2 c1 = new Vector2(x - width / 2.0f, 4f);
            Vector2 c2 = new Vector2(x + width / 2.0f - ctrlSpread, 4f);
            BezierCurve line = new BezierCurve(p0, c1, c2, pf, 20 / pxUnit);
            drawer.Draw(spriteRenderer, line);
            x += 1;
            ctrlSpread += spreadStep;
        }
    }
}
