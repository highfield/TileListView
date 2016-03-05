/*
 * Copyright 2016 by Mario Vernari, Cet Electronics
 *
 * Licensed under the MIT License
 */
using Cet.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Cet.UI.TileListView
{
    public class TileListView
        : ListView
    {
        public TileListView()
        {
            this.DefaultStyleKey = typeof(TileListView);
            this.DragItemsCompleted += TileListView_DragItemsCompleted;
        }


        private TileBlock _containingBlock;
        private int[,] _tilemap;
        private static TileItemDragInfo _sharedDragInfo;


        private void TileListView_DragItemsCompleted(
            ListViewBase sender,
            DragItemsCompletedEventArgs args
            )
        {
            _sharedDragInfo = null;
        }


        #region DP Columns

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(int),
            typeof(TileListView),
            new PropertyMetadata(
                1,
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.ColumnsChanged(args);
                }));


        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }


        private void ColumnsChanged(DependencyPropertyChangedEventArgs args)
        {
            this._tilemap = null;
            this.UpdateUI(true);
        }

        #endregion


        #region DP Rows

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            "Rows",
            typeof(int),
            typeof(TileListView),
            new PropertyMetadata(
                1,
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.RowsChanged(args);
                }));


        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }


        private void RowsChanged(DependencyPropertyChangedEventArgs args)
        {
            this._tilemap = null;
            this.UpdateUI(true);
        }

        #endregion


        #region DP BlockSize

        public static readonly DependencyProperty BlockSizeProperty = DependencyProperty.Register(
            "BlockSize",
            typeof(double),
            typeof(TileListView),
            new PropertyMetadata(
                120.0,
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.BlockSizeChanged(args);
                }));


        public double BlockSize
        {
            get { return (double)GetValue(BlockSizeProperty); }
            set { SetValue(BlockSizeProperty, value); }
        }


        private void BlockSizeChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI(true);
        }

        #endregion


        #region DP BlockMargin

        public static readonly DependencyProperty BlockMarginProperty = DependencyProperty.Register(
            "BlockMargin",
            typeof(double),
            typeof(TileListView),
            new PropertyMetadata(
                10.0,
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.BlockMarginChanged(args);
                }));


        public double BlockMargin
        {
            get { return (double)GetValue(BlockMarginProperty); }
            set { SetValue(BlockMarginProperty, value); }
        }


        private void BlockMarginChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI(true);
        }

        #endregion


        #region DP GridLineThickness

        public static readonly DependencyProperty GridLineThicknessProperty = DependencyProperty.Register(
            "GridLineThickness",
            typeof(double),
            typeof(TileListView),
            new PropertyMetadata(
                1.0,
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.GridLineThicknessChanged(args);
                }));


        public double GridLineThickness
        {
            get { return (double)GetValue(GridLineThicknessProperty); }
            set { SetValue(GridLineThicknessProperty, value); }
        }


        private void GridLineThicknessChanged(DependencyPropertyChangedEventArgs args)
        {
            //this.UpdateUI();
        }

        #endregion


        #region DP GridLineBrush

        public static readonly DependencyProperty GridLineBrushProperty = DependencyProperty.Register(
            "GridLineBrush",
            typeof(Brush),
            typeof(TileListView),
            new PropertyMetadata(
                new SolidColorBrush(Colors.Gray),
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.GridLineBrushChanged(args);
                }));


        public Brush GridLineBrush
        {
            get { return (Brush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }


        private void GridLineBrushChanged(DependencyPropertyChangedEventArgs args)
        {
            //this.UpdateUI();
        }

        #endregion


        #region DP IsEditable

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(TileListView),
            new PropertyMetadata(
                false,
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.IsEditableChanged(args);
                }));


        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }


        private void IsEditableChanged(DependencyPropertyChangedEventArgs args)
        {
            //this.UpdateUI();
            this.CanDragItems = this.AllowDrop = (bool)args.NewValue;
        }

        #endregion


        #region DP BlockHighlight

        public static readonly DependencyProperty BlockHighlightProperty = DependencyProperty.Register(
            "BlockHighlight",
            typeof(ITileBlock),
            typeof(TileListView),
            new PropertyMetadata(
                null,
                (obj, args) =>
                {
                    var ctl = (TileListView)obj;
                    ctl.BlockHighlightChanged(args);
                }));


        public ITileBlock BlockHighlight
        {
            get { return (ITileBlock)GetValue(BlockHighlightProperty); }
            internal set { SetValue(BlockHighlightProperty, value); }
        }


        private void BlockHighlightChanged(DependencyPropertyChangedEventArgs args)
        {
            //
        }

        #endregion


        /// <summary>
        /// Refresh the control appearance
        /// </summary>
        private void UpdateUI(
            bool updateChildren
            )
        {
            bool rebuild = this._tilemap == null;
            var size = this.BlockSize + 2 * this.BlockMargin;

            this.Width = this.Columns * size;
            this.Height = this.Rows * size;

            this._containingBlock = new TileBlock()
            {
                StartRow = 0,
                EndRow = this.Rows - 1,
                StartCol = 0,
                EndCol = this.Columns - 1,
            };

            if (rebuild)
            {
                if (this._containingBlock.IsValid)
                {
                    this._tilemap = new int[this.Rows, this.Columns];
                    this.RaiseDebugMapEvent();
                }
            }

            if (rebuild || updateChildren)
            {
                for (int i=0; i<this.Items.Count; i++)
                {
                    var tlvi = this.ContainerFromIndex(i) as TileListViewItem;
                    if (tlvi != null) tlvi.UpdateUI();

                }
            }
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.UpdateUI(false);
        }


        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TileListViewItem();
        }


        protected override void PrepareContainerForItemOverride(
            DependencyObject element,
            object item
            )
        {
            var tlvi = element as TileListViewItem;
            if (tlvi != null)
            {
                //link some properties to the child elements
                tlvi.SetBinding(
                    TileListViewItem.IsEditableProperty,
                    new Binding()
                    {
                        Path = new PropertyPath("IsEditable"),
                        Source = this,
                    });
            }

            base.PrepareContainerForItemOverride(element, item);
        }


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TileListViewItem;
        }


        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            this.CancelDrop();
        }


        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            this.CancelDrop();
        }


        /// <summary>
        /// Update the map which stores the tile-item hashes
        /// </summary>
        /// <param name="item"></param>
        /// <param name="existent"></param>
        internal void UpdateMap(
            TileListViewItem item,
            bool existent
            )
        {
            if (this._tilemap == null) return;

            var hash = item.GetHashCode();
            TileBlock tblock = item.ContainingBlock;
            //System.Diagnostics.Debug.WriteLine($"lv={this.GetHashCode()}; count={this.Items.Count}; hash={hash}; exist={existent}");

            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Columns; c++)
                {
                    if (existent &&
                        tblock.Contains(r, c)
                        )
                    {
                        //mark the cell with the item's hash value
                        this._tilemap[r, c] = hash;
                    }
                    else if (this._tilemap[r, c] == hash)
                    {
                        //unmark (free) the cell
                        this._tilemap[r, c] = 0;
                    }
                }
            }

            //raise the map-debugging event
            this.RaiseDebugMapEvent();
        }


        /// <summary>
        /// Test a block if fits inside the map
        /// </summary>
        /// <param name="tblock"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool MayFitIntoMap(
            TileBlock tblock,
            TileListViewItem item
            )
        {
            if (this._tilemap == null ||
                this._containingBlock == null ||
                this._containingBlock.Contains(tblock) == false
                )
            {
                return false;
            }

            //allowed cells are:
            // - free cells
            // - same-hashed cells
            int hash = (item != null) ? item.GetHashCode() : 0;
            for (int r = 0; r < this.Rows; r++)
            {
                for (int c = 0; c < this.Columns; c++)
                {
                    if (tblock.Contains(r, c))
                    {
                        int cell = this._tilemap[r, c];
                        if (cell != 0 && cell != hash)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Reveal how much free space is available in order to enlarge an edge of the tile
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tblock"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        internal int MeasureNeighbor(
            TileListViewItem owner,
            TileBlock tblock,
            TileItemHotArea edge
            )
        {
            var hash = owner.GetHashCode();

            if (edge.HasFlag(TileItemHotArea.Left))
            {
                //check left edge
                int result = tblock.StartCol;
                for (int c = tblock.StartCol - 1; c >= 0; c--)
                {
                    for (int r = tblock.StartRow; r <= tblock.EndRow; r++)
                    {
                        int cell;
                        if ((cell = _tilemap[r, c]) != 0 && cell != hash)
                        {
                            return result;
                        }
                    }
                    result = c;
                }
                return result;
            }

            if (edge.HasFlag(TileItemHotArea.Right))
            {
                //check right edge
                int result = tblock.EndCol;
                for (int c = tblock.EndCol + 1; c < this.Columns; c++)
                {
                    for (int r = tblock.StartRow; r <= tblock.EndRow; r++)
                    {
                        int cell;
                        if ((cell = _tilemap[r, c]) != 0 && cell != hash)
                        {
                            return result;
                        }
                    }
                    result = c;
                }
                return result;
            }

            if (edge.HasFlag(TileItemHotArea.Top))
            {
                //check top edge
                int result = tblock.StartRow;
                for (int r = tblock.StartRow - 1; r >= 0; r--)
                {
                    for (int c = tblock.StartCol; c <= tblock.EndCol; c++)
                    {
                        int cell;
                        if ((cell = _tilemap[r, c]) != 0 && cell != hash)
                        {
                            return result;
                        }
                    }
                    result = r;
                }
                return result;
            }

            if (edge.HasFlag(TileItemHotArea.Bottom))
            {
                //check bottom edge
                int result = tblock.EndRow;
                for (int r = tblock.EndRow + 1; r < this.Rows; r++)
                {
                    for (int c = tblock.StartCol; c <= tblock.EndCol; c++)
                    {
                        int cell;
                        if ((cell = _tilemap[r, c]) != 0 && cell != hash)
                        {
                            return result;
                        }
                    }
                    result = r;
                }
                return result;
            }

            throw new NotSupportedException();
        }


        internal PointerMovingRange GetMovingRangeRB(
            int startIndex,
            int endIndex
            )
        {
            var grossSize = this.BlockSize + 2 * this.BlockMargin;

            var range = new PointerMovingRange();
            range.Min = (startIndex + 1) * grossSize - this.BlockMargin;
            range.Max = (endIndex + 1) * grossSize - this.BlockMargin;
            return range;
        }


        internal PointerMovingRange GetMovingRangeLT(
            int startIndex,
            int endIndex
            )
        {
            var grossSize = this.BlockSize + 2 * this.BlockMargin;

            var range = new PointerMovingRange();
            range.Min = startIndex * grossSize + this.BlockMargin;
            range.Max = endIndex * grossSize + this.BlockMargin;
            return range;
        }


        internal TileRect GetRectFromBlock(
            TileBlock tblock
            )
        {
            if (tblock.IsValid == false)
            {
                throw new ArgumentException();
            }

            var grossSize = this.BlockSize + 2 * this.BlockMargin;

            return new TileRect(
                tblock.StartCol * grossSize + this.BlockMargin,
                tblock.StartRow * grossSize + this.BlockMargin,
                (tblock.EndCol + 1) * grossSize - this.BlockMargin,
                (tblock.EndRow + 1) * grossSize - this.BlockMargin
                );
        }


        internal TileBlock GetBlockFromRect(
            TileRect rect
            )
        {
            //System.Diagnostics.Debug.WriteLine($"rect={rect}");
            var grossSize = this.BlockSize + 2 * this.BlockMargin;

            var left = rect.Left - this.BlockMargin;
            var top = rect.Top - this.BlockMargin;
            var right = rect.Right + this.BlockMargin;
            var bottom = rect.Bottom + this.BlockMargin;

            var tblock = new TileBlock();
            tblock.StartCol = (int)Math.Round(left / grossSize);
            tblock.StartRow = (int)Math.Round(top / grossSize);
            tblock.EndCol = Math.Max((int)Math.Round(right / grossSize) - 1, tblock.StartCol);
            tblock.EndRow = Math.Max((int)Math.Round(bottom / grossSize) - 1, tblock.StartRow);
            return tblock;
        }


        public ITileBlock CanDrop(
            int rowspan,
            int colspan,
            Point ptrpos
            )
        {
            if (rowspan < 1) rowspan = 1;
            if (colspan < 1) colspan = 1;

            var grossSize = this.BlockSize + 2 * this.BlockMargin;
            var left = ptrpos.X;
            var top = ptrpos.Y;

            TileListViewItem lvi;
            if (_sharedDragInfo != null &&
                _sharedDragInfo.ViewItem.TryGetTarget(out lvi)
                )
            {
                left -= _sharedDragInfo.Offset.X;
                top -= _sharedDragInfo.Offset.Y;
            }
            else
            {
                lvi = null;
                _sharedDragInfo = null;
                left -= colspan * grossSize * 0.5;
                top -= rowspan * grossSize * 0.5;
            }
            //System.Diagnostics.Debug.WriteLine($"l={left};t={top}");

            var tblock = TileBlock.FromSpans(
                (int)Math.Round(top / grossSize),
                (int)Math.Round(left / grossSize),
                rowspan,
                colspan
                );

            this.BlockHighlight = tblock;

            bool fit = this.MayFitIntoMap(
                tblock,
                lvi
                );

            return fit ? tblock : null;
        }


        internal void SetDragInfo(
            TileItemDragInfo info
            )
        {
            _sharedDragInfo = info;
        }


        public void CancelDrop()
        {
            this.BlockHighlight = null;
        }


        private void RaiseDebugMapEvent()
        {
            if (this.DebugMap != null)
            {
                this.DebugMap(this, new DebugMapEventArgs() { Map = _tilemap });
            }
        }

        internal event EventHandler<DebugMapEventArgs> DebugMap;
    }


    internal class DebugMapEventArgs : EventArgs
    {
        public int[,] Map;
    }
}
