using System;
using System.Collections.Generic;

namespace RobotKarel.Jadro.Android
{
    /// <summary>
    /// Informace potřebné pro nastavení serverového robota.
    /// </summary>
    public static class ServerRobotSetup
    {
        /// <summary>
        /// Náhodná oslovení.
        /// </summary>
        private static string[] poleOsloveni = { "Robote", "Androide", "Poklicko", "Zehlicko", "Pracko", "Sklenice", "Sasku" };

        /// <summary>
        /// Náhodné tajné zprávy.
        /// </summary>
        private static string[] poleZprav = { "Znamenite.", "Vyborne.", "Velmi dobre.", "Dobry vykon.", "Hotovo.", "Jsi v cili." };

        /// <summary>
        /// Minimální krajní souřadnice, kde se robot může objevit (X i Y).
        /// </summary>
        private const int MIN_POSITION = Mesto.MIN_POSITION - 1;

        /// <summary>
        /// Maximální krajní souřadnice, kde se robot může objevit (X i Y).
        /// </summary>
        private const int MAX_POSITION = Mesto.MAX_POSITION - 1;

        /// <summary>
        /// Kolik maximálně může robot ujít kroků, aniž by se rozbil.
        /// </summary>
        public const int MAX_KROKU = 9;

        /// <summary>
        /// Náhodně vybere ID pro robota.
        /// </summary>
        public static int NahodneID()
        {
            return new Random().Next(1, 100);
        }

        /// <summary>
        /// Náhodně vybere oslovení pro robota.
        /// </summary>
        public static string VybratOsloveni()
        {
            return poleOsloveni[new Random().Next(0, poleOsloveni.Length)];
        }

        /// <summary>
        /// Zformuluje uvítací zprávu pro klienta.
        /// </summary>
        public static string UvitaciZprava(string osloveni)
        {
            return string.Format("200 Ahoj, tady .net robot. Oslovuj mne {0}.", osloveni);
        }

        /// <summary>
        /// Vrátí náhodnou orientaci.
        /// </summary>
        private static Smer NahodnaOrientace()
        {
            var list = new List<Smer>() { Smer.Jih, Smer.Sever, Smer.Vychod, Smer.Zapad };
            return list[new Random().Next(0, list.Count)];
        }

        /// <summary>
        /// Vygeneruje náhodné umístění a orientaci robota.
        /// </summary>
        public static Pozice VygenerovatUmisteni()
        {
            return new Pozice()
            {
                X = new Random().Next(MIN_POSITION, MAX_POSITION),
                Y = new Random().Next(MIN_POSITION, MAX_POSITION),
                Orientace = NahodnaOrientace()
            };
        }

        /// <summary>
        /// Vybere náhodnou "tajnou zprávu".
        /// </summary>
        public static string TajnaZprava()
        {
            return poleZprav[new Random().Next(0, poleZprav.Length)];
        }
    }
}
