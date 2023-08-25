using GeomDraw;
using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    public class BucketSubWindow : SubWindow
    {
        Color color = Color.white;
        float sensitivity = 0.1f;
        Vector2 point;

        public override string Header => "Bucket tool";

        public override bool HasDrawButton => true;

        protected override void DisplayParameters()
        {
            point = EditorGUILayout.Vector2Field("Start point ", point);
            color = EditorGUILayout.ColorField("Color", color);
            sensitivity = EditorGUILayout.Slider("Sensitivity", sensitivity, 0, 1);
        }

        protected override void DrawBotton(Drawer drawer, SpriteRenderer renderer)
        {
            Bucket bucket = new Bucket(renderer, point, color, sensitivity);
            bucket.Run();
        }
    }
}