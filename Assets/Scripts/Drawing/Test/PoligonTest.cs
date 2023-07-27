using Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PoligonTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer();
        drawer.NewEmptySprite(spriteRenderer, 7, 7, 100, Color.white);


        List<Vector2> poliTest1 = new List<Vector2>() {
                    new Vector2(0, 1), new Vector2(0.45f ,2.75f), new Vector2(3.25f, 0), new Vector2(2.75f ,3.25f),
                    new Vector2(4 ,1.75f), new Vector2(2 ,2.5f), new Vector2(3.75f ,0.5f), new Vector2(1.75f, 2.5f),
                    new Vector2(1.5f ,0.05f), new Vector2(4.05f ,3)
                };

        Poligon poligon1 = new Poligon(poliTest1.ToArray(), Color.red, new LineStyle(0.02f, Color.black));
        drawer.Draw(spriteRenderer, poligon1);

        List<Vector2> poliTest2 = new List<Vector2>() {
                    new Vector2(5, 3), new Vector2(6, 1), new Vector2(6, 2), new Vector2(6, 3),
                    new Vector2(6, 4), new Vector2(6.5f, 2.5f), new Vector2(5, 1), new Vector2(5, 1.5f)
                };

        Poligon poligon2 = new Poligon(poliTest2.ToArray(), Color.green, new LineStyle(0.05f, Color.black));
        drawer.Draw(spriteRenderer, poligon2);

        // Known bug: poligon with two overlapping sides
        //List<Vector2> poliTest3 = new List<Vector2>() {
        //            new Vector2(0.5f, 3.5f), new Vector2(0.5f, 5.5f), new Vector2(0.5f + Mathf.Sqrt(2), 4.5f),
        //            new Vector2(0.5f + 2*Mathf.Sqrt(2), 5.5f)
        //        };

        //Poligon poligon3 = new Poligon(poliTest3.ToArray(), Color.green, new LineStyle(0.05f, Color.black));
        //drawer.DrawShape(spriteRenderer, poligon3);
    }
}
