using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GeomDraw
{
    public class DrawBezierSubWindow : SubWindow
    {
        public Vector2 startPoint, endPoint;
        public Vector2 ctrlPoint1, ctrlPoint2;

        public float thickness = 1;
        public Color borderColor = Color.black;


        public override string Header => "Draw a Bezier curve";

        public override bool HasDrawButton => true;

        protected override void DisplayParameters()
        {
            GUIContent startPointCont = new GUIContent("Start point", "Coords of the initial point of the curve");
            startPoint = EditorGUILayout.Vector2Field(startPointCont, startPoint);

            GUIContent endPointCont = new GUIContent("End point", "Coords of the final point of the curve");
            endPoint = EditorGUILayout.Vector2Field(endPointCont, endPoint);

            GUIContent ctrlPoint1Cont = new GUIContent("Control point 1", "Coords of the first control point");
            ctrlPoint1 = EditorGUILayout.Vector2Field(ctrlPoint1Cont, ctrlPoint1);

            GUIContent ctrlPoint2Cont = new GUIContent("Control point 2", "Coords of the second control point");
            ctrlPoint2 = EditorGUILayout.Vector2Field(ctrlPoint2Cont, ctrlPoint2);

            GUIContent thickCont = new GUIContent("Line thickness", "Line thickness in pixels");
            thickness = EditorGUILayout.Slider(thickCont, thickness, 1, 20);
            borderColor = EditorGUILayout.ColorField("Border color", borderColor);
        }

        protected override void DrawBotton(DrawerSprite drawer, SpriteRenderer renderer)
        {
            LineStyle borderStyle = new LineStyle(thickness / renderer.sprite.pixelsPerUnit, borderColor);

            BezierCurve curve = new BezierCurve(startPoint, ctrlPoint1, ctrlPoint2, endPoint, borderStyle);
            drawer.Draw(curve, true);
        }
    }
}

