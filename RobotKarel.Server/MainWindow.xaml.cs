using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using RobotKarel.Jadro;
using RobotKarel.Jadro.Android;
using RobotKarel.Jadro.Logovani;

namespace RobotKarel.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogListener listener;
        public TcpServer Server { get; set; }

        /// <summary>
        /// Seznam všech klientů připojených k serveru.
        /// </summary>
        public ThreadSafeObservableCollection<ServerRobot> Klienti
        {
            get { return (ThreadSafeObservableCollection<ServerRobot>)GetValue(KlientiProperty); }
            set { SetValue(KlientiProperty, value); }
        }

        public static readonly DependencyProperty KlientiProperty = DependencyProperty.Register("Klienti", typeof(ThreadSafeObservableCollection<ServerRobot>), typeof(Window), new UIPropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();

            Setup();
        }

        private void btnSpustitServer_Click(object sender, RoutedEventArgs e)
        {
            btnSpustitServer.Visibility = Visibility.Hidden;
            btnUkoncitServer.Visibility = Visibility.Visible;
            Server.SpustitServer(txbServer.Text, int.Parse(txbPort.Text), Klienti);
        }

        private void btnUkoncitServer_Click(object sender, RoutedEventArgs e)
        {
            btnSpustitServer.Visibility = Visibility.Visible;
            btnUkoncitServer.Visibility = Visibility.Hidden;
        }

        #region /*** Main ***/

        /// <summary>
        /// Nastaví základní věci.
        /// </summary>
        private void Setup()
        {
            listener = new LogListener();
            Trace.Listeners.Add(listener);
            listener.OnWriteLine += new LogListener.LogListenerHandler(listener_OnWriteLine);
            Server = new TcpServer();
            Klienti = new ThreadSafeObservableCollection<ServerRobot>();
        }

        void listener_OnWriteLine(object sender, LogListenerEventArgs args)
        {
            txbLog.Dispatcher.Invoke(new Action(() => {
                txbLog.Text += args.Message;
                txbLog.ScrollToEnd();
            }));            
        }

        /// <summary>
        /// Vymaže obsah logu.
        /// </summary>
        private void SmazatLog()
        {
            txbLog.Text = "";
        }

        #endregion
    }
}
