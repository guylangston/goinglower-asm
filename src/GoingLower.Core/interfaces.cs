﻿using System;
using System.Collections.Generic;
using SkiaSharp;

namespace GoingLower.Core
{
     
    public enum DOrient
    {
        Horz,
        Vert
    }

    public enum LineAnchor
    {
        Left, Middle, Right
    }

    public enum VertAnchor
    {
        Top, Centre, Bottom
    }
    
    
    public enum BlockAnchor
    {
        None,
        TL, TM, TR,
        ML, MM, MR,
        BL, BM, BR
    }
    

    public interface IBorder
    {
        float Top    { get; }
        float Bottom { get; }
        float Left   { get; }
        float Right  { get; }
    }
}