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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Cet.UI.TileListView
{
    public class TileListViewItem
        : ListViewItem
    {
        private const double HotAreaThickness = 20;


        public TileListViewItem()
        {
            this.DefaultStyleKey = typeof(TileListViewItem);
            this.Unloaded += TileListViewItem_Unloaded;
        }


        private bool _isLoaded;
        private TileListView _parentListView;
        private Canvas _adornerOverlay;
        private Rectangle _sizingBorder;

        private uint? _pointerId = default(uint?);
        private TileItemHotArea _initialDragHotArea;
        //private TileItemHotArea _dynamicDragHotArea;
        private Point _origPos;
        private TileRect _origRect;
        private TileRect _currRect;

        private int _actualRowSpanMin;
        private int _actualRowSpanMax;
        private int _actualColSpanMin;
        private int _actualColSpanMax;


        #region PROP ContainingBlock

        private TileBlock _contaniningBlock;

        internal TileBlock ContainingBlock
        {
            get { return this._contaniningBlock; }
        }

        #endregion


        #region DP Column

        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register(
            "Column",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                0,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.ColumnChanged(args);
                }));


        public int Column
        {
            get { return (int)GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }


        private void ColumnChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP Row

        public static readonly DependencyProperty RowProperty = DependencyProperty.Register(
            "Row",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                0,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.RowChanged(args);
                }));


        public int Row
        {
            get { return (int)GetValue(RowProperty); }
            set { SetValue(RowProperty, value); }
        }


        private void RowChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP ColSpan

        public static readonly DependencyProperty ColSpanProperty = DependencyProperty.Register(
            "ColSpan",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                1,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.ColSpanChanged(args);
                }));


        public int ColSpan
        {
            get { return (int)GetValue(ColSpanProperty); }
            set { SetValue(ColSpanProperty, value); }
        }


        private void ColSpanChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP ColSpanMin

        public static readonly DependencyProperty ColSpanMinProperty = DependencyProperty.Register(
            "ColSpanMin",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                0,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.ColSpanMinChanged(args);
                }));


        public int ColSpanMin
        {
            get { return (int)GetValue(ColSpanMinProperty); }
            set { SetValue(ColSpanMinProperty, value); }
        }


        private void ColSpanMinChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateSpans();
        }

        #endregion


        #region DP ColSpanMax

        public static readonly DependencyProperty ColSpanMaxProperty = DependencyProperty.Register(
            "ColSpanMax",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                0,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.ColSpanMaxChanged(args);
                }));


        public int ColSpanMax
        {
            get { return (int)GetValue(ColSpanMaxProperty); }
            set { SetValue(ColSpanMaxProperty, value); }
        }


        private void ColSpanMaxChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateSpans();
        }

        #endregion


        #region DP RowSpan

        public static readonly DependencyProperty RowSpanProperty = DependencyProperty.Register(
            "RowSpan",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                1,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.RowSpanChanged(args);
                }));


        public int RowSpan
        {
            get { return (int)GetValue(RowSpanProperty); }
            set { SetValue(RowSpanProperty, value); }
        }


        private void RowSpanChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateUI();
        }

        #endregion


        #region DP RowSpanMin

        public static readonly DependencyProperty RowSpanMinProperty = DependencyProperty.Register(
            "RowSpanMin",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                0,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.RowSpanMinChanged(args);
                }));


        public int RowSpanMin
        {
            get { return (int)GetValue(RowSpanMinProperty); }
            set { SetValue(RowSpanMinProperty, value); }
        }


        private void RowSpanMinChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateSpans();
        }

        #endregion


        #region DP RowSpanMax

        public static readonly DependencyProperty RowSpanMaxProperty = DependencyProperty.Register(
            "RowSpanMax",
            typeof(int),
            typeof(TileListViewItem),
            new PropertyMetadata(
                0,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.RowSpanMaxChanged(args);
                }));


        public int RowSpanMax
        {
            get { return (int)GetValue(RowSpanMaxProperty); }
            set { SetValue(RowSpanMaxProperty, value); }
        }


        private void RowSpanMaxChanged(DependencyPropertyChangedEventArgs args)
        {
            this.UpdateSpans();
        }

        #endregion


        #region DPA ColumnBindingPath

        public static readonly DependencyProperty ColumnBindingPathProperty = DependencyProperty.RegisterAttached(
            "ColumnBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetColumnBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnBindingPathProperty);
        }

        public static void SetColumnBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnBindingPathProperty, value);
        }

        #endregion


        #region DPA RowBindingPath

        public static readonly DependencyProperty RowBindingPathProperty = DependencyProperty.RegisterAttached(
            "RowBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetRowBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(RowBindingPathProperty);
        }

        public static void SetRowBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(RowBindingPathProperty, value);
        }

        #endregion


        #region DPA ColSpanBindingPath

        public static readonly DependencyProperty ColSpanBindingPathProperty = DependencyProperty.RegisterAttached(
            "ColSpanBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetColSpanBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(ColSpanBindingPathProperty);
        }

        public static void SetColSpanBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(ColSpanBindingPathProperty, value);
        }

        #endregion


        #region DPA ColSpanMinBindingPath

        public static readonly DependencyProperty ColSpanMinBindingPathProperty = DependencyProperty.RegisterAttached(
            "ColSpanMinBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetColSpanMinBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(ColSpanMinBindingPathProperty);
        }

        public static void SetColSpanMinBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(ColSpanMinBindingPathProperty, value);
        }

        #endregion


        #region DPA ColSpanMaxBindingPath

        public static readonly DependencyProperty ColSpanMaxBindingPathProperty = DependencyProperty.RegisterAttached(
            "ColSpanMaxBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetColSpanMaxBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(ColSpanMaxBindingPathProperty);
        }

        public static void SetColSpanMaxBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(ColSpanMaxBindingPathProperty, value);
        }

        #endregion


        #region DPA RowSpanBindingPath

        public static readonly DependencyProperty RowSpanBindingPathProperty = DependencyProperty.RegisterAttached(
            "RowSpanBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetRowSpanBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(RowSpanBindingPathProperty);
        }

        public static void SetRowSpanBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(RowSpanBindingPathProperty, value);
        }

        #endregion


        #region DPA RowSpanMinBindingPath

        public static readonly DependencyProperty RowSpanMinBindingPathProperty = DependencyProperty.RegisterAttached(
            "RowSpanMinBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetRowSpanMinBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(RowSpanMinBindingPathProperty);
        }

        public static void SetRowSpanMinBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(RowSpanMinBindingPathProperty, value);
        }

        #endregion


        #region DPA RowSpanMaxBindingPath

        public static readonly DependencyProperty RowSpanMaxBindingPathProperty = DependencyProperty.RegisterAttached(
            "RowSpanMaxBindingPath",
            typeof(string),
            typeof(TileListViewItem),
            new PropertyMetadata(
                null,
                BindingPathPropertyChanged
                ));

        public static string GetRowSpanMaxBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(RowSpanMaxBindingPathProperty);
        }

        public static void SetRowSpanMaxBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(RowSpanMaxBindingPathProperty, value);
        }

        #endregion


        #region DP IsEditable

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(TileListViewItem),
            new PropertyMetadata(
                false,
                (obj, args) =>
                {
                    var ctl = (TileListViewItem)obj;
                    ctl.IsEditableChanged(args);
                }));


        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            internal set { SetValue(IsEditableProperty, value); }
        }


        private void IsEditableChanged(DependencyPropertyChangedEventArgs args)
        {
            //this.UpdateUI();
        }

        #endregion


        private static void BindingPathPropertyChanged(
            DependencyObject obj,
            DependencyPropertyChangedEventArgs e
            )
        {
            var propertyPath = e.NewValue as string;
            if (propertyPath != null)
            {
                DependencyProperty dp;
                if (e.Property == ColumnBindingPathProperty)
                {
                    dp = ColumnProperty;
                }
                else if (e.Property == RowBindingPathProperty)
                {
                    dp = RowProperty;
                }
                else if (e.Property == ColSpanBindingPathProperty)
                {
                    dp = ColSpanProperty;
                }
                else if (e.Property == ColSpanMinBindingPathProperty)
                {
                    dp = ColSpanMinProperty;
                }
                else if (e.Property == ColSpanMaxBindingPathProperty)
                {
                    dp = ColSpanMaxProperty;
                }
                else if (e.Property == RowSpanBindingPathProperty)
                {
                    dp = RowSpanProperty;
                }
                else if (e.Property == RowSpanMinBindingPathProperty)
                {
                    dp = RowSpanMinProperty;
                }
                else if (e.Property == RowSpanMaxBindingPathProperty)
                {
                    dp = RowSpanMaxProperty;
                }
                else
                {
                    throw new NotSupportedException();
                }

                if (propertyPath.StartsWith("="))
                {
                    //two-way binding
                    BindingOperations.SetBinding(
                        obj,
                        dp,
                        new Binding
                        {
                            Path = new PropertyPath(propertyPath.Substring(1)),
                            Mode = BindingMode.TwoWay,
                        });
                }
                else
                {
                    //one-way binding
                    BindingOperations.SetBinding(
                        obj,
                        dp,
                        new Binding
                        {
                            Path = new PropertyPath(propertyPath)
                        });
                }
            }
        }


        /// <summary>
        /// Update an easier way to get the actual cell spans
        /// </summary>
        private void UpdateSpans()
        {
            this._actualRowSpanMin = this.RowSpanMin > 0 ? this.RowSpanMin : 1;
            this._actualRowSpanMax = this.RowSpanMax > 0 ? this.RowSpanMax : 10000000;
            this._actualColSpanMin = this.ColSpanMin > 0 ? this.ColSpanMin : 1;
            this._actualColSpanMax = this.ColSpanMax > 0 ? this.ColSpanMax : 10000000;
        }


        /// <summary>
        /// Main update of the control appearance
        /// </summary>
        internal void UpdateUI()
        {
            if (this._pointerId.HasValue ||
                this._isLoaded == false
                )
            {
                return;
            }

            this._parentListView = this._parentListView ?? ItemsControl.ItemsControlFromItemContainer(this) as TileListView;
            if (this._parentListView != null)
            {
                //update the tile item position/size upon its properties
                this._contaniningBlock = TileBlock.FromSpans(
                    this.Row,
                    this.Column,
                    this.RowSpan,
                    this.ColSpan
                    );

                this.UpdateFromRect(
                    this._parentListView.GetRectFromBlock(this._contaniningBlock)
                    );

                //notify the parent map about the covered cells
                this._parentListView.UpdateMap(this, true);
            }
        }


        /// <summary>
        /// Update the position/size of the control from a tile-rect instance
        /// </summary>
        /// <param name="rect"></param>
        internal void UpdateFromRect(
            TileRect rect
            )
        {
            Canvas.SetLeft(this, rect.Left);
            Canvas.SetTop(this, rect.Top);
            this.Width = rect.Width;
            this.Height = rect.Height;
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //prepare the control for the very first usage
            this.UpdateSpans();
            this._adornerOverlay = this.GetTemplateChild("AdornerOverlay") as Canvas;
            this._adornerOverlay.PointerPressed += _adornerOverlay_PointerPressed;
            this._adornerOverlay.PointerMoved += _adornerOverlay_PointerMoved;
            this._adornerOverlay.PointerReleased += _adornerOverlay_PointerReleased;
            this._adornerOverlay.PointerExited += _adornerOverlay_PointerExited;

            this._isLoaded = true;
            this.UpdateUI();
        }


        private void TileListViewItem_Unloaded(object sender, RoutedEventArgs e)
        {
            //mark the control disposal
            this._isLoaded = false;
            this._parentListView.UpdateMap(this, false);
        }


        /// <summary>
        /// Pointer pressed: begins some user interaction (move/resize)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _adornerOverlay_PointerPressed(
            object sender,
            PointerRoutedEventArgs e
            )
        {
            //skip if no resize action is requested
            if (this._parentListView == null ||
                this._initialDragHotArea == TileItemHotArea.None
                )
            {
                //flush the adorner layer
                this.UpdateHandlerIcon(TileItemHotArea.None, new Point());
                return;
            }
            else if (this._initialDragHotArea == TileItemHotArea.Move)
            {
                //update the shared info about the item being dragged
                var info = new TileItemDragInfo()
                {
                    ViewItem = new WeakReference<TileListViewItem>(this),
                    Offset = e.GetCurrentPoint(this).Position,
                };

                this._parentListView.SetDragInfo(info);

                //flush the adorner layer
                this.UpdateHandlerIcon(TileItemHotArea.None, new Point());
                return;
            }

            //let the transparent overlay (adorner) captures hereinafter the pointer events
            if (this._adornerOverlay.CapturePointer(e.Pointer))
            {
                //create the tile sizing helper
                //store initial status
                this._pointerId = e.Pointer.PointerId;
                this._origPos = e.GetCurrentPoint(null).Position;
                this._origRect = TileRect.FromRect(
                    Canvas.GetLeft(this),
                    Canvas.GetTop(this),
                    this.Width,
                    this.Height
                    );

                this._currRect = this._origRect.Clone();

                //detect which is the hot-area covered by the pointer
                var pt = e.GetCurrentPoint(this._adornerOverlay).Position;
                this._initialDragHotArea = this.GetHotAreaAtPointer(pt);

                //show the related icon
                this.UpdateHandlerIcon(
                    this._initialDragHotArea,
                    pt
                    );

                //update the sizing border visual
                this.UpdateSizingBorder(
                    this._currRect,
                    this._currRect
                    );
            }
        }


        /// <summary>
        /// Pointer moved: the move/resize interaction is in progress.
        /// Also used as hovering manager for the control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _adornerOverlay_PointerMoved(
            object sender,
            PointerRoutedEventArgs e
            )
        {
            if (this._parentListView == null)
            {
                return;
            }

            if (this._pointerId.HasValue)
            {
                //some resize action in progress
                TileBlock tblock = this.HandleResizing(e);
                TileRect trect = this._parentListView.GetRectFromBlock(tblock);

                //update the adorner layer
                this.UpdateSizingBorder(
                    this._currRect,
                    trect
                    );

                this.UpdateFromRect(trect);

                //detect where the pointer is, and update the "handler" icon
                var pt = e.GetCurrentPoint(this._adornerOverlay).Position;
                this.UpdateHandlerIcon(this._initialDragHotArea, pt);
            }
            else
            {
                //just normal hovering
                //detect which is the hot-area covered by the pointer
                var pt = e.GetCurrentPoint(this._adornerOverlay).Position;
                this._initialDragHotArea = this.GetHotAreaAtPointer(pt);

                //show the related icon
                this.UpdateHandlerIcon(
                    this._initialDragHotArea,
                    pt
                    );

                //set the tile as draggable when the pointer falls inside
                this.CanDrag = this._initialDragHotArea == TileItemHotArea.Move;
            }
        }


        /// <summary>
        /// Pointer released: the move/resize interaction ends
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _adornerOverlay_PointerReleased(
            object sender,
            PointerRoutedEventArgs e
            )
        {
            if (this._parentListView == null)
            {
                return;
            }

            if (this._pointerId.HasValue)
            {
                //some resize action in progress
                TileBlock tblock = this.HandleResizing(e);

                //commit the new values
                this.Row = tblock.StartRow;
                this.Column = tblock.StartCol;
                this.RowSpan = tblock.RowSpan;
                this.ColSpan = tblock.ColSpan;
            }

            //flush the adorner layer
            this.UpdateSizingBorder(null, null);
            this.UpdateHandlerIcon(TileItemHotArea.None, new Point());

            //reset the resizing session
            this._pointerId = default(uint?);
            this._adornerOverlay.ReleasePointerCaptures();

            this.UpdateUI();
        }


        /// <summary>
        /// The pointer exited from the control area: no feasible interaction could take place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _adornerOverlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (this._pointerId.HasValue == false)
            {
                //hide/remove the visual "handler" icon
                this.UpdateHandlerIcon(TileItemHotArea.None, new Point());
            }
        }


        /// <summary>
        /// Handle the edges resizing interaction upon the pointer position
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private TileBlock HandleResizing(
            PointerRoutedEventArgs e
            )
        {
            //calculate the delta from the initial (original) pointer position
            Point newpos = e.GetCurrentPoint(null).Position;
            double dx = newpos.X - this._origPos.X;
            double dy = newpos.Y - this._origPos.Y;

            //apply the change to the proper edge(s)
            if (this._initialDragHotArea.HasFlag(TileItemHotArea.Left))
            {
                this.MoveLeftEdge(dx);
            }

            if (this._initialDragHotArea.HasFlag(TileItemHotArea.Top))
            {
                this.MoveTopEdge(dy);
            }

            if (this._initialDragHotArea.HasFlag(TileItemHotArea.Right))
            {
                this.MoveRightEdge(dx);
            }

            if (this._initialDragHotArea.HasFlag(TileItemHotArea.Bottom))
            {
                this.MoveBottomEdge(dy);
            }

            //update the highlight selection rectangle
            TileBlock resultingBlock = this._parentListView.GetBlockFromRect(this._currRect);
            this._parentListView.BlockHighlight = resultingBlock;

            return resultingBlock;
        }


        /// <summary>
        /// Handler for resizing the left edge
        /// </summary>
        /// <param name="dx"></param>
        private void MoveLeftEdge(
            double dx
            )
        {
            TileBlock currentBlock = this._parentListView.GetBlockFromRect(this._currRect);

            int startIndex = this._parentListView.MeasureNeighbor(
                this,
                currentBlock,
                TileItemHotArea.Left
                );

            startIndex = Math.Max(startIndex, currentBlock.EndCol - this._actualColSpanMax + 1);
            int endIndex = currentBlock.EndCol - this._actualColSpanMin + 1;

            this._currRect.SetLeft(
                this._origRect.Left + dx,
                this._parentListView.GetMovingRangeLT(startIndex, endIndex)
                );
        }


        /// <summary>
        /// Handler for resizing the right edge
        /// </summary>
        /// <param name="dx"></param>
        private void MoveRightEdge(
            double dx
            )
        {
            TileBlock currentBlock = this._parentListView.GetBlockFromRect(this._currRect);

            int endIndex = this._parentListView.MeasureNeighbor(
                this,
                currentBlock,
                TileItemHotArea.Right
                );

            endIndex = Math.Min(endIndex, currentBlock.StartCol + this._actualColSpanMax - 1);
            int startIndex = currentBlock.StartCol + this._actualColSpanMin - 1;

            this._currRect.SetRight(
                this._origRect.Right + dx,
                this._parentListView.GetMovingRangeRB(startIndex, endIndex)
                );
        }


        /// <summary>
        /// Handler for resizing the top edge
        /// </summary>
        /// <param name="dy"></param>
        private void MoveTopEdge(
            double dy
            )
        {
            TileBlock currentBlock = this._parentListView.GetBlockFromRect(this._currRect);

            int startIndex = this._parentListView.MeasureNeighbor(
                this,
                currentBlock,
                TileItemHotArea.Top
                );

            startIndex = Math.Max(startIndex, currentBlock.EndRow - this._actualRowSpanMax + 1);
            int endIndex = currentBlock.EndRow - this._actualRowSpanMin + 1;

            this._currRect.SetTop(
                this._origRect.Top + dy,
                this._parentListView.GetMovingRangeLT(startIndex, endIndex)
                );
        }


        /// <summary>
        /// Handler for resizing the bottom edge
        /// </summary>
        /// <param name="dy"></param>
        private void MoveBottomEdge(
            double dy
            )
        {
            TileBlock currentBlock = this._parentListView.GetBlockFromRect(this._currRect);

            int endIndex = this._parentListView.MeasureNeighbor(
                this,
                currentBlock,
                TileItemHotArea.Bottom
                );

            endIndex = Math.Min(endIndex, currentBlock.StartRow + this._actualRowSpanMax - 1);
            int startIndex = currentBlock.StartRow + this._actualRowSpanMin - 1;

            this._currRect.SetBottom(
                this._origRect.Bottom + dy,
                this._parentListView.GetMovingRangeRB(startIndex, endIndex)
                );
        }


        /// <summary>
        /// Detect the kind of hot-area below the current pointer position
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private TileItemHotArea GetHotAreaAtPointer(
            Point pt
            )
        {
            TileItemHotArea result = TileItemHotArea.None;

            if (pt.X < 0 ||
                pt.X > this._adornerOverlay.ActualWidth ||
                pt.Y < 0 ||
                pt.Y > this._adornerOverlay.ActualHeight
                )
            {
                return result;
            }

            if (pt.X < HotAreaThickness)
            {
                result |= TileItemHotArea.Left;
            }
            else if (pt.X > this._adornerOverlay.ActualWidth - HotAreaThickness)
            {
                result |= TileItemHotArea.Right;
            }

            if (pt.Y < HotAreaThickness)
            {
                result |= TileItemHotArea.Top;
            }
            else if (pt.Y > this._adornerOverlay.ActualHeight - HotAreaThickness)
            {
                result |= TileItemHotArea.Bottom;
            }

            if (result != TileItemHotArea.None)
            {
                return result;
            }
            else
            {
                return TileItemHotArea.Move;
            }
        }

#if false
        private TileItemHotArea GetActualHotAreaValue(
            ITileBlock tblock,
            TileItemHotArea area,
            TileItemHotArea neighborInfo
            )
        {
            if (area == TileItemHotArea.None ||
                area == TileItemHotArea.Move
                )
            {
                return area;
            }

            //check left edge
            if (area.HasFlag(TileItemHotArea.Left))
            {
                bool canExpand =
                    this.CanExpandHorizontally(tblock) &&
                    neighborInfo.HasFlag(TileItemHotArea.Left);

                if (canExpand == false &&
                    this.CanShrinkHorizontally(tblock) == false
                    )
                {
                    area &= TileItemHotArea.Left;
                }
            }

            //check right edge
            if (area.HasFlag(TileItemHotArea.Right))
            {
                bool canExpand =
                    this.CanExpandHorizontally(tblock) &&
                    neighborInfo.HasFlag(TileItemHotArea.Right);

                if (canExpand == false &&
                    this.CanShrinkHorizontally(tblock) == false
                    )
                {
                    area &= TileItemHotArea.Right;
                }
            }

            //check top edge
            if (area.HasFlag(TileItemHotArea.Top))
            {
                bool canExpand =
                    this.CanExpandVertically(tblock) &&
                    neighborInfo.HasFlag(TileItemHotArea.Top);

                if (canExpand == false &&
                    this.CanShrinkVertically(tblock) == false
                    )
                {
                    area &= TileItemHotArea.Top;
                }
            }

            //check bottom edge
            if (area.HasFlag(TileItemHotArea.Bottom))
            {
                bool canExpand =
                    this.CanExpandVertically(tblock) &&
                    neighborInfo.HasFlag(TileItemHotArea.Bottom);

                if (canExpand == false &&
                    this.CanShrinkVertically(tblock) == false
                    )
                {
                    area &= TileItemHotArea.Bottom;
                }
            }

            return area;
        }


        private bool CanShrinkHorizontally(
            ITileBlock tblock
            )
        {
            return tblock.ColSpan > Math.Max(1, this.ColSpanMin);
        }


        private bool CanExpandHorizontally(
            ITileBlock tblock
            )
        {
            return
                this.ColSpanMax == 0 ||
                tblock.ColSpan < this.ColSpanMax;
        }


        private bool CanShrinkVertically(
            ITileBlock tblock
            )
        {
            return tblock.RowSpan > Math.Max(1, this.RowSpanMin);
        }


        private bool CanExpandVertically(
            ITileBlock tblock
            )
        {
            return
                this.RowSpanMax == 0 ||
                tblock.RowSpan < this.RowSpanMax;
        }
#endif

        /// <summary>
        /// Update the handler icon below the pointer
        /// </summary>
        /// <param name="area"></param>
        /// <param name="pt"></param>
        private void UpdateHandlerIcon(
            TileItemHotArea area,
            Point pt
            )
        {
            const string UrlPrefix = "ms-appx:///Cet.UI.TileListView/Assets/pointers/";

            Uri uri = null;
            Point center = pt;

            if (area == TileItemHotArea.Move)
            {
                //uri = new Uri(this.BaseUri, "/Assets/pointers/move-64-000000.png");
                uri = new Uri(UrlPrefix + "move-64-000000.png");
            }
            else if (
                area == TileItemHotArea.Left ||
                area == TileItemHotArea.Right
                )
            {
                uri = new Uri(UrlPrefix + "resize-horizontal-64-000000.png");

            }
            else if (
                area == TileItemHotArea.Top ||
                area == TileItemHotArea.Bottom
                )
            {
                uri = new Uri(UrlPrefix + "resize-vertical-64-000000.png");

            }
            else if (
                area == TileItemHotArea.TopLeft ||
                area == TileItemHotArea.BottomRight
                )
            {
                uri = new Uri(UrlPrefix + "resize-tl-br-64-000000.png");

            }
            else if (
                area == TileItemHotArea.TopRight ||
                area == TileItemHotArea.BottomLeft
                )
            {
                uri = new Uri(UrlPrefix + "resize-tr-bl-64-000000.png");

            }
            else
            {
                //do nothing
            }

            //look for the image instance
            var img = this._adornerOverlay
                .Children
                .OfType<Image>()
                .FirstOrDefault();

            if (uri != null)
            {
                //create a brand new image if not existent yet
                if (img == null)
                {
                    img = new Image();
                    img.Width = img.Height = 24;
                    this._adornerOverlay.Children.Add(img);
                }

                //update appearance and position
                img.Source = new BitmapImage(uri);
                Canvas.SetLeft(img, center.X - 12);
                Canvas.SetTop(img, center.Y - 12);
            }
            else if (img != null)
            {
                //remove the image
                this._adornerOverlay.Children.Remove(img);
            }
        }


        /// <summary>
        /// Update the sizing border visual
        /// </summary>
        /// <param name="currRect"></param>
        /// <param name="blockRect"></param>
        private void UpdateSizingBorder(
            TileRect currRect,
            TileRect blockRect
            )
        {
            if (currRect == null ||
                this._parentListView == null
                )
            {
                if (this._sizingBorder != null)
                {
                    //remove the border
                    this._adornerOverlay.Children.Remove(this._sizingBorder);
                    this._sizingBorder = null;
                }
            }
            else
            {
                //create a brand new shape if not existent yet
                if (this._sizingBorder == null)
                {
                    this._sizingBorder = new Rectangle();
                    this._sizingBorder.Stroke = this._parentListView.GridLineBrush;
                    this._sizingBorder.StrokeThickness = 4;
                    this._adornerOverlay.Children.Add(this._sizingBorder);
                }

                //update position and size of the border
                Canvas.SetLeft(this._sizingBorder, currRect.Left - blockRect.Left);
                Canvas.SetTop(this._sizingBorder, currRect.Top - blockRect.Top);
                this._sizingBorder.Width = currRect.Width;
                this._sizingBorder.Height = currRect.Height;
            }
        }

    }
}
