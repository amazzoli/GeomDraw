using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LinesEx : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer = new Drawer(spriteRenderer);
        drawer.NewEmptySprite(4, 4, 100, Color.white);

        PoligonRegular pentagon = new PoligonRegular(5, new Vector2(1, 1), new Vector2(1, 1), new Color(1, 0, 0, 0.5f));
        drawer.Draw(pentagon);

        DrawnSprite draw = GetComponent<DrawnSprite>();
        draw.SavePng("Transf_exe1");

        pentagon.Deform(Axis.x, 2f);
        pentagon.Deform(Axis.y, 2f);
        drawer.Draw(pentagon);
        draw.SavePng("Transf_exe2");

        pentagon.Translate(new Vector2(2, 2));
        drawer.Draw(pentagon);
        draw.SavePng("Transf_exe3");

        pentagon.Rotate(Mathf.PI / 5.0f, Vector2.zero);
        drawer.Draw(pentagon);
        draw.SavePng("Transf_exe4");
    }

}