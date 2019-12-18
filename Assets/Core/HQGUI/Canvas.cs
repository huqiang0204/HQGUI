﻿using System;
using huqiang;
using UnityEngine;

namespace Assets.Core.HQGUI
{
    public sealed class Canvas:AsyncScript
    {
        GUIElement[] PipeLine = new GUIElement[4096];
        AsyncScript[] scripts = new AsyncScript[4096];
        int point = 0;
        int max;
        /// <summary>
        /// 信息采集
        /// </summary>
        /// <param name="trans"></param>
        void Collection(Transform trans)
        {
            PipeLine[point].localPos = trans.localPosition;
            PipeLine[point].localRot = trans.localRotation;
            PipeLine[point].localScale = trans.localScale;
            PipeLine[point].trans = trans;
            var script= trans.GetComponent<AsyncScript>();
            if(script!=null)
            {
                scripts[max] = script;
                max++;
            }
            PipeLine[point].script = script;
            int c = trans.childCount;
            PipeLine[point].childCount = c;
            point++;
            for (int i = 0; i < c; i++)
                Collection(trans.GetChild(i));
        }
        private void Update()
        {
            point = 0;
            max = 0;
            Collection(transform);
            for (int i = 0; i < scripts.Length; i++)
                scripts[i].MainUpdate();
            thread.AddSubMission((o) => {
                int len = max;
                if (scripts != null)
                    for (int i = 0; i < len; i++)
                        scripts[i].SubUpdate();
            }, null);
        }
    }
}
