using Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Drawing
{
    public class Circle : CircularShape
    {
        public float radius { get; protected set; }


        public Circle(Vector2 center, float radius, Color color, LineStyle borderStyle = new LineStyle()) : base()
        {
            this.center = center;
            this.radius = radius;
            this.color = color;
            this.borderStyle = borderStyle;
        }

        // IDRAWABLE

        public override IDrawable Copy()
        {
            return new Circle(center, radius, color, BorderStyle);
        }

        public override bool CheckDrawability(float pixelsPerUnit)
        {
            if (radius <= 1.0 / pixelsPerUnit)
            {
                Debug.LogError("Radius smaller than a pixel");
                return false;
            }

            if (radius <= borderStyle.thickness / 2.0)
            {
                Debug.LogError("Radius smaller than the border thickness");
                return false;
            }

            return true;
        }

        // IDRAWABLE TRANSFORMATIONS
        // Translate inherited by circular shape

        public override void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            if (!isRelative)
                rotCenter -= center;
            if (rotCenter.x != 0 || rotCenter.y != 0)
                center = Utl.Rotate(-rotCenter, radAngle) + rotCenter + center;
        }

        public override void Reflect(Vector2 axis)
        {
            throw new System.NotImplementedException();
        }

        public override void Deform(Vector2 axis, float factor)
        {
            throw new System.NotImplementedException();
        }

        // BORDER DISCRETIZATION

        protected override Vector2[] ComputeBorder(float pixelsPerUnit)
        {
            int pixelDiameter = Mathf.CeilToInt((2 * radius + borderStyle.thickness) * pixelsPerUnit);
            int nSteps = Mathf.Min(Mathf.CeilToInt(angleResolution), pixelDiameter * 3);
            borderCache = new Vector2[nSteps];

            float step = 2 * Mathf.PI / nSteps;
            for (int k = 0; k < nSteps; k++)
            {
                float x = Mathf.Cos(k * step) * radius + center.x;
                float y = Mathf.Sin(k * step) * radius + center.y;
                borderCache[nSteps - k - 1] = new Vector2(x, y);
            }

            return borderCache;
        }
    }
}