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
        private int Frequency;
        private int Period;

        private SettingView settingView;
        private SettingViewViewModel settingViewViewModel;

        private Employee _selectedEmployee;

        public ObservableCollection<Employee> Employees { get; set; }

        private TimerCallback timerCallback;

        private Timer timer;

        private string _name;
        private string _surname;
        private string _middlename;
        private DateTime _datebirthday;
        public Window window;

        public string InputName
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
                AddEmployee.RaiseCanExecuteChanged();
            }
        }
        public string InputSurname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                RaisePropertyChanged("Surname");
                AddEmployee.RaiseCanExecuteChanged();
            }
        }
        public string InputMiddlename
        {
            get { return _middlename; }
            set
            {
                _middlename = value;
                RaisePropertyChanged("Middlename");
                AddEmployee.RaiseCanExecuteChanged();
            }
        }
        public DateTime InputDate
        {
            get { return _datebirthday.Date; }
            set
            {
                _datebirthday = value.Date;
                RaisePropertyChanged("Datebirthday");
                AddEmployee.RaiseCanExecuteChanged();
            }
        }

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged("SelectedEmployee");
                RemoveEmployee.RaiseCanExecuteChanged();
            }
        }

        public Employee BallonEmployee;

        public MainWindowViewModel()
        {
            AddNotifyService();
            AddEmployee = new RelayCommand(AddEmployee_Execute, AddEmployee_CanExecute);
            RemoveEmployee = new RelayCommand(RemoveEmployee_Execute, RemoveEmployee_CanExecute);
            OpenSetting = new RelayCommand(OpenSetting_Execute);
            CloseApp = new RelayCommand(CloseApp_Execute);

            InputDate = new DateTime(1990, 1, 1);
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

        public RelayCommand AddEmployee { get; private set; }

        public void AddEmployee_Execute()
        {
            if (Employees == null)
            {
                Employees = new ObservableCollection<Employee>();
            }
            Employee employee = new Employee(InputName, InputSurname, InputMiddlename, InputDate.Date);
            Employees.Add(employee);
        }

        public bool AddEmployee_CanExecute()
        {
            return ValidAddEmploeey();
        }

        public bool ValidAddEmploeey()
        {
            if (InputName != null && InputSurname != null && InputMiddlename != null && InputDate != null)
            {
                if (Regex.IsMatch(InputSurname, @"[a-z, A-Z, а-я, А-Я]")
                    && Regex.IsMatch(InputName, @"[a-z, A-Z, а-я, А-Я]")
                    && Regex.IsMatch(InputMiddlename, @"[a-z, A-Z, а-я, А-Я]")
                    //&& !Regex.IsMatch(InputDate.Date.ToString(), @"[a-z, A-Z, а-я, А-Я]")
                    )
                {
                    return true;
                }
                return false;
            }
            return false;
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
            settingView = new SettingView
            {
                DataContext = settingViewViewModel
            };
            settingView.ResizeMode = ResizeMode.NoResize;
            settingView.Closed += SettingView_Closed;
            settingView.Show();
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
        }

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

        public void Disponse()
        {
            DataManager.Save(Employees, "Employees.xml");
        }

        private readonly NotifyService notifyService = new NotifyService();

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
    }
}
