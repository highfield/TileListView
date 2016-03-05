using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cet.UI.TileListView.Demo
{
    public class MySidebarItemVM
        : ViewModelBase
    {
        public string TypeId { get; set; }


        private MySidebarCategoryVM _owner;
        public MySidebarCategoryVM Owner
        {
            get { return this._owner; }
            set { this._owner = value; }
        }


        private string _title;
        public string Title
        {
            get { return this._title; }
            set { Set(ref this._title, value); }
        }


        private int _rowspan = 1;
        public int RowSpan
        {
            get { return this._rowspan; }
            set { Set(ref this._rowspan, value); }
        }


        private int _rowspanMin;
        public int RowSpanMin
        {
            get { return this._rowspanMin; }
            set { Set(ref this._rowspanMin, value); }
        }


        private int _rowspanMax;
        public int RowSpanMax
        {
            get { return this._rowspanMax; }
            set { Set(ref this._rowspanMax, value); }
        }


        private int _colspan = 1;
        public int ColSpan
        {
            get { return this._colspan; }
            set { Set(ref this._colspan, value); }
        }


        private int _colspanMin;
        public int ColSpanMin
        {
            get { return this._colspanMin; }
            set { Set(ref this._colspanMin, value); }
        }


        private int _colspanMax;
        public int ColSpanMax
        {
            get { return this._colspanMax; }
            set { Set(ref this._colspanMax, value); }
        }

    }
}
