using System.ComponentModel;

namespace RobotKarel.Jadro.Android
{
    /// <summary>
    /// Informace o technickém stavu robota.
    /// </summary>
    public class Stav : INotifyPropertyChanged
    {
        public const string KOD_ROZBITI = "580";

        private int rozbityProcesor;
        /// <summary>
        /// Číslo rozbitého procesoru.
        /// </summary>
        public int RozbityProcesor
        {
            get { return rozbityProcesor; }
            set
            {
                rozbityProcesor = value;
                OnPropertyChanged("JeRozbity");
                OnPropertyChanged("Stav");
            }
        }

        /// <summary>
        /// Kolik kroků ušel robot od posledního rozbití procesoru.
        /// </summary>
        public int NerozbitKroku { get; set; }

        /// <summary>
        /// Indikuje, jestli je robot rozbitý.
        /// </summary>
        public bool JeRozbity
        {
            get
            {
                return RozbityProcesor > 0;
            }
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
    }
}
