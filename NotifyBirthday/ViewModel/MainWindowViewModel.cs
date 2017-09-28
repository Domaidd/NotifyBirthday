using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace NotifyBirthday
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Private properties
        private int Frequency;

        private int Period;

        private SettingView settingView;

        private AddEmployeeView addEmployeeView;

        private DetailEmployeeView detailEmployeeView;

        private SettingViewViewModel settingViewViewModel;

        private AddEmployeeViewViewModel addEmployeeViewViewModel;

        private DetailEmployeeViewViewModel detailEmployeeViewViewModel;

        private Employee _selectedEmployee;

        private TimerCallback timerCallback;

        private Timer timer;

        private readonly NotifyService notifyService = new NotifyService();
        #endregion

        #region Public properties
        public ObservableCollection<Employee> Employees { get; set; }

        public Window window;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged("SelectedEmployee");
                RemoveEmployee.RaiseCanExecuteChanged();
                OpenDetailEmployeeView.RaiseCanExecuteChanged();
            }
        }

        public Employee BallonEmployee;
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            AddNotifyService();
            RemoveEmployee = new RelayCommand(RemoveEmployee_Execute, RemoveEmployee_CanExecute);
            OpenSetting = new RelayCommand(OpenSetting_Execute, OpenSetting_CanExecute);
            OpenAddEmployeeView = new RelayCommand(OpenAddEmployeeView_Execute, OpenAddEmployeeView_CanExecute);
            OpenDetailEmployeeView = new RelayCommand(OpenDetailEmployeeView_Execute, OpenDetailEmployeeView_CanExecute);
            CloseApp = new RelayCommand(CloseApp_Execute);

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
            addEmployeeView.Closed += AddEmployeeView_Closed;
            addEmployeeView.Show();
        }

        public bool OpenAddEmployeeView_CanExecute()
        {
            return addEmployeeView == null && detailEmployeeView == null && settingView == null;
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
            detailEmployeeViewViewModel.InputName = SelectedEmployee.Name;
            detailEmployeeViewViewModel.InputSurname = SelectedEmployee.Surname;
            detailEmployeeViewViewModel.InputMiddlename = SelectedEmployee.Middlename;
            detailEmployeeViewViewModel.InputDate = SelectedEmployee.Datebirthday;
            detailEmployeeView.Closed += DetailEmployeeView_Closed;
            detailEmployeeView.Show();
        }

        private void DetailEmployeeView_Closed(object sender, EventArgs e)
        {
            detailEmployeeView = null;
        }

        public bool OpenDetailEmployeeView_CanExecute()
        {
            return SelectedEmployee != null && addEmployeeView == null && detailEmployeeView == null && settingView == null;
        }

        public RelayCommand OpenSetting { get; private set; }

        public void OpenSetting_Execute()
        {
            settingViewViewModel = new SettingViewViewModel
            {
                Config = DataManager.Load<Config>("Config.xml")
            };
            settingViewViewModel.Frequency = settingViewViewModel.Config.Frequency;
            settingViewViewModel.Period = settingViewViewModel.Config.Period;
            settingViewViewModel.SetIndex();
            settingViewViewModel.SelectFrequency = new ComboBoxItem
            {
                Content = Frequency
            };
            settingView = new SettingView
            {
                DataContext = settingViewViewModel
            };
            settingView.ResizeMode = ResizeMode.NoResize;
            settingView.Closed += SettingView_Closed;
            settingView.Show();
        }

        private bool OpenSetting_CanExecute()
        {
            return addEmployeeView == null && detailEmployeeView == null && settingView == null;
        }

        private void SettingView_Closed(object sender, EventArgs e)
        {
            settingViewViewModel.Disponse();
            int prevFrequency = Frequency;
            int prevPeriod = Period;
            LoadConfig();
            if (Frequency > 0 && Period > 0 && (prevFrequency != Frequency || prevPeriod != Period))
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
                date = Convert.ToDateTime(Employees[i].Datebirthday.Day + "." + Employees[i].Datebirthday.Month + "." + DateTime.Now.Year);
                rdt = date - Convert.ToDateTime(DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year);
                int d = (int)rdt.TotalDays;
                if (Employees[i].Flag)
                {
                    if (d > 1 && d < Period)
                    {
                        BallonEmployee = Employees[i];
                        notifyService.Notify(d + " дня до дня рождения " + Employees[i].Name + " " + Employees[i].Surname + " " + Employees[i].Middlename + ".");
                    }
                    else if (d == 1)
                    {
                        BallonEmployee = Employees[i];
                        notifyService.Notify("Завтра день рождения у " + Employees[i].Name + " " + Employees[i].Surname + " " + Employees[i].Middlename + ".");
                    }
                    else if (d == 0)
                    {
                        BallonEmployee = Employees[i];
                        notifyService.Notify("Сегодня день рождения у " + Employees[i].Name + " " + Employees[i].Surname + " " + Employees[i].Middlename + ".");
                    }
                    else Employees[i].Flag = true;
                }
                if (BallonEmployee != null)
                {
                    if (!BallonEmployee.Flag)
                    {
                        Employees[i].Flag = false;
                    }
                }
            }
        }
        #endregion
    }
}
