/*
 * Copyright 2016 by Mario Vernari, Cet Electronics
 *
 * Licensed under the MIT License
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Cet.UI.TileListView
{
    internal class PointerMovingRange
    {
        public double Min;
        public double Max;

        public double Coerce(double value)
        {
            if (value < this.Min) return this.Min;
            if (value > this.Max) return this.Max;
            return value;
        }
    }


    internal class TileItemDragInfo
    {
        public Point Offset { get; set; }
        public WeakReference<TileListViewItem> ViewItem { get; set; }
    }


    [Flags]
    internal enum TileItemHotArea
    {
        None = 0,

        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,

        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,

        Move = 16,
    }
}
