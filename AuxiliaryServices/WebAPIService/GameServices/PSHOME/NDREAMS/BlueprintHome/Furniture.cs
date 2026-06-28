using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.BlueprintHome
{
    public class Furniture
    {
        public const int MaxSlots = 5;
        public const int MaxFurnSlots = 5;

        public static string ProcessFurniture(
            DateTime CurrentDate,
            byte[] PostData,
            string ContentType,
            string baseurl,
            string apipath
        )
        {
            var blueprint_name = string.Empty;
            var name = string.Empty;
            var slot = string.Empty;
            var owner = string.Empty;
            var key = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    name = data.GetParameterValue("name");
                    var func = data.GetParameterValue("func");
                    key = data.GetParameterValue("key");

                    try
                    {
                        var territory = data.GetParameterValue("territory");
                        var style = data.GetParameterValue("style");
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
                        var blueprint_furn = data.GetParameterValue("blueprint_furn");
                    }
                    catch
                    {
                        // Not Important.
                    }

                    ms.Flush();

                    var ExpectedHash = string.Empty;

                    switch (func)
                    {
                        case "save_furniture":
                            ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                                "blueprint_" + blueprint_name,
                                name,
                                "furn_save_slot_" + slot + "_" + name,
                                CurrentDate
                            );

                            if (key == ExpectedHash)
                            {
                                byte[] BlueprintDataBytes = null;

                                foreach (var file in data.Files)
                                {
                                    using (var filedata = file.Data)
                                    {
                                        filedata.Position = 0;

                                        // Find the number of bytes in the stream
                                        var contentLength = (int)filedata.Length;

                                        // Create a byte array
                                        var buffer = new byte[contentLength];

                                        // Read the contents of the memory stream into the byte array
                                        filedata.Read(buffer, 0, contentLength);

                                        if (file.FileName == "blueprint_furn.dat")
                                            BlueprintDataBytes = buffer;

                                        filedata.Flush();
                                    }
                                }

                                if (BlueprintDataBytes != null && int.TryParse(slot, out var value))
                                {
                                    if (
                                        File.Exists(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"
                                        )
                                    )
                                    {
                                        var bpslots = JsonConvert.DeserializeObject<
                                            List<BluePrintSlots>
                                        >(
                                            File.ReadAllText(
                                                apipath
                                                    + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"
                                            )
                                        );

                                        if (bpslots != null)
                                        {
                                            var i = 0;
                                            foreach (var bpslot in bpslots)
                                            {
                                                if (bpslot.position == value)
                                                {
                                                    File.WriteAllText(
                                                        apipath
                                                            + $"/NDREAMS/BlueprintHome/Furniture/{name}/CurrentSlot.txt",
                                                        slot
                                                    );
                                                    File.WriteAllBytes(
                                                        apipath
                                                            + $"/NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{slot}.xml",
                                                        BlueprintDataBytes
                                                    );

                                                    bpslots[i] = new BluePrintSlots()
                                                    {
                                                        position = value,
                                                        name = blueprint_name,
                                                        url =
                                                            baseurl
                                                            + $"NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{value}.xml",
                                                        used = "true",
                                                    };

                                                    break;
                                                }

                                                i++;
                                            }

                                            File.WriteAllText(
                                                apipath
                                                    + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json",
                                                JsonConvert.SerializeObject(bpslots)
                                            );
                                        }
                                        else
                                        {
                                            var errMsg =
                                                $"[nDreams] - Furniture: The saving process errored out!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><error>Saving error</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                        }
                                    }
                                    else
                                    {
                                        var errMsg =
                                            $"[nDreams] - Furniture: Cannot save a slot while not being registered!";
                                        CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                        return $"<xml><error>Forbidden action</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                    }

                                    return "<xml></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[nDreams] - Furniture: Invalid Blueprint data sent for saving!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><error>Invalid Multi-Part File sent</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                }
                            }
                            else
                            {
                                var errMsg =
                                    $"[nDreams] - Furniture: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                return $"<xml><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                            }
                        case "save":
                            ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                                "blueprint_" + blueprint_name,
                                name,
                                "save_slot_" + slot + "_" + name,
                                CurrentDate
                            );

                            if (key == ExpectedHash)
                            {
                                byte[] BlueprintDataBytes = null;

                                foreach (var file in data.Files)
                                {
                                    using (var filedata = file.Data)
                                    {
                                        filedata.Position = 0;

                                        // Find the number of bytes in the stream
                                        var contentLength = (int)filedata.Length;

                                        // Create a byte array
                                        var buffer = new byte[contentLength];

                                        // Read the contents of the memory stream into the byte array
                                        filedata.Read(buffer, 0, contentLength);

                                        if (file.FileName == "blueprint.dat")
                                            BlueprintDataBytes = buffer;

                                        filedata.Flush();
                                    }
                                }

                                if (BlueprintDataBytes != null && int.TryParse(slot, out var value))
                                {
                                    if (
                                        File.Exists(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"
                                        )
                                    )
                                    {
                                        var bpslots = JsonConvert.DeserializeObject<
                                            List<BluePrintSlots>
                                        >(
                                            File.ReadAllText(
                                                apipath
                                                    + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"
                                            )
                                        );

                                        if (bpslots != null)
                                        {
                                            var i = 0;
                                            foreach (var bpslot in bpslots)
                                            {
                                                if (bpslot.position == value)
                                                {
                                                    File.WriteAllText(
                                                        apipath
                                                            + $"/NDREAMS/BlueprintHome/Layout/{name}/CurrentSlot.txt",
                                                        slot
                                                    );
                                                    File.WriteAllBytes(
                                                        apipath
                                                            + $"/NDREAMS/BlueprintHome/Layout/{name}/blueprint_{slot}.xml",
                                                        BlueprintDataBytes
                                                    );

                                                    bpslots[i] = new BluePrintSlots()
                                                    {
                                                        position = value,
                                                        name = blueprint_name,
                                                        url =
                                                            baseurl
                                                            + $"NDREAMS/BlueprintHome/Layout/{name}/blueprint_{value}.xml",
                                                        used = "true",
                                                    };

                                                    break;
                                                }

                                                i++;
                                            }

                                            File.WriteAllText(
                                                apipath
                                                    + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json",
                                                JsonConvert.SerializeObject(bpslots)
                                            );
                                        }
                                        else
                                        {
                                            var errMsg =
                                                $"[nDreams] - Furniture: The saving process errored out!";
                                            CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                            return $"<xml><error>Saving error</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                        }
                                    }
                                    else
                                    {
                                        var errMsg =
                                            $"[nDreams] - Furniture: Cannot save a slot while not being registered!";
                                        CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                        return $"<xml><error>Forbidden action</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                    }

                                    return "<xml></xml>";
                                }
                                else
                                {
                                    var errMsg =
                                        $"[nDreams] - Furniture: Invalid Blueprint data sent for saving!";
                                    CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                    return $"<xml><error>Invalid Multi-Part File sent</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                                }
                            }
                            else
                            {
                                var errMsg =
                                    $"[nDreams] - Furniture: invalid key sent! Received:{key} Expected:{ExpectedHash}";
                                CustomLogger.LoggerAccessor.LogWarn(errMsg);
                                return $"<xml><error>Signature Mismatch</error><extra>{errMsg}</extra><function>ProcessFurniture</function></xml>";
                            }
                        case "init":
                            ExpectedHash = NDREAMSServerUtils.Server_GetSignature(
                                "blueprint",
                                name,
                                owner,
                                CurrentDate
                            );

                            if (key == ExpectedHash)
                            {
                                var Currentfurnslot = 0;
                                var Currentslot = 0;
                                var furnslotsXmlResult = "<furn_slots>";
                                var slotsXmlResult = "<slots>";
                                var slotsUrlListDRM = string.Empty;

                                List<BluePrintSlots> furnSlots = [];
                                List<BluePrintSlots> Slots = [];

                                if (
                                    File.Exists(
                                        apipath
                                            + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"
                                    )
                                )
                                {
                                    var bpslots = JsonConvert.DeserializeObject<
                                        List<BluePrintSlots>
                                    >(
                                        File.ReadAllText(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json"
                                        )
                                    );

                                    if (bpslots != null)
                                    {
                                        foreach (var bpslot in bpslots)
                                        {
                                            furnSlots.Add(bpslot);
                                            furnslotsXmlResult +=
                                                $"<furn_slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></furn_slot>";
                                        }
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(
                                        apipath + $"/NDREAMS/BlueprintHome/Furniture/{name}"
                                    );

                                    for (var i = 1; i <= MaxFurnSlots; i++)
                                    {
                                        var bpslot = new BluePrintSlots()
                                        {
                                            position = i,
                                            url =
                                                baseurl
                                                + $"NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{i}.xml",
                                        };
                                        furnSlots.Add(bpslot);
                                        furnslotsXmlResult +=
                                            $"<furn_slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></furn_slot>";
                                        File.WriteAllText(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Furniture/{name}/blueprint_{i}.xml",
                                            "<xml></xml>"
                                        );
                                    }

                                    File.WriteAllText(
                                        apipath
                                            + $"/NDREAMS/BlueprintHome/Furniture/{name}/SlotData.json",
                                        JsonConvert.SerializeObject(furnSlots)
                                    );
                                }

                                if (
                                    File.Exists(
                                        apipath
                                            + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"
                                    )
                                )
                                {
                                    var bpslots = JsonConvert.DeserializeObject<
                                        List<BluePrintSlots>
                                    >(
                                        File.ReadAllText(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json"
                                        )
                                    );

                                    if (bpslots != null)
                                    {
                                        foreach (var bpslot in bpslots)
                                        {
                                            Slots.Add(bpslot);
                                            slotsUrlListDRM += bpslot.url;
                                            slotsXmlResult +=
                                                $"<slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></slot>";
                                        }
                                    }
                                }
                                else
                                {
                                    Directory.CreateDirectory(
                                        apipath + $"/NDREAMS/BlueprintHome/Layout/{name}"
                                    );

                                    for (var i = 1; i <= MaxSlots; i++)
                                    {
                                        var bpslot = new BluePrintSlots()
                                        {
                                            position = i,
                                            url =
                                                baseurl
                                                + $"NDREAMS/BlueprintHome/Layout/{name}/blueprint_{i}.xml",
                                        };
                                        Slots.Add(bpslot);
                                        slotsUrlListDRM += bpslot.url;
                                        slotsXmlResult +=
                                            $"<slot url=\"{bpslot.url}\"><name>{bpslot.name}</name><used>{bpslot.used}</used></slot>";
                                        File.WriteAllText(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Layout/{name}/blueprint_{i}.xml",
                                            "<xml></xml>"
                                        );
                                    }

                                    File.WriteAllText(
                                        apipath
                                            + $"/NDREAMS/BlueprintHome/Layout/{name}/SlotData.json",
                                        JsonConvert.SerializeObject(Slots)
                                    );
                                }

                                if (
                                    File.Exists(
                                        apipath
                                            + $"/NDREAMS/BlueprintHome/Layout/{name}/CurrentSlot.txt"
                                    )
                                    && int.TryParse(
                                        File.ReadAllText(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Layout/{name}/CurrentSlot.txt"
                                        ),
                                        out var value
                                    )
                                )
                                    Currentslot = value;

                                if (
                                    File.Exists(
                                        apipath
                                            + $"/NDREAMS/BlueprintHome/Furniture/{name}/CurrentSlot.txt"
                                    )
                                    && int.TryParse(
                                        File.ReadAllText(
                                            apipath
                                                + $"/NDREAMS/BlueprintHome/Furniture/{name}/CurrentSlot.txt"
                                        ),
                                        out var value1
                                    )
                                )
                                    Currentfurnslot = value1;

                                furnslotsXmlResult += "</furn_slots>";
                                slotsXmlResult += "</slots>";

                                return $"<xml><owner>{(!string.IsNullOrEmpty(owner) && name == owner ? "true" : "false")}</owner><max_slots>{MaxSlots}</max_slots><max_furn_slots>{MaxFurnSlots}</max_furn_slots>{slotsXmlResult}{furnslotsXmlResult}<current>{Currentslot}</current><current_furn_slot>{Currentfurnslot}</current_furn_slot><confirm>{NDREAMSServerUtils.Server_GetSignature("blueprint", slotsUrlListDRM, key, CurrentDate)}</confirm></xml>";
                            }
                            else
                            {
                                var errMsg =
                                    $"[nDreams] - Furniture: invalid key sent! Received:{key} Expected:{ExpectedHash}";
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
