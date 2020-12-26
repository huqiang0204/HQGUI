﻿using huqiang.Data;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;
using huqiang;
using Assets.Scripts;
using huqiang.UIModel;
using huqiang.Core.HGUI;

public class UITest : TestPageHelper
{
    public HGUIRender GUIRender;
    public override void LoadTestPage()
    {
        UIElement.DisposeAll();
        HCanvas.RegCanvas(GUIRender);
        UISystem.PhysicalScale =1f;
        //HCanvas can = new HCanvas();
        //can.DesignSize = GUIRender.DesignSize;
        //can.SizeDelta = GUIRender.DesignSize;
        //can.name = GUIRender.name;
        //GUIRender.canvas = can;
        HCanvas.CurrentCanvas = GUIRender.canvas;
        App.Initial(GUIRender.canvas);
        //RemoteLog.Instance.Connection("192.168.0.144",8899);
        Application.targetFrameRate = 1000;
#if UNITY_IPHONE || UNITY_ANDROID
        //Scale.DpiScale = true;
#endif
#if UNITY_EDITOR
        UIPage.LoadPage<StartPage>();
#else
        //ElementAsset.LoadAssetsAsync("base.unity3d",(o,e)=> { UIPage.LoadPage<ChatPage>(); });
#endif
        DataGrid.CursorX = DockPanelLine.CursorX = Resources.Load<Texture2D>("StretchWX");
        DataGrid.CursorY = DockPanelLine.CursorY = Resources.Load<Texture2D>("StretchWY");
    }
    public override void OnUpdate()
    {
        //if (IME.CompStringChanged)
        //    Debug.Log(IME.CompString);
        //if (IME.InputDone)
        //    Debug.Log(IME.ResultString);
    }
}