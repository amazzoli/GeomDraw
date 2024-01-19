using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    /// <summary>
    /// Generic drawable line
    /// </summary>
    public interface IDrawableLine : IDrawable
    {
        /// <summary> Discretized path of the line </summary>
        public Vector2[] Discretization(float pixelsPerUnit);

        /// <summary> Right and left path following the discretization of give width </summary>
        public (Vector2[], Vector2[]) LeftRightDiscretization(float pixelsPerUnit);

        public LineStyle Style { get; }
    }
}