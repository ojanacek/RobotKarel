using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RobotKarel.Jadro.Packet;
using System.IO;

namespace RobotKarel.Jadro.Android
{
    public class Fotografie
    {
        private UdpKlient klient;
        private List<UdpPacket> data;

        private byte[] kodSpojeni = null;
        private ushort lastAck = 0;

        public Fotografie()
        {
            klient = new UdpKlient("baryk.fit.cvut.cz", 4000);
            data = new List<UdpPacket>();
        }

        public void StahnoutFotografii()
        {
            byte[] result = null;
            UdpPacket rcvdPacket = null;

            /*** Navazani komunikace ***/
            var synPacket = new UdpPacket(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01 }, Komunikace.SEND);

            for (int i = 0; i < 20; i++, result = null)
            {                
                Trace.WriteLine(synPacket.ToString());
                klient.ZaslatZpravu(synPacket.Packet);
                
                while (result == null)                    
                    result = klient.PrijmoutZpravu();

                rcvdPacket = new UdpPacket(0, result, Komunikace.RECV);
                Trace.WriteLine(rcvdPacket.ToString());

                if (Helper.ByteArrayToHexaString(rcvdPacket.Priznak) != "01")
                {
                    result = null;
                    while (true) // zahazuju vsechny packety az na potvrzeni RST, ktere prijde nakonec
                    {
                        result = klient.PrijmoutZpravu();
                        if (result != null && result[8] == 0x04)
                            break;
                    }

                    rcvdPacket = new UdpPacket(0, result, Komunikace.RECV);
                    Trace.WriteLine(rcvdPacket.ToString() + Environment.NewLine);

                    if (i < 19)
                    {
                        Trace.WriteLine("Server nepotvrdil SYN, čekám na RST a posílám SYN." + Environment.NewLine + "---------------------------------------------------------------");
                        continue;
                    }
                    else
                    {
                        Trace.WriteLine("20 neúspěšných SYN packetů, končím spojení." + Environment.NewLine + "----------------------------------------------------------------------");
                        klient.UkoncitSpojeni();
                        return;
                    }
                }

                kodSpojeni = rcvdPacket.Identifikator;
                break;
            }

            /*********************************/

            var snddata = new byte[9];
            Array.Copy(kodSpojeni, 0, snddata, 0, 4);

            var watches = new Stopwatch();
            watches.Start();

            int j = 0;
            string actPriznak = "";
            while (actPriznak != "02" && actPriznak != "04")
            {
                Trace.WriteLine("**************** " + j++ + " *************");

                result = null;
                while (result == null)
                    result = klient.PrijmoutZpravu();

                rcvdPacket = new UdpPacket(watches.ElapsedMilliseconds, result, Komunikace.RECV);
                actPriznak = Helper.ByteArrayToHexaString(rcvdPacket.Priznak);

                if (rcvdPacket.Identifikator.SequenceEqual(kodSpojeni))
                {
                    Trace.WriteLine(rcvdPacket.ToString());

                    if (!data.Any(p => Helper.ByteArrayToInt(p.SekvencniCislo) == Helper.ByteArrayToInt(rcvdPacket.SekvencniCislo)))
                    {
                        if (actPriznak != "02")
                        {
                            data.Add(rcvdPacket);
                            data = data.OrderBy(p => Helper.ByteArrayToInt(p.SekvencniCislo)).ToList();
                        }
                    }

                    foreach (var p in data)
                        if (Helper.ByteArrayToInt(p.SekvencniCislo) == lastAck)
                            lastAck += (ushort)p.Data.Length;

                    Array.Copy(Helper.IntToByteArray(lastAck), 0, snddata, 6, 2);

                    if (actPriznak == "02")
                        snddata[8] = 0x02;
                    if (actPriznak == "04")
                        snddata[8] = 0x04;

                    klient.ZaslatZpravu(snddata);

                    var sndPacket = new UdpPacket(watches.ElapsedMilliseconds, snddata, Komunikace.SEND);
                    Trace.WriteLine(sndPacket.ToString());
                }
            }

            watches.Stop();
            klient.UkoncitSpojeni();

            if (actPriznak == "02")
                VytvoritFotografii();
        }

        /// <summary>
        /// Z prijatych dat sestavi fotografii a ulozi na disk.
        /// </summary>
        private void VytvoritFotografii()
        {
            using (var fs = new FileStream("c:/foto.png", FileMode.Create))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    lastAck = 0;
                    for (int i = 0; i < data.Count; i++)
                    {
                        var packet = data.Where(p => Helper.ByteArrayToInt(p.SekvencniCislo) == lastAck).Single();
                        lastAck += (ushort)packet.Data.Length;
                        bw.Write(packet.Data);
                    }
                }
            }
        }
    }
}