using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeomDraw;
using System;

public class TextureEx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Main renderer over which the new texture will be merged
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Drawer drawer1 = new Drawer(spriteRenderer);
        drawer1.NewEmptySprite(4, 4, 100, Color.red);

        // Creating a first smaller texture on a new Sprite renderer
        GameObject auxGO = new GameObject();
        SpriteRenderer auxRenderer = auxGO.AddComponent<SpriteRenderer>();
        Drawer drawer2 = new Drawer(auxRenderer);
        drawer2.NewEmptySprite(2, 2, 100, Color.white);

        // Drawing something on the first smaller texture
        List<Color> gradCols = new List<Color> {
            ColorUtils.ColorHEX("#C6FFDD"),
            ColorUtils.ColorHEX("#FBD786"),
            ColorUtils.ColorHEX("#f7797d")
        };
        int nCols = 7;
        Vector2 center = new Vector2(1, 1);
        for (int i = nCols-1; i >= 0; i--){
            Color color = ColorUtils.Gradient(i / (float)(nCols - 1), gradCols);
            Quad quad = new Quad(center, 2 * (i + 1) / (float)nCols, color);
            drawer2.Draw(quad);
        }
        drawer2.SavePng("Texture_small_exe");

        // Converting the auxiliary smaller texture in an object that can be 
        // drawn by the Drawer
        DrawableTexture dText = new DrawableTexture(auxRenderer.sprite, Vector2.zero);
        // Drawing the auxiliary texture on the main texture
        drawer1.Draw(dText);

        // Transforming the small texture and drawing it again
        dText.Translate(new Vector2(2, 2));
        dText.Deform(Axis.x, 0.75f);
        dText.Rotate(Mathf.PI / 4.0f, Vector2.zero, true);
        drawer1.Draw(dText);

        drawer1.SavePng("Texture_exe");
    }
}
