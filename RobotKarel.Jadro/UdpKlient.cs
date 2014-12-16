using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RobotKarel.Jadro
{
    /// <summary>
    /// Ovládání klienta přes přes UDP protokol.
    /// </summary>
    public class UdpKlient
    {
        private UdpClient klient;

        public UdpKlient(string address, int port)
        {
            klient = new UdpClient(port);
            klient.Connect(address, port);

            klient.Client.ReceiveTimeout = 100;
        }

        /// <summary>
        /// Zašle zprávu na server.
        /// </summary>
        public void ZaslatZpravu(byte[] data)
        {
            klient.Send(data, data.Length);
        }

        /// <summary>
        /// Přijme zprávu ze serveru.
        /// </summary>
        public byte[] PrijmoutZpravu()
        {
            byte[] result = null;
            var host = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                result = klient.Receive(ref host);
            }
            catch { }

            return result;
        }

        public void UkoncitSpojeni()
        {
            klient.Close();
        }
    }
}
