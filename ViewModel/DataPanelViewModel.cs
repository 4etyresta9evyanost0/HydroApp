using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroApp
{
    internal class DataPanelViewModel : ViewModel
    {
        public ObservableCollection<object> Collection;
        public int SelectedValue { get; set; }

        // Зеркальность
        public int Min = 0;

        public int _max;
        public int Max
        {
            get => _max;
            set
            {
                _max = value;
                OnPropertyChanged(nameof(Max));
            }
        }

        public DataPanelViewModel() { }

    }
}
