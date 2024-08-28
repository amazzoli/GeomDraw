using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BucketEx : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            DrawerSprite drawer = new DrawerSprite(spriteRenderer);
            drawer.NewEmptySprite(4, 4, 100, Color.white);

            LineStyle style = new LineStyle(2.0f / 100.0f, new Color(0.6f, 0.6f, 0.6f));
            BrokenLine line = new BrokenLine(new Vector2[2] { new Vector2(4, 0), new Vector2(0, 4) }, false, style);
            drawer.Draw(line);

            PolygonRegular pentagon = new PolygonRegular(5, new Vector2(2, 2), new Vector2(2, 2), Color.black);
            drawer.Draw(pentagon, true);

            Undoer undo = GetComponent<Undoer>();
            drawer.SavePng("Bucket_exe1");

            drawer.Bucket(new Vector2(0, 0), Color.yellow, 0.0f, true);
            drawer.SavePng("Bucket_exe2");

            undo.Undo(); // Undoing the last bucket
            drawer.Bucket(new Vector2(0, 0), Color.yellow, 0.5f, true);
            drawer.SavePng("Bucket_exe3");

            undo.Undo(); // Undoing the last bucket
            drawer.Bucket(new Vector2(0, 0), Color.yellow, 1, true);
            drawer.SavePng("Bucket_exe4");
        }
    }
}