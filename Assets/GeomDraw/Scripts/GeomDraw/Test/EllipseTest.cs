using GeomDraw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeomDraw
{
    public class EllipseTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            DrawerSprite drawer = new DrawerSprite(spriteRenderer);
            drawer.NewEmptySprite(10, 5, 100, Color.white);


            Color shapeColor = new Color(0, 0, 0, 0.1f);
            Color lineColor = new Color(0, 0, 0, 0.6f);

            LineStyle lineStyle = new LineStyle(1f / spriteRenderer.sprite.pixelsPerUnit, lineColor);
            float rStart = lineStyle.thickness / 2.0f + 1 / spriteRenderer.sprite.pixelsPerUnit, rMax = 1.2f, nSteps = 5;
            float r = rStart, rStep = (rMax - rStart) / (nSteps - 1);
            while (r <= rMax)
            {
                Ellipse c = new Ellipse(new Vector2(1.5f, 2.5f), r, r * 2, 10, shapeColor, lineStyle);
                drawer.Draw(c);
                r += rStep;
            }

            lineStyle = new LineStyle(20f / spriteRenderer.sprite.pixelsPerUnit, lineColor);
            rStart = lineStyle.thickness / 2.0f + 0.5f / spriteRenderer.sprite.pixelsPerUnit;
            r = rStart; rStep = (rMax - rStart) / (nSteps - 1);
            while (r <= rMax)
            {
                Ellipse c = new Ellipse(new Vector2(4.5f, 2.5f), r, r * 2, 10, shapeColor, lineStyle);
                drawer.Draw(c);
                r += rStep;
            }

            lineStyle = new LineStyle(1 / spriteRenderer.sprite.pixelsPerUnit, lineColor);
            rMax = 1.2f / 4.0f;
            rStart = lineStyle.thickness / 2.0f + 1 / spriteRenderer.sprite.pixelsPerUnit;
            r = rStart;
            rStep = (rMax - rStart) / (nSteps - 1);
            while (r <= rMax)
            {
                Ellipse c = new Ellipse(new Vector2(7f, 2.5f), r, r * 8, 10, shapeColor, lineStyle);
                drawer.Draw(c);
                r += rStep;
            }

            // L'ellisse con bordo largo ed estremamente schiacciata non crea una discretizzazione smooth
            // Ma questo rimane difficile da correggiere per come è impostato il codice ora e non mi 
            // pare troppo rilevante
            lineStyle = new LineStyle(20 / spriteRenderer.sprite.pixelsPerUnit, lineColor);
            nSteps = 2;
            rStart = lineStyle.thickness / 2.0f + 1 / spriteRenderer.sprite.pixelsPerUnit;
            r = rStart;
            rStep = (rMax - rStart) / (nSteps - 1);
            while (r <= rMax)
            {
                Ellipse c = new Ellipse(new Vector2(8.5f, 2.5f), r, r * 8, 10, shapeColor, lineStyle);
                drawer.Draw(c);
                r += rStep;
            }
        }
    }
}