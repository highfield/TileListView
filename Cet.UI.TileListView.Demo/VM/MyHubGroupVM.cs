using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cet.UI.TileListView.Demo
{
    public class MyHubGroupVM
        : ViewModelBase
    {
        public ObservableCollection<MyTileItemVM> Children { get; private set; } = new ObservableCollection<MyTileItemVM>();


        private string _title;
        public string Title
        {
            get { return this._title; }
            set { Set(ref this._title, value); }
        }


        private bool _isEditable;
        public bool IsEditable
        {
            get { return this._isEditable; }
            set { Set(ref this._isEditable, value); }
        }


        private int _rows;
        public int Rows
        {
            get { return this._rows; }
            set { Set(ref this._rows, value); }
        }


        private int _columns;
        public int Columns
        {
            get { return this._columns; }
            set { Set(ref this._columns, value); }
        }


        private double _blockSize;
        public double BlockSize
        {
            get { return this._blockSize; }
            set { Set(ref this._blockSize, value); }
        }


        private double _blockMargin;
        public double BlockMargin
        {
            get { return this._blockMargin; }
            set { Set(ref this._blockMargin, value); }
        }

    }
}
