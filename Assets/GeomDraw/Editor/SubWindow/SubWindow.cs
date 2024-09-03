using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using static PlasticGui.PlasticTableColumn;


namespace GeomDraw
{
    [System.Serializable]
    public class SubWindowEvent : UnityEvent<SubWindow> { }


    public abstract class SubWindow
    {
        bool isShown = false;
        public SubWindowEvent openEvent;

        // Case of a window open in an already drawn sprite to edit. Otherwise is in the DrawWindow menu
        bool isDrawnSprite;

        SpriteRenderer spriteRenderer;
        MeshRenderer meshRenderer;


        public SubWindow(bool isDrawnSprite = false)
        {
            openEvent = new SubWindowEvent();
        }

        public bool IsShown
        {
            get { return isShown; }
            set
            {
                if (!isShown && value) openEvent.Invoke(this);
                isShown = value;
            }
        }

        public abstract string Header { get; }

        public abstract bool HasDrawButton { get; }

        public void Draw(DrawWindow window, bool undoAndDraw = false)
        {
            FindRenderer();
            DisplayParameters();

            if (!HasDrawButton)
                return;

            if (GUILayout.Button("Draw", GUILayout.Height(20)))
            {
                if (meshRenderer != null)
                {
                    DrawerMesh drawerMesh = new(meshRenderer);
                    drawerMesh.Antialiase = window.antialiasing;
                    drawerMesh.UpdateMipMaps = window.updateMipMap;
                    DrawBotton(drawerMesh, meshRenderer);
                }
                else if (spriteRenderer != null)
                {
                    DrawerSprite drawerSprite = new DrawerSprite(spriteRenderer);
                    //drawerSprite.Antialiase = window.antialiasing;
                    drawerSprite.UpdateMipMaps = window.updateMipMap;
                    if (undoAndDraw) UndoSprite(spriteRenderer);
                    DrawBotton(drawerSprite, spriteRenderer);
                }
                else
                    Debug.Log("Select an object with a SpriteRenderer or a MeshRenderer attached");
            }
        }

        protected abstract void DisplayParameters();

        protected virtual void DrawBotton(DrawerSprite drawer, SpriteRenderer renderer) { }

        protected virtual void DrawBotton(DrawerMesh drawer, MeshRenderer renderer) { }

        protected void FindRenderer()
        {
            if (isDrawnSprite)
                return;
            //renderer = spriteRenderer;

            spriteRenderer = null;
            meshRenderer = null;    
            if (Selection.activeTransform != null)
            {
                if (Selection.activeTransform.GetComponent<SpriteRenderer>() != null)
                    spriteRenderer = Selection.activeTransform.GetComponent<SpriteRenderer>();
                else if (Selection.activeTransform.GetComponent<MeshRenderer>() != null)
                    meshRenderer = Selection.activeTransform.GetComponent<MeshRenderer>();
            }
            //if (renderer == null)
            //    Debug.Log("Select an object with a SpriteRenderer attached");
        }

        protected void UndoSprite(SpriteRenderer renderer)
        {
            Undoer undo = renderer.GetComponent<Undoer>();
            if (undo == null)
            {
                Debug.LogError("No Undoer found");
                return;
            }
            undo.Undo();
        }
    }
}