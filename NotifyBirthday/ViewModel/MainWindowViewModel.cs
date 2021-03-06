﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Threading;

namespace NotifyBirthday
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private int Frequency;

        private int Period;

        private SettingView settingView;

        private AddEmployeeView addEmployeeView;

        private NotifyBalloonView notifyBalloonView;

        private DetailEmployeeView detailEmployeeView;

        private SettingViewViewModel settingViewViewModel;

        private AddEmployeeViewViewModel addEmployeeViewViewModel;

        private NotifyBalloonViewViewModel notifyBalloonViewViewModel;

        private DetailEmployeeViewViewModel detailEmployeeViewViewModel;

        private bool flag = true;
        
        private Employee _selectedEmployee;

        private ObservableCollection<Employee> _employees;

        private CollectionViewSource _employeesView;

        private string _sortColumn;

        private ListSortDirection _sortDirection;

        private TimerCallback timerCallback;

        private Timer timer;

        private readonly NotifyService notifyService = new NotifyService();
        #endregion

        #region Properties
        public ObservableCollection<Employee> Employees
        {
            get { return _employees; }
            set
            {
                if (_employees != value)
                {
                    _employees = value;
                    _employeesView = new CollectionViewSource
                    {
                        Source = _employees
                    };
                    RaisePropertyChanged();
                }
            }
        }

        public ListCollectionView EmployeesView
        {
            get { return (ListCollectionView)_employeesView.View; }
            set
            {
                _employeesView.Source = value;
                RaisePropertyChanged();
            }
        }

        public Window window;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged("SelectFrequency");
                OpenDetailEmployeeView.RaiseCanExecuteChanged();
                RemoveEmployee.RaiseCanExecuteChanged();
            }
        }

        public Employee BallonEmployee;
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            AddNotifyService();
            RemoveEmployee = new RelayCommand(RemoveEmployee_Execute, RemoveEmployee_CanExecute);
            OpenSetting = new RelayCommand(OpenSetting_Execute);
            OpenAddEmployeeView = new RelayCommand(OpenAddEmployeeView_Execute);
            OpenDetailEmployeeView = new RelayCommand(OpenDetailEmployeeView_Execute, OpenDetailEmployeeView_CanExecute);
            ExportXml = new RelayCommand(ExportXml_Execute);
            CloseApp = new RelayCommand(CloseApp_Execute);
            ImportXml = new RelayCommand(ImportXml_Execute);
            SortListView = new CustomRelayCommand(SortListView_Execute);

            LoadConfig();
            if (Frequency > 0 && Period > 0)
            {
                timerCallback = new TimerCallback(CompareDate);
                timer = new Timer(timerCallback, null, 10000, Frequency * 3600000);
            }
            else
            {
                timerCallback = new TimerCallback(CompareDate);
                timer = new Timer(timerCallback, null, 10000, 3600000);
                Frequency = 1;
                Period = 1;
            }
        }
        #endregion

        #region Commands
        public RelayCommand OpenAddEmployeeView { get; private set; }

        private void OpenAddEmployeeView_Execute()
        {
            addEmployeeViewViewModel = new AddEmployeeViewViewModel
            {
                Employees = Employees
            };
            addEmployeeView = new AddEmployeeView
            {
                DataContext = addEmployeeViewViewModel
            };
            addEmployeeViewViewModel.Window = addEmployeeView;
            addEmployeeView.Closed += AddEmployeeView_Closed;
            addEmployeeView.ShowDialog();
        }

        private void AddEmployeeView_Closed(object sender, EventArgs e)
        {
            addEmployeeView = null;
        }

        public RelayCommand OpenDetailEmployeeView { get; private set; }

        private void OpenDetailEmployeeView_Execute()
        {
            detailEmployeeViewViewModel = new DetailEmployeeViewViewModel
            {
                SelectedEmployee = SelectedEmployee
            };
            detailEmployeeView = new DetailEmployeeView
            {
                DataContext = detailEmployeeViewViewModel
            };
            detailEmployeeViewViewModel.Window = detailEmployeeView;
            detailEmployeeViewViewModel.InputName = SelectedEmployee.Name;
            detailEmployeeViewViewModel.InputSurname = SelectedEmployee.Surname;
            detailEmployeeViewViewModel.InputMiddlename = SelectedEmployee.Middlename;
            detailEmployeeViewViewModel.InputDate = SelectedEmployee.Datebirthday;
            detailEmployeeView.Closed += DetailEmployeeView_Closed;
            detailEmployeeView.ShowDialog();
        }

        private void DetailEmployeeView_Closed(object sender, EventArgs e)
        {
            List<Employee> collection = new List<Employee>();
            foreach (var item in Employees)
            {
                collection.Add(item);
            }
            Employees.Clear();
            foreach (var item in collection)
            {
                Employees.Add(item);
            }
            detailEmployeeView = null;
        }

        public bool OpenDetailEmployeeView_CanExecute()
        {
            return SelectedEmployee != null;
        }

        public RelayCommand OpenSetting { get; private set; }

        public void OpenSetting_Execute()
        {
            settingViewViewModel = new SettingViewViewModel
            {
                Config = DataManager.Load<Config>("Config.xml")
            };
            if (settingViewViewModel.Config.Frequency != 0)
            {
                settingViewViewModel.Frequency = settingViewViewModel.Config.Frequency;
                settingViewViewModel.Period = settingViewViewModel.Config.Period;
                settingViewViewModel.prevFrequency = settingViewViewModel.Config.Frequency;
                settingViewViewModel.prevPeriod = settingViewViewModel.Config.Period;
            } 
            else
            {
                settingViewViewModel.Frequency = Frequency;
                settingViewViewModel.Period = Period;
                settingViewViewModel.prevFrequency = Frequency;
                settingViewViewModel.prevPeriod = Period;
            }
            settingViewViewModel.SetIndex();
            settingViewViewModel.SelectFrequency = new ComboBoxItem
            {
                Content = Frequency
            };
            settingView = new SettingView
            {
                DataContext = settingViewViewModel
            };
            settingViewViewModel.Window = settingView;
            settingView.ResizeMode = ResizeMode.NoResize;
            settingView.Closed += SettingView_Closed;
            settingView.ShowDialog();
        }

        private void SettingView_Closed(object sender, EventArgs e)
        {
            settingViewViewModel.Disponse();
            int prevFrequency = Frequency;
            int prevPeriod = Period;
            LoadConfig();
            if (Frequency > 0 && Period >= 0 && (prevFrequency != Frequency || prevPeriod != Period))
            {
                timer.Change(60000, Frequency * 3600000);
            }
            settingView = null;
        }

        public RelayCommand RemoveEmployee { get; private set; }

        public void RemoveEmployee_Execute()
        {
            for (int i = 0; i < Employees.Count; i++)
            {
                if (Employees[i] == SelectedEmployee)
                {
                    Employees.RemoveAt(i);
                }
            }
        }

        public bool RemoveEmployee_CanExecute()
        {
            return SelectedEmployee != null;
        }

        private void ExpendApp_Click(object sender, RoutedEventArgs e)
        {
            window.Show();
            window.WindowState = WindowState.Normal;
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            window.Visibility = Visibility.Hidden;
            window.Close();
        }

        public RelayCommand CloseApp { get; private set; }

        public void CloseApp_Execute()
        {
            window.Visibility = Visibility.Hidden;
            window.Close();
        }

        public RelayCommand ExportXml { get; set; }
        public void ExportXml_Execute()
        {
            DataManager.Export(Employees);
        }

        public RelayCommand ImportXml { get; set; }
        public void ImportXml_Execute()
        {
            ObservableCollection<Employee> collection = DataManager.Import<ObservableCollection<Employee>>();
            if (collection != null)
            {
                Employees = collection;
                RaisePropertyChanged("EmployeesView");
            }
        }
        #endregion

        #region Services
        private void AddNotifyService()
        {
            MenuItem item1 = new MenuItem()
            {
                Header = "Открыть"
            };
            item1.Click += ExpendApp_Click;
            MenuItem item2 = new MenuItem()
            {
                Header = "Закрыть"
            };
            item2.Click += CloseApp_Click;
            Separator separator = new Separator();
            notifyService.icon.ContextMenu.Items.Add(item1);
            notifyService.icon.ContextMenu.Items.Add(separator);
            notifyService.icon.ContextMenu.Items.Add(item2);
            notifyService.icon.TrayBalloonTipClicked += Icon_TrayBalloonTipClicked;
            notifyService.icon.LeftClickCommand = OpenApp_Click;
        }
        private ICommand _openApp;
        public ICommand OpenApp_Click
        {
            get
            {
                return _openApp ?? (_openApp = new RelayCommand(() =>
                {
                    window.Show();
                    window.WindowState = WindowState.Normal;
                }));
            }
        }

        private void Icon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            BallonEmployee.Flag = false;
        }

        public void Disponse()
        {
            DataManager.Save(Employees, "Employees.xml");
        }
        #endregion

        #region Other methods
        public void LoadConfig()
        {
            XmlDocument xmlDocument = new XmlDocument();
            if (File.Exists("Config.xml"))
            {
                xmlDocument.Load("Config.xml");
                XmlElement xmlElement = xmlDocument.DocumentElement;
                foreach (XmlNode xmlNode in xmlElement)
                {
                    if (xmlNode.Name == "Frequency") Frequency = Convert.ToInt32(xmlNode.InnerText);
                    if (xmlNode.Name == "Period") Period = Convert.ToInt32(xmlNode.InnerText);
                }
            }
        }

        public void CompareDate(object obj)
        {
            DateTime date;
            TimeSpan rdt;
            for (int i = 0; i < Employees.Count; i++)
            {
                while (!flag)
                {
                    Thread.Sleep(10000);
                }
                date = Convert.ToDateTime(Employees[i].Datebirthday.Day + "." + Employees[i].Datebirthday.Month + "." + DateTime.Now.Year);
                rdt = date - Convert.ToDateTime(DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year);
                int d = (int)rdt.TotalDays;
                if (Employees[i].Flag)
                {
                    if (d > 1 && d < Period)
                    {
                        if (Employees[i].Middlename != null || Employees[i].Middlename == " " || Employees[i].Middlename == "")
                        {
                            ShowNotifyBalloon(d + " дня до дня рождения " + Employees[i].Name + " " + Employees[i].Surname + " " + Employees[i].Middlename + ".", Employees[i]);
                            flag = false;
                        }
                        else
                        {
                            ShowNotifyBalloon(d + " дня до дня рождения " + Employees[i].Name + " " + Employees[i].Surname + ".", Employees[i]);
                            flag = false;
                        }
                    }
                    else if (d == 1 && d == Period)
                    {
                        if (Employees[i].Middlename != null || Employees[i].Middlename == " " || Employees[i].Middlename == "")
                        {
                            ShowNotifyBalloon("Завтра день рождения у " + Employees[i].Name + " " + Employees[i].Surname + " " + Employees[i].Middlename + ".", Employees[i]);
                            flag = false;
                        }
                        else
                        {
                            ShowNotifyBalloon("Завтра день рождения у " + Employees[i].Name + " " + Employees[i].Surname + ".", Employees[i]);
                            flag = false;
                        }
                    }
                    else if (d == 0)
                    {
                        if (Employees[i].Middlename != null || Employees[i].Middlename == " " || Employees[i].Middlename == "")
                        {
                            ShowNotifyBalloon("Сегодня день рождения у " + Employees[i].Name + " " + Employees[i].Surname + " " + Employees[i].Middlename + ".", Employees[i]);
                            flag = false;
                        }
                        else
                        {
                            ShowNotifyBalloon("Сегодня день рождения у " + Employees[i].Name + " " + Employees[i].Surname + ".", Employees[i]);
                            flag = false;
                        }
                    }
                } else if (d < 0 && Employees[i].Flag == false)
                {
                    Employees[i].Flag = true;
                }
            }
        }

        static object locker = new object();

        [STAThread]
        public void ShowNotifyBalloon(string notifyMessage, Employee employee)
        {
            window.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                notifyBalloonView = new NotifyBalloonView();
                notifyBalloonViewViewModel = new NotifyBalloonViewViewModel
                {
                    Window = notifyBalloonView,
                    NotifyText = notifyMessage,
                    employee = employee

                };
                double screenHeight = SystemParameters.FullPrimaryScreenHeight;
                double screenWidth = SystemParameters.FullPrimaryScreenWidth;
                notifyBalloonViewViewModel.Window.Top = screenHeight - notifyBalloonViewViewModel.Window.Height;
                notifyBalloonViewViewModel.Window.Left = screenWidth - notifyBalloonViewViewModel.Window.Width;
                notifyBalloonView.DataContext = notifyBalloonViewViewModel;
                notifyBalloonView.Closed += notifyBalloonView_Closed;
                notifyBalloonView.Show();
            }));
        }

        private void notifyBalloonView_Closed(object sender, EventArgs e)
        {
            for (int i = 0; i < Employees.Count; i++)
            {
                if (Employees[i].Name == notifyBalloonViewViewModel.employee.Name &&
                    Employees[i].Surname == notifyBalloonViewViewModel.employee.Surname &&
                    Employees[i].Middlename == notifyBalloonViewViewModel.employee.Middlename &&
                    Employees[i].Datebirthday == notifyBalloonViewViewModel.employee.Datebirthday)
                {
                    Employees[i] = notifyBalloonViewViewModel.employee;
                    RaisePropertyChanged("Employees");
                    flag = true;
                    break;
                }
            }
        }
        #endregion

        #region Sort
        public CustomRelayCommand SortListView { get; private set; }

        public void SortListView_Execute(object parameter)
        {
            string column = parameter as string;
            if (_sortColumn == column)
            {
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
            }
            _employeesView.SortDescriptions.Clear();
            _employeesView.SortDescriptions.Add(new SortDescription(_sortColumn, _sortDirection));
        }
        #endregion
    }
}