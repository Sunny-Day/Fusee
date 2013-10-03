﻿using System.Runtime.InteropServices;

namespace Fusee.Engine
{
    /// <summary>
    /// Sets the bounding box of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rectangle
    {
        /// <summary>
        /// The left x-coordinate
        /// </summary>
        public int Left;

        /// <summary>
        /// The right x-coordinate
        /// </summary>
        public int Right;

        /// <summary>
        /// The upper y-coordinate
        /// </summary>
        public int Top;

        /// <summary>
        /// The lower y-coordinate
        /// </summary>
        public int Bottom;

        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Width { get { return Right - Left; } }
        public int Height { get { return Bottom - Top; } }
    }
}
