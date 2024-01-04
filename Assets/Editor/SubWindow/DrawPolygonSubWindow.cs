using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GeomDraw
{
    public class DrawPolygonSubWindow : SubWindow
    {
        public List<Vector2> vertices = new List<Vector2>();
        public int nVert;
        public Color shapeColor = Color.white;
        public float borderThickness;
        public Color borderColor = Color.black;
        public bool drawBorder = true;

        public override string Header => "Draw a poligon";

        public override bool HasDrawButton => true;

        protected override void DisplayParameters()
        {
            nVert = Mathf.Max(0, EditorGUILayout.IntField("N vertices", vertices.Count));

            while (nVert < vertices.Count)
                vertices.RemoveAt(vertices.Count - 1);
            while (nVert > vertices.Count)
                vertices.Add(new Vector2());

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = EditorGUILayout.Vector2Field("Vert " + (i+1).ToString(), vertices[i]);

            shapeColor = EditorGUILayout.ColorField("Color", shapeColor);
            drawBorder = EditorGUILayout.ToggleLeft("Draw border", drawBorder);
            if (drawBorder)
            {
                GUIContent thickCont = new GUIContent("Border thickness", "Thickness of the border in pixels");
                borderThickness = EditorGUILayout.Slider(thickCont, borderThickness, 1, 20);
                borderColor = EditorGUILayout.ColorField("Border color", borderColor);
            }
        }

        protected override void DrawBotton(Drawer drawer, SpriteRenderer renderer)
        {
            LineStyle borderStyle = new LineStyle(borderThickness / renderer.sprite.pixelsPerUnit, borderColor);
            if (!drawBorder)
                borderStyle.thickness = 0;

            Polygon shape = new Polygon(vertices.ToArray(), shapeColor, borderStyle);
            drawer.Draw(shape, true);
        }
    }
}

