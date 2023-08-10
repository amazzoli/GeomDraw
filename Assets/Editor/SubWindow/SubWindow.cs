using Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Drawing
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

        public void Draw(bool undoAndDraw = false)
        {
            SpriteRenderer renderer = FindRenderer();
            Drawer drawer = new Drawer(renderer);

            DisplayParameters();

            if (!HasDrawButton)
                return;

            if (GUILayout.Button("Draw", GUILayout.Height(20)))
            {
                if (renderer != null)
                {
                    if (undoAndDraw)
                        UndoSprite(renderer);
                    DrawBotton(drawer, renderer);
                }
                else
                    Debug.Log("Select an object with a SpriteRenderer attached");
            }
        }

        protected abstract void DisplayParameters();

        protected virtual void DrawBotton(Drawer drawer, SpriteRenderer renderer) { }

        protected SpriteRenderer FindRenderer()
        {
            SpriteRenderer renderer = null;
            if (isDrawnSprite) 
                renderer = spriteRenderer;
            else if (Selection.activeTransform != null)
                if (Selection.activeTransform.GetComponent<SpriteRenderer>() != null)
                    renderer = Selection.activeTransform.GetComponent<SpriteRenderer>();

            //if (renderer == null)
            //    Debug.Log("Select an object with a SpriteRenderer attached");

            return renderer;
        }

        protected void UndoSprite(SpriteRenderer renderer)
        {
            DrawnSprite sprite = renderer.GetComponent<DrawnSprite>();
            if (sprite == null)
            {
                Debug.LogError("No sprite found for UndoAndDraw");
                return;
            }
            sprite.Undo();
        }
    }
}