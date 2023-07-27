using Drawing;
using UnityEditor;
using UnityEngine;

public interface IDrawableShape : IDrawable
{
    /// <summary>
    /// Internal color of the shape
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Array of consecutive border vertices in clockwise order and without intersections
    /// </summary>
    public Vector2[] Border(float pixelsPerUnit);

    /// <summary>
    /// Parameters specifying the style of the border
    /// </summary>
    public LineStyle BorderStyle { get; }
}
