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
            set
            {
                _frequency = value;
                SaveConfig.RaiseCanExecuteChanged();
            }
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

        private int _selectIndex;

        public int SelectIndex
        {
            get { return _selectIndex; }
            set
            {
                _selectIndex = value;
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
        public void SetIndex()
        {
            switch (Frequency)
            {
                case 1:
                    SelectIndex = 0;
                    break;
                case 2:
                    SelectIndex = 1;
                    break;
                case 3:
                    SelectIndex = 2;
                    break;
                case 4:
                    SelectIndex = 3;
                    break;
                case 6:
                    SelectIndex = 4;
                    break;
                case 8:
                    SelectIndex = 5;
                    break;
                case 12:
                    SelectIndex = 6;
                    break;
                default:
                    break;
            }
        }

        public RelayCommand SaveConfig { get; private set; }

        public void SaveConfig_Execute()
        {
            Config = new Config(Convert.ToInt32(SelectFrequency.Content.ToString()), Period);
        }

        public bool SaveConfig_CanExecute()
        {
            return true;
            //return SelectFrequency != null && Period > 0;
        }

        public void Disponse()
        {
            DataManager.Save(Config, "Config.xml");
        }
    }
}
