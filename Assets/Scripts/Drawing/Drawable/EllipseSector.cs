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
            this.startAngle = startAngle % (Mathf.PI * 2);
            this.endAngle = endAngle % (Mathf.PI * 2);
            if (this.endAngle <= this.startAngle) 
                this.endAngle += (Mathf.PI * 2);
        }

        // IDRAWABLE

        public override IDrawable Copy()
        {
            Color newColor = new Color(color.r, color.g, color.b, color.a);
            return new EllipseSector(new Vector2(center.x, center.y), semiAxisX, semiAxisY, 
                startAngle, endAngle, rotationAngle, newColor, BorderStyle.Copy());
         }

        public override bool CheckDrawability(float pixelsPerUnit)
        {
            if (!base.CheckDrawability(pixelsPerUnit))
                return false;

            float angle = endAngle - startAngle;
            if (angle < borderStyle.thickness / (Mathf.Max(semiAxisY, semiAxisX) - borderStyle.thickness / 2.0f))
            {
                Debug.LogError("The angle spanned by the circular sector is too small");
                return false;
            }
            if (endAngle - startAngle == Mathf.PI * 2)
            {
                Debug.LogError("This is a circle");
                return false;
            }

            return true;
        }

        // IDRAWABLE TRANSFORMATIONS

        public override void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            base.Rotate(radAngle, rotCenter, isRelative);
            startAngle += radAngle;
            endAngle += radAngle;
        }

        public override void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            base.Reflect(axis, coord, isRelative);
            float auxAngle = startAngle;
            if (axis == Axis.x)
            {
                startAngle = (-endAngle) % (Mathf.PI * 2);
                endAngle = (-auxAngle) % (Mathf.PI * 2);
            }
            else
            {
                startAngle = (Mathf.PI - endAngle) % (Mathf.PI * 2);
                endAngle = (Mathf.PI - auxAngle) % (Mathf.PI * 2);
            }
        }

        // BORDER DISCRETIZATION

        protected override Vector2[] ComputeBorder(float pixelsPerUnit)
        {
            List<Vector2> border = new List<Vector2>() { center };

            float t = startAngle, dt1 = AuxFunct(startAngle) / 2.0f, dt2;
            cosRot = Mathf.Cos(rotationAngle);
            sinRot = Mathf.Sin(rotationAngle);

            dt2 = BorderStep(border, t, dt1, pixelsPerUnit);
            t += dt1;
            dt1 = dt2;

            int iter = 0;
            while (t <= endAngle - dt2 / 2.0f)
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
           
            BorderStep(border, endAngle, dt1, pixelsPerUnit);

            Vector2[] borderArr = new Vector2[border.Count];
            for (int i = 0; i < border.Count; i++) borderArr[border.Count - i - 1] = border[i];
            return borderArr;
        }
    }
}