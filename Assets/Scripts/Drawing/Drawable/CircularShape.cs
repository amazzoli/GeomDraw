using UnityEditor;
using UnityEngine;


namespace GeomDraw
{
    public abstract class CircularShape : IDrawableShape
    {
        /// <summary> Angular resolution for discretizing the border </summary>
        protected readonly float angleResolution = 200;

        protected Vector2[] borderCache;

        public Vector2 center { get; protected set; }

        public Color color { get; protected set; }

        public LineStyle borderStyle { get; protected set; }


        public CircularShape()
        {
            borderCache = new Vector2[0];
        }

        // IDRAWABLE

        public virtual IDrawable Copy()
        {
            return null;
        }

        public virtual bool CheckDrawability(float pixelsPerUnit)
        {
            return true;
        }

        // IDRAWABLE TRANSFORMATIONS

        public void Translate(Vector2 translation)
        {
            center += translation;
            borderCache = new Vector2[0];
        }

        public virtual void Rotate(float radAngle, Vector2 center, bool isRelative) { }

        public virtual void Reflect(Axis axis, float coord = 0, bool isRelative = true) 
        {
            float cRefl = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cRefl += center.y;
                center = new Vector2(center.x, 2 * cRefl - center.y);
            }
            else
            {
                if (isRelative) cRefl += center.x;
                center = new Vector2(2 * cRefl - center.x, center.y);
            }
            borderCache = new Vector2[0];
        }

        public virtual bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true) 
        {
            borderCache = new Vector2[0];
            return false;
        }

        // IDRAWABLE SHAPE

        public Color Color => color;

        public LineStyle BorderStyle => borderStyle;

        public Vector2[] Border(float pixelsPerUnit)
        {
            if (borderCache.Length > 0) 
                return borderCache;
            borderCache = ComputeBorder(pixelsPerUnit);
            return borderCache;
        }

        protected abstract Vector2[] ComputeBorder(float pixelsPerUnit);
    }
}