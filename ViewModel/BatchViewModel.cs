using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit.Primitives;

namespace HydroApp
{
    internal class BatchViewModel : ViewModel
    {
        public HydropressDbContext Context { get; set; }
        public BatchViewModel(HydropressDbContext context)
        {
            Context = context;
            Context.SaveChangesFailed += (x, e) =>
            {
                MessageBox.Show(e.Exception.Message, "Ошибка");
            };

        }
        int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        Batch _selectedItem;
        public Batch SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        //CommissionDetail _selectedCommissionDetail;
        //public CommissionDetail SelectedCommissionDetail
        //{
        //    get => _selectedCommissionDetail;
        //    set
        //    {
        //        _selectedCommissionDetail = value;
        //        OnPropertyChanged(nameof(SelectedCommissionDetail));
        //    }
        //}

        ObservableCollection<Detail> _details;
        public ObservableCollection<Detail> Details
        {
            get => _details;
            set
            {
                _details = value;
                OnPropertyChanged(nameof(Details));
            }
        }
        ObservableCollection<Foreman> _foremen;
        public ObservableCollection<Foreman> Foremen 
        {
            get => _foremen;
            set
            {
                _foremen = value;
                OnPropertyChanged(nameof(Foremen));
            }
        }
        ObservableCollection<Batch> _batches;
        public ObservableCollection<Batch> Batches 
        {
            get => _batches;
            set 
            {
                _batches = value;
                OnPropertyChanged(nameof(Batches));
            }
        }

        public void Init()
        {

            Context.Constructions.Load();
            Context.Details.Load();

            Context.Employees.Load();
            Context.Foremen.Load();

            Context.Workshops.Load();

            Context.Batches.Load();

            Details = Context.Details.Local.ToObservableCollection();
            Details.CollectionChanged += (x, e) => { OnPropertyChanged(nameof(Details)); };

            Foremen = Context.Foremen.Local.ToObservableCollection();
            Foremen.CollectionChanged += (x, e) => { OnPropertyChanged(nameof(Foremen)); };

            Batches = Context.Batches.Local.ToObservableCollection();
            Batches.CollectionChanged += (x, e) => { OnPropertyChanged(nameof(Batches)); };
        }

        #region Commands

        RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(obj =>
                {
                    var _newBatch = new Batch();
                    Context.Batches.Add(_newBatch);

                    SelectedIndex = Batches.Count - 1;
                });
            }
        }

        RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(obj =>
                {
                    int amountAdded;
                    string error = "";

                    try
                    {
                        amountAdded = Context.SaveChanges();
                        Batches = Context.Batches.Local.ToObservableCollection();
                        Batches.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Batches));
                    }
                    catch (Exception ex)
                    {
                        amountAdded = -1;
                        error = ex.Message;
                    }
                    MessageBox.Show($"Код: {(amountAdded == -1 ? $"-1\r\nПроизошла ошибка: {error}" : (amountAdded == 0 ? "0\r\nЗаписи не были добавлены" : $"1\r\nКоличество добавленных/изменённых записей: {amountAdded}"))}");
                });
            }
        }


        RelayCommand _removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??= new RelayCommand(obj =>
                {
                    // TODO: добавить возможность убрать окно предупреждения
                    // TODO: добавить ограничение только для админов
                    if (MessageBox.Show("Вы уверены, что хотите удалить выделенный элемент?", "") == MessageBoxResult.OK)
                    {
                        Batches.Remove(SelectedItem);
                    }
                });
            }
        }


        public RelayCommand _updateCommand;
        public RelayCommand UpdateCommand
        {
            get => _updateCommand ??= new RelayCommand(obj =>
            {
                Init();
            });
        }



        #endregion
    }
}
