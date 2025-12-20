using System.Net;

namespace QuazalServer.AtariMelbourneHouse
{
    public class AmhLairProxy
    {
        public static bool TryConvertIpAddressToHex(string ipAddressString, out uint result)
        {
            if (IPAddress.TryParse(ipAddressString, out IPAddress? ipAddress))
            {
                byte[] ipBytes = ipAddress.GetAddressBytes();

                // If it's an IPv4 address, pass. EdNet is not IPV6 compatible.
                if (ipBytes.Length == 4)
                {
                    if (!BitConverter.IsLittleEndian)
                        Array.Reverse(ipBytes);

                    result = BitConverter.ToUInt32(ipBytes, 0);
                    return true;
                }
            }

            result = 0x0100007F; // Localhost endian swaped, game will reject it as it's forbidden value in the game code.
            return false;
        }
    }
}
