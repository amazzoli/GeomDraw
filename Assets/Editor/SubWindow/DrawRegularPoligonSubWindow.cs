using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GeomDraw
{
    public class DrawRegularPoligonSubWindow : SubWindow
    {
        public int nVert = 3;
        public Vector2 center;
        public Vector2 scale = Vector2.one;
        public float rotation;
        public Color shapeColor = Color.white;
        public float borderThickness;
        public Color borderColor = Color.black;
        public bool drawBorder = true;

        public override string Header => "Draw a regular poligon";

        public override bool HasDrawButton => true;

        protected override void DisplayParameters()
        {
            GUIContent nVertCont = new GUIContent("N vertices", "Number of vertices of the poligon");
            nVert = EditorGUILayout.IntField(nVertCont, nVert);

            GUIContent centerCont = new GUIContent("Center", "Center of the ellipse in which the poligon is inscribed");
            center = EditorGUILayout.Vector2Field(centerCont, center);

            GUIContent scaleCont = new GUIContent("Size", "Axis lengths of the ellipse in which the poligon is inscribed");
            scale = EditorGUILayout.Vector2Field(scaleCont, scale);

            rotation = EditorGUILayout.FloatField("Rotation", rotation);
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

            PoligonRegular shape = new PoligonRegular(nVert, center, scale, rotation, shapeColor, borderStyle);
            drawer.Draw(shape, true);
        }
    }
}

