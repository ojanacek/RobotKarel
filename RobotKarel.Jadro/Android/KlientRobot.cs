using System;
using System.Diagnostics;
using RobotKarel.Jadro;

namespace RobotKarel.Jadro.Android
{
    public class KlientRobot : Robot
    {
        #region /*** Základní nastavení ***/

        /// <summary>
        /// Zjistí a nastaví oslovení robota.
        /// </summary>
        protected override void NastavitOsloveni()
        {
            var text = TcpKlient.PrecistOdpoved();
            int start = text.IndexOf("Oslovuj mne") + 12;
            int konec = text.IndexOf(".", start);
            Osloveni = text.Substring(start, konec - start);
        }

        /// <summary>
        /// Zjistí a nastaví pozici robota.
        /// </summary>
        protected override void NastavitPozici()
        {
            ZjistitPolohuAOrientaci();
        }

        /// <summary>
        /// Získá základní vlastnosti robota ze serveru.
        /// </summary>
        public override void NastavitRobota()
        {
            NastavitOsloveni();
            NastavitPozici();
        }

        #endregion

        /// <summary>
        /// Navede robota na značku.
        /// </summary>
        public void NavestRobota()
        {
            if (Pozice.Y > 0)
                Posun(Smer.Jih, Pozice.ZmensitY);
            else
                if (Pozice.Y == 0)
                    if (Pozice.X > 0)
                        Posun(Smer.Zapad, Pozice.ZmensitX);
                    else
                        Posun(Smer.Vychod, Pozice.ZvetsitX);
                else
                    Posun(Smer.Sever, Pozice.ZvetsitY);
        }

        /// <summary>
        /// Posune robota správným směrem podle parametrů.
        /// </summary>
        private void Posun(Smer smer, Action aktualizaceSouradnic)
        {
            string odpoved;

            if (Pozice.Orientace != smer)
            {
                PosliPrikaz(Osloveni, Prikazy.VLEVO);
                OtocitSeDoleva();
                return;
            }

            odpoved = PosliPrikaz(Osloveni, Prikazy.KROK);
            if (SelhaniProcesoru(odpoved))
                PosliPrikaz(Osloveni, Prikazy.OPRAVIT, CisloRozbitehoProcesoru(odpoved));
            else
                aktualizaceSouradnic();
        }

        /// <summary>
        /// Zvedne značku a přečte zprávu na ní.
        /// </summary>
        public void ZvednoutZnacku()
        {
            PosliPrikaz(Osloveni, Prikazy.ZVEDNI);
            Trace.WriteLine("Spojení se serverem ukončeno.");
        }        

        /// <summary>
        /// Zjistí polohu a orientaci robota ve městě.
        /// </summary>
        private void ZjistitPolohuAOrientaci()
        {
            var odpoved = PosliPrikaz(Osloveni, Prikazy.VLEVO);
            var tmpSouradnice = ZiskatSouradnice(odpoved);

            odpoved = PosliPrikaz(Osloveni, Prikazy.KROK);
            while (SelhaniProcesoru(odpoved))
            {
                PosliPrikaz(Osloveni, Prikazy.OPRAVIT, CisloRozbitehoProcesoru(odpoved));
                odpoved = PosliPrikaz(Osloveni, Prikazy.KROK);
            }

            var souradnice = ZiskatSouradnice(odpoved);
            souradnice.Orientace = ZjistitOrientaci(souradnice - tmpSouradnice);

            Pozice = souradnice;
        }

        /// <summary>
        /// Udělí příkaz robotovi a získá odpověď.
        /// </summary>
        private static string PosliPrikaz(string osloveni, Prikazy prikaz, string procesor = "")
        {
            var zprava = string.Format("{0} {1}{2}{3}", osloveni, prikaz.ToString(), procesor, Environment.NewLine);
            TcpKlient.ZaslatZpravu(zprava);
            return TcpKlient.PrecistOdpoved();
        }

        /// <summary>
        /// Z rozdílu dvou souřadnic zjistí orientaci robota.
        /// </summary>
        private static Smer ZjistitOrientaci(Pozice souradnice)
        {
            if (souradnice.Y == 1)
                return Smer.Sever;
            if (souradnice.Y == -1)
                return Smer.Jih;
            if (souradnice.X == 1)
                return Smer.Vychod;
            if (souradnice.X == -1)
                return Smer.Zapad;

            throw new Exception("Neznámé souřadnice" + Environment.NewLine);
        }

        /// <summary>
        /// Vyčte z textu souřadnice robota.
        /// </summary>
        private static Pozice ZiskatSouradnice(string text)
        {
            var split = text.Split(new char[] { ',', '(', ')' });
            return new Pozice(int.Parse(split[1]), int.Parse(split[2]));
        }

        /// <summary>
        /// Z odpovědi zjistí, jestli selhal procesor.
        /// </summary>
        private static bool SelhaniProcesoru(string odpoved)
        {
            return odpoved.Contains(Stav.KOD_ROZBITI);
        }

        /// <summary>
        /// Z odpovědi zjistí číslo rozbitého procesoru.
        /// </summary>
        private static string CisloRozbitehoProcesoru(string odpoved)
        {
            return odpoved.Substring(odpoved.Length - 4, 2);
        }
    }
}
