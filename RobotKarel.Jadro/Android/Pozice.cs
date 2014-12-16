using System.ComponentModel;

namespace RobotKarel.Jadro.Android
{
    /// <summary>
    /// Reprezentuje souřadnice a orientaci na mapě.
    /// </summary>
    public class Pozice : INotifyPropertyChanged
    {
        private int x;
        private int y;
        private Smer orientace;

        public int X 
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged("X");
                OnPropertyChanged("PoziceVypis");
            }
        }

        public int Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged("Y");
                OnPropertyChanged("PoziceVypis");
            }
        }

        public string PoziceVypis
        {
            get
            {
                return string.Format("({0}, {1})", X, Y);
            }
        }

        public Smer Orientace
        {
            get { return orientace; }
            set
            {
                orientace = value;
                OnPropertyChanged("Orientace");
            }
        }

        public Pozice(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public static Pozice operator -(Pozice a, Pozice b)
        {
            return new Pozice(a.X - b.X, a.Y - b.Y);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public void ZmensitX()
        {
            X--;
        }

        public void ZvetsitX()
        {
            X++;
        }

        public void ZmensitY()
        {
            Y--;
        }

        public void ZvetsitY()
        {
            Y++;
        }
    }
}
