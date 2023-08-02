using UnityEditor;
using UnityEngine;


namespace Drawing
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

        public virtual void Reflect(Vector2 axis) { }

        public virtual void Deform(Vector2 axis, float factor) { }

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