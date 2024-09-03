using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace GeomDraw
{
    public class DrawWindow : EditorWindow
    {
        List<SubWindow> subWindows;

        // OPTIONS
        public bool antialiasing;
        public bool updateMipMap;
        bool optionsShown;


        [MenuItem("Window/Drawer")]
        static void OpenWindow()
        {
            DrawWindow window = (DrawWindow)GetWindow(typeof(DrawWindow));
            window.minSize = new UnityEngine.Vector2(200, 200);
            window.Show();
        }

        private void OnEnable()
        {
            subWindows = new List<SubWindow>()
        {
             new CreateCanvasSubWindow(),
             new DrawLineSubWindow(),
             new DrawBezierSubWindow(),
             new DrawCircleSubWindow(),
             new DrawPolygonSubWindow(),
             new DrawRegularPolygonSubWindow(),
             new BucketSubWindow(),
             new DrawTextureSubWindow()
        };
            foreach (SubWindow subWindow in subWindows)
                subWindow.openEvent.AddListener(CloseOldTabs);
        }

        private void OnGUI()
        {
            foreach (SubWindow subWindow in subWindows)
            {
                subWindow.IsShown = EditorGUILayout.BeginFoldoutHeaderGroup(subWindow.IsShown, subWindow.Header);
                if (subWindow.IsShown) subWindow.Draw(this);
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            optionsShown = EditorGUILayout.BeginFoldoutHeaderGroup(optionsShown, "OPTIONS");
            antialiasing = EditorGUILayout.Toggle("Antialiasing", antialiasing);
            updateMipMap = EditorGUILayout.Toggle("UpdateMipMap", updateMipMap);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void CloseOldTabs(SubWindow newOpenedTab)
        {
            foreach (SubWindow subWindow in subWindows)
                if (subWindow != newOpenedTab) subWindow.IsShown = false;
        }
    }
}