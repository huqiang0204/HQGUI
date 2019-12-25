﻿using Assets.Core.HGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HText), true)]
[CanEditMultipleObjects]
public class HTextEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        HText txt = target as HText;
        if (txt != null)
        {
            txt.Color= EditorGUILayout.ColorField("Color",txt.Color);
            EditorGUILayout.LabelField("Text");
            var style = GUI.skin.textArea;
            style.wordWrap = true;
            txt.Text = EditorGUILayout.TextArea(txt.Text,style);
            txt.Font = EditorGUILayout.ObjectField("Font", txt.Font, typeof(Font), true) as Font;
            if(GUI.changed)
            {
                var can = FindHCanvas(txt.transform);
                if (can != null)
                    can.Refresh();
            }
        }
    }
    HCanvas FindHCanvas(Transform trans)
    {
        if (trans == null)
            return null;
        var can = trans.GetComponent<HCanvas>();
        if (can == null)
            return FindHCanvas(trans.parent);
        return can;
    }
}