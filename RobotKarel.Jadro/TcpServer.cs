using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RobotKarel.Jadro.Android;

namespace RobotKarel.Jadro
{
    /// <summary>
    /// Umožňuje komunikaci s klientem přes TCP protokol.
    /// </summary>
    public class TcpServer
    {
        private TcpListener listener;
        private object locker = new object();

        /// <summary>
        /// Spustí naslouchání na dané adrese a portu.
        /// </summary>
        public void SpustitServer(string adresa, int port, ObservableCollection<ServerRobot> klientList)
        {
            if (port < 3000 || port > 3999)
            {
                Trace.WriteLine("Číslo portu musí být v rozsahu 3000-3999." + Environment.NewLine);
                return;
            }

            try
            {
                listener = new TcpListener(IPAddress.Parse(adresa), port);
                Task.Factory.StartNew(() => CekatNaKlienta(klientList));
                Trace.WriteLine("Server byl spuštěn." + Environment.NewLine);
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Nastala chyba při vytváření serveru." + Environment.NewLine);
                Debug.WriteLine(ex.ToString() + Environment.NewLine);
            }
        }

        /// <summary>
        /// Čeká na připojení klienta.
        /// </summary>
        private void CekatNaKlienta(ObservableCollection<ServerRobot> klientList)
        {
            try
            {
                listener.Start();

                while (true)
                {
                    var klient = listener.AcceptTcpClient();
                    Task.Factory.StartNew(() =>
                    {
                        var robotKlient = new ServerRobot() { Klient = klient };
                        Trace.WriteLine("Nový klient připojen -> ID " + robotKlient.ID + Environment.NewLine);

                        lock (locker)
                        {
                            klientList.Add(robotKlient);
                        }

                        robotKlient.Komunikace();
                        klient.Close();

                        lock (locker)
                        {
                            klientList.Remove(robotKlient);
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Nastala chyba při čekání na klienta." + Environment.NewLine);
                Debug.WriteLine(ex.ToString() + Environment.NewLine);
            }
        }        

        /// <summary>
        /// Zašle zprávu klientovi.
        /// </summary>
        public static void ZaslatZpravu(NetworkStream stream, string zprava, int idKlienta)
        {
            try
            {
                var encoding = new ASCIIEncoding();
                var buffer = encoding.GetBytes(zprava + Environment.NewLine);
                stream.Write(buffer, 0, buffer.Length);
                Trace.WriteLine(string.Format("--> {0} - {1}{2}", idKlienta, zprava, Environment.NewLine));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Nepodařilo se zaslat zprávu klientovi." + Environment.NewLine);
                Debug.WriteLine(ex.ToString() + Environment.NewLine);
            }
        }
    }
}
