using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Drawing
{
    public class Ellipse : CircularShape
    {
        readonly float ellipseAngleResolution = 200;

        public float semiAxisX { get; protected set; }
        public float semiAxisY { get; protected set; }
        public float rotationAngle { get; protected set; }

        // Auxiliary coefficients
        float resolutionAux;
        protected float cosRot, sinRot;

        public Ellipse(Vector2 center, float semiAxisX, float semiAxisY, float rotationDegAngle, 
            Color color, LineStyle borderStyle = new LineStyle()) : base()
        {
            this.center = center;
            this.semiAxisX = semiAxisX;
            this.semiAxisY = semiAxisY;
            this.rotationAngle = rotationDegAngle;
            this.color = color;
            this.borderStyle = borderStyle;

            resolutionAux = 4 * Mathf.PI / ellipseAngleResolution / semiAxisX / semiAxisY;
        }

        public override IDrawable Copy()
        {
            return new Ellipse(center, semiAxisX, semiAxisY, rotationAngle, color, BorderStyle);
        }

        public override bool CheckDrawability(float pixelsPerUnit)
        {
            if (semiAxisX <= 1.0 / pixelsPerUnit)
            {
                Debug.LogError("Semi-axis X smaller than a pixel");
                return false;
            }

            if (semiAxisY <= 1.0 / pixelsPerUnit)
            {
                Debug.LogError("Semi-axis Y smaller than a pixel");
                return false;
            }

            if (semiAxisX <= borderStyle.thickness / 2.0)
            {
                Debug.LogError("Semi-axis X smaller than the border thickness");
                return false;
            }

            if (semiAxisY <= borderStyle.thickness / 2.0)
            {
                Debug.LogError("Semi-axis Y smaller than the border thickness");
                return false;
            }

            return true;
        }

        protected override Vector2[] ComputeBorder(float pixelsPerUnit)
        {
            // The discretization of the border is not homogeneous. The steps of angle
            // t of the parametric ellipse are chosen such that the internal angles of
            // the poligon spanned by the discretized ellipse are equal.
            // This leads to smaller steps around the pole of the larger axis and vice-versa.
            // The formula used are approximated for small dt

            List<Vector2> border = new List<Vector2>();

            float t = 0, dt1 = AuxFunct(0) / 2.0f, dt2;
            cosRot = Mathf.Cos(-rotationAngle * Mathf.Deg2Rad);
            sinRot = Mathf.Sin(-rotationAngle * Mathf.Deg2Rad);

            int iter = 0;
            while (t < 2 * Mathf.PI)
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

            //if (border.Count <= 2)
            //    Debug.LogError("Ellipse size smaller than the pixel resolution");

            Vector2[] borderArr = new Vector2[border.Count];
            for (int i = 0; i < border.Count; i++) borderArr[border.Count - i - 1] = border[i];
            return borderArr;
        }

        protected float AuxFunct(float t)
        {
            float aux = Mathf.Pow(semiAxisX * Mathf.Sin(t), 2) + Mathf.Pow(semiAxisY * Mathf.Cos(t), 2);
            return aux * resolutionAux;
        }

        protected float BorderStep(List<Vector2> border, float t, float dt1, float pixelsPerUnit)
        {
            float x1 = Mathf.Cos(t) * semiAxisX;
            float y1 = Mathf.Sin(t) * semiAxisY;
            float x = cosRot * x1 - sinRot * y1 + center.x;
            float y = sinRot * x1 + cosRot * y1 + center.y;

            // Check on steps smaller then a pixel
            if (border.Count > 0)
            {
                float xb = border[border.Count - 1].x, yb = border[border.Count - 1].y;
                if ((xb - x) * (xb - x) + (yb - y) * (yb - y) > 2 / pixelsPerUnit / pixelsPerUnit)
                    border.Add(new Vector2(x, y));
            }
            else
                border.Add(new Vector2(x, y));

            return AuxFunct(t) - dt1;
        }
    }
}