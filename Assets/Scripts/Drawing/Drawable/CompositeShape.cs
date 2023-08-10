using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Drawing
{
    public class CompositeShape : IDrawableShape
    {
        IDrawableLine[] lines;
        Color color;
        LineStyle borderStyle;
        Vector2[] borderCache;


        /// <summary> Shape having as border the array of lines in clockwise order </summary>
        public CompositeShape(IDrawableLine[] lines, Color color, LineStyle lineStyle)
        {
            this.lines = lines;
            this.color = color;
            this.borderStyle = lineStyle;
            borderCache = new Vector2[0];
        }

        // IDRAWABLE

        public IDrawable Copy()
        {
            return new CompositeShape(lines, color, borderStyle);
        }

        public bool CheckDrawability(float pixelsPerUnit)
        {
            if (borderCache.Length  == 0) borderCache = Border(pixelsPerUnit);

            // Using the same checkDrawability of a poligon given the discretization
            Poligon myPoligon = new Poligon(borderCache, Color, borderStyle);
            if (!myPoligon.CheckDrawability(pixelsPerUnit))
                return false;
            borderCache = myPoligon.Border(pixelsPerUnit);
            return true;
        }


        // IDRAWABLE TRANSFROMATIONS

        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < lines.Length; i++) lines[i].Translate(translation);
            borderCache = new Vector2[0];
        }

        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative)
        {
            borderCache = new Vector2[0];
        }

        public void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            borderCache = new Vector2[0];
        }

        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            borderCache = new Vector2[0];
            return false;
        }

        // IDRAWABLE SHAPE

        public Color Color => color;

        public LineStyle BorderStyle => borderStyle;

        public Vector2[] Border(float pixelsPerUnit)
        {
            if (borderCache.Length > 0) return borderCache;

            List<Vector2> borderList = new List<Vector2>();
            for (int i = 0; i < lines.Length; i++)
            {
                Vector2[] lineBorder = lines[i].Discretization(pixelsPerUnit);

                // If the last point of the line discretization is closer to the last point
                // of the border list, the line discretization is inverted
                if (i > 0)
                {
                    Vector2 lastP = borderList[borderList.Count - 1];
                    int L = lineBorder.Length;
                    if ((lineBorder[0] - lastP).magnitude > (lineBorder[L - 1] - lastP).magnitude)
                    {
                        Vector2[] aux = new Vector2[L];
                        for (int j = 0; j < L; j++) aux[j] = lineBorder[L - j - 1];
                        lineBorder = aux;
                    }
                }
                borderList.AddRange(lineBorder);
            }
            borderCache = borderList.ToArray();

            return borderCache;
        }
    }
}