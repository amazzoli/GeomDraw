using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeomDraw
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

        public Ellipse(Vector2 center, float semiAxisX, float semiAxisY, float rotationAngle, 
            Color color, LineStyle borderStyle = new LineStyle()) : base()
        {
            this.center = center;
            this.semiAxisX = semiAxisX;
            this.semiAxisY = semiAxisY;
            this.rotationAngle = rotationAngle;
            rotationAngle %= Mathf.PI * 2;
            this.color = color;
            this.borderStyle = borderStyle;

            resolutionAux = 4 * Mathf.PI / ellipseAngleResolution / semiAxisX / semiAxisY;
        }

        public Ellipse(Vector2 center, float semiAxisX, float semiAxisY,
            Color color, LineStyle borderStyle = new LineStyle()) : base()
        {
            this.center = center;
            this.semiAxisX = semiAxisX;
            this.semiAxisY = semiAxisY;
            this.rotationAngle = 0;
            rotationAngle %= Mathf.PI * 2;
            this.color = color;
            this.borderStyle = borderStyle;

            resolutionAux = 4 * Mathf.PI / ellipseAngleResolution / semiAxisX / semiAxisY;
        }

        // IDRAWABLE

        public override IDrawable Copy()
        {
            Color newColor = new Color(color.r, color.g, color.b, color.a);
            return new Ellipse(new Vector2(center.x, center.y), semiAxisX, semiAxisY, rotationAngle, newColor, BorderStyle.Copy());
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

        // IDRAWABLE TRANSFORMATION
        // Translate inherited by circular shape

        public override void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            if (!isRelative)
                rotCenter -= center;
            if (rotCenter.x != 0 || rotCenter.y != 0)
                center = Utl.Rotate(-rotCenter, radAngle) + rotCenter + center;
            rotationAngle += radAngle;
            rotationAngle %= Mathf.PI * 2;
            borderCache = new Vector2[0];
        }

        public override void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            base.Reflect(axis, coord, isRelative);
            rotationAngle *= -1;
            rotationAngle %= Mathf.PI * 2;
        }

        public override bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            if (rotationAngle % Mathf.PI != 0)
            {
                Debug.Log("Rotated ellipse not deformable");
                return false;
            }

            float cDef = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cDef += center.x;
                center = new Vector2(factor * (center.x - cDef) + cDef, center.y);
                semiAxisX *= factor;
            }
            else
            {
                if (isRelative) cDef += center.y;
                center = new Vector2(center.x, factor * (center.y - cDef) + cDef);
                semiAxisY *= factor;
            }
            borderCache = new Vector2[0];
            return true;
        }

        // BORDER DISCRETIZATION

        protected override Vector2[] ComputeBorder(float pixelsPerUnit)
        {
            // The discretization of the border is not homogeneous. The steps of angle
            // t of the parametric ellipse are chosen such that the internal angles of
            // the poligon spanned by the discretized ellipse are equal.
            // This leads to smaller steps around the pole of the larger axis and vice-versa.
            // The formula used are approximated for small dt

            List<Vector2> border = new List<Vector2>();

            float t = 0, dt1 = AuxFunct(0) / 2.0f, dt2;
            cosRot = Mathf.Cos(rotationAngle);
            sinRot = Mathf.Sin(rotationAngle);

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