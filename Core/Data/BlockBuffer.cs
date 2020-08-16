﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Video;

namespace huqiang.Data
{
    public interface Addressable
    {
        IntPtr Addr { get; }
        void Release(ref BlockInfo block);
    }
    public struct BlockInfo
    {
        public int DataCount;
        int Index;
        int Length;
        int areaSize;
        Addressable address;
        public unsafe byte* Addr
        {
            get
            {
                return (byte*)address.Addr + Index*areaSize;
            }
        }
        public int Offset { get => Index; }
        /// <summary>
        /// 可以存放的元素尺寸
        /// </summary>
        public int Size { get => Length; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="index"></param>
        /// <param name="len"></param>
        /// <param name="area">每区域的大小,即块的尺寸x元素的尺寸</param>
        public BlockInfo(Addressable addr, int index, int len,int area)
        {
            address = addr;
            Index = index;
            Length = len;
            areaSize = area;
            DataCount = 0;
        }
        public void Release()
        {
            if (Length == 0)
                return;
            address.Release(ref this);
            Length = 0;
        }
    }
    public struct BlockInfoT<T> where T:unmanaged
    {
        public int DataCount;
        int Index;
        int Length;
        int areaSize;
        unsafe T* address;
        public unsafe T* Addr
        {
            get
            {
                return Addr + Index * areaSize;
            }
        }
        public int Offset { get => Index; }
        public int Size { get => Length; }
        public BlockInfoT(IntPtr addr, int index, int len, int area)
        {
            unsafe { address = (T*)addr; }
            Index = index;
            Length = len;
            areaSize = area;
            DataCount = 0;
        }
        public void Clear()
        {
            Length = 0;
        }
    }
    public class BlockBuffer<T> : Addressable, IDisposable where T : unmanaged
    {
        IntPtr ptr;
        int blockSize;//块大小
        int pe;//pe头尺寸
        int dataLength;//数据总体尺寸
        int allLength;//总计申请的内存尺寸
        int eSize;//每个元素的大小

        public IntPtr Addr => ptr + pe;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="block">块大小,32则为一个块可以包含32个元素</param>
        /// <param name="len">32,32 则为32*32*sizeof(T)总缓存大小</param>
        public unsafe BlockBuffer(int block = 32, int len = 32)
        {
            eSize = sizeof(T);
            pe = len;
            blockSize = block;
            dataLength = len * blockSize *eSize;
            allLength = pe + dataLength;
            ptr = Marshal.AllocHGlobal(allLength);
            byte* bp = (byte*)ptr;
            for (int i = 0; i < pe; i++)//填0
                bp[i] = 0;
        }
        public unsafe BlockInfo RegNew(int len)
        {
            byte* bp = (byte*)ptr;
            int block = len / blockSize;
            if (len % blockSize > 0)
                block++;
            int c = 0;
            int index = 0;
            for (int i = 0; i < pe; i++)
            {
                if (bp[i] == 0)
                {
                    c++;
                    if (c >= block)
                    {
                        index = i - block+1;
                        break;
                    }
                }
                else c = 0;
            }
            int o = index;
            for(int i=0;i<block;i++)
            {
                bp[o] = 1;
                o++;
            }
            len = block * blockSize;
            int all = len * eSize;
            int area = blockSize * eSize;
            IntPtr p = ptr + pe;
            p += index * area;
            unsafe
            {
                bp = (byte*)p;
                for (int i = 0; i < all; i++)
                {
                    *bp = 0;
                    bp++;
                }
            }
            return new BlockInfo(this, index, len, area);
        }
        public unsafe void Release(ref BlockInfo blockInfo)
        {
            int block = blockInfo.Size / blockSize;//计算有多少个块
            byte* bp = (byte*)ptr;
            int o = blockInfo.Offset;
            for (int i=0;i<block;i++)
            {
                bp[o] = 0;
                o++;
            }
        }
        public unsafe void RegNew(ref BlockInfoT<T> blockInfo, int len)
        {
            byte* bp = (byte*)ptr;
            int block = len / blockSize;
            if (len % blockSize > 0)
                block++;
            int c = 0;
            int index = 0;
            for (int i = 0; i < pe; i++)
            {
                if (bp[i] == 0)
                {
                    c++;
                    if (c >= block)
                    {
                        index = i - block + 1;
                        break;
                    }
                }
                else c = 0;
            }
            int o = index;
            for (int i = 0; i < block; i++)
            {
                bp[o] = 1;
                o++;
            }
            int os = index * blockSize * eSize + pe;
            len = block * blockSize;
            blockInfo = new BlockInfoT<T>(ptr + pe, index, len, blockSize * eSize);
        }
        public unsafe void Release(ref BlockInfoT<T> blockInfo)
        {
            if (blockInfo.Size == 0)
                return;
            int block = blockInfo.Size / blockSize;
            byte* bp = (byte*)ptr;
            int o = blockInfo.Offset;
            for (int i = 0; i < block; i++)
            {
                bp[o] = 0;
                o++;
            }
            blockInfo.Clear();
        }
        /// <summary>
        /// 容量不够时扩容
        /// </summary>
        unsafe void Expansion()
        {
            int tl = pe * 2;
            int dl = tl * blockSize * eSize;
            int al = tl + dl;
            IntPtr pl = Marshal.AllocHGlobal(al);
            byte* src = (byte*)ptr;
            byte* tar = (byte*)ptr;
            for (int i = 0; i < pe; i++)
                tar[i] = src[i];
            for (int i = pe; i < tl; i++)//填0
                tar[i] = 0;
            int ts = tl;
            int ss = pe;
            for(int i=0;i<dataLength;i++)
            {
                tar[ts] = src[ss];
                ts++;
                ss++;
            }
            Marshal.FreeHGlobal(ptr);
            pe = tl;
            ptr = pl;
            dataLength = dl;
            allLength = al;
        }
        public void Dispose()
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
