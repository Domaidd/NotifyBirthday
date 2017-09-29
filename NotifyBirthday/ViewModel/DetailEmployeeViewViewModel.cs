using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NotifyBirthday
{
    public class DetailEmployeeViewViewModel : ViewModelBase
    {
        private string _name;
        private string _surname;
        private string _middlename;
        private DateTime _datebirthday;
        private Employee _selectedEmployee;

        public string InputName
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
                DetailEmployee.RaiseCanExecuteChanged();
            }
        }
        public string InputSurname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                RaisePropertyChanged("Surname");
                DetailEmployee.RaiseCanExecuteChanged();
            }
        }
        public string InputMiddlename
        {
            get { return _middlename; }
            set
            {
                _middlename = value;
                RaisePropertyChanged("Middlename");
                DetailEmployee.RaiseCanExecuteChanged();
            }
        }
        public DateTime InputDate
        {
            get { return _datebirthday.Date; }
            set
            {
                _datebirthday = value.Date;
                RaisePropertyChanged("Datebirthday");
                DetailEmployee.RaiseCanExecuteChanged();
            }
        }

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged("SelectedEmployee");
                DetailEmployee.RaiseCanExecuteChanged();
            }
        }

        public DetailEmployeeViewViewModel()
        {
            DetailEmployee = new RelayCommand(DetailEmployee_Execute, DetailEmployee_CanExecute);
            InputDate = new DateTime(1990, 1, 1);
        }

        public RelayCommand DetailEmployee { get; private set; }

        public void DetailEmployee_Execute()
        {
            SelectedEmployee.Name = InputName;
            SelectedEmployee.Surname = InputSurname;
            SelectedEmployee.Middlename = InputMiddlename;
            SelectedEmployee.Datebirthday = InputDate;
        }

        public bool DetailEmployee_CanExecute()
        {
            return ValidAddEmploeey();
        }

        public bool ValidAddEmploeey()
        {
            if (InputName != null && InputSurname != null && InputDate != null)
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
    }
}
