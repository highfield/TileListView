/*
 * Copyright 2016 by Mario Vernari, Cet Electronics
 *
 * Licensed under the MIT License
 */
#define DUBUGMAP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Cet.UI.TileListView
{
    public class TiledCanvas
        : Canvas
    {
#if DEBUGMAP
        public TiledCanvas()
        {
            this.Loaded += TiledCanvas_Loaded;
        }

        private void TiledCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            var tlv = Cet.UI.VisualTreeHelperX.FindAncestorByType<TileListView>(this);
            tlv.DebugMap += Tlv_DebugMap;

        }
#endif

        private List<Line> _hlines = new List<Line>();
        private List<Line> _vlines = new List<Line>();
        private Rectangle _blockHilite;


        #region DP ColumnCount

        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register(
            "ColumnCount",
            typeof(int),
            typeof(TiledCanvas),
            new PropertyMetadata(
                1,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.ColumnCountChanged(args);
                }));


        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }


        private void ColumnCountChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP RowCount

        public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register(
            "RowCount",
            typeof(int),
            typeof(TiledCanvas),
            new PropertyMetadata(
                1,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.RowCountChanged(args);
                }));


        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }


        private void RowCountChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP ColumnWidth

        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.Register(
            "ColumnWidth",
            typeof(double),
            typeof(TiledCanvas),
            new PropertyMetadata(
                120.0,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.ColumnWidthChanged(args);
                }));


        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }


        private void ColumnWidthChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP RowHeight

        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register(
            "RowHeight",
            typeof(double),
            typeof(TiledCanvas),
            new PropertyMetadata(
                120.0,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.RowHeightChanged(args);
                }));


        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }


        private void RowHeightChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP BlockMargin

        public static readonly DependencyProperty BlockMarginProperty = DependencyProperty.Register(
            "BlockMargin",
            typeof(double),
            typeof(TiledCanvas),
            new PropertyMetadata(
                10.0,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.BlockMarginChanged(args);
                }));


        public double BlockMargin
        {
            get { return (double)GetValue(BlockMarginProperty); }
            set { SetValue(BlockMarginProperty, value); }
        }


        private void BlockMarginChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP GridLineThickness

        public static readonly DependencyProperty GridLineThicknessProperty = DependencyProperty.Register(
            "GridLineThickness",
            typeof(double),
            typeof(TiledCanvas),
            new PropertyMetadata(
                1.0,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.GridLineThicknessChanged(args);
                }));


        public double GridLineThickness
        {
            get { return (double)GetValue(GridLineThicknessProperty); }
            set { SetValue(GridLineThicknessProperty, value); }
        }


        private void GridLineThicknessChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP GridLineBrush

        public static readonly DependencyProperty GridLineBrushProperty = DependencyProperty.Register(
            "GridLineBrush",
            typeof(Brush),
            typeof(TiledCanvas),
            new PropertyMetadata(
                new SolidColorBrush(Colors.Gray),
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.GridLineBrushChanged(args);
                }));


        public Brush GridLineBrush
        {
            get { return (Brush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }


        private void GridLineBrushChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP ShowGridLines

        public static readonly DependencyProperty ShowGridLinesProperty = DependencyProperty.Register(
            "ShowGridLines",
            typeof(bool),
            typeof(TiledCanvas),
            new PropertyMetadata(
                false,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.ShowGridLinesChanged(args);
                }));


        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }


        private void ShowGridLinesChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP BlockHighlight

        public static readonly DependencyProperty BlockHighlightProperty = DependencyProperty.Register(
            "BlockHighlight",
            typeof(ITileBlock),
            typeof(TiledCanvas),
            new PropertyMetadata(
                null,
                (obj, args) =>
                {
                    var ctl = (TiledCanvas)obj;
                    ctl.BlockHighlightChanged(args);
                }));


        public ITileBlock BlockHighlight
        {
            get { return (ITileBlock)GetValue(BlockHighlightProperty); }
            set { SetValue(BlockHighlightProperty, value); }
        }


        private void BlockHighlightChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateBlockHighlight();
        }

        #endregion


        /// <summary>
        /// Refresh the visual appearance of the contro
        /// </summary>
        private void UpdateUI()
        {
            int rows = 0;
            int cols = 0;

            if (this.RowCount > 0 &&
                this.RowCount <= 100 &&
                this.ColumnCount > 0 &&
                this.ColumnCount <= 100 &&
                this.RowHeight > 0 &&
                this.RowHeight <= 1000 &&
                this.ColumnWidth > 0 &&
                this.ColumnWidth <= 1000
                )
            {
                rows = this.RowCount;
                cols = this.ColumnCount;
            }

            //calculate the actual block size
            var grossWidth = Math.Max(this.ColumnWidth + 2 * this.BlockMargin, 0);
            var grossHeight = Math.Max(this.RowHeight + 2 * this.BlockMargin, 0);

            this.Width = grossWidth * cols;
            this.Height = grossHeight * rows;

            if (rows > 0 &&
                cols > 0 &&
                this.ShowGridLines
                )
            {
                //adjust the number of lines on the canvas
                if (this._hlines.Count != this.RowCount + 1)
                {
                    for (int i = this._hlines.Count - 1; i > this.RowCount; i--)
                    {
                        this.Children.Remove(this._hlines[i]);
                        this._hlines.RemoveAt(i);
                    }
                    while (this._hlines.Count < this.RowCount + 1)
                    {
                        var line = new Line();
                        this._hlines.Add(line);
                        this.Children.Add(line);
                    }
                }

                if (this._vlines.Count != this.ColumnCount + 1)
                {
                    for (int i = this._vlines.Count - 1; i > this.ColumnCount; i--)
                    {
                        this.Children.Remove(this._vlines[i]);
                        this._vlines.RemoveAt(i);
                    }
                    while (this._vlines.Count < this.ColumnCount + 1)
                    {
                        var line = new Line();
                        this._vlines.Add(line);
                        this.Children.Add(line);
                    }
                }

                //horizontal lines
                for (int i = 0; i < this._hlines.Count; i++)
                {
                    var line = this._hlines[i];
                    line.Stroke = this.GridLineBrush;
                    line.StrokeThickness = this.GridLineThickness;
                    line.X1 = 0;
                    line.X2 = this.Width;
                    line.Y1 = line.Y2 = i * grossHeight;
                }

                //vertical lines
                for (int i = 0; i < this._vlines.Count; i++)
                {
                    var line = this._vlines[i];
                    line.Stroke = this.GridLineBrush;
                    line.StrokeThickness = this.GridLineThickness;
                    line.X1 = line.X2 = i * grossWidth;
                    line.Y1 = 0;
                    line.Y2 = this.Height;
                }
            }
            else
            {
                //remove any lines
                while (this._hlines.Count > 0)
                {
                    this.Children.Remove(this._hlines[0]);
                    this._hlines.RemoveAt(0);
                }
                while (this._vlines.Count > 0)
                {
                    this.Children.Remove(this._vlines[0]);
                    this._vlines.RemoveAt(0);
                }
            }

            //refresh the highlight rectangle, if applies
            this.UpdateBlockHighlight();

#if DEBUGMAP
            if (this._tilemap != null &&
                rows > 0 && 
                cols > 0
                )
            {
                if (this._hashmap == null)
                {
                    this._hashmap = new TextBlock[this.RowCount, this.ColumnCount];
                    for (int r = 0; r < this.RowCount; r++)
                    {
                        for (int c = 0; c < this.ColumnCount; c++)
                        {
                            var t = this._hashmap[r, c] = new TextBlock();
                            t.Foreground =new SolidColorBrush( Colors.Tomato);
                            Canvas.SetLeft(t, c * grossWidth);
                            Canvas.SetTop(t, r * grossHeight);
                            this.Children.Add(t);
                        }
                    }
                }
                for (int r = 0; r < this.RowCount; r++)
                {
                    for (int c = 0; c < this.ColumnCount; c++)
                    {
                        this._hashmap[r, c].Text = this._tilemap[r, c].ToString();
                    }
                }
            }
#endif
        }


        /// <summary>
        /// Refresh the highlight rectangle
        /// </summary>
        private void UpdateBlockHighlight()
        {
            if (this.Width > 0 &&
                this.Height > 0 &&
                this.ShowGridLines &&
                this.BlockHighlight != null &&
                this.BlockHighlight.IsValid
                )
            {
                //calculate the actual block size
                var grossWidth = Math.Max(this.ColumnWidth + 2 * this.BlockMargin, 0);
                var grossHeight = Math.Max(this.RowHeight + 2 * this.BlockMargin, 0);

                var innerMargin = this.BlockMargin * 0.5;

                if (this._blockHilite == null)
                {
                    //create a new rectangle
                    var dc = new DoubleCollection();
                    dc.Add(3.0);
                    dc.Add(3.0);

                    this._blockHilite = new Rectangle();
                    this._blockHilite.Stroke = this.GridLineBrush;
                    this._blockHilite.StrokeThickness = this.GridLineThickness * 3;
                    this._blockHilite.StrokeDashArray = dc;
                    this.Children.Add(this._blockHilite);
                }

                //define position and size of the shape
                var colspan = this.BlockHighlight.EndCol - this.BlockHighlight.StartCol + 1;
                var rowspan = this.BlockHighlight.EndRow - this.BlockHighlight.StartRow + 1;

                Canvas.SetLeft(this._blockHilite, this.BlockHighlight.StartCol * grossWidth + innerMargin);
                Canvas.SetTop(this._blockHilite, this.BlockHighlight.StartRow * grossHeight + innerMargin);

                double width = colspan * grossWidth - 2 * innerMargin;
                double height = rowspan * grossHeight - 2 * innerMargin;

                this._blockHilite.Width = width;
                this._blockHilite.Height = height;
            }
            else if (this._blockHilite != null)
            {
                this.Children.Remove(this._blockHilite);
                this._blockHilite = null;
            }
        }


#if DEBUGMAP
        private void Tlv_DebugMap(object sender, DebugMapEventArgs e)
        {
            this._tilemap = e.Map;
            this.UpdateUI();
        }

        private int[,] _tilemap;
        private TextBlock[,] _hashmap;
#endif
    }
}
