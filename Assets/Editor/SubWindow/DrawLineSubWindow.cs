using Drawing;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Drawing
{
    public class DrawLineSubWindow : SubWindow
    {
        public int nPoints = 2;
        public List<Vector2> points = new List<Vector2>();
        public float thickness = 1;
        public Color borderColor = Color.black;
        public bool isClosed = false;


        public override string Header => "Draw a line";

        public override bool HasDrawButton => true;

        protected override void DisplayParameters()
        {
            isClosed = EditorGUILayout.ToggleLeft("Is closed", isClosed);
            GUIContent nPointsCont = new GUIContent("N points", "Number of consecutive points to join with a line");
            nPoints = Mathf.Max(0, EditorGUILayout.IntField(nPointsCont, points.Count));

            while (nPoints < points.Count)
                points.RemoveAt(points.Count - 1);
            while (nPoints > points.Count)
                points.Add(new Vector2());

            for (int i = 0; i < points.Count; i++)
                points[i] = EditorGUILayout.Vector2Field("Vert " + (i+1).ToString(), points[i]);

            GUIContent thickCont = new GUIContent("Line thickness", "Line thickness in pixels");
            thickness = EditorGUILayout.Slider(thickCont, thickness, 1, 20);
            borderColor = EditorGUILayout.ColorField("Border color", borderColor);
        }

        protected override void DrawBotton(Drawer drawer, SpriteRenderer renderer)
        {
            if (points.Count < 2)
            {
                Debug.LogError("A line needs at least two points");
                return;
            }

            if (thickness < 0.5f)
            {
                Debug.LogError("Thickness must be larger than 0.5 pixels");
                return;
            }
            LineStyle borderStyle = new LineStyle(thickness / renderer.sprite.pixelsPerUnit, borderColor);

            BrokenLine line = new BrokenLine(points.ToArray(), isClosed, borderStyle);
            drawer.Draw(renderer, line);
        }
    }
}

