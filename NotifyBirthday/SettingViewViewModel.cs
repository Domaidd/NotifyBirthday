using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Controls;

namespace NotifyBirthday
{
    public class SettingViewViewModel : ViewModelBase
    {
        public Config Config;

        private int _frequency;

        public int Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }

        private int _period;

        public int Period
        {
            get { return _period; }
            set
            {
                _period = value;
                RaisePropertyChanged("Period");
                SaveConfig.RaiseCanExecuteChanged();
            }
        }

        private ComboBoxItem _selectFrequency;

        public ComboBoxItem SelectFrequency
        {
            get { return _selectFrequency; }
            set
            {
                _selectFrequency = value;
                RaisePropertyChanged("SelectFrequency");
                SaveConfig.RaiseCanExecuteChanged();
            }
        }

        public SettingViewViewModel()
        {
            SaveConfig = new RelayCommand(SaveConfig_Execute, SaveConfig_CanExecute);
        }

        public RelayCommand SaveConfig { get; private set; }

        public void SaveConfig_Execute()
        {
            Config = new Config(Convert.ToInt32(SelectFrequency.Content.ToString()), Period);
        }

        public bool SaveConfig_CanExecute()
        {
            return SelectFrequency != null && Period > 0;
        }

        public void Disponse()
        {
            DataManager.Save(Config, "Config.xml");
        }
    }
}
