using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cet.UI.TileListView.Demo
{
    public class MySidebarCategoryVM
        : ViewModelBase
    {
        public ObservableCollection<MySidebarItemVM> Children { get; private set; } = new ObservableCollection<MySidebarItemVM>();


        private string _title;
        public string Title
        {
            get { return this._title; }
            set { Set(ref this._title, value); }
        }

    }
}
