using System;
using System.Collections.Generic;
using System.IO;
using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using Newtonsoft.Json;
using WebAPIService.GameServices.NDREAMS;

namespace WebAPIService.GameServices.NDREAMS.BlueprintHome
{
    public class Furniture
    {
        public const int MaxSlots = 5;
        public const int MaxFurnSlots = 5;

        public static string ProcessFurniture(DateTime CurrentDate, byte[] PostData, string ContentType, string baseurl, string apipath)
        {
            string blueprint_name = string.Empty;
            string blueprint_furn = string.Empty;
            string territory = string.Empty;
            string name = string.Empty;
            string slot = string.Empty;
            string style = string.Empty;
            string owner = string.Empty;
            string func = string.Empty;
            string key = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    name = data.GetParameterValue("name");
                    func = data.GetParameterValue("func");
                    key = data.GetParameterValue("key");

                    try
                    {
                        territory = data.GetParameterValue("territory");
                        style = data.GetParameterValue("style");
                        owner = data.GetParameterValue("owner");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    try
                    {
                        blueprint_name = data.GetParameterValue("blueprint_name");
                        slot = data.GetParameterValue("slot");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    try
                    {
                        blueprint_furn = data.GetParameterValue("blueprint_furn");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();

                    string ExpectedHash = string.Empty;

                    switch (func)
                    {
                        case "save_furniture":
                            ExpectedHash = NDREAMSServerUtils.Server_GetSignature("blueprint_" + blueprint_name, name, "furn_save_slot_" + slot + "_" + name, CurrentDate);

                            if (key == ExpectedHash)
                            {
                                byte[] BlueprintDataBytes = null;

                                foreach (FilePart file in data.Files)
                                {
                                    using (Stream filedata = file.Data)
                                    {
                                        filedata.Position = 0;

                                        // Find the number of bytes in the stream
                                        int contentLength = (int)filedata.Length;

                                        // Create a byte array
                                        byte[] buffer = new byte[contentLength];

                                        // Read the contents of the memory stream into the byte array
                                        filedata.Read(buffer, 0, contentLength);

                                        if (file.FileName == "blueprint_furn.dat")
                                            BlueprintDataBytes = buffer;

                                        filedata.Flush();
                                    }
                                }

                                if (BlueprintDataBytes != null && int.TryParse(slot, out int value))
                                {
                                    if (File.Exists(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"))
                                    {
                                        List<BluePrintSlots> bpslots = JsonConvert.DeserializeObject<List<BluePrintSlots>>(File.ReadAllText(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"));

                                        if (bpslots != null)
                                        {
                                            int i = 0;
                                            foreach (BluePrintSlots bpslot in bpslots)
                                            {
                                                if (bpslot.position == value)
                                                {
                                                    File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/CurrentSlot.txt", slot);
                                                    File.WriteAllBytes(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{slot}.xml", BlueprintDataBytes);

                                                    bpslots[i] = new BluePrintSlots() { position = value, name = blueprint_name, url = baseurl + $"NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{value}.xml", used = "true" };

                                                    break;
                                                }

                                                i++;
                                            }

                                            File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json", JsonConvert.SerializeObject(bpslots));
                                        }
                                        else
                                        {
                                            string errMsg = $"[nDreams] - Furniture: The saving process errored out!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><error>Saving error</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                        }
                                    }
                                    else
                                    {
                                        string errMsg = $"[nDreams] - Furniture: Cannot save a slot while not being registered!";
                                        CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                        return $"<xml><error>Forbidden action</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                    }

                                    return "<xml></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[nDreams] - Furniture: Invalid Blueprint data sent for saving!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><error>Invalid Multi-Part File sent</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                }
                            }
                            else
                            {
                                string errMsg = $"[nDreams] - Furniture: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                return $"<xml><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                            }
                        case "save":
                            ExpectedHash = NDREAMSServerUtils.Server_GetSignature("blueprint_" + blueprint_name, name, "save_slot_" + slot + "_" + name, CurrentDate);

                            if (key == ExpectedHash)
                            {
                                byte[] BlueprintDataBytes = null;

                                foreach (FilePart file in data.Files)
                                {
                                    using (Stream filedata = file.Data)
                                    {
                                        filedata.Position = 0;

                                        // Find the number of bytes in the stream
                                        int contentLength = (int)filedata.Length;

                                        // Create a byte array
                                        byte[] buffer = new byte[contentLength];

                                        // Read the contents of the memory stream into the byte array
                                        filedata.Read(buffer, 0, contentLength);

                                        if (file.FileName == "blueprint.dat")
                                            BlueprintDataBytes = buffer;

                                        filedata.Flush();
                                    }
                                }

                                if (BlueprintDataBytes != null && int.TryParse(slot, out int value))
                                {
                                    if (File.Exists(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"))
                                    {
                                        List<BluePrintSlots> bpslots = JsonConvert.DeserializeObject<List<BluePrintSlots>>(File.ReadAllText(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"));

                                        if (bpslots != null)
                                        {
                                            int i = 0;
                                            foreach (BluePrintSlots bpslot in bpslots)
                                            {
                                                if (bpslot.position == value)
                                                {
                                                    File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/CurrentSlot.txt", slot);
                                                    File.WriteAllBytes(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/blueprint_{slot}.xml", BlueprintDataBytes);

                                                    bpslots[i] = new BluePrintSlots() { position = value, name = blueprint_name, url = baseurl + $"NDREAMS/BlueprintHome/Layout/{name}/blueprint_{value}.xml", used = "true" };

                                                    break;
                                                }

                                                i++;
                                            }

                                            File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json", JsonConvert.SerializeObject(bpslots));
                                        }
                                        else
                                        {
                                            string errMsg = $"[nDreams] - Furniture: The saving process errored out!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><error>Saving error</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                        }
                                    }
                                    else
                                    {
                                        string errMsg = $"[nDreams] - Furniture: Cannot save a slot while not being registered!";
                                        CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                        return $"<xml><error>Forbidden action</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                    }

                                    return "<xml></xml>";
                                }
                                else
                                {
                                    string errMsg = $"[nDreams] - Furniture: Invalid Blueprint data sent for saving!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><error>Invalid Multi-Part File sent</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                }
                            }
                            else
                            {
                                string errMsg = $"[nDreams] - Furniture: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                return $"<xml><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                            }
                        case "init":
                            ExpectedHash = NDREAMSServerUtils.Server_GetSignature("blueprint", name, owner, CurrentDate);

                            if (key == ExpectedHash)
                            {
                                int Currentfurnslot = 0;
                                int Currentslot = 0;
                                string furnslotsXmlResult = "<furn_slots>";
                                string slotsXmlResult = "<slots>";
                                string slotsUrlListDRM = string.Empty;

                                List<BluePrintSlots> furnSlots = new List<BluePrintSlots>();
                                List<BluePrintSlots> Slots = new List<BluePrintSlots>();

                                if (File.Exists(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"))
                                {
                                    List<BluePrintSlots> bpslots = JsonConvert.DeserializeObject<List<BluePrintSlots>>(File.ReadAllText(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"));

                                    if (bpslots != null)
                                    {
                                        foreach (BluePrintSlots bpslot in bpslots)
                                        {
                                            furnSlots.Add(bpslot);
                                            furnslotsXmlResult += $"<furn_slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></furn_slot>";
                                        }
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}");

                                    for (int i = 1; i <= MaxFurnSlots; i++)
                                    {
                                        BluePrintSlots bpslot = new BluePrintSlots() { position = i, url = baseurl + $"NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{i}.xml" };
                                        furnSlots.Add(bpslot);
                                        furnslotsXmlResult += $"<furn_slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></furn_slot>";
                                        File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{i}.xml", "<xml></xml>");
                                    }

                                    File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json", JsonConvert.SerializeObject(furnSlots));
                                }

                                if (File.Exists(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"))
                                {
                                    List<BluePrintSlots> bpslots = JsonConvert.DeserializeObject<List<BluePrintSlots>>(File.ReadAllText(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"));

                                    if (bpslots != null)
                                    {
                                        foreach (BluePrintSlots bpslot in bpslots)
                                        {
                                            Slots.Add(bpslot);
                                            slotsUrlListDRM += bpslot.url;
                                            slotsXmlResult += $"<slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></slot>";
                                        }
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}");

                                    for (int i = 1; i <= MaxSlots; i++)
                                    {
                                        BluePrintSlots bpslot = new BluePrintSlots() { position = i, url = baseurl + $"NDREAMS/BlueprintHome/Layout/{name}/blueprint_{i}.xml" };
                                        Slots.Add(bpslot);
                                        slotsUrlListDRM += bpslot.url;
                                        slotsXmlResult += $"<slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></slot>";
                                        File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/blueprint_{i}.xml", "<xml></xml>");
                                    }

                                    File.WriteAllText(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json", JsonConvert.SerializeObject(Slots));
                                }

                                if (File.Exists(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/CurrentSlot.txt") && int.TryParse(File.ReadAllText(apipath + $"/NDREAMS/BlueprintHome/Layout/{name}/CurrentSlot.txt"), out int value))
                                    Currentslot = value;

                                if (File.Exists(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/CurrentSlot.txt") && int.TryParse(File.ReadAllText(apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}/CurrentSlot.txt"), out int value1))
                                    Currentfurnslot = value1;

                                furnslotsXmlResult += "</furn_slots>";
                                slotsXmlResult += "</slots>";

                                return $"<xml><owner>{(!string.IsNullOrEmpty(owner) && name == owner ? "true" : "false")}</owner><max_slots>{MaxSlots}</max_slots><max_furn_slots>{MaxFurnSlots}</max_furn_slots>{slotsXmlResult}{furnslotsXmlResult}<current>{Currentslot}</current><current_furn_slot>{Currentfurnslot}</current_furn_slot><confirm>{NDREAMSServerUtils.Server_GetSignature("blueprint", slotsUrlListDRM, key, CurrentDate)}</confirm></xml>";
                            }
                            else
                            {
                                string errMsg = $"[nDreams] - Furniture: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                return $"<xml><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                            }
                    }
                }
            }

            return null;
        }
    }

    public class BluePrintSlots
    {
        public int position { get; set; } = 0;
        public string name { get; set; } = "NONE";
        public string url { get; set; }
        public string used { get; set; } = "false";
    }
}
