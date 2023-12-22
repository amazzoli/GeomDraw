using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    public class Poligon : IDrawableShape
    {
        protected Vector2[] vertices;
        protected Color color;
        protected LineStyle lineStyle;
        /// <summary>
        /// It quantifies the small randomization of vertex coordinates to facilitate the
        /// self intersection algorithm
        /// </summary>
        public float Tollerance { get; private set; }


        /// <summary> Poligon given the list of vertices in world units in clockwise order </summary>
        public Poligon(Vector2[] vertices, Color color, LineStyle lineStyle = new LineStyle())
        {
            this.vertices = vertices;
            this.color = color;
            this.lineStyle = lineStyle;
        }


        // IDRAWABLE

        public IDrawable Copy()
        {
            Color newColor = new Color(color.r, color.g, color.b, color.a);
            Vector2[] newPoints = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) newPoints[i] = new Vector2(vertices[i].x, vertices[i].y);
            return new Poligon(newPoints, newColor, BorderStyle.Copy());
        }

        public bool CheckDrawability(float pixelsPerUnit)
        {
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
            }
            vertices = newVert.ToArray();

            // Self intersections
            PoligonSelfIntersection self = new PoligonSelfIntersection(this);
            vertices = self.FindExternalPath(pixelsPerUnit).ToArray();
            Tollerance = self.Tollerance;

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

        // IDRAWABLE TRANSFROMATIONS

        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < vertices.Length; i++) vertices[i] += translation;
            Debug.Log(Center.x + "\t" + Center.y);
        }

        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative = true)
        {
            if (isRelative)
                rotCenter += Center;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 auxP = vertices[i] - rotCenter;
                vertices[i] = Utl.Rotate(auxP, radAngle) + rotCenter;
            }
            Debug.Log(Center.x + "\t" + Center.y);
        }

        public void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            float cRefl = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cRefl += Center.y;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].y = 2 * cRefl - vertices[i].y;
            }
            else
            {
                if (isRelative) cRefl += Center.x;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].x = 2 * cRefl - vertices[i].x;
            }
        }

        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            float cDef = coord;
            if (axis == Axis.x)
            {
                if (isRelative) cDef += Center.x;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].x = factor * (vertices[i].x - cDef) + cDef;
            }
            else
            {
                if (isRelative) cDef += Center.y;
                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].y = factor * (vertices[i].y - cDef) + cDef;
            }
            Debug.Log(Center.x + "\t" + Center.y);
            return true;
        }

        // IDRAWABLE SHAPE

        public Color Color => color;

        public LineStyle BorderStyle => lineStyle;

        public Vector2[] Border(float pixelsPerUnit) => vertices;

        protected Vector2 Center {
            get{ return Utl.RectCenter(vertices); }
        }
    }
}
