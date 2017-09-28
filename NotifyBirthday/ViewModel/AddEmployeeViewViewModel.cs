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
    public class AddEmployeeViewViewModel : ViewModelBase
    {
        private string _name;
        private string _surname;
        private string _middlename;
        private DateTime _datebirthday;

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
            Employee employee = new Employee(InputName, InputSurname, InputMiddlename, InputDate.Date);
            Employees.Add(employee);;
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
    }
}
