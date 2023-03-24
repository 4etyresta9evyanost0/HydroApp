using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Primitives;

namespace HydroApp
{
    class SupplyViewModel : ViewModel
    {
        public HydropressDbContext Context { get; set; }
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


        ObservableCollection<Supply> _supplies;
        public ObservableCollection<Supply> Supplies
        {
            get => _supplies;
            set
            {
                _supplies = value;
                OnPropertyChanged(nameof(Supplies));
            }
        }

        Supply _selectedItem;
        public Supply SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        SupplyDetail _selectedSupplyDetail;
        public SupplyDetail SelectedSupplyDetail
        {
            get => _selectedSupplyDetail;
            set
            {
                _selectedSupplyDetail = value;
                OnPropertyChanged(nameof(SelectedSupplyDetail));
            }
        }

        public SupplyViewModel(HydropressDbContext context)
        {
            Context = context;
            Context.SaveChangesFailed += (x, e) =>
            {
                MessageBox.Show(e.Exception.Message, "Ошибка");
            };
            Context.SavedChanges += (x, e) =>
            {
                //OnPropertyChanged(nameof(Designers));
                //OnPropertyChanged(nameof(Details));
                //OnPropertyChanged(nameof(Products));
            };
        }

        public void Init()
        {
            Context.Suppliers.Load();
            Context.Supplies.Load();
            Context.SupplyDetails.Load();
        }


        #region Commands

        RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(obj =>
                {
                    var _newSupply = new Supply();
                    Context.Supplies.Add(_newSupply);

                    SelectedIndex = Supplies.Count - 1;
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
                        Supplies = Context.Supplies.Local.ToObservableCollection();
                        Supplies.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Supplies));
                        //SupplyLb.Items.Refresh();
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
                        var empl = SelectedItem;
                        // нужно ли это?
                        // foreach(var cd in SelectedItem.CommissionDetails)
                        //     Context.CommissionDetails.Remove(cd); // spooky (better without ".Local" or with it?)
                        Supplies.Remove(SelectedItem);
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
