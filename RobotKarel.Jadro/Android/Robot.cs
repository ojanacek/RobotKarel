using System;
using System.ComponentModel;
using System.Net.Sockets;

namespace RobotKarel.Jadro.Android
{
    /// <summary>
    /// Reprezentuje klienta připojeného k serveru a udržuje informace o něm.
    /// </summary>
    public abstract class Robot : INotifyPropertyChanged
    {
        private string osloveni;
        private Pozice pozice;
        private string zprava;

        /// <summary>
        /// Identifikační číslo klienta.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Oslovení robota.
        /// </summary>
        public string Osloveni
        {
            get { return osloveni; }
            set
            {
                osloveni = value;
                OnPropertyChanged("Osloveni");
            }
        }

        /// <summary>
        /// Pozice, na které se robot nachází a směr, kterým se dívá.
        /// </summary>
        public Pozice Pozice
        {
            get { return pozice; }
            set
            {
                pozice = value;
                OnPropertyChanged("Pozice");
            }
        }

        /// <summary>
        /// Odkaz na aktivní připojení.
        /// </summary>
        public TcpClient Klient { get; set; }

        /// <summary>
        /// Zpráva, která se objeví po zvednutí značky.
        /// </summary>
        public string Zprava
        {
            get { return zprava; }
            set
            {
                zprava = value;
                OnPropertyChanged("Zprava");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Zjistí, jestli je robot na značce.
        /// </summary>
        public bool JeNaZnacce()
        {
            return Pozice.X == 0 && Pozice.Y == 0;
        }

        /// <summary>
        /// Nastaví orientaci - otočení se o 45 stupňů doleva. 
        /// </summary>
        public void OtocitSeDoleva()
        {
            if (Pozice.Orientace == Smer.Sever)
            {
                Pozice.Orientace = Smer.Zapad;
                return;
            }

            if (Pozice.Orientace == Smer.Zapad)
            {
                Pozice.Orientace = Smer.Jih;
                return;
            }

            if (Pozice.Orientace == Smer.Jih)
            {
                Pozice.Orientace = Smer.Vychod;
                return;
            }

            if (Pozice.Orientace == Smer.Vychod)
            {
                Pozice.Orientace = Smer.Sever;
                return;
            }
        }

        /// <summary>
        /// Ukončí síťovou komunikaci.
        /// </summary>
        protected void UkoncitSpojeni()
        {
            if (Klient != null)
                Klient.Close();
        }

        /// <summary>
        /// Nastaví oslovení robota.
        /// </summary>
        protected abstract void NastavitOsloveni();

        /// <summary>
        /// Nastaví pozici robota.
        /// </summary>
        protected abstract void NastavitPozici();

        /// <summary>
        /// Nastaví robota.
        /// </summary>
        public abstract void NastavitRobota();
    }
}
