using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using RobotKarel.Jadro;
using RobotKarel.Jadro.Android;
using RobotKarel.Jadro.Logovani;

namespace RobotKarel.Klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogListener listener;
        private KlientRobot robot;
        private DispatcherTimer timer;
        private const int MSEC_INTERVAL = 100;

        public MainWindow()
        {
            InitializeComponent();

            Setup();
        }

        private void btnZahajitSpojeni_Click(object sender, RoutedEventArgs e)
        {
            SmazatLog();
            TcpKlient.NavazatSpojeni(txbServer.Text, int.Parse(txbPort.Text));            
            robot.NastavitRobota();
            btnNavestRobota.Visibility = Visibility.Visible;
        }

        private void btnNavestRobota_Click(object sender, RoutedEventArgs e)
        {
            btnNavestRobota.Visibility = Visibility.Hidden;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            robot.NavestRobota();
            if (robot.JeNaZnacce())
            {
                timer.Stop();
                robot.ZvednoutZnacku();
            }
        }

        #region /*** Main ***/

        /// <summary>
        /// Nastaví základní věci.
        /// </summary>
        private void Setup()
        {
            PostavMesto();
            listener = new LogListener();
            Trace.Listeners.Add(listener);
            listener.OnWriteLine += new LogListener.LogListenerHandler(listener_OnWriteLine);
            robot = new KlientRobot();
            robotInfo.DataContext = cityCanvas.DataContext = robot;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(MSEC_INTERVAL);
            timer.Tick += new EventHandler(timer_Tick);
        }

        void listener_OnWriteLine(object sender, LogListenerEventArgs args)
        {
            txbLog.Text += args.Message;
            txbLog.ScrollToEnd();
        }

        /// <summary>
        /// Postaví město.
        /// </summary>
        public void PostavMesto()
        {
            for (int x = 0; x < 44; x++)
            {
                var line = new Line();
                line.X1 = x * 25;
                line.X2 = x * 25;
                line.Y1 = 0;
                line.Y2 = 1075;
                line.Stroke = Brushes.Black;
                cityCanvas.Children.Add(line);
            }

            for (int y = 0; y < 44; y++)
            {
                var line = new Line();
                line.X1 = 0;
                line.X2 = 1075;
                line.Y1 = y * 25;
                line.Y2 = y * 25;
                line.Stroke = Brushes.Black;
                cityCanvas.Children.Add(line);
            }

            // vykreslení souřadnic ve městě
            /*for (int x = 0; x < 43; x++)
            {
                for (int y = 0; y < 43; y++)
                {
                    var text = string.Format("({0},{1})", x - 21, y - 21);
                    var block = new TextBlock();
                    block.Text = text;
                    block.FontSize = 7;
                    block.FontWeight = FontWeights.Normal;
                    Canvas.SetLeft(block, (double)x * 25);
                    Canvas.SetTop(block, (double)y * 25);
                    cityCanvas.Children.Add(block);
                }
            }*/
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
