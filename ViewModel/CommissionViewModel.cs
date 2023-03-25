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

namespace HydroApp
{
    class CommissionViewModel : ViewModel
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

        ObservableCollection<Commission> _commissions;
        public ObservableCollection<Commission> Commissions
        {
            get=> _commissions;
            set
            {
                _commissions = value;
                OnPropertyChanged(nameof(Commissions));
            }
        }

        ObservableCollection<Client> _clients;
        public ObservableCollection<Client> Clients
        {
            get=> _clients;
            set
            {
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }
        }

        ObservableCollection<CommissionDetail> _commissionDetails;
        public ObservableCollection<CommissionDetail> CommissionDetails
        {
            get => _commissionDetails;
            set
            {
                _commissionDetails = value;
                OnPropertyChanged(nameof(CommissionDetails));
            }
        }

        Commission _selectedItem;
        public Commission SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        CommissionDetail _selectedCommissionDetail;
        public CommissionDetail SelectedCommissionDetail
        {
            get=> _selectedCommissionDetail;
            set
            {
                _selectedCommissionDetail = value;
                OnPropertyChanged(nameof(SelectedCommissionDetail));
            }
        }


        public CommissionViewModel(HydropressDbContext context)
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
            
            Context.Constructions.Load(); //
            Context.Productions.Load(); //
            Context.Details.Load(); // Для отображения в MtMLb

            Context.Commissions.Load();
            Context.Clients.Load();
            Context.CommissionDetails.Load();

            CommissionDetails = Context.CommissionDetails.Local.ToObservableCollection();
            CommissionDetails.CollectionChanged += (x, e) => { OnPropertyChanged(nameof(CommissionDetails)); };

            Clients = Context.Clients.Local.ToObservableCollection();
            Clients.CollectionChanged += (x, e) => { OnPropertyChanged(nameof(Clients)); };

            Commissions = Context.Commissions.Local.ToObservableCollection();
            Commissions.CollectionChanged += (x, e) => { OnPropertyChanged(nameof(Commissions)); };
        }


        #region Commands

        RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ??= new RelayCommand(obj =>
                {
                    var _newCommission = new Commission();
                    Context.Commissions.Add(_newCommission);

                    SelectedIndex = Commissions.Count - 1;
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
                        Commissions = Context.Commissions.Local.ToObservableCollection();
                        Commissions.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Commissions));
                        //CommissionLb.Items.Refresh();
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
                        Commissions.Remove(SelectedItem);
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
