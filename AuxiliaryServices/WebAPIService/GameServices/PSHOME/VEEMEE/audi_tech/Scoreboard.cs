using System.IO;
using System.Xml;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.audi_tech
{
    public static class Scoreboard
    {
        public static string MockedScoreBoard = GenerateRandomScoreBoardXML();

        private static string GenerateRandomScoreBoardXML()
        {
            // System.Random random = new System.Random();

            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("AudiTechHighScore");

                    for (byte track = 1; track <= 3; track++)
                    {
                        for (byte challenge = 1; challenge <= 2; challenge++)
                        {
                            for (byte modifier = 1; modifier <= 4; modifier++)
                            {
                                writer.WriteStartElement($"Table{track}{challenge}{modifier}");

                                /*for (byte i = 0; i < 11; i++)  // Generate 10 random entries per table
                                {
                                    writer.WriteElementString("Name", "Player" + random.Next(1, 100).ToString());
                                    writer.WriteElementString("TotalScore", random.Next(1000, 10000).ToString());
                                    writer.WriteElementString("Time", random.Next(30, 300).ToString());
                                    writer.WriteElementString("Penalties", random.Next(0, 5).ToString());
                                    writer.WriteElementString("Comfort", random.Next(1, 10).ToString());
                                    writer.WriteElementString("Efficiency", random.Next(1, 10).ToString());
                                }*/

                                writer.WriteEndElement();  // End of Table
                            }
                        }
                    }

                    writer.WriteEndElement();  // End of AudiTechHighScore
                    writer.WriteEndDocument();
                }

                return stringWriter.ToString();
            }
        }
    }
}
