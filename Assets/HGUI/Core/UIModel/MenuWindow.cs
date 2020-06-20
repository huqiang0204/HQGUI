﻿using UnityEngine;
#if Hot
namespace huqiang.HotUIModel
#else
namespace huqiang.UIModel
#endif
{
    public class MenuWindow : UIBase
    {
        public static Vector3 Dock(Vector2 pos, Vector2 size)
        {
            if (UIMenu.Root == null)
                return size;
            var x = pos.x;
            if (x < 0)
                x += size.x * 0.5f;
            else x -= size.x * 0.5f;
            var y = pos.y;
            if (y < 0)
                y += size.y * 0.5f;
            //return UIMenu.Root.ScreenToLocal(new Vector3(x,y,0));
            return pos;
        }
        protected UIBase Context;
        public virtual void Show(UIBase context, Vector2 pos, object obj = null)
        {
            Context = context;
            var mod = model;
            if (mod != null)
            {
                //mod.data.localPosition = Dock(pos,mod.data.sizeDelta);

            }
        }
        public virtual void Hide()
        {

        }
        public virtual bool Handling(string cmd, object dat)
        {
            return false;
        }
    }
}

