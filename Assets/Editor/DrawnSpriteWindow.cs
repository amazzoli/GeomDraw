using Drawing;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;
using static UnityEditor.ShaderData;
using System.Linq;


[CustomEditor(typeof(DrawnSprite))]
public class DrawnSpriteWindow : Editor
{
    bool saveShawn = false;
    bool editShown = false;

    bool translateShown = false;
    Vector2 translateVec;

    bool rotateShown = false;
    Vector2 rotCenter;
    float rotAngle;
    bool rotRelative = true;

    bool reflectShown = false;
    Axis reflectAxis;
    //float reflCoord;
    //bool reflRelative = true;

    bool deformShown;
    Axis deformAxis;
    float defFactor = 1;
    float defCoord;
    bool defRelative;

    string saveName = "";
    string dirPath = "/../SaveImages/";

    Drawer drawer;
    IDrawable drawableShown;
    SubWindow redrawWindow;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawnSprite sprite = (DrawnSprite)target;
        drawer = new Drawer(sprite.GetComponent<SpriteRenderer>());

        Undo(sprite);
        Redrawing(sprite);
        Translate(sprite);
        Rotate(sprite);
        Reflect(sprite);
        Deform(sprite);
        DrawSave(sprite);
    }

    private void Undo(DrawnSprite sprite)
    {
        if (sprite.Undoable)
            if (GUILayout.Button("Undo", GUILayout.Height(20)))
                sprite.Undo();
    }


    private void Redrawing(DrawnSprite sprite)
    {
        editShown = EditorGUILayout.BeginFoldoutHeaderGroup(editShown, "Edit last drawing");

        if (editShown)
        {
            //Debug.Log(sprite.lastDrawable + " " +  drawableShown);
            if (sprite.lastDrawable != null)
            {
                if (drawableShown != sprite.lastDrawable)
                {
                    if (sprite.lastDrawable is CircularShape)
                        RedrawCircularShape(sprite);
                    else if(sprite.lastDrawable is Poligon)
                        RedrawPoligon(sprite);
                    else if (sprite.lastDrawable is BrokenLine)
                        RedrawLine(sprite);
                    else if (sprite.lastDrawable is BezierCurve)
                        RedrawBezier(sprite);
                    else
                        Debug.LogError("Shape not recognized");
                }

                drawableShown = sprite.lastDrawable;
                redrawWindow.Draw(true);
            }

        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void Translate(DrawnSprite sprite)
    {
        translateShown = EditorGUILayout.BeginFoldoutHeaderGroup(translateShown, "Translate");

        if (translateShown)
        {
            if (sprite.lastDrawable != null)
            {
                translateVec = EditorGUILayout.Vector2Field("Translation", translateVec);
                if (GUILayout.Button("Apply", GUILayout.Height(20)))
                {
                    sprite.lastDrawable.Translate(translateVec);
                    IDrawable newDraw = sprite.lastDrawable.Copy();
                    sprite.Undo();
                    drawer.Draw(newDraw);
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void Rotate(DrawnSprite sprite)
    {
        rotateShown = EditorGUILayout.BeginFoldoutHeaderGroup(rotateShown, "Rotate");

        if (rotateShown)
        {
            if (sprite.lastDrawable != null)
            {
                rotAngle = EditorGUILayout.FloatField("Deg. angle", rotAngle);
                rotCenter = EditorGUILayout.Vector2Field("Rotational center", rotCenter);
                rotRelative = EditorGUILayout.ToggleLeft("Relative rot. center", rotRelative);

                if (GUILayout.Button("Apply", GUILayout.Height(20)))
                {
                    sprite.lastDrawable.Rotate(rotAngle * Mathf.Deg2Rad, rotCenter, rotRelative);
                    IDrawable newDraw = sprite.lastDrawable.Copy();
                    sprite.Undo();
                    drawer.Draw(newDraw);
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void Reflect(DrawnSprite sprite)
    {
        reflectShown = EditorGUILayout.BeginFoldoutHeaderGroup(reflectShown, "Reflect");

        if (reflectShown)
        {
            if (sprite.lastDrawable != null)
            {
                reflectAxis = (Axis)EditorGUILayout.EnumPopup("Axis", (System.Enum)reflectAxis);
                //reflCoord = EditorGUILayout.FloatField("Reflection coordinate", reflCoord);
                //reflRelative = EditorGUILayout.ToggleLeft("Relative refl. coord", reflRelative);

                if (GUILayout.Button("Apply", GUILayout.Height(20)))
                {
                    //sprite.lastDrawable.Reflect(reflectAxis, reflCoord, reflRelative);
                    sprite.lastDrawable.Reflect(reflectAxis);
                    IDrawable newDraw = sprite.lastDrawable.Copy();
                    sprite.Undo();
                    drawer.Draw(newDraw);
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void Deform(DrawnSprite sprite)
    {
        deformShown = EditorGUILayout.BeginFoldoutHeaderGroup(deformShown, "Deform");

        if (deformShown)
        {
            if (sprite.lastDrawable != null)
            {
                deformAxis = (Axis)EditorGUILayout.EnumPopup("Axis", (System.Enum)deformAxis);
                defFactor = EditorGUILayout.FloatField("Deformation factor", defFactor);
                defCoord = EditorGUILayout.FloatField("Deformation coordinate", defCoord);
                defRelative = EditorGUILayout.ToggleLeft("Relative def. coord", defRelative);

                if (GUILayout.Button("Apply", GUILayout.Height(20)))
                {
                    sprite.lastDrawable.Deform(deformAxis, defFactor, defCoord, defRelative);
                    IDrawable newDraw = sprite.lastDrawable.Copy();
                    sprite.Undo();
                    drawer.Draw(newDraw);
                }
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void RedrawLine(DrawnSprite sprite)
    {
        BrokenLine line = (BrokenLine)sprite.lastDrawable;
        Vector2[] points = line.Points;

        redrawWindow = new DrawLineSubWindow();
        ((DrawLineSubWindow)redrawWindow).nPoints = points.Length;
        ((DrawLineSubWindow)redrawWindow).points = new List<Vector2>(points);
        ((DrawLineSubWindow)redrawWindow).borderColor = line.Style.color;
        float pxUnit = sprite.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        ((DrawLineSubWindow)redrawWindow).thickness = line.Style.thickness * pxUnit;
    }

    private void RedrawBezier(DrawnSprite sprite)
    {
        BezierCurve curve = (BezierCurve)sprite.lastDrawable;
        Vector2[] points = curve.Points;

        redrawWindow = new DrawBezierSubWindow();
        ((DrawBezierSubWindow)redrawWindow).startPoint = points[0];
        ((DrawBezierSubWindow)redrawWindow).endPoint = points[3];
        ((DrawBezierSubWindow)redrawWindow).ctrlPoint1 = points[1];
        ((DrawBezierSubWindow)redrawWindow).ctrlPoint2 = points[2];
        ((DrawBezierSubWindow)redrawWindow).borderColor = curve.Style.color;
        float pxUnit = sprite.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        ((DrawBezierSubWindow)redrawWindow).thickness = curve.Style.thickness * pxUnit;
    }

    private void RedrawCircularShape(DrawnSprite sprite)
    {
        float pxUnit = sprite.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        DrawCirclePars pars = new DrawCirclePars
        {
            center = ((CircularShape)sprite.lastDrawable).center,
            shapeColor = ((CircularShape)sprite.lastDrawable).color,
            borderColor = ((CircularShape)sprite.lastDrawable).borderStyle.color,
            borderThickness = ((CircularShape)sprite.lastDrawable).borderStyle.thickness * pxUnit
        };
        if (sprite.lastDrawable is Circle)
        {
            pars.radius = ((Circle)sprite.lastDrawable).radius;
            pars.isSector = false;
            pars.isEllipse = false;
            if (sprite.lastDrawable is CircularSector)
            {
                pars.isSector = true;
                pars.startAngle = ((CircularSector)sprite.lastDrawable).angle1;
                pars.endAngle = ((CircularSector)sprite.lastDrawable).angle2;
            }
        }
        else if (sprite.lastDrawable is Ellipse)
        {
            pars.isSector = false;
            pars.isEllipse = true;
            pars.semiAxisX = ((Ellipse)sprite.lastDrawable).semiAxisX;
            pars.semiAxisY = ((Ellipse)sprite.lastDrawable).semiAxisY;
            pars.ellipseRotation = ((Ellipse)sprite.lastDrawable).rotationAngle;
            if (sprite.lastDrawable is EllipseSector)
            {
                pars.isSector = true;
                pars.startAngle = ((EllipseSector)sprite.lastDrawable).startAngle;
                pars.endAngle = ((EllipseSector)sprite.lastDrawable).endAngle;
            }
        }
        else
            Debug.LogError("Circular shape not recognized");

        redrawWindow = new DrawCircleSubWindow();
        ((DrawCircleSubWindow)redrawWindow).p = pars;
    }

    private void RedrawPoligon(DrawnSprite sprite)
    {
        float pxUnit = sprite.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        Poligon poli = (Poligon)sprite.lastDrawable;

        Vector2[] verts = poli.Border(0).ToArray();
        Vector2[] roundedVerts = new Vector2[verts.Length];
        int decimals = -Mathf.RoundToInt(Mathf.Log10(poli.Tollerance));
        for (int i = 0; i < roundedVerts.Length; i++)
            roundedVerts[i] = new Vector2((float)Decimal.Round((decimal)verts[i].x, decimals), verts[i].y);


        redrawWindow = new DrawPoligonSubWindow();
        ((DrawPoligonSubWindow)redrawWindow).vertices = new List<Vector2> (roundedVerts);
        ((DrawPoligonSubWindow)redrawWindow).shapeColor = poli.Color;
        ((DrawPoligonSubWindow)redrawWindow).borderColor = poli.BorderStyle.color;
        ((DrawPoligonSubWindow)redrawWindow).borderThickness = poli.BorderStyle.thickness * pxUnit;
    }

    private void DrawSave(DrawnSprite sprite)
    {
        saveShawn = EditorGUILayout.BeginFoldoutHeaderGroup(saveShawn, "Export png");

        if (saveShawn)
        {
            GUIContent nameCont = new GUIContent("Name", "Exported image name");
            saveName = EditorGUILayout.TextField(nameCont, saveName);

            GUIContent dirCont = new GUIContent("Directory", "Directory relative to the application path");
            dirPath = EditorGUILayout.TextField(dirCont, dirPath);

            if (GUILayout.Button("Save png", GUILayout.Height(20)))
                sprite.Save(saveName, Application.dataPath + dirPath);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}
