using UnityEditor;
using UnityEngine;


namespace GeomDraw
{
    public class CreateCanvasSubWindow : SubWindow
    {
        Transform parent;
        Color color = Color.white;
        Vector2 size = new Vector2(1, 1);
        float pixelsPerUnit = 100;
        bool isMesh = true;

        public override string Header => "Create a new canvas";

        public override bool HasDrawButton => false;

        protected override void DisplayParameters()
        {
            isMesh = EditorGUILayout.Toggle("Is Mesh (tic) or Sprite", isMesh);
            size = EditorGUILayout.Vector2Field("Size", size);
            pixelsPerUnit = EditorGUILayout.FloatField("Pixels per unit", pixelsPerUnit);
            color = EditorGUILayout.ColorField("Color", color);
            parent = EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true) as Transform;

            if (GUILayout.Button("Create", GUILayout.Height(20)))
            {
                GameObject go;
                if (isMesh)
                {
                    go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    go.transform.Rotate(new Vector3(180, 0, 0));
                    GameObject.DestroyImmediate(go.GetComponent<MeshCollider>());
                    MeshRenderer renderer = go.GetComponent<MeshRenderer>();
                    renderer.sharedMaterial = Resources.Load<Material>("BaseMaterial");
                    DrawerMesh drawer = new DrawerMesh(renderer);
                    drawer.NewEmptyMesh(size[0], size[1], pixelsPerUnit, color);
                }
                else
                {
                    go = new GameObject();
                    SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                    DrawerSprite drawer = new DrawerSprite(renderer);
                    drawer.NewEmptySprite(size[0], size[1], pixelsPerUnit, color);
                }
                if (parent != null)
                    go.transform.parent = parent;
            }
        }
    }
}