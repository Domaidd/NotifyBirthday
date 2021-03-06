﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace NotifyBirthday
{
    public class AddEmployeeViewViewModel : ViewModelBase
    {
        private string _name;
        private string _surname;
        private string _middlename;
        private DateTime _datebirthday;
        private bool _check;

        public Window Window;

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
                _datebirthday = value;
                RaisePropertyChanged("Datebirthday");
                AddEmployee.RaiseCanExecuteChanged();
            }
        }
        public bool Check
        {
            get { return _check; }
            set
            {
                _check = value;
                RaisePropertyChanged("Check");
                AddEmployee.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<Employee> Employees { get; set; }

        public AddEmployeeViewViewModel()
        {
            AddEmployee = new RelayCommand(AddEmployee_Execute, AddEmployee_CanExecute);
            InputDate = new DateTime(1990, 1, 1);
        }

        public RelayCommand AddEmployee { get; private set; }

        public void AddEmployee_Execute()
        {
            if (Employees == null)
            {
                Employees = new ObservableCollection<Employee>();
            }
            Employee employee = new Employee(InputName, InputSurname, InputMiddlename, InputDate);
            Employees.Add(employee);
            Window.Close();
        }

        public bool AddEmployee_CanExecute()
        {
            return ValidAddEmploeey();
        }

        public bool ValidAddEmploeey()
        {
            if (InputName != null && InputSurname != null && InputDate != null && (InputMiddlename == null || InputMiddlename == ""))
            {
                if (Regex.IsMatch(InputSurname, @"[a-z, A-Z, а-я, А-Я]") && Regex.IsMatch(InputName, @"[a-z, A-Z, а-я, А-Я]"))
                {
                    return true;
                }
                return false;
            } else if (InputName != null && InputSurname != null && InputDate != null && InputMiddlename != null)
            {
                if (Regex.IsMatch(InputSurname, @"[a-z, A-Z, а-я, А-Я]") && Regex.IsMatch(InputName, @"[a-z, A-Z, а-я, А-Я]") && Regex.IsMatch(InputMiddlename, @"[a-z, A-Z, а-я, А-Я]"))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
