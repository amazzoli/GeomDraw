using UnityEditor;
using UnityEngine;

namespace GeomDraw
{
    public class DrawTextureSubWindow : SubWindow
    {
        public SpriteRenderer sprite;
        public Vector2 origin;
        public Vector2 size;

        public override string Header => "Draw sprite";

        public override bool HasDrawButton => true;

        protected override void DisplayParameters()
        {
            sprite = EditorGUILayout.ObjectField("Texture", sprite, typeof(SpriteRenderer), true, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as SpriteRenderer;
            origin = EditorGUILayout.Vector2Field("Origin", origin);
            size = EditorGUILayout.Vector2Field("Size", size);
        }

        protected override void DrawBotton(Drawer drawer, SpriteRenderer renderer)
        {
            Texture2D text = sprite.sprite.texture;
            //Vector2 size = new Vector2(text.width, text.height) / renderer.sprite.pixelsPerUnit;
            DrawableTexture texture = new DrawableTexture(text.GetPixels(), text.width, origin, size);
            drawer.Draw(texture);
        }
    }
}