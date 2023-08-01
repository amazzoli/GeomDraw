//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace Drawing
//{
//    public class CompositeShape : IDrawableShape
//    {
//        IDrawableLine[] lines;
//        Color color;
//        Color borderColor;
//        float borderThickness;
//        Vector2[] borderCache;


//        /// <summary> Shape having as border the array of lines in clockwise order </summary>
//        public CompositeShape(IDrawableLine[] lines, Color color, Color borderColor, float borderThickness = 0)
//        {
//            this.lines = lines;
//            this.color = color;
//            this.borderColor = borderColor;
//            this.borderThickness = borderThickness;
//            borderCache = new Vector2[0];
//        }

//        public Color Color => color;

//        public LineStyle BorderStyle => new LineStyle(borderThickness, borderColor);

//        public Vector2[] Border(float pixelsPerUnit)
//        {
//            if (borderCache.Length > 0) return borderCache;

//            List<Vector2> borderList = new List<Vector2>();
//            for (int i = 0; i < lines.Length; i++)
//            {
//                Vector2[] lineBorder = lines[i].Discretization(pixelsPerUnit);

//                // If the last point of the line discretization is closer to the last point
//                // of the border list, the line discretization is inverted
//                if (i > 0)
//                {
//                    Vector2 lastP = borderList[borderList.Count - 1];
//                    int L = lineBorder.Length;
//                    if ((lineBorder[0] - lastP).magnitude > (lineBorder[L - 1] - lastP).magnitude)
//                    {
//                        Vector2[] aux = new Vector2[L];
//                        for (int j = 0; j < L; j++) aux[j] = lineBorder[L - j - 1];
//                        lineBorder = aux;
//                    }
//                }
//                borderList.AddRange(lineBorder);
//            }
//            borderCache = borderList.ToArray();

//            return borderCache;
//        }

//        public void CheckAntialiasability(float pixelsPerUnit)
//        {
//            if (borderCache.Length == 0) Border(pixelsPerUnit);

//            // Clockwise check
//            //if (!Utl.IsClockwisePath(vertices))
//            //    vertices = Utl.InvertClockwise(vertices);

//                // Short side check
//            List<Vector2> newVert = new List<Vector2>();
//            Vector2 oldVert = borderCache[0];
//            for (int i=0; i< borderCache.Length; i++)
//            {
//                int iNext = (i + 1) % borderCache.Length;
//                if ((oldVert - borderCache[iNext]).magnitude * pixelsPerUnit > 1)
//                {
//                    newVert.Add(borderCache[i]);
//                    oldVert = borderCache[iNext];
//                }
//                else
//                    Debug.Log("Vertex eliminated by vertex proximity");
//            }
//            borderCache = newVert.ToArray();

//            // TODO: check on side intersections!
//        }

//        public bool CheckDrawability(float pixelsPerUnit)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}