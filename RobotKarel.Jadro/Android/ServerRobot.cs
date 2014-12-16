using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace RobotKarel.Jadro.Android
{
    /// <summary>
    /// Reprezentuje klienta připojeného k serveru.
    /// </summary>
    public class ServerRobot : Robot
    {
        /// <summary>
        /// Stav robota - informuje jestli je robot rozbitý, případně jaký procesor má rozbitý nebo kroků už se nerozbil.
        /// </summary>
        public Stav Stav { get; set; }

        public ServerRobot()
        {
            ID = ServerRobotSetup.NahodneID();
            Stav = new Stav();            
        }

        #region /*** Základní nastavení ***/

        /// <summary>
        /// Nastaví oslovení robota.
        /// </summary>
        protected override void NastavitOsloveni()
        {
            Osloveni = ServerRobotSetup.VybratOsloveni();
        }

        /// <summary>
        /// Nastaví pozici robota.
        /// </summary>
        protected override void NastavitPozici()
        {
            Pozice = ServerRobotSetup.VygenerovatUmisteni();
        }

        /// <summary>
        /// Nastaví tajnou zprávu na značce pro daného klienta.
        /// </summary>
        private void NastavitTajnouZpravu()
        {
            Zprava = ServerRobotSetup.TajnaZprava();
        }

        /// <summary>
        /// Nastaví základní vlastnosti robota.
        /// </summary>
        public override void NastavitRobota()
        {
            NastavitOsloveni();
            NastavitPozici();
            NastavitTajnouZpravu();
        }

        #endregion

        /// <summary>
        /// Zjistí, jestli robot vyšel mimo město.
        /// </summary>
        private bool MimoMesto()
        {
            return (Pozice.X < Mesto.MIN_POSITION || Pozice.X > Mesto.MAX_POSITION) || (Pozice.Y < Mesto.MIN_POSITION || Pozice.Y > Mesto.MAX_POSITION);
        }

        //////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Posune robota o jedno pole vpřed v závislosti na orientaci.
        /// </summary>
        private void Krok()
        {
            if (Pozice.Orientace == Smer.Sever)
                Pozice.ZvetsitY();
            if (Pozice.Orientace == Smer.Jih)
                Pozice.ZmensitY();
            if (Pozice.Orientace == Smer.Vychod)
                Pozice.ZvetsitX();
            if (Pozice.Orientace == Smer.Zapad)
                Pozice.ZmensitX();
        }

        public void Komunikace()
        {
            var stream = Klient.GetStream();
            byte[] buffer;
            bool svinarna = false;
            stream.ReadTimeout = 2000;

            NastavitRobota();
            TcpServer.ZaslatZpravu(stream, ServerRobotSetup.UvitaciZprava(Osloveni), ID);

            int bytesRead;
            var odpoved = "";
            while (true)
            {
                if (svinarna)
                {
                    var split = odpoved.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length > 1)
                        odpoved = split[1];
                    svinarna = false;
                }
                else
                    odpoved = "";

                while (!odpoved.Contains(Environment.NewLine))
                {
                    buffer = new byte[100];
                    try
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        Trace.WriteLine(ID + " - Klient ukončil spojení" + Environment.NewLine);
                        return;
                    }

                    if (odpoved.Length < 512)
                        odpoved += ASCIIEncoding.Default.GetString(buffer, 0, bytesRead);
                    else
                        odpoved = ASCIIEncoding.Default.GetString(buffer, 0, bytesRead);
                }

                Trace.WriteLine(ID + " - " + odpoved);

                var response = "";

                var splitt = odpoved.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (splitt.Length > 1)
                {
                    response = ReagovatNaPrikaz(splitt[0] + Environment.NewLine);
                    TcpServer.ZaslatZpravu(stream, response, ID);
                    if (splitt[1].Length > 8)
                    {
                        response = ReagovatNaPrikaz(splitt[1] + Environment.NewLine);
                        TcpServer.ZaslatZpravu(stream, response, ID);

                    }
                    else
                        svinarna = true;
                }
                else
                {
                    response = ReagovatNaPrikaz(odpoved);
                    TcpServer.ZaslatZpravu(stream, response, ID);
                }

                if (response.Contains("210") || response.Contains("530") || response.Contains("550") || response.Contains("571") || response.Contains("572"))
                    break;
            }
        }

        private string ReagovatNaPrikaz(string odpoved)
        {
            if (odpoved == string.Format("{0} {1}{2}", Osloveni, "KROK", Environment.NewLine))
            {
                if (Stav.JeRozbity)
                    return "572 ROBOT SE ROZPADL";
                else
                {
                    var random = new Random();
                    var num = random.Next(1, 6);
                    if (num == random.Next(1, 6) || Stav.NerozbitKroku >= ServerRobotSetup.MAX_KROKU)
                    {
                        Stav.NerozbitKroku = 0;

                        num = Stav.RozbityProcesor = random.Next(1, 9);
                        return "580 SELHANI PROCESORU " + num;
                    }
                    else
                    {
                        Krok();
                        if (MimoMesto())
                            return "530 HAVARIE";
                        else
                        {
                            Stav.NerozbitKroku++;
                            return string.Format("240 OK ({0},{1})", Pozice.X, Pozice.Y);
                        }
                    }
                }
            }

            if (odpoved == string.Format("{0} {1}{2}", Osloveni, "VLEVO", Environment.NewLine))
            {
                OtocitSeDoleva();
                return string.Format("240 OK ({0},{1})", Pozice.X, Pozice.Y);
            }

            if (odpoved == string.Format("{0} {1}{2}", Osloveni, "ZVEDNI", Environment.NewLine))
            {
                if (JeNaZnacce())
                    return "210 USPECH " + Zprava;
                else
                    return "550 NELZE ZVEDNOUT ZNACKU";
            }

            if (Regex.IsMatch(odpoved, string.Format("{0} {1} ", Osloveni, "OPRAVIT") + "[1-9]" + Environment.NewLine))
            {
                var split = odpoved.Split(' ');
                if (int.Parse(split[2]) == Stav.RozbityProcesor)
                {
                    Stav.RozbityProcesor = 0;
                    return string.Format("240 OK ({0},{1})", Pozice.X, Pozice.Y);
                }
                else
                    return "571 PROCESOR FUNGUJE";
            }

            return "500 NEZNAMY PRIKAZ";
        }
    }
}
