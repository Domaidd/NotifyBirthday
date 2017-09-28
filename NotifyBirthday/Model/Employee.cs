using System;

namespace NotifyBirthday
{
    [Serializable]
    public class Employee
    {
        private string _name;
        private string _surname;
        private string _middlename;
        private DateTime _datebirthday;
        private bool _flag;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }
        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
            }
        }
        public string Middlename
        {
            get { return _middlename; }
            set
            {
                _middlename = value;
            }
        }
        public DateTime Datebirthday
        {
            get { return _datebirthday; }
            set
            {
                _datebirthday = value;
            }
        }
        public bool Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
            }
        }

        public Employee()
        {

        }

        public Employee(string name, string surname, string middlename, DateTime datebirthday)
        {
            Name = name;
            Surname = surname;
            Middlename = middlename;
            Datebirthday = datebirthday;
            Flag = true;
        }
    }
}
