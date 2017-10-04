using System;

namespace NotifyBirthday
{
    [Serializable]
    public class Config
    {
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
            set { _period = value; }
        }

        public Config()
        {

        }

        public Config(int frequency, int period)
        {
            Frequency = frequency;
            Period = period;
        }
    }
}
