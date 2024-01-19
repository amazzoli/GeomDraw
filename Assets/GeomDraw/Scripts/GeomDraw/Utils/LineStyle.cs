using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    /// <summary>
    /// Style of the drawable line
    /// </summary>
    public struct LineStyle
    {
        public float thickness;
        public Color color;

        public LineStyle(float thickness, Color color)
        {
            this.thickness = thickness;
            this.color = color;
        }

        public LineStyle Copy()
        {
            return new LineStyle(thickness, color);
        }
    }
}