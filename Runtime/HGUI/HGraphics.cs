﻿using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public struct HVertex
    {
        public Vector3 position;
        public Vector3 normal;
        //public Vector4 tangent;
        public Color32 color;
        public Vector2 uv;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
        public Vector2 uv4;
        public int picture;
    }
    [DisallowMultipleComponent]
    public class HGraphics:UIElement
    {
        protected static BlockBuffer<int> trisBuffer = new BlockBuffer<int>(48, 1024);
        internal static Shader DefShader { get {
                if(shader==null)
                    shader = Shader.Find("HGUI/UIDef");//HGUI/UIDef
                return shader;
            } }
        public static Shader shader;
        [SerializeField]
        internal Color32 m_color = Color.white;
        public Vector4 uvrect = new Vector4(0,0,1,1);
        public override Color32 MainColor { get => m_color; set { m_color = value; m_colorChanged = true;} }
        public override Vector2 SizeDelta { get => m_sizeDelta; set { m_sizeDelta = value; m_vertexChange = true; } }
        internal int[] tris;
        internal BlockInfo<HVertex> vertInfo;
        internal BlockInfo<int> trisInfo;
        internal BlockInfo<int> trisInfo2;
        [SerializeField]
        internal Material m_material;
        public Material Material { get => m_material; set {
                m_material = value;
                if (value == null)
                    MatID = 0;
                else MatID = value.GetInstanceID();
            } }
        internal int MatID;
        [HideInInspector]
        public bool m_dirty = true;
        [HideInInspector]
        public bool m_vertexChange = true;
        [HideInInspector]
        public bool m_colorChanged = true;
        [HideInInspector]
        [SerializeField]
        internal Texture[] textures = new Texture[4];
        [HideInInspector]
        [SerializeField]
        internal int[] texIds = new int[4];
        [HideInInspector]
        [SerializeField]
        internal bool[] fillColors = new bool[4];
        public Texture MainTexture
        {
            get => textures[0];
            set {
                textures[0] = value;
                if (value != null)
                    texIds[0] = value.GetInstanceID();
                else
                    texIds[0] = 0;
            }
        }
        public Texture STexture
        {
            get => textures[1];
            set
            {
                textures[1] = value;
                if (value != null)
                    texIds[1] = value.GetInstanceID();
                else
                    texIds[1] = 0;
            }
        }
        public Texture TTexture
        {
            get => textures[2];
            set
            {
                textures[2] = value;
                if (value != null)
                    texIds[2] = value.GetInstanceID();
                else
                    texIds[2] = 0;
            }
        }
        public Texture FTexture
        {
            get => textures[3];
            set
            {
                textures[3] = value;
                if (value != null)
                    texIds[3] = value.GetInstanceID();
                else
                    texIds[3] = 0;
            }
        }
        public bool Shadow;
        public Vector2 shadowOffsset=new Vector2(1,-1);
        public Color32 shadowColor=new Color32(0,0,0,255);
        public override void ReSized()
        {
            base.ReSized();
            m_vertexChange = true;
        }
        public virtual void UpdateMesh()
        {
            if(m_colorChanged)
            {
                if (vertInfo.DataCount>0)
                {
                    unsafe
                    {
                        HVertex* hv = vertInfo.Addr;
                        for (int i = 0; i < vertInfo.DataCount; i++)
                            hv[i].color = m_color;
                    }
                }
                m_colorChanged = false;
            }
        }
        private void Start()
        {
            m_dirty = true;
        }
        protected virtual void OnDestroy()
        {
            vertInfo.Release();
            trisInfo.Release();
            trisInfo2.Release();
        }
        public void LoadFromMesh(List<HVertex> vert, List<int> tris)
        {
            LoadVert(vert);
            LoadTris(tris);
        }
        public void LoadVert(HVertex[] vert)
        {
            m_dirty = false;
            int c = vert.Length;
            if (c > vertInfo.Size | c + 32 < vertInfo.Size)
            {
                vertInfo.Release();
                vertInfo = HGUIMesh.blockBuffer.RegNew(c);
            }
            unsafe
            {
                HVertex* hv = vertInfo.Addr;
                for (int i = 0; i < c; i++)
                {
                    hv[i] = vert[i];
                }
                vertInfo.DataCount = c;
            }
        }
        public void LoadVert(List<HVertex> vert)
        {
            m_dirty = false;
            int c = vert.Count;
            if (c > vertInfo.Size | c + 32 < vertInfo.Size)
            {
                vertInfo.Release();
                vertInfo = HGUIMesh.blockBuffer.RegNew(c);
            }
            unsafe
            {
                HVertex* hv = vertInfo.Addr;
                for (int i = 0; i < c; i++)
                {
                    hv[i] = vert[i];
                }
                vertInfo.DataCount = c;
            }
        }    
        void LoadTris(int[] tri, ref BlockInfo<int> info)
        {
            tris = null;
            int tc = tri.Length;
            if (tc > info.Size | tc + 48 < info.Size)
            {
                info.Release();
                info = trisBuffer.RegNew(tc);
            }
            unsafe
            {
                int* ht = (int*)info.Addr;
                for (int i = 0; i < tc; i++)
                {
                    ht[i] = tri[i];
                }
                info.DataCount = tc;
            }
        }
        public void LoadTris(int[] tri)
        {
            LoadTris(tri,ref trisInfo);
        }
        public void LoadTris2(int[] tri)
        {
            LoadTris(tri, ref trisInfo2);
        }
        void LoadTris(List<int> tri, ref BlockInfo<int> info)
        {
            tris = null;
            int tc = tri.Count;
            if (tc > info.Size | tc + 48 < info.Size)
            {
                info.Release();
                info = trisBuffer.RegNew(tc);
            }
            unsafe
            {
                int* ht = (int*)info.Addr;
                for (int i = 0; i < tc; i++)
                {
                    ht[i] = tri[i];
                }
                info.DataCount = tc;
            }
        }
        public void LoadTris(List<int> tri)
        {
            LoadTris(tri, ref trisInfo);
        }
        public void LoadTris2(List<int> tri)
        {
            LoadTris(tri, ref trisInfo2);
        }
    }
}
