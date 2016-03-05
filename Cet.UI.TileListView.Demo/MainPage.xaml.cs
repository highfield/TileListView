using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cet.UI.TileListView.Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        private List<MyHubGroupVM> _hubGroups = new List<MyHubGroupVM>();


        private ObservableCollection<MySidebarCategoryVM> _sidebarCategories = new ObservableCollection<MySidebarCategoryVM>();
        public IEnumerable<MySidebarCategoryVM> SidebarCategories { get { return this._sidebarCategories; } }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.FillSidebar();
            this.FillHub();
        }


        private void FillSidebar()
        {
            for (int r = 1; r <= 3; r++)
            {
                for (int c = 1; c <= 3; c++)
                {
                    var cat = new MySidebarCategoryVM();
                    cat.Title = $"{r}x{c} bricks";
                    this._sidebarCategories.Add(cat);

                    {
                        var item = new MySidebarItemVM();
                        item.Owner = cat;
                        item.TypeId = $"R{r}C{c}H=V=";
                        item.Title = "Fixed";
                        item.RowSpan = r;
                        item.ColSpan = c;
                        item.RowSpanMin = item.RowSpanMax = r;
                        item.ColSpanMin = item.ColSpanMax = c;
                        cat.Children.Add(item);
                    }
                    {
                        var item = new MySidebarItemVM();
                        item.Owner = cat;
                        item.TypeId = $"R{r}C{c}H-V=";
                        item.Title = "Shrink H; Fixed V";
                        item.RowSpan = r;
                        item.ColSpan = c;
                        item.RowSpanMin = item.RowSpanMax = r;
                        item.ColSpanMin = 0;
                        item.ColSpanMax = c;
                        cat.Children.Add(item);
                    }
                    {
                        var item = new MySidebarItemVM();
                        item.Owner = cat;
                        item.TypeId = $"R{r}C{c}H+V-";
                        item.Title = "Expand H; Shrink V";
                        item.RowSpan = r;
                        item.ColSpan = c;
                        item.RowSpanMin = 0;
                        item.RowSpanMax = r;
                        item.ColSpanMin = c;
                        item.ColSpanMax = 0;
                        cat.Children.Add(item);
                    }
                    {
                        var item = new MySidebarItemVM();
                        item.Owner = cat;
                        item.TypeId = $"R{r}C{c}H*V+";
                        item.Title = "Free H; Expand V";
                        item.RowSpan = r;
                        item.ColSpan = c;
                        item.RowSpanMin = r;
                        item.RowSpanMax = 0;
                        item.ColSpanMin = 0;
                        item.ColSpanMax = 0;
                        cat.Children.Add(item);
                    }

                }
            }
        }


        private void FillHub()
        {
            for (int i = 1; i <= 6; i++)
            {
                var hg = new MyHubGroupVM();
                hg.Title = $"Columns: {i}";
                hg.Rows = 5;
                hg.Columns = i;
                hg.BlockSize = this.SliderBlocksSize.Value;
                hg.BlockMargin = this.SliderBlocksMargin.Value;
                this._hubGroups.Add(hg);

                var hs = new HubSection();
                hs.Header = hg.Title;
                hs.DataContext = hg;
                hs.ContentTemplate = (DataTemplate)this.Resources["dtplHubSection"];
                this.Hub1.Sections.Add(hs);
            }
        }


        #region Sidebar list management

        private WeakReference<MySidebarItemVM> _lastHoveredItem;


        private void GridView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var id = e.Items
                .OfType<MySidebarItemVM>()
                .Select(_ => _.TypeId)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(id))
            {
                e.Cancel = true;
            }
            else
            {
                e.Data.SetData(StandardDataFormats.Text, id);
            }
        }


        private async void GridView_DragOver(object sender, DragEventArgs e)
        {
            var gv = sender as GridView;
            var pt = e.GetPosition(null);
            this._lastHoveredItem = null;

            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            //e.DragUIOverride.IsCaptionVisible = true;


            Action<MySidebarItemVM> sidebarHandler = si =>
            {
                //reorder the sidebar items within a certain category
                var list = VisualTreeHelper.FindElementsInHostCoordinates(pt, gv);
                var gvi = list.OfType<GridViewItem>().FirstOrDefault();
                if (gvi != null)
                {
                    var hoveredItem = gvi.Content as MySidebarItemVM;
                    if (hoveredItem != null &&
                        hoveredItem.Owner == si.Owner
                        )
                    {
                        e.AcceptedOperation = DataPackageOperation.Move;
                        e.DragUIOverride.IsContentVisible = true;
                        this._lastHoveredItem = new WeakReference<MySidebarItemVM>(hoveredItem);
                    }
                }
            };


            Action<MyTileItemVM> tileHandler = ti =>
            {
                //trash a tile
                e.AcceptedOperation = DataPackageOperation.Move;
            };


            string id = await e.DataView.GetTextAsync();
            this.Switcher(
                id,
                sidebarHandler,
                tileHandler
                );
        }


        private async void GridView_Drop(object sender, DragEventArgs e)
        {
            var gv = sender as GridView;
            var pt = e.GetPosition(null);


            Action<MySidebarItemVM> sidebarHandler = si =>
            {
                //reorder the sidebar items within a certain category
                MySidebarItemVM hoveredItem;
                if (this._lastHoveredItem != null &&
                    this._lastHoveredItem.TryGetTarget(out hoveredItem)
                    )
                {
                    var collection = si.Owner.Children;
                    collection.Remove(si);
                    int idx = collection.IndexOf(hoveredItem);
                    collection.Insert(idx, si);
                }

                this._lastHoveredItem = null;
            };


            Action<MyTileItemVM> tileHandler = ti =>
            {
                //trash a tile
                ti.Owner.Children.Remove(ti);
                ti.Owner = null;
            };


            string id = await e.DataView.GetTextAsync();
            this.Switcher(
                id,
                sidebarHandler,
                tileHandler
                );
        }


        private void GridView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            //not used
        }

        #endregion


        #region Hub area management

        private void TileListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var tlv = sender as TileListView;
            var item = e.Items
                .OfType<MyTileItemVM>()
                .FirstOrDefault();

            if (item != null)
            {
                e.Data.SetData(StandardDataFormats.Text, item.InstanceId);
            }
            else
            {
                e.Cancel = true;
            }
        }


        private async void TileListView_DragOver(object sender, DragEventArgs e)
        {
            var tlv = sender as TileListView;

            e.AcceptedOperation = DataPackageOperation.None;
            e.DragUIOverride.IsContentVisible = true;
            e.DragUIOverride.IsGlyphVisible = true;
            //e.DragUIOverride.IsCaptionVisible = true;


            Action<MySidebarItemVM> sidebarHandler = si =>
            {
                //dragging a new tile over the hub control
                if (tlv.CanDrop(si.RowSpan, si.ColSpan, e.GetPosition(tlv)) != null)
                {
                    e.AcceptedOperation = DataPackageOperation.Move;
                }
            };


            Action<MyTileItemVM> tileHandler = ti =>
            {
                //tile being dragged within the hub control
                if (tlv.CanDrop(ti.RowSpan, ti.ColSpan, e.GetPosition(tlv)) != null)
                {
                    e.AcceptedOperation = DataPackageOperation.Move;
                }
            };


            string id = await e.DataView.GetTextAsync();
            this.Switcher(
                id,
                sidebarHandler,
                tileHandler
                );
        }


        private async void TileListView_Drop(object sender, DragEventArgs e)
        {
            var tlv = sender as TileListView;
            var hg = tlv.DataContext as MyHubGroupVM;


            Action<MySidebarItemVM> sidebarHandler = si =>
            {
                //dragging a new tile over the hub control
                ITileBlock tblock = tlv.CanDrop(
                    si.RowSpan,
                    si.ColSpan,
                    e.GetPosition(tlv)
                    );

                if (tblock != null)
                {
                    //create a brand new tile-item VM instance
                    var ti = new MyTileItemVM();
                    ti.Owner = hg;
                    ti.TypeId = si.TypeId;
                    ti.InstanceId = Guid.NewGuid().ToString();
                    ti.Title = si.Title;
                    ti.Row = tblock.StartRow;
                    ti.RowSpan = si.RowSpan;
                    ti.RowSpanMin = si.RowSpanMin;
                    ti.RowSpanMax = si.RowSpanMax;
                    ti.Column = tblock.StartCol;
                    ti.ColSpan = si.ColSpan;
                    ti.ColSpanMin = si.ColSpanMin;
                    ti.ColSpanMax = si.ColSpanMax;
                    hg.Children.Add(ti);
                }
            };


            Action<MyTileItemVM> tileHandler = ti =>
            {
                //tile being dragged within the hub control
                ITileBlock tblock = tlv.CanDrop(
                    ti.RowSpan,
                    ti.ColSpan,
                    e.GetPosition(tlv)
                    );

                if (tblock != null)
                {
                    //update/move the tile to the new position
                    ti.Owner.Children.Remove(ti);
                    ti.Owner = hg;
                    ti.Row = tblock.StartRow;
                    ti.Column = tblock.StartCol;
                    hg.Children.Add(ti);
                }
            };


            string id = await e.DataView.GetTextAsync();
            this.Switcher(
                id,
                sidebarHandler,
                tileHandler
                );
        }


        private void TileListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            //not used
        }

        #endregion


        private void ToggleEdit_Click(object sender, RoutedEventArgs e)
        {
            foreach (var hg in this._hubGroups)
            {
                hg.IsEditable = this.ToggleEdit.IsChecked ?? false;
            }
        }


        private void SliderBlocksSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            foreach (var hg in this._hubGroups)
            {
                hg.BlockSize = e.NewValue;
            }
        }


        private void SliderBlocksMargin_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            foreach (var hg in this._hubGroups)
            {
                hg.BlockMargin = e.NewValue;
            }
        }


        private void Switcher(
            string id,
            Action<MySidebarItemVM> sidebarHandler,
            Action<MyTileItemVM> tileHandler
            )
        {
            MySidebarItemVM sidebarItem = this.SidebarCategories
                .SelectMany(_ => _.Children)
                .FirstOrDefault(_ => _.TypeId == id);

            if (sidebarItem != null)
            {
                sidebarHandler(sidebarItem);
                return;
            }

            MyTileItemVM tileItem = this._hubGroups
                .SelectMany(_ => _.Children)
                .FirstOrDefault(_ => _.InstanceId == id);

            if (tileItem != null)
            {
                tileHandler(tileItem);
                return;
            }
        }

    }
}
