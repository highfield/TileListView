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

namespace Cet.UI.TileListView
{
    public interface ITileBlock
    {
        int StartRow { get; }
        int EndRow { get; }
        int StartCol { get; }
        int EndCol { get; }
        bool IsValid { get; }
    }


    internal sealed class TileBlock
        : ITileBlock
    {
        internal TileBlock() { }

        public int StartRow { get; internal set; }
        public int EndRow { get; internal set; }
        public int StartCol { get; internal set; }
        public int EndCol { get; internal set; }

        public int RowSpan
        {
            get { return this.EndRow - this.StartRow + 1; }
        }

        public int ColSpan
        {
            get { return this.EndCol - this.StartCol + 1; }
        }

        public bool IsValid
        {
            get
            {
                return
                    this.StartRow >= 0 &&
                    this.StartRow <= this.EndRow &&
                    this.StartCol >= 0 &&
                    this.StartCol <= this.EndCol;
            }
        }

        public bool Contains(
            TileBlock tblock
            )
        {
            return
                this.IsValid &&
                tblock.IsValid &&
                this.StartRow <= tblock.StartRow &&
                tblock.EndRow <= this.EndRow &&
                this.StartCol <= tblock.StartCol &&
                tblock.EndCol <= this.EndCol;
        }

        public bool Contains(
            int row,
            int col
            )
        {
            return
                this.IsValid &&
                this.StartRow <= row &&
                row <= this.EndRow &&
                this.StartCol <= col &&
                col <= this.EndCol;
        }

        public static TileBlock FromSpans(
            int row,
            int col,
            int rowspan,
            int colspan
            )
        {
            var instance = new TileBlock();
            instance.StartRow = row;
            instance.StartCol = col;
            instance.EndRow = row + rowspan - 1;
            instance.EndCol = col + colspan - 1;
            return instance;
        }

    }
}
