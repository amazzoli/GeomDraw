using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CompositeTest : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            DrawerSprite drawer = new DrawerSprite(spriteRenderer);
            drawer.NewEmptySprite(5, 5, 100, Color.white);


            Vector2[] blPoints1 = new Vector2[] {
                    new Vector2(0.1f, 0.1f), new Vector2(1, 0.1f), new Vector2(2, 0.5f), new Vector2(2, 1),
                };
            Vector2[] blPoints2 = new Vector2[] {
                    new Vector2(1,1), new Vector2(0.1f, 1)
                };

            BrokenLine brokenLine1 = new BrokenLine(blPoints1, false, 0.02f);
            BrokenLine brokenLine2 = new BrokenLine(blPoints2, false, 0.02f);

            drawer.Draw(brokenLine1);
            drawer.Draw(brokenLine2);

            CompositeShape cs = new CompositeShape(new BrokenLine[] { brokenLine1, brokenLine2 }, new Color(1, 0, 0, 0.5f), new LineStyle());
            drawer.Draw(cs);


            brokenLine1.Translate(new Vector2(0, 1.5f));
            Vector2[] blPoints3 = new Vector2[] {
                    new Vector2(0.3f, 3f), new Vector2(1, 2f), new Vector2(0.1f, 2f), new Vector2(1, 3f)
                };
            BrokenLine brokenLine3 = new BrokenLine(blPoints3, false, 0.02f);
            drawer.Draw(brokenLine1);
            drawer.Draw(brokenLine3);

            CompositeShape cs2 = new CompositeShape(new BrokenLine[] { brokenLine1, brokenLine3 }, new Color(1, 0, 0, 0.5f), new LineStyle());
            drawer.Draw(cs2);

            brokenLine1.Translate(new Vector2(0, 2f));
            BezierCurve bz1 = new BezierCurve(new Vector2(0.1f, 4.5f), new Vector2(0.1f, 4f), new Vector2(1f, 4f), new Vector2(1f, 4.5f), 0.02f);
            bz1.Translate(new Vector2(0, -0.7f));
            drawer.Draw(brokenLine1);
            drawer.Draw(bz1);

            CompositeShape cs3 = new CompositeShape(new IDrawableLine[] { brokenLine1, bz1 }, new Color(1, 0, 0, 0.5f), new LineStyle());
            drawer.Draw(cs3);

            CompositeShape cs3a = (CompositeShape)cs3.Copy();
            cs3a.Translate(new Vector2(2.5f, 0));
            cs3a.Rotate(30 * Mathf.Deg2Rad, Vector2.zero, true);
            drawer.Draw(cs3a);

            CompositeShape cs3b = (CompositeShape)cs3.Copy();
            cs3b.Translate(new Vector2(2.5f, -1.8f));
            cs3b.Reflect(Axis.y, 0, true);
            drawer.Draw(cs3b);

            CompositeShape cs3c = (CompositeShape)cs3.Copy();
            cs3c.Translate(new Vector2(2.5f, -3.2f));
            cs3c.Deform(Axis.x, 0.5f, 0, true);
            drawer.Draw(cs3c);

            bz1.Translate(new Vector2(0, 0.7f));
            BezierCurve bz2 = (BezierCurve)bz1.Copy();
            bz2.Reflect(Axis.x, 4.5f, false);
            CompositeShape cs4 = new CompositeShape(new IDrawableLine[] { bz1, bz2 }, new Color(1, 0, 0, 0.5f), new LineStyle());
            drawer.Draw(cs4);
        }
    }
}
