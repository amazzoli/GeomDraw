using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    public class CompositeShape : IDrawableShape
    {
        IDrawableLine[] lines;
        Color color;
        LineStyle borderStyle;
        Vector2[] borderCache;


        /// <summary> Shape having as border the array of lines in clockwise order </summary>
        public CompositeShape(IDrawableLine[] lines, Color color, LineStyle lineStyle = new LineStyle())
        {
            this.lines = lines;
            this.color = color;
            this.borderStyle = lineStyle;
            borderCache = new Vector2[0];
        }

        // IDRAWABLE

        public IDrawable Copy()
        {
            Color newColor = new Color(color.r, color.g, color.b, color.a);
            IDrawableLine[] newLines = new IDrawableLine[lines.Length];
            for(int i = 0; i < lines.Length; i ++) newLines[i] = (IDrawableLine)lines[i].Copy();
            return new CompositeShape(newLines, newColor, borderStyle.Copy());
        }

        public bool CheckDrawability(float pixelsPerUnit)
        {
            if (borderCache.Length == 0) borderCache = Border(pixelsPerUnit);

            // Using the same checkDrawability of a poligon given the discretization
            Polygon myPoligon = new Polygon(borderCache, Color, borderStyle);
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

        public void Rotate(float radAngle, Vector2 rotCenter, bool isRelative = true)
        {
            borderCache = new Vector2[0];
            if (isRelative) rotCenter += RectCenter();
            foreach (IDrawableLine line in lines)
                line.Rotate(radAngle, rotCenter, false);
        }

        public void Reflect(Axis axis, float coord = 0, bool isRelative = true)
        {
            borderCache = new Vector2[0];

            if (axis == Axis.x && isRelative)
                coord += RectCenter().y;
            else if(axis == Axis.y && isRelative)
                coord += RectCenter().x;

            foreach (IDrawableLine line in lines)
                line.Reflect(axis, coord, false);
        }

        public bool Deform(Axis axis, float factor, float coord = 0, bool isRelative = true)
        {
            borderCache = new Vector2[0];

            if (axis == Axis.x && isRelative)
                coord += RectCenter().x;
            else if (axis == Axis.y && isRelative)
                coord += RectCenter().y;

            foreach (IDrawableLine line in lines)
                if (!line.Deform(axis, factor, coord, false))
                    return false;
            return true;
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

        private Vector2 RectCenter()
        {
            bool firstLine = true;
            float xMin = 0, xMax = 0, yMin = 0, yMax = 0;
            foreach (IDrawableLine line in lines)
            {
                Rect rect = new Rect();
                if (line is BezierCurve)
                    rect = Utl.ContainingRect(((BezierCurve)line).Points);
                else if (line is BrokenLine)
                    rect = Utl.ContainingRect(((BrokenLine)line).Points);
                else
                    Debug.LogError("IDrawable line in composite shape not recognized");

                if (firstLine)
                {
                    xMin = rect.xMin; xMax = rect.xMax;
                    yMin = rect.yMin; yMax = rect.yMax;
                    firstLine = false;
                }
                else
                {
                    if (xMin > rect.xMin) xMin = rect.xMin;
                    if (xMax < rect.xMax) xMax = rect.xMax;
                    if (yMin > rect.yMin) yMin = rect.yMin;
                    if (yMax < rect.yMax) yMax = rect.yMax;
                }
            }
            return new Vector2((xMin + xMax) / 2.0f, (yMin + yMax) / 2.0f);
        }
    }

}
