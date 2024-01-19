using UnityEngine;
using GeomDraw;
using System;
using TMPro;


namespace GeomDraw{
    /// <summary>
    /// Drawable shape: square
    /// </summary>
    public class Quad : PolygonRegular {

        public Quad(Vector2 center, float side, Color color, LineStyle borderStyle = new LineStyle()) : 
            base(4, center, new Vector2(side, side) * Mathf.Sqrt(2), color, borderStyle) {}

        public Quad(Vector2 center, float side, float rotation, Color color, LineStyle borderStyle = new LineStyle()) : 
            base(4, center, new Vector2(side, side) * Mathf.Sqrt(2), rotation, color, borderStyle) {}

    }
}
