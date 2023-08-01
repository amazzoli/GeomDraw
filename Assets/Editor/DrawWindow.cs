using Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;



public class DrawWindow : EditorWindow
{
    Drawer drawer;
    List<SubWindow> subWindows;

    public List<Vector2> prova;


    [MenuItem("Window/Drawer")]
    static void OpenWindow()
    {
        DrawWindow window = (DrawWindow)GetWindow(typeof(DrawWindow));
        window.minSize = new UnityEngine.Vector2(200, 200);
        window.Show();
    }

    private void OnEnable()
    {
        drawer = new Drawer();

        subWindows = new List<SubWindow>()
        {
             new CreateSpriteSubWindow(),
             new DrawLineSubWindow(),
             new DrawBezierSubWindow(),
             new DrawCircleSubWindow(),
             new DrawPoligonSubWindow(),
             new DrawRegularPoligonSubWindow(),
             new BucketSubWindow(),
        };

       foreach (SubWindow subWindow in subWindows)
            subWindow.openEvent.AddListener(CloseOldTabs);
    }

    private void OnGUI()
    {
        foreach (SubWindow subWindow in subWindows)
        {
            subWindow.IsShown = EditorGUILayout.BeginFoldoutHeaderGroup(subWindow.IsShown, subWindow.Header);
            if (subWindow.IsShown) subWindow.Draw(drawer);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    private void CloseOldTabs(SubWindow newOpenedTab)
    {
        foreach (SubWindow subWindow in subWindows)
            if (subWindow != newOpenedTab) subWindow.IsShown = false;
    }
}
