using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Drawing
{
    public class EllipseSector : Ellipse
    {
        public float startAngle { get; protected set; }
        public float endAngle { get; protected set; }


        public EllipseSector(Vector2 center, float semiAxisX, float semiAxisY, 
            float startAngle, float endAngle, float rotationDegAngle, Color color, LineStyle borderStyle) : 
            base(center, semiAxisX, semiAxisY, rotationDegAngle, color, borderStyle)
        {
            this.startAngle = startAngle % 360;
            this.endAngle = endAngle % 360;
            if (this.endAngle <= this.startAngle) 
                this.endAngle += 360;
        }

        public override IDrawable Copy()
        {
            return new EllipseSector(center, semiAxisX, semiAxisY, startAngle, endAngle, rotationAngle, color, BorderStyle);
        }

        public override bool CheckDrawability(float pixelsPerUnit)
        {
            if (!base.CheckDrawability(pixelsPerUnit))
                return false;

            float angle = (endAngle - startAngle) * Mathf.Deg2Rad;
            if (angle < borderStyle.thickness / (Mathf.Max(semiAxisY, semiAxisX) - borderStyle.thickness / 2.0f))
            {
                Debug.LogError("The angle spanned by the circular sector is too small");
                return false;
            }
            if (endAngle - startAngle == 360)
            {
                Debug.LogError("This is a circle");
                return false;
            }

            return true;
        }

        protected override Vector2[] ComputeBorder(float pixelsPerUnit)
        {
            List<Vector2> border = new List<Vector2>() { center };

            float t = startAngle * Mathf.Deg2Rad, dt1 = AuxFunct(startAngle * Mathf.Deg2Rad) / 2.0f, dt2;
            cosRot = Mathf.Cos(-rotationAngle * Mathf.Deg2Rad);
            sinRot = Mathf.Sin(-rotationAngle * Mathf.Deg2Rad);

            dt2 = BorderStep(border, t, dt1, pixelsPerUnit);
            t += dt1;
            dt1 = dt2;

            int iter = 0;
            while (t <= endAngle * Mathf.Deg2Rad - dt2 / 2.0f)
            {
                dt2 = BorderStep(border, t, dt1, pixelsPerUnit);
                t += dt1;
                dt1 = dt2;
                iter++;
                if (iter > 10000)
                {
                    Debug.LogError("Ellipse iteration error");
                    return new Vector2[border.Count];
                }    
            }
           
            BorderStep(border, endAngle * Mathf.Deg2Rad, dt1, pixelsPerUnit);

            Vector2[] borderArr = new Vector2[border.Count];
            for (int i = 0; i < border.Count; i++) borderArr[border.Count - i - 1] = border[i];
            return borderArr;
        }
    }
}