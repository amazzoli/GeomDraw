using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BucketEx : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer(spriteRenderer);
        drawer.NewEmptySprite(4, 4, 100, Color.white);

        LineStyle style = new LineStyle(2.0f/100.0f, new Color(0.6f, 0.6f, 0.6f));
        BrokenLine line = new BrokenLine(new Vector2[2] { new Vector2(4,0), new Vector2(0,4)}, false, style);
        drawer.Draw(line);

        PoligonRegular pentagon = new PoligonRegular(5, new Vector2(2, 2), new Vector2(2, 2), Color.black);
        drawer.Draw(pentagon);

        DrawnSprite draw = GetComponent<DrawnSprite>();
        draw.SavePng("Bucket_exe1");

        drawer.Bucket(new Vector2(0,0), Color.yellow, 0.0f);
        draw.SavePng("Bucket_exe2");

        draw.Undo(); // Undoing the last bucket
        drawer.Bucket(new Vector2(0,0), Color.yellow, 0.5f);
        draw.SavePng("Bucket_exe3");

        draw.Undo(); // Undoing the last bucket
        drawer.Bucket(new Vector2(0,0), Color.yellow, 1);
        draw.SavePng("Bucket_exe4");
    }
}
