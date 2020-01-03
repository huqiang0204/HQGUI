﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using huqiang.Data;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public unsafe struct HGraphicsData
    {
        public AsyncScriptData scriptData;
        public Int32 shader;
        public Int32 asset;
        public Int32 MainTexture;
        public Int32 STexture;
        public Int32 TTexture;
        public Int32 FTexture;
        public Color color;
        public static int Size = sizeof(HGraphicsData);
        public static int ElementSize = Size / 4;
    }
    public class HGraphicsLoader:AsyncScriptLoader
    {
        protected string shader;
        protected string asset;
        protected string MainTexture;
        protected string STexture;
        protected string TTexture;
        protected string FTexture;
        protected unsafe void LoadHGraphics(FakeStruct fake, HGraphics tar)
        {
            HGraphicsData* src = (HGraphicsData*)fake.ip;
            asset = fake.GetData<string>(src->asset);
            if (asset != null)
            {
                MainTexture = fake.GetData<string>(src->MainTexture);
                if (MainTexture != null)
                    tar.MainTexture = ElementAsset.FindTexture(asset, MainTexture);
                STexture = fake.GetData<string>(src->STexture);
                if (STexture != null)
                    tar.STexture = ElementAsset.FindTexture(asset, STexture);
                TTexture = fake.GetData<string>(src->TTexture);
                if (TTexture != null)
                    tar.TTexture = ElementAsset.FindTexture(asset, TTexture);
                FTexture = fake.GetData<string>(src->FTexture);
                if (FTexture != null)
                    tar.FTexture = ElementAsset.FindTexture(asset, FTexture);
            }
            shader = fake.GetData<string>(src->shader);
            if (shader != null)
                tar.Material = new Material(Shader.Find(shader));
            tar.m_color = src->color;
        }
        protected unsafe void SaveHGraphics(FakeStruct fake, HGraphics src)
        {
            HGraphicsData* tar = (HGraphicsData*)fake.ip;
            var tex = src.MainTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->MainTexture = fake.buffer.AddData(tex.name);
                }
            }
            tex = src.STexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->STexture = fake.buffer.AddData(tex.name);
                }
            }
            tex = src.TTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->TTexture = fake.buffer.AddData(tex.name);
                }
            }
            tex = src.FTexture;
            if (tex != null)
            {
                var an = ElementAsset.TxtureFormAsset(tex.name);
                if (an != null)
                {
                    tar->asset = fake.buffer.AddData(an);
                    tar->FTexture = fake.buffer.AddData(tex.name);
                }
            }
            if (src.m_material != null)
                tar->shader = fake.buffer.AddData(src.m_material.shader.name);
            tar->color = src.m_color;
        }
        public unsafe override void LoadToObject(FakeStruct fake, Component com)
        {
            var hg = com as HGraphics;
            if (hg == null)
                return;
            LoadScript(fake.ip, hg);
            LoadHGraphics(fake, hg);
        }
        public unsafe override FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var src = com as HGraphics;
            if (src == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, HGraphicsData.ElementSize);
            SaveScript(fake.ip, src);
            SaveHGraphics(fake,src);
            return fake;
        }
      
    }
}