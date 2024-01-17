using UnityEditor;
using UnityEngine;


namespace GeomDraw
{
    public class CreateSpriteSubWindow : SubWindow
    {
        Transform spriteParent;
        Color spriteColor = Color.white;
        Vector2 spriteSize = new Vector2(1, 1);
        float pixelsPerUnit = 100;

        public override string Header => "Create a new sprite";

        public override bool HasDrawButton => false;

        protected override void DisplayParameters()
        {
            spriteSize = EditorGUILayout.Vector2Field("Size", spriteSize);
            pixelsPerUnit = EditorGUILayout.FloatField("Pixels per unit", pixelsPerUnit);
            spriteColor = EditorGUILayout.ColorField("Color", spriteColor);
            spriteParent = EditorGUILayout.ObjectField("Parent", spriteParent, typeof(Transform), true) as Transform;

            if (GUILayout.Button("Create", GUILayout.Height(20)))
            {
                GameObject go = new GameObject();
                if (spriteParent != null)
                    go.transform.parent = spriteParent;
                SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                Drawer drawer = new Drawer(renderer);
                drawer.NewEmptySprite(spriteSize[0], spriteSize[1], pixelsPerUnit, spriteColor);
            }
        }
    }
}