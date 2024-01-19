using UnityEngine;


namespace GeomDraw
{
    /// <summary>
    /// Auxiliary class used by the Drawer
    /// </summary>
    public class PixelCoord
    {
        /// <summary> 
        /// Storing the info of the pixel position and its boundaries.
        /// A pixel has an integer coordinate and its side measures 1.
        /// </summary>
        public PixelCoord(Vector2 p)
        {
            x = Mathf.RoundToInt(p.x);
            y = Mathf.RoundToInt(p.y);
            x1 = x - 0.5f;
            x2 = x + 0.5f;
            y1 = y - 0.5f;
            y2 = y + 0.5f;
        }

        /// <summary> X coordinate of the pixel </summary>
        public int x { get; private set; }
        /// <summary> Y coordinate of the pixel </summary>
        public int y { get; private set; }
        /// <summary> X coordinate of the left boundary </summary>
        public float x1 { get; private set; }
        /// <summary> X coordinate of the right boundary </summary>
        public float x2 { get; private set; }
        /// <summary> Y coordinate of the bottom boundary </summary>
        public float y1 { get; private set; }
        /// <summary> Y coordinate of the top boundary </summary>
        public float y2 { get; private set; }
    }
}