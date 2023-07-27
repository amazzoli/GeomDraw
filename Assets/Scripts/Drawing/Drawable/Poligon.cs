using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Drawing
{
    public class Poligon : IDrawableShape
    {
        protected Vector2[] vertices;
        protected Color color;
        protected LineStyle lineStyle;
        public float Tollerance { get; private set; }

        /// <summary> Poligon given the list of vertices in world units in clockwise order </summary>
        public Poligon(Vector2[] vertices, Color color, LineStyle lineStyle)
        {
            this.vertices = vertices;
            this.color = color;
            this.lineStyle = lineStyle;
        }

        public Color Color => color;

        public LineStyle BorderStyle => lineStyle;

        public Vector2[] Border(float pixelsPerUnit) => vertices;

        public bool CheckDrawability(float pixelsPerUnit)
        {
            // Self intersections
            PoligonSelfIntersection self = new PoligonSelfIntersection(this);
            vertices = self.FindExternalPath(pixelsPerUnit).ToArray();
            Tollerance = self.Tollerance;

            // Short side check
            List<Vector2> newVert = new List<Vector2>();
            Vector2 oldVert = vertices[0];
            for (int i = 0; i < vertices.Length; i++)
            {
                int iNext = (i + 1) % vertices.Length;
                if ((oldVert - vertices[iNext]).magnitude * pixelsPerUnit > 1)
                {
                    newVert.Add(vertices[i]);
                    oldVert = vertices[iNext];
                }
                //else
                //    Debug.Log("Vertex eliminated by vertex proximity");
            }
            vertices = newVert.ToArray();

            if (vertices.Length < 3)
            {
                Debug.LogError("Poligon with less than 3 sides");
                return false;
            }

            // Clockwise check
            if (!Utl.IsClockwisePath(vertices))
                vertices = Utl.InvertClockwise(vertices);

            return true;
        }
    }
}