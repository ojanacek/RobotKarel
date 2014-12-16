using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace RobotKarel.Jadro
{
    /// <summary>
    /// Ovládání klienta přes přes TCP protokol.
    /// </summary>
    public static class TcpKlient
    {
        private static TcpClient klient;
        private static NetworkStream stream;

        /// <summary>
        /// Naváže spojení se serverem.
        /// </summary>
        public static void NavazatSpojeni(string server, int port)
        {
            try
            {
                klient = new TcpClient(server, port);
                stream = klient.GetStream(); 
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Nepodařilo se navázat spojení se serverem." + Environment.NewLine);
                Debug.WriteLine(ex.ToString() + Environment.NewLine);
                UkoncitSpojeni();
            }
        }

        /// <summary>
        /// Zašle zprávu na server.
        /// </summary>
        public static void ZaslatZpravu(string zprava)
        {
            try
            {
                var encoding = new ASCIIEncoding();
                var buffer = encoding.GetBytes(zprava);
                stream.Write(buffer, 0, buffer.Length);
                Trace.WriteLine("-->" + zprava);
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Nepodařilo se zaslat zprávu na server." + Environment.NewLine);
                Debug.WriteLine(ex.ToString() + Environment.NewLine);
            }
        }

        /// <summary>
        /// Přečte odpověď ze serveru.
        /// </summary>
        public static string PrecistOdpoved()
        {
            try
            {
                byte[] buffer;
                var result = "";

                while (!result.Contains(Environment.NewLine))
                {
                    buffer = new byte[100];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    result += ASCIIEncoding.Default.GetString(buffer, 0, bytesRead);
                }

                Trace.WriteLine(result);
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Nepodařilo se přečíst zprávu ze serveru." + Environment.NewLine);
                Debug.WriteLine(ex.ToString() + Environment.NewLine);
                return "";
            }
        }

        /// <summary>
        /// Ukončí spojení se serverem.
        /// </summary>
        public static void UkoncitSpojeni()
        {
            if (stream != null)
                stream.Close();
            if (klient != null)
                klient.Close();

            Trace.WriteLine("Spojení se serverem bylo ukončeno." + Environment.NewLine);
        }
    }
}
