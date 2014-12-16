using System;

namespace RobotKarel.Jadro.Packet
{
    public class UdpPacket
    {
        public byte[] Packet { get; private set; }
        private Komunikace komunikace;

        public byte[] Identifikator
        {
            get
            {
                return Helper.SubArray(Packet, 0, 4);
            }
        }

        public byte[] SekvencniCislo
        {
            get
            {
                return Helper.SubArray(Packet, 4, 2);
            }
        }

        public byte[] CisloPotvrzeni
        {
            get
            {
                return Helper.SubArray(Packet, 6, 2);
            }
        }

        public byte[] Priznak
        {
            get
            {
                return Helper.SubArray(Packet, 8, 1);
            }
        }

        public byte[] Data
        {
            get
            {
                return Helper.SubArray(Packet, 9);
            }
        }

        public long Time { get; set; }

        public UdpPacket(long time, byte[] data, Komunikace komunikace)
        {
            Time = time;
            Packet = data;
            this.komunikace = komunikace;            
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} {2} seq={3} ack={4} flag={5} data({6}): {7}",
                Time, Helper.ByteArrayToHexaString(Identifikator), komunikace.ToString(), Helper.ByteArrayToInt(SekvencniCislo).ToString(), Helper.ByteArrayToInt(CisloPotvrzeni).ToString(),
                Helper.ByteArrayToHexaString(Priznak), Data.Length, Data.Length > 2 ? Helper.ByteArrayToHexaString(Helper.SubArray(Data, 0, 8)) : Helper.ByteArrayToHexaString(Data));
        }
    }
}
