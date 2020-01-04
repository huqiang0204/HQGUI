﻿using huqiang.Core.HGUI;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HCanvas), true)]
[CanEditMultipleObjects]
public class HCanvasEditor:AsyncScriptEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
            Refresh(target as HCanvas);
    }
    void Refresh(HCanvas canvas)
    {
        if (canvas == null)
            return;

        canvas.Refresh();
    }
}
