using Drawing;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static PlasticGui.PlasticTableColumn;


namespace Drawing
{
    public class DrawCirclePars
    {
        public Vector2 center;
        public float radius = 1;
        public float semiAxisX = 2;
        public float semiAxisY = 1;
        public float ellipseRotation = 0;
        public Color shapeColor = Color.white;

        public float borderThickness = 1;
        public Color borderColor = Color.black;
        public bool drawBorder = true;

        public bool isSector = false;
        public bool isEllipse = false;
        public float startAngle;
        public float endAngle;
    }
}


namespace Drawing
{
    public class DrawCircleSubWindow : SubWindow
    {
        public DrawCirclePars p = new DrawCirclePars();

        public override string Header => "Draw a circle";

        public override bool HasDrawButton => true;

        protected override void DisplayParameters()
        {
            p.isEllipse = EditorGUILayout.ToggleLeft("Is an ellipse", p.isEllipse);

            GUIContent centerCont = new GUIContent("Center", "Center in the system of reference of the sprite in world units");
            p.center = EditorGUILayout.Vector2Field(centerCont, p.center);

            if (!p.isEllipse)
            {
                GUIContent radiusCont = new GUIContent("Radius", "Radius in world units");
                p.radius = EditorGUILayout.FloatField(radiusCont, p.radius);
            }
            else
            {
                GUIContent semiAxisXCont = new GUIContent("Semiaxis X", "Semiaxis X in world units");
                p.semiAxisX = EditorGUILayout.FloatField(semiAxisXCont, p.semiAxisX);
                GUIContent semiAxisYCont = new GUIContent("Semiaxis Y", "Semiaxis Y in world units");
                p.semiAxisY = EditorGUILayout.FloatField(semiAxisYCont, p.semiAxisY);
                GUIContent eRotCont = new GUIContent("Rotation", "Ellipse rotation angle along z axis in degrees");
                p.ellipseRotation = EditorGUILayout.FloatField(eRotCont, p.ellipseRotation);
            }

            p.shapeColor = EditorGUILayout.ColorField("Color", p.shapeColor);

            p.drawBorder = EditorGUILayout.ToggleLeft("Draw border", p.drawBorder);
            if (p.drawBorder)
            {
                GUIContent thickCont = new GUIContent("Border thickness", "Thickness of the border in pixels");
                p.borderThickness = EditorGUILayout.Slider(thickCont, p.borderThickness, 1, 20);
                p.borderColor = EditorGUILayout.ColorField("Border color", p.borderColor);
            }

            p.isSector = EditorGUILayout.ToggleLeft("Is a circular sector", p.isSector);
            if (p.isSector)
            {
                GUIContent startAngleCont = new GUIContent("Start angle", "Start angle of the sector in degree");
                p.startAngle = EditorGUILayout.FloatField(startAngleCont, p.startAngle);
                GUIContent endAngleCont = new GUIContent("End angle", "Start angle of the sector in degree");
                p.endAngle = EditorGUILayout.FloatField(endAngleCont, p.endAngle);
            }
        }

        protected override void DrawBotton(Drawer drawer, SpriteRenderer renderer)
        {
            LineStyle borderStyle = new LineStyle(p.borderThickness / renderer.sprite.pixelsPerUnit, p.borderColor);
            CircularShape shape;
            if (!p.drawBorder)
                borderStyle.thickness = 0;

            if (!p.isEllipse && p.radius < 1 / renderer.sprite.pixelsPerUnit)
            {
                Debug.LogError("Radius smaller than 1 pixel");
                return;
            }
            if (p.drawBorder && !p.isEllipse && p.radius <= borderStyle.thickness / 2.0)
            {
                Debug.LogError("Radius smaller than the thickness");
                return;
            }

            if (p.isSector)
            {
                float startAngleRad = p.startAngle;
                float endAngleRad = p.endAngle;
                if (!p.isEllipse)
                    shape = new CircularSector(p.center, p.radius, startAngleRad, endAngleRad,
                        p.shapeColor, borderStyle);
                else
                    shape = new EllipseSector(p.center, p.semiAxisX, p.semiAxisY, startAngleRad,
                        endAngleRad, p.ellipseRotation, p.shapeColor, borderStyle);
            }
            else
            {
                if (!p.isEllipse)
                    shape = new Circle(p.center, p.radius, p.shapeColor, borderStyle);
                else
                    shape = new Ellipse(p.center, p.semiAxisX, p.semiAxisY, p.ellipseRotation, p.shapeColor, borderStyle);
            }

            drawer.Draw(renderer, shape);
        }
    }
}