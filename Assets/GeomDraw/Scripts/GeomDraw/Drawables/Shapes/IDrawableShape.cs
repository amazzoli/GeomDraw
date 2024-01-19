using UnityEditor;
using UnityEngine;

namespace GeomDraw
{

    /// <summary>
    /// Generic Drawable shape
    /// </summary>
    public interface IDrawableShape : IDrawable
    {
        /// <summary>
        /// Array of consecutive border vertices in clockwise order and without intersections
        /// </summary>
        public Vector2[] Border(float pixelsPerUnit);

        /// <summary>
        /// Internal color of the shape
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Parameters specifying the style of the border
        /// </summary>
        public LineStyle BorderStyle { get; }
    }
}
