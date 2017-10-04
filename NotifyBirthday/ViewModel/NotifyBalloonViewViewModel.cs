using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;

namespace NotifyBirthday
{
    public class NotifyBalloonViewViewModel : ViewModelBase
    {
        private string _notifyText;

        public Employee employee;

        public string NotifyText
        {
            get { return _notifyText; }
            set { _notifyText = value; }
        }

        public Window Window;

        public RelayCommand CloseBalloon { get; private set; }

        private void CloseBalloon_Execute()
        {
            Window.Close();
            employee.Flag = false;
        }

        public NotifyBalloonViewViewModel()
        {
            CloseBalloon = new RelayCommand(CloseBalloon_Execute);
        }
    }
}
