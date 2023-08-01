using Drawing;
using UnityEditor;
using UnityEngine;

public interface IDrawableLine : IDrawable
{
    /// <summary> Discretized path of the line </summary>
    //public Vector2[] Discretization(float pixelsPerUnit);

    /// <summary> Right and left path following the discretization of give width </summary>
    public (Vector2[], Vector2[]) LeftRightDiscretization(float pixelsPerUnit);

    public LineStyle Style { get; }
}


public struct LineStyle
{
    public float thickness;
    public Color color;

    public LineStyle(float thickness, Color color)
    {
        this.thickness = thickness;
        this.color = color;
    }
}