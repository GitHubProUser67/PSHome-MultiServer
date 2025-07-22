using CustomLogger;
using System.IO;
using System.Net;
using WatsonWebserver.Core;
using WebAPIService.GameServices.OHS;

namespace ApacheNet.Extensions.Lockwood
{
    internal static class Sodium
    {
        public static void BuildSodiumPlugin(WebserverBase server)
        {
            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/webassets/Sodium/{project}/{version}/HoloTip_Data.xml", async (ctx) =>
            {
                string? project = ctx.Request.Url.Parameters["project"];
                if (string.IsNullOrEmpty(project))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string xmlPath = $"/webassets/Sodium/{project}/{ctx.Request.Url.Parameters["version"]}/HoloTip_Data.xml";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.ContentType = "text/xml";
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - HoloTip_Data data was not found for project:{project}, falling back to server file.");

                switch (project)
                {
                    case "sodium_blimp":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(@"<lua>
                    <SALES_04>
                        <radius type='num'>8.298</radius>
                        <dotFov type='num'>0.58</dotFov>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <textDataConditional>
                            <_>PURCHASE_ME_1</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <condition>
                                <_>AND</_>
                                <_>PLEB</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <textData>
                            <_>PURCHASE_ME_3</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <Y_step type='num'>-222.6</Y_step>
                            <itterations type='num'>8.16</itterations>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <jitterBias type='num'>1</jitterBias>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.60412</innerRadius>
                        <enableCondition>
                            <_>AND</_>
                            <_>PLEB</_>
                        </enableCondition>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <height type='num'>-0.9</height>
                        <look type='vec'>4.092,13.488,-12.977,0.000</look>
                        <tilt type='num'>359.2</tilt>
                        <pos type='vec'>5.448,13.029,-13.597,0.000</pos>
                    </SALES_04>
                    <PORTAL_JATOS_01>
                        <radius type='num'>8.298</radius>
                        <dotFov type='num'>0.74</dotFov>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <beamScale type='vec'>10.500,10.500,10.500,10.500</beamScale>
                        <enableCondition>
                            <_>AND</_>
                            <_>JATO</_>
                            <_>NOT</_>
                            <_>MINERALS</_>
                        </enableCondition>
                        <textData>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_2</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterBias type='num'>1</jitterBias>
                            <itterations type='num'>8.16</itterations>
                            <X_size type='num'>3.8327</X_size>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <tilt type='num'>21.3</tilt>
                        <height type='num'>-0.9</height>
                        <textDataConditional>
                            <_>PORTAL_HINT_7</_>
                            <_>PORTAL_HINT_8</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <condition>
                                <_>AND</_>
                                <_>JATO</_>
                                <_>NOT</_>
                                <_>MINERALS</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <look type='vec'>-8.445,5.728,-12.525,0.000</look>
                        <pos type='vec'>-10.345,5.757,-11.706,0.000</pos>
                    </PORTAL_JATOS_01>
                    <PILOT_01>
                        <tilt type='num'>21.3</tilt>
                        <pos type='vec'>-0.042,13.277,3.755,0.000</pos>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <dotFov type='num'>0.74</dotFov>
                        <look type='vec'>-0.014,13.217,2.241,0.000</look>
                        <textData>
                            <_>PURCHASE_ME_1</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterBias type='num'>1</jitterBias>
                            <itterations type='num'>8.16</itterations>
                            <X_size type='num'>3.8327</X_size>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <enableCondition>
                            <_>AND</_>
                            <_>PLEB</_>
                        </enableCondition>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <height type='num'>-0.9</height>
                        <textDataConditional>
                            <_>DENIED</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <condition>
                                <_>AND</_>
                                <_>PLEB</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <radius type='num'>8.298</radius>
                        <beamScale type='vec'>10.500,10.500,10.500,10.500</beamScale>
                    </PILOT_01>
                    <PORTAL_RES_01>
                        <radius type='num'>8.298</radius>
                        <beamScale type='vec'>10.500,10.500,10.500,10.500</beamScale>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <dotFov type='num'>0.74</dotFov>
                        <look type='vec'>-8.445,5.728,-12.525,0.000</look>
                        <height type='num'>-0.9</height>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <tilt type='num'>21.3</tilt>
                        <textData>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_2</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <Y_step type='num'>-222.6</Y_step>
                            <itterations type='num'>8.16</itterations>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <enableCondition>
                            <_>AND</_>
                            <_>NOT</_>
                            <_>JATO</_>
                            <_>MINERALS</_>
                        </enableCondition>
                        <textDataConditional>
                            <_>PORTAL_HINT_5</_>
                            <_>PORTAL_HINT_6</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <condition>
                                <_>AND</_>
                                <_>NOT</_>
                                <_>JATO</_>
                                <_>MINERALS</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <Y_step type='num'>-222.6</Y_step>
                            <jitterBias type='num'>1</jitterBias>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <pos type='vec'>-10.345,5.757,-11.706,0.000</pos>
                    </PORTAL_RES_01>
                    <PILOT_03>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>-1.039,3.324,16.767,0.000</pos>
                        <dotFov type='num'>-0.62</dotFov>
                        <enableCondition>
                            <_>AND</_>
                            <_>PLEB</_>
                        </enableCondition>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.461,0.000,0.194,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <height type='num'>1</height>
                        <tilt type='num'>357.6</tilt>
                        <textData>
                            <_>DENIED</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <Y_size type='num'>7.9428</Y_size>
                            <alignment>right</alignment>
                            <jitterHi type='num'>30.453</jitterHi>
                            <shadowColour type='vec'>0.801,0.433,0.000,0.311</shadowColour>
                            <itterations type='num'>8.16</itterations>
                            <X_size type='num'>7.4207</X_size>
                            <Y_step type='num'>-222.6</Y_step>
                            <colour type='vec'>0.960,0.864,0.000,1.000</colour>
                        </textData>
                        <look type='vec'>-0.992,4.036,15.758,0.000</look>
                        <beamSquash type='vec'>0.813,0.476,0.508,0.000</beamSquash>
                        <beamScale type='vec'>4.573,10.500,2.893,10.500</beamScale>
                    </PILOT_03>
                    <PORTAL_RES_02>
                        <radius type='num'>8.298</radius>
                        <dotFov type='num'>0.74</dotFov>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <beamScale type='vec'>10.500,10.500,10.500,10.500</beamScale>
                        <enableCondition>
                            <_>AND</_>
                            <_>NOT</_>
                            <_>JATO</_>
                            <_>MINERALS</_>
                        </enableCondition>
                        <textData>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_2</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterBias type='num'>1</jitterBias>
                            <itterations type='num'>8.16</itterations>
                            <X_size type='num'>3.8327</X_size>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <tilt type='num'>21.3</tilt>
                        <height type='num'>-0.9</height>
                        <textDataConditional>
                            <_>PORTAL_HINT_5</_>
                            <_>PORTAL_HINT_6</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <condition>
                                <_>AND</_>
                                <_>NOT</_>
                                <_>JATO</_>
                                <_>MINERALS</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <look type='vec'>-3.653,5.432,-9.088,0.000</look>
                        <pos type='vec'>-4.149,5.755,-6.054,0.000</pos>
                    </PORTAL_RES_02>
                    <PORTAL_DEFAULT_02>
                        <radius type='num'>8.298</radius>
                        <dotFov type='num'>0.74</dotFov>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <beamScale type='vec'>10.500,10.500,10.500,10.500</beamScale>
                        <enableCondition>
                            <_>AND</_>
                            <_>NOT</_>
                            <_>JATO</_>
                            <_>NOT</_>
                            <_>MINERALS</_>
                        </enableCondition>
                        <textData>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_2</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterBias type='num'>1</jitterBias>
                            <itterations type='num'>8.16</itterations>
                            <X_size type='num'>3.8327</X_size>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <tilt type='num'>21.3</tilt>
                        <height type='num'>-0.9</height>
                        <textDataConditional>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_4</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <condition>
                                <_>AND</_>
                                <_>NOT</_>
                                <_>JATO</_>
                                <_>NOT</_>
                                <_>MINERALS</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <look type='vec'>-2.519,5.615,-8.572,0.000</look>
                        <pos type='vec'>-4.149,5.755,-6.054,0.000</pos>
                    </PORTAL_DEFAULT_02>
                    <BINOCULAR_01>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>-10.239,13.029,-9.983,0.000</pos>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                        <look type='vec'>-8.837,13.432,-11.384,0.000</look>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <textData>
                            <_>BINOC_HINT</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <X_size type='num'>3.8327</X_size>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <tilt type='num'>359.2</tilt>
                        <height type='num'>-0.9</height>
                        <enableCondition>
                            <_>AND</_>
                            <_>ALWAYS</_>
                        </enableCondition>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <dotFov type='num'>0.74</dotFov>
                    </BINOCULAR_01>
                    <PORTAL_JATOS_02>
                        <tilt type='num'>21.3</tilt>
                        <beamScale type='vec'>10.500,10.500,10.500,10.500</beamScale>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <enableCondition>
                            <_>AND</_>
                            <_>JATO</_>
                            <_>NOT</_>
                            <_>MINERALS</_>
                        </enableCondition>
                        <height type='num'>-0.9</height>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <radius type='num'>8.298</radius>
                        <dotFov type='num'>0.74</dotFov>
                        <textData>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_2</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <Y_step type='num'>-222.6</Y_step>
                            <itterations type='num'>8.16</itterations>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <textDataConditional>
                            <_>PORTAL_HINT_7</_>
                            <_>PORTAL_HINT_8</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <condition>
                                <_>AND</_>
                                <_>JATO</_>
                                <_>NOT</_>
                                <_>MINERALS</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <jitterBias type='num'>1</jitterBias>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <look type='vec'>-3.049,5.616,-8.660,0.000</look>
                        <pos type='vec'>-4.149,5.755,-6.054,0.000</pos>
                    </PORTAL_JATOS_02>
                    <BINOCULAR_04>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>-6.553,8.176,-34.355,0.000</pos>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                        <look type='vec'>-4.821,8.477,-33.990,0.000</look>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <textData>
                            <_>BINOC_HINT</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <itterations type='num'>8.16</itterations>
                            <Y_step type='num'>-222.6</Y_step>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <tilt type='num'>359.2</tilt>
                        <height type='num'>-0.9</height>
                        <enableCondition>
                            <_>AND</_>
                            <_>ALWAYS</_>
                        </enableCondition>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <dotFov type='num'>0.74</dotFov>
                    </BINOCULAR_04>
                    <BINOCULAR_02>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>7.616,13.030,1.513,0.000</pos>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <look type='vec'>6.506,13.268,0.230,0.000</look>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <textData>
                            <_>BINOC_HINT</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <jitterBias type='num'>1</jitterBias>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                        <height type='num'>-0.9</height>
                        <enableCondition>
                            <_>AND</_>
                            <_>ALWAYS</_>
                        </enableCondition>
                        <tilt type='num'>359.2</tilt>
                        <dotFov type='num'>0.74</dotFov>
                    </BINOCULAR_02>
                    <TELEPORTER_PARKED>
                        <tilt type='num'>359.2</tilt>
                        <pos type='vec'>6.507,13.024,3.995,0.000</pos>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <look type='vec'>4.803,13.495,2.517,0.000</look>
                        <enableCondition>
                            <_>AND</_>
                            <_>TELEPORTER</_>
                        </enableCondition>
                        <height type='num'>-0.9</height>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                        <radius type='num'>8.298</radius>
                        <textData>
                            <_>TELEPORTER_HINT_3</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <Y_step type='num'>-222.6</Y_step>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <textDataConditional>
                            <_>TELEPORTER_HINT_4</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <condition>
                                <_>AND</_>
                                <_>TELEPORTER</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <dotFov type='num'>0.74</dotFov>
                    </TELEPORTER_PARKED>
                    <BINOCULAR_03>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>-6.452,5.755,-17.727,0.000</pos>
                        <tilt type='num'>359.2</tilt>
                        <look type='vec'>-5.461,6.177,-16.880,0.000</look>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <textData>
                            <_>BINOC_HINT</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <X_size type='num'>3.8327</X_size>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <height type='num'>-0.9</height>
                        <enableCondition>
                            <_>AND</_>
                            <_>ALWAYS</_>
                        </enableCondition>
                        <dotFov type='num'>0.74</dotFov>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                    </BINOCULAR_03>
                    <BINOCULAR_05>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>6.626,8.163,-33.107,0.000</pos>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                        <look type='vec'>5.315,8.449,-33.745,0.000</look>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <textData>
                            <_>BINOC_HINT</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <Y_step type='num'>-222.6</Y_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <tilt type='num'>359.2</tilt>
                        <height type='num'>-0.9</height>
                        <enableCondition>
                            <_>AND</_>
                            <_>ALWAYS</_>
                        </enableCondition>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <dotFov type='num'>0.74</dotFov>
                    </BINOCULAR_05>
                    <INFO_POINT>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>-7.475,13.042,3.772,0.000</pos>
                        <tilt type='num'>359.2</tilt>
                        <look type='vec'>-7.204,13.134,3.621,0.000</look>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <textData>
                            <_>INFO_HINT_1</_>
                            <Y_step type='num'>-222.6</Y_step>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <X_size type='num'>3.8327</X_size>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <height type='num'>-0.9</height>
                        <enableCondition>
                            <_>AND</_>
                            <_>ALWAYS</_>
                        </enableCondition>
                        <dotFov type='num'>0.74</dotFov>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                    </INFO_POINT>
                    <PILOT_02>
                        <radius type='num'>8.298</radius>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <pos type='vec'>0.960,3.328,16.767,0.000</pos>
                        <beamScale type='vec'>4.573,10.500,2.893,10.500</beamScale>
                        <enableCondition>
                            <_>AND</_>
                            <_>PLEB</_>
                        </enableCondition>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.461,0.000,0.194,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <height type='num'>1</height>
                        <tilt type='num'>357.6</tilt>
                        <textData>
                            <_>DENIED</_>
                            <X_size type='num'>7.4207</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <Y_size type='num'>7.9428</Y_size>
                            <jitterHi type='num'>30.453</jitterHi>
                            <alignment>left</alignment>
                            <shadowColour type='vec'>0.801,0.433,0.000,0.311</shadowColour>
                            <itterations type='num'>8.16</itterations>
                            <Y_step type='num'>-222.6</Y_step>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <colour type='vec'>0.960,0.864,0.000,1.000</colour>
                        </textData>
                        <look type='vec'>0.906,4.060,15.608,0.000</look>
                        <beamSquash type='vec'>0.813,0.476,0.508,0.000</beamSquash>
                        <dotFov type='num'>-0.78</dotFov>
                    </PILOT_02>
                    <SALES_01>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <dotFov type='num'>0.74</dotFov>
                        <textDataConditional>
                            <_>PURCHASE_ME_1</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <condition>
                                <_>AND</_>
                                <_>PLEB</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <Y_step type='num'>-222.6</Y_step>
                            <jitterHi type='num'>30.453</jitterHi>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <itterations type='num'>8.16</itterations>
                            <jitterBias type='num'>1</jitterBias>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <radius type='num'>8.298</radius>
                        <tilt type='num'>359.2</tilt>
                        <textData>
                            <_>PURCHASE_ME_2</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <maxWidth type='num'>1280</maxWidth>
                            <Y_step type='num'>-222.6</Y_step>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <pos type='vec'>-7.478,13.037,-2.730,0.000</pos>
                        <look type='vec'>-5.906,13.493,-3.382,0.000</look>
                        <height type='num'>-0.9</height>
                        <enableCondition>
                            <_>AND</_>
                            <_>PLEB</_>
                        </enableCondition>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                    </SALES_01>
                    <PORTAL_DEFAULT_01>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <beamScale type='vec'>10.500,10.500,10.500,10.500</beamScale>
                        <enableCondition>
                            <_>AND</_>
                            <_>NOT</_>
                            <_>JATO</_>
                            <_>NOT</_>
                            <_>MINERALS</_>
                        </enableCondition>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <radius type='num'>8.298</radius>
                        <tilt type='num'>21.3</tilt>
                        <dotFov type='num'>0.74</dotFov>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <look type='vec'>-8.430,5.514,-12.581,0.000</look>
                        <textData>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_2</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <X_size type='num'>3.8327</X_size>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterBias type='num'>1</jitterBias>
                            <itterations type='num'>8.16</itterations>
                            <Y_step type='num'>-222.6</Y_step>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <height type='num'>-0.9</height>
                        <textDataConditional>
                            <_>PORTAL_HINT_1</_>
                            <_>PORTAL_HINT_3</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <condition>
                                <_>AND</_>
                                <_>NOT</_>
                                <_>JATO</_>
                                <_>NOT</_>
                                <_>MINERALS</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <X_size type='num'>3.8327</X_size>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterBias type='num'>1</jitterBias>
                            <itterations type='num'>8.16</itterations>
                            <Y_step type='num'>-222.6</Y_step>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <pos type='vec'>-10.345,5.757,-11.706,0.000</pos>
                    </PORTAL_DEFAULT_01>
                    <TELEPORTER_MOVING>
                        <radius type='num'>8.298</radius>
                        <pos type='vec'>6.507,13.024,3.995,0.000</pos>
                        <scale type='vec'>0.457,0.450,0.247,1.000</scale>
                        <tilt type='num'>359.2</tilt>
                        <look type='vec'>4.803,13.495,2.517,0.000</look>
                        <height type='num'>-0.9</height>
                        <models>
                            <tintLevel type='num'>1.2975</tintLevel>
                            <beam>HoloBeam</beam>
                            <tint type='vec'>0.000,0.046,0.461,1.000</tint>
                            <screen>Holo_Screen1</screen>
                            <unit>HoloUnit</unit>
                        </models>
                        <innerRadius type='num'>0.98264</innerRadius>
                        <enableCondition>
                            <_>AND</_>
                            <_>NOT</_>
                            <_>TELEPORTER</_>
                        </enableCondition>
                        <beamSquash type='vec'>0.877,0.294,0.465,0.000</beamSquash>
                        <textData>
                            <_>TELEPORTER_HINT_1</_>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <X_step type='num'>0</X_step>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <X_size type='num'>3.8327</X_size>
                            <itterations type='num'>8.16</itterations>
                            <jitterBias type='num'>1</jitterBias>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textData>
                        <textDataConditional>
                            <_>TELEPORTER_HINT_2</_>
                            <X_size type='num'>3.8327</X_size>
                            <X_step type='num'>0</X_step>
                            <jitterBias type='num'>1</jitterBias>
                            <condition>
                                <_>AND</_>
                                <_>NOT</_>
                                <_>TELEPORTER</_>
                                <_>Timer_10sec</_>
                            </condition>
                            <maxWidth type='num'>1280</maxWidth>
                            <colour type='vec'>0.000,0.096,0.960,1.000</colour>
                            <jitterHi type='num'>30.453</jitterHi>
                            <jitterLow type='num'>1.8545</jitterLow>
                            <itterations type='num'>8.16</itterations>
                            <shadowColour type='vec'>0.000,0.320,0.801,0.311</shadowColour>
                            <Y_step type='num'>-222.6</Y_step>
                            <Y_size type='num'>10.574</Y_size>
                        </textDataConditional>
                        <dotFov type='num'>0.74</dotFov>
                        <beamScale type='vec'>5.959,5.613,13.926,10.500</beamScale>
                    </TELEPORTER_MOVING>
                </lua>");
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - HoloTip_Data data was not found for project:{project}!");
                        break;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.Send();
            });
        }

        public static void BuildSodiumBlimpPlugin(WebserverBase server)
        {
            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/webassets/Sodium/sodium_blimp/{version}/{xmldef}", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";

                string version = ctx.Request.Url.Parameters["version"] ?? "7";
                string xmldef = ctx.Request.Url.Parameters["xmldef"] ?? "en-US";

                string xmlPath = $"/webassets/Sodium/sodium_blimp/{version}/{xmldef}";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - SodiumBlimp regional definition data was not found for xmldef:{xmldef}, falling back to empty data.");

                await ctx.Response.Send();
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/SodiumBlimp/{version}/defs/{defs}", async (ctx) =>
            {
                string? defs = ctx.Request.Url.Parameters["defs"];
                if (string.IsNullOrEmpty(defs))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string xmlPath = $"/static/SodiumBlimp/{ctx.Request.Url.Parameters["version"]}/defs/{defs}";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.ContentType = "text/xml";
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - SodiumBlimp definition data was not found for defs:{defs}, falling back to static file.");

                switch (defs)
                {
                    case "dig_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                         local TableFromInput = {
                            defs = {
                                rewards = { 
                                    ""SURVEY_TABLE"",
                                    ""TECHSUIT_CONCEPT_WALL_ART_SILVER"",
                                    ""CHROME_CATSUIT_CONCEPT_WALL_ART"",
                                    ""TASET_ROBO_BEETLE_ORNAMENT_BLACK_N_WHITE"",
                                    ""FASON_ROBO_PANGOLIN_ORNAMENT_SILVER"",
                                    ""SMALL_FLORO_CACTUS_GREEN"",
                                    ""PROPULSE_TWO_SEATER_AISLE_CHAIR"",
                                    ""TECHSUIT_CONCEPT_WALL_ART_BLACK"",
                                    ""BUMBLEBEE_BODY_SUIT_CONCEPT_WALL_ART"",
                                    ""TASET_ROBO_BEETLE_ORNAMENT_BURNT_ORANGE"",
                                    ""FASON_ROBO_PANGOLIN_ORNAMENT_YELLOW"",
                                    ""SMALL_FLORO_CACTUS_ORANGE"",
                                    ""FUSELAGE_ACTIVE_OBJECT_BED"",
                                },
                                tolerance = 10,
                                resources = {
                                    silicon = { weighting = 100 },
						            silver = { weighting = 90 },
						            gold = { weighting = 80 },
						            JATO_basic_Sink = { weighting = 60 },
						            JATO_Ride_Sink = { weighting = 50 },
						            JATO_uber_Sink = { weighting = 20 },
						            JATO_SSRB_x_Sink = { weighting = 5 },
                                },
                                rockDanceProps = { 'ROCK_001','ROCK_001','ROCK_003','ROCK_004','ROCK_005','ROCK_006','ROCK_007','ROCK_008','ROCK_009','ROCK_010','ROCK_011' }
                            }
                        }

                        return Encode(TableFromInput, 4, 4)
                        "));
                        return;
                    case "sound_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                         local TableFromInput = {
                            defs = {
                                TELEPAD_HUM = {
                                    volume = 0.6,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 4,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                TELEPAD_POW = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 50,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                TELEPAD_S_POW = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 50,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_CHARGE = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_L_SERVO = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_STOMP = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_HOVER = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 1,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_HUM = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                ARROW_DEPLOY = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_DEPLOY = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                ARROW_SPIN = {
                                    volume = 1,
                                    pitch = 0.1,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_SPAWN = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                THUMPER_HOVER = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                BLIMP_WIND = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                MOVING_WIND = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                DIG = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                DIG_SUCCESS = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                DIG_FAIL = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                FANFARE = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                ODOMETER = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                MENU_ACCEPT = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                BINOC_ZOOM = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                BINOC_ROTATE = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                BLIMP_CANOPY = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                AMBIENT = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                },
                                ENGINE = {
                                    volume = 1,
                                    pitch = 0,
                                    falloff_start = 0,
                                    falloff_end = 100,
                                    pan = -1,
                                    allow_cull = false,
                                }
                            }
                        }

                        return Encode(TableFromInput, 4, 4)
                        "));
                                        return;
                    case "burner_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(@"<defs>
                            <burner>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>1.406,19.366,-4.964,0.000</pos>
                            </burner>
                            <burner_0>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>-3.220,19.366,-4.964,0.000</pos>
                            </burner_0>
                            <burner_1>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>-3.519,20.172,-7.094,0.000</pos>
                            </burner_1>
                            <burner_2>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>1.079,20.172,-7.094,0.000</pos>
                            </burner_2>
                            <burner_3>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>1.079,20.172,-11.102,0.000</pos>
                            </burner_3>
                            <burner_4>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>-3.519,20.172,-11.102,0.000</pos>
                            </burner_4>
                            <burner_5>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>-3.220,19.366,-1.000,0.000</pos>
                            </burner_5>
                            <burner_6>
                                <model>burner_001.mdl</model>
                                <pos type='vec'>1.406,19.366,-1.000,0.000</pos>
                            </burner_6>
                        </defs>");
                        return;
                    case "cloth_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(@"<defs>
                            <cloth>
                                <col type='vec'>1.000,0.000,0.000,1.000</col> <!-- Red -->
                                <model>streamer_small.mdl</model>
                                <pos type='vec'>0.000,16.980,-22.793,0.000</pos>
                            </cloth>
                            <cloth_0>
                                <col type='vec'>0.000,1.000,0.000,1.000</col> <!-- Green -->
                                <model>streamer_medium.mdl</model>
                                <pos type='vec'>8.143,11.467,-27.885,0.000</pos>
                            </cloth_0>
                            <cloth_1>
                                <col type='vec'>1.000,1.000,0.000,1.000</col> <!-- Yellow -->
                                <model>streamer_small.mdl</model>
                                <pos type='vec'>-0.019,0.496,1.324,0.000</pos>
                            </cloth_1>
                            <cloth_2>
                                <col type='vec'>0.000,1.000,1.000,1.000</col> <!-- Cyan -->
                                <model>streamer_large.mdl</model>
                                <pos type='vec'>4.119,7.023,-43.151,0.000</pos>
                                <rot type='vec'>-90.000,0.000,0.000,0.000</rot>
                            </cloth_2>
                            <cloth_3>
                                <col type='vec'>0.000,1.000,1.000,1.000</col> <!-- Cyan -->
                                <model>streamer_large.mdl</model>
                                <pos type='vec'>-4.119,7.023,-43.151,0.000</pos>
                                <rot type='vec'>-90.000,0.000,0.000,0.000</rot>
                            </cloth_3>
                            <cloth_4>
                                <col type='vec'>0.000,1.000,1.000,1.000</col> <!-- Cyan -->
                                <model>streamer_large.mdl</model>
                                <pos type='vec'>-10.197,4.607,-17.537,0.000</pos>
                                <rot type='vec'>-90.000,0.000,0.000,0.000</rot>
                            </cloth_4>
                            <cloth_5>
                                <col type='vec'>0.000,1.000,1.000,1.000</col> <!-- Cyan -->
                                <model>streamer_large.mdl</model>
                                <pos type='vec'>6.119,4.607,-17.537,0.000</pos>
                                <rot type='vec'>-90.000,0.000,0.000,0.000</rot>
                            </cloth_5>
                            <cloth_6>
                                <col type='vec'>0.000,1.000,1.000,1.000</col> <!-- Cyan -->
                                <model>streamer_medium.mdl</model>
                                <pos type='vec'>16.319,37.152,-21.773,0.000</pos>
                            </cloth_6>
                            <cloth_7>
                                <col type='vec'>0.000,1.000,1.000,1.000</col> <!-- Cyan -->
                                <model>streamer_medium.mdl</model>
                                <pos type='vec'>-16.259,37.152,-21.773,0.000</pos>
                            </cloth_7>
                            <cloth_8>
                                <col type='vec'>1.000,0.000,0.000,1.000</col> <!-- Red -->
                                <model>streamer_small.mdl</model>
                                <pos type='vec'>5.334,18.312,-22.910,0.000</pos>
                            </cloth_8>
                            <cloth_9>
                                <col type='vec'>1.000,0.000,0.000,1.000</col> <!-- Red -->
                                <model>streamer_small.mdl</model>
                                <pos type='vec'>-4.892,18.205,-22.910,0.000</pos>
                            </cloth_9>
                            <cloth_10>
                                <col type='vec'>1.000,0.000,0.000,1.000</col> <!-- Red -->
                                <model>streamer_small.mdl</model>
                                <pos type='vec'>-6.989,18.894,20.017,0.000</pos>
                            </cloth_10>
                            <cloth_11>
                                <col type='vec'>1.000,0.000,0.000,1.000</col> <!-- Red -->
                                <model>streamer_small.mdl</model>
                                <pos type='vec'>6.989,18.894,20.017,0.000</pos>
                            </cloth_11>
                            <cloth_12>
                                <col type='vec'>1.000,1.000,1.000,1.000</col> <!-- White -->
                                <model>streamer_medium.mdl</model>
                                <pos type='vec'>11.699,3.213,1.252,0.000</pos>
                            </cloth_12>
                            <cloth_13>
                                <col type='vec'>1.000,1.000,1.000,1.000</col> <!-- White -->
                                <model>streamer_medium.mdl</model>
                                <pos type='vec'>-11.699,3.213,1.252,0.000</pos>
                            </cloth_13>
                        </defs>");
                        return;
                    case "light_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(@"<defs>
                            <light_dummy>
                                <atten_end type='num'>0</atten_end>
                                <atten_power type='num'>0</atten_power>
                                <atten_start type='num'>0</atten_start>
                                <col type='vec'>0.000,0.000,0.000,0.000</col>
                                <light_type>point</light_type>
                                <pos type='vec'>0.000,0.000,0.000,0.000</pos>
                                <mask>mask_002.dds</mask>
                            </light_dummy>
                        </defs>");
                        return;
                    case "gfx_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                         local TableFromInput = {
                            defs = {
                                CLOUD_002 = {
                                    efx = ""cloud_002.efx"",
                                    count = 3,
                                    attach_type = ""render""
                                },
                                BUY_BAG = {
                                    efx = ""glowser.efx"",
                                    count = 1,
                                    attach_type = ""emitter""
                                },
                                CLOUD_003 = {
                                    efx = ""cloud_003.efx"",
                                    count = 4,
                                    attach_type = ""render""
                                },
                                DIG = {
                                    efx = ""dig.efx"",
                                    count = 10,
                                    attach_type = ""emitter""
                                },
                                DUST_001 = {
                                    efx = ""dust_cloud.efx"",
                                    count = 5,
                                    attach_type = ""render""
                                },
                                GEYSER_001_JET = {
                                    efx = ""geyser_001_jet.efx"",
                                    count = 3,
                                    attach_type = ""emitter""
                                },
                                GEYSER_001_STEAM = {
                                    efx = ""geyser_001_steam.efx"",
                                    count = 6,
                                    attach_type = ""emitter""
                                },
                                THUMPER_HOVER = {
                                    efx = ""hoverBlast.efx"",
                                    count = 5,
                                    attach_type = ""emitter""
                                },
                                TELEPAD = {
                                    efx = ""telepad_send.efx"",
                                    count = 1,
                                    attach_type = ""emitter""
                                },
                                THUMPER_CHARGE = {
                                    efx = ""thumperCharge.efx"",
                                    count = 1,
                                    attach_type = ""emitter""
                                },
                                THUMPER_THUD = {
                                    efx = ""thumperThud.efx"",
                                    count = 1,
                                    attach_type = ""emitter""
                                },
                                WIND_STREAK = {
                                    efx = ""windStreak.efx"",
                                    count = 3,
                                    attach_type = ""emitter""
                                },
                            }
                        }

                        return Encode(TableFromInput, 4, 4)
                        "));
                        return;
                    case "map_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                [""def""] = {
                                    [""cloud_gfx""] = { ""CLOUD_002"", ""CLOUD_003"" },
                                    [""horizon_inner""] = ""inner_ring.mdl"",
                                    [""horizon_centre""] = ""centre_ring.mdl"",
                                    [""horizon_outer""] = ""outer_ring.mdl"",
                                    [""master_seed""] = 15959,
                                    [""max_cloud_height""] = 5600,
                                    [""min_cloud_height""] = 700,
                                    [""sky_model""] = ""sky_001.mdl"",
                                    [""terrain_collision""] = ""terrain_001.hkx"",
                                    [""terrain_model""] = ""terrain_002.mdl"",
                                    [""terrain_radius""] = 1000000,
                                    [""terrain_tile_size""] = 10000
                                }
                            }

                            return Encode(TableFromInput, 4, 4)
                            "));
                        return;
                    case "blimp_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(@"<defs>
                            <fwd_accel type='num'>300</fwd_accel>
                            <fwd_speed type='num'>999</fwd_speed>
                            <max_height type='num'>5050</max_height>
                            <min_height type='num'>20</min_height>
                            <pitch_speed type='num'>250</pitch_speed>
                            <start_height type='num'>30</start_height>
                            <streaks>
                                <_ type='vec'>-12.557,30.923,-47.163,0.000</_>
                                <_ type='vec'>12.557,30.923,-47.163,0.000</_>
                            </streaks>
                            <turn_speed type='num'>30</turn_speed>
                        </defs>");
                        return;
                    case "prop_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                [""defs""] = {
                                    [""PROPS""] = {},
                                    [""GROUPS""] = { dummy = { weight = 100 } }
                                }
                            }

                            return Encode(TableFromInput, 4, 4)
                            "));
                        return;
                    case "craft_defs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                [""defs""] = {
                                    [""SKIN_basic""] = {
                                        [""skin_texture""] = ""jetcraft_body_d.dds""
                                    },
                                    [""SKIN_basic_01""] = {
                                        [""skin_texture""] = ""jet_skin_01_d.dds""
                                    },
                                    [""SKIN_basic_02""] = {
                                        [""skin_texture""] = ""jet_skin_02_d.dds""
                                    },
                                    [""SKIN_basic_03""] = {
                                        [""skin_texture""] = ""jet_skin_03_d.dds""
                                    },
                                    [""SKIN_basic_04""] = {
                                        [""skin_texture""] = ""jet_skin_04_d.dds""
                                    },
                                    [""SKIN_basic_05""] = {
                                        [""skin_texture""] = ""jet_skin_05_d.dds""
                                    },
                                    [""SKIN_basic_06""] = {
                                        [""skin_texture""] = ""jet_skin_06_d.dds""
                                    },
                                    [""SKIN_basic_07""] = {
                                        [""skin_texture""] = ""jet_skin_07_d.dds""
                                    },
                                    [""SKIN_basic_08""] = {
                                        [""skin_texture""] = ""jet_skin_08_d.dds""
                                    },
                                    [""SKIN_basic_09""] = {
                                        [""skin_texture""] = ""jet_skin_09_d.dds""
                                    },
                                    [""SKIN_basic_10""] = {
                                        [""skin_texture""] = ""jet_skin_10_d.dds""
                                    },
                                    [""SKIN_basic_11""] = {
                                        [""skin_texture""] = ""jet_skin_11_d.dds""
                                    },
                                    [""SKIN_basic_12""] = {
                                        [""skin_texture""] = ""jet_skin_12_d.dds""
                                    },
                                    [""SKIN_basic_13""] = {
                                        [""skin_texture""] = ""jet_skin_13_d.dds""
                                    },
                                    [""SKIN_basic_14""] = {
                                        [""skin_texture""] = ""jet_skin_14_d.dds""
                                    },
                                    [""SKIN_basic_15""] = {
                                        [""skin_texture""] = ""jet_skin_15_d.dds""
                                    },
                                    [""SKIN_basic_16""] = {
                                        [""skin_texture""] = ""jet_skin_16_d.dds""
                                    },
                                    [""SKIN_basic_17""] = {
                                        [""skin_texture""] = ""jet_skin_17_d.dds""
                                    },
                                    [""SKIN_basic_18""] = {
                                        [""skin_texture""] = ""jet_skin_18_d.dds""
                                    },
                                    [""SKIN_basic_19""] = {
                                        [""skin_texture""] = ""jet_skin_19_d.dds""
                                    },
                                }
                            }

                            return Encode(TableFromInput, 4, 4)
                            "));
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - SodiumBlimp definition data was not found for defs:{defs}!");
                        break;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.Send();
            });
        }

        public static void BuildSodium2Plugin(WebserverBase server)
        {
            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/webassets/Sodium/sodium2/scores/{version}/{lang}", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";

                string xmlPath = $"/webassets/Sodium/sodium2/scores/{ctx.Request.Url.Parameters["version"]}/{ctx.Request.Url.Parameters["lang"]}";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - sodium2 score definition data was not found, falling back to static file.");

                await ctx.Response.Send(@"
                    <?xml version='1.0' encoding='UTF-8'?>
                    <!--Archived from: https://web.archive.org/web/20110110184845/http://www.outso-srv1.com:80/webassets/Sodium/sodium2/scores/v1.0/en-GB.xml-->
                    <data>
                        <CompletedRace>
                            <id>1</id>
                            <bling>0</bling>
                            <text>Completed race</text>
                            <xp>15</xp>
                            <xp_max>100</xp_max>
                            <xp_curve>1</xp_curve>
                            <cr>2</cr>
                            <cr_max>10</cr_max>
                            <cr_curve>1</cr_curve>
                            <Evaluate>lerpf( c.rating, 0, 1, 0.3, 1 ) </Evaluate>
                        </CompletedRace>
                        <Single_FirstPlace>
                            <id>2</id>
                            <group>Single</group>
                            <bling>2</bling>
                            <text>Victory!</text>
                            <xp>40</xp>
                            <cr>25</cr>
                            <Evaluate>c.single and (c.place == 1)</Evaluate>
                        </Single_FirstPlace>
                        <Single_SecondPlace>
                            <id>3</id>
                            <group>Single</group>
                            <bling>1</bling>
                            <text>Finished second place</text>
                            <xp>40</xp>
                            <cr>25</cr>
                            <Evaluate>c.single and (c.place == 2)</Evaluate>
                        </Single_SecondPlace>
                        <Single_ThirdPlace>
                            <id>4</id>
                            <group>Single</group>
                            <bling>0</bling>
                            <text>3rd.. For the cause</text>
                            <xp>20</xp>
                            <cr>6</cr>
                            <Evaluate>c.single and (c.place == 3)</Evaluate>
                        </Single_ThirdPlace>
                        <MovingUpInStyle_Challenge>
                            <id>5</id>
                            <group>Place_Challenge</group>
                            <bling>3</bling>
                            <text>Moving up in style</text>
                            <xp>50</xp>
                            <cr>10</cr>
                            <Evaluate>c.multi and (c.jatosUsed == 2) and (c.ghosts == 2) and (c.place == 1) and c.rating</Evaluate>
                        </MovingUpInStyle_Challenge>
                        <MovingUp_Challenge>
                            <id>6</id>
                            <group>Place_Challenge</group>
                            <bling>2</bling>
                            <text>Moving up in the world</text>
                            <xp>50</xp>
                            <cr>10</cr>
                            <Evaluate>c.multi and (c.ghosts == 2) and (c.place == 1) and c.rating</Evaluate>
                        </MovingUp_Challenge>
                        <StandingGroundInStyle_Challenge>
                            <id>7</id>
                            <group>Place_Challenge</group>
                            <bling>2</bling>
                            <text>Standing your ground in style</text>
                            <xp>50</xp>
                            <cr>5</cr>
                            <Evaluate>c.multi and (c.ghosts == 2) and (c.place == 2) and c.rating</Evaluate>
                        </StandingGroundInStyle_Challenge>
                        <StandingGround_Challenge>
                            <id>8</id>
                            <group>Place_Challenge</group>
                            <bling>1</bling>
                            <text>Standing your ground</text>
                            <xp>25</xp>
                            <cr>5</cr>
                            <Evaluate>c.multi and (c.ghosts == 2) and (c.place == 2) and c.rating</Evaluate>
                        </StandingGround_Challenge>
                        <RecordTime_World>
                            <id>9</id>
                            <group>RecordTime</group>
                            <bling>4</bling>
                            <text>WORLD RECORD LAP TIME!</text>
                            <xp>250</xp>
                            <cr>400</cr>
                            <Evaluate>c.time &lt; t.world</Evaluate>
                        </RecordTime_World>
                        <RecordTime_Daily>
                            <id>10</id>
                            <group>RecordTime</group>
                            <bling>3</bling>
                            <text>DAILY WORLD RECORD LAP TIME!</text>
                            <xp>250</xp>
                            <cr>400</cr>
                            <Evaluate>c.time &lt; t.daily</Evaluate>
                        </RecordTime_Daily>
                        <RecordTime_Friend>
                            <id>11</id>
                            <group>RecordTime</group>
                            <bling>2</bling>
                            <text>FRIEND BOARD LAP TIME RECORD</text>
                            <xp>100</xp>
                            <cr>150</cr>
                            <Evaluate>(t.numFriends&gt;0) and (c.time &lt; t.friend) and ((1+c.rating)*0.5)</Evaluate>
                        </RecordTime_Friend>
                        <RecordTime_Personal>
                            <id>12</id>
                            <group>RecordTime</group>
                            <bling>1</bling>
                            <text>Personal best time</text>
                            <xp>100</xp>
                            <cr>150</cr>
                            <Evaluate>c.time &lt; t.user and c.rating</Evaluate>
                        </RecordTime_Personal>
                        <RecordSpeed_World>
                            <id>13</id>
                            <group>RecordSpeed</group>
                            <bling>4</bling>
                            <text>WORLD SPEED RECORD!</text>
                            <xp>150</xp>
                            <cr>250</cr>
                            <Evaluate>c.speed &gt; s.world</Evaluate>
                        </RecordSpeed_World>
                        <RecordSpeed_Daily>
                            <id>14</id>
                            <group>RecordSpeed</group>
                            <bling>3</bling>
                            <text>DAILY WORLD SPEED RECORD!</text>
                            <xp>150</xp>
                            <cr>250</cr>
                            <Evaluate>c.speed &gt; s.daily</Evaluate>
                        </RecordSpeed_Daily>
                        <RecordSpeed_Friend>
                            <id>15</id>
                            <group>RecordSpeed</group>
                            <bling>2</bling>
                            <text>FRIEND BOARD SPEED RECORD</text>
                            <xp>50</xp>
                            <cr>100</cr>
                            <Evaluate>(s.numFriends&gt;0) and (c.speed &gt; s.friend)</Evaluate>
                        </RecordSpeed_Friend>
                        <RecordSpeed_Personal>
                            <id>16</id>
                            <group>RecordSpeed</group>
                            <bling>1</bling>
                            <text>Personal speed record</text>
                            <xp>50</xp>
                            <cr>100</cr>
                            <Evaluate>c.speed &gt; s.user</Evaluate>
                        </RecordSpeed_Personal>
                        <ExpertStyle>
                            <id>17</id>
                            <group>Style</group>
                            <bling>2</bling>
                            <text>Ballistic finish style bonus</text>
                            <xp>125</xp>
                            <xp_max>150</xp_max>
                            <cr>15</cr>
                            <cr_max>15</cr_max>
                            <Evaluate>c.jatoFinish and c.noBreaks and c.rating</Evaluate>
                        </ExpertStyle>
                        <InStyle>
                            <id>18</id>
                            <group>Style</group>
                            <bling>1</bling>
                            <text>Rocket finish bonus</text>
                            <xp>50</xp>
                            <xp_max>100</xp_max>
                            <cr>10</cr>
                            <cr_max>10</cr_max>
                            <Evaluate>c.jatoFinish and (c.rating+0.01)</Evaluate>
                        </InStyle>
                        <NoBreaks>
                            <id>19</id>
                            <group>Style</group>
                            <bling>1</bling>
                            <text>Untouched airbreaks</text>
                            <xp>75</xp>
                            <cr>17</cr>
                            <Evaluate>c.noBreaks and c.rating</Evaluate>
                        </NoBreaks>
                        <MadAirBonus>
                            <id>20</id>
                            <group>Air</group>
                            <bling>2</bling>
                            <text>Flying Scotsman</text>
                            <xp>50</xp>
                            <xp_max>100</xp_max>
                            <xp_curve>1</xp_curve>
                            <cr>15</cr>
                            <cr_max>10</cr_max>
                            <Evaluate>lerpf( c.airDist, 7000, 10000, 0, 1 )</Evaluate>
                        </MadAirBonus>
                        <AirBonus>
                            <id>21</id>
                            <group>Air</group>
                            <bling>1</bling>
                            <text>Air distance bonus</text>
                            <xp>25</xp>
                            <xp_max>50</xp_max>
                            <xp_curve>1</xp_curve>
                            <cr>5</cr>
                            <cr_max>5</cr_max>
                            <Evaluate>lerpf( c.airDist, 2000, 8000, 0, 1 )</Evaluate>
                        </AirBonus>
                        <SuperSonic>
                            <id>22</id>
                            <group>Sonic</group>
                            <bling>1</bling>
                            <text>Stayed supersonic</text>
                            <xp>150</xp>
                            <cr>32</cr>
                            <Evaluate>(c.boom == 1) and (c.subSonic == 0) and (c.firstBoomDist&lt;2000)</Evaluate>
                        </SuperSonic>
                        <BarrierDancing>
                            <id>23</id>
                            <group>Sonic</group>
                            <bling>2</bling>
                            <text>Messing with sound</text>
                            <xp>0</xp>
                            <xp_max>250</xp_max>
                            <xp_curve>3</xp_curve>
                            <cr>0</cr>
                            <cr_max>25</cr_max>
                            <cr_curve>3</cr_curve>
                            <Evaluate>lerpf( c.boom, 10, 40, 0, 1 )</Evaluate>
                        </BarrierDancing>
                        <Solo_HardTraining>
                            <id>24</id>
                            <group>Solo</group>
                            <bling>2</bling>
                            <text>Rocket Man! </text>
                            <xp>0</xp>
                            <xp_max>200</xp_max>
                            <xp_curve>2</xp_curve>
                            <cr>0</cr>
                            <cr_max>20</cr_max>
                            <cr_curve>2</cr_curve>
                            <Evaluate>c.solo and ( c.jatosUsed &gt; 1) and (c.ghosts &gt; 1) and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </Solo_HardTraining>
                        <Solo_SelfImprovement>
                            <id>25</id>
                            <group>Solo</group>
                            <bling>1</bling>
                            <text>Honing your skills</text>
                            <xp>0</xp>
                            <xp_max>200</xp_max>
                            <xp_curve>2</xp_curve>
                            <cr>0</cr>
                            <cr_max>20</cr_max>
                            <cr_curve>2</cr_curve>
                            <Evaluate>c.solo and (c.ghosts &gt; 1) and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </Solo_SelfImprovement>
                        <NinjaSkillBonus>
                            <id>26</id>
                            <group>Skill</group>
                            <bling>3</bling>
                            <text>Ninja skill bonus!</text>
                            <xp>200</xp>
                            <xp_max>300</xp_max>
                            <xp_curve>1</xp_curve>
                            <cr>50</cr>
                            <cr_max>30</cr_max>
                            <cr_curve>1</cr_curve>
                            <Evaluate>c.onTrack and (c.jatosUsed &gt; 1) and (c.place &lt; 2) and (c.ghosts &gt; 1) and (c.boom == 1) and (c.subSonic == 0) and (c.rating &gt; 0.3) and c.rating</Evaluate>
                        </NinjaSkillBonus>
                        <ExpertSkillBonus>
                            <id>27</id>
                            <group>Skill</group>
                            <bling>2</bling>
                            <text>Expert skill bonus</text>
                            <xp>100</xp>
                            <xp_max>150</xp_max>
                            <xp_curve>1</xp_curve>
                            <cr>25</cr>
                            <cr_max>15</cr_max>
                            <cr_curve>1</cr_curve>
                            <Evaluate>c.onTrack and (c.jatosUsed &gt; 0) and (c.place &lt; 2) and (c.ghosts &gt; 0) and (c.rating &gt; 0.3) and c.rating</Evaluate>
                        </ExpertSkillBonus>
                        <SkillBonus>
                            <id>28</id>
                            <group>Skill</group>
                            <bling>1</bling>
                            <text>Skill bonus</text>
                            <xp>50</xp>
                            <xp_max>100</xp_max>
                            <xp_curve>1</xp_curve>
                            <cr>12</cr>
                            <cr_max>10</cr_max>
                            <cr_curve>1</cr_curve>
                            <Evaluate>c.onTrack and (c.jatosUsed &gt; 0) and (c.place &lt; 3) and (c.ghosts &gt; 0) and c.rating</Evaluate>
                        </SkillBonus>
                        <FinishWithABang>
                            <id>29</id>
                            <bling>2</bling>
                            <text>Finished with a bang</text>
                            <xp>50</xp>
                            <xp_max>150</xp_max>
                            <xp_curve>2</xp_curve>
                            <cr>0</cr>
                            <cr_max>15</cr_max>
                            <cr_curve>2</cr_curve>
                            <Evaluate>(c.finishBangDist &lt; 1000) and lerpf( c.finishBangDist, 1000, 0, 0, 1 )</Evaluate>
                        </FinishWithABang>
                        <HDC_Champion>
                            <id>30</id>
                            <group>HDC</group>
                            <bling>4</bling>
                            <text>HDC  world champion</text>
                            <cr>150</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.hdc &gt; 4) and (c.time &lt; t.world) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </HDC_Champion>
                        <HDC_DailyChampion>
                            <id>31</id>
                            <group>HDC</group>
                            <bling>4</bling>
                            <text>HDC daily world champion</text>
                            <cr>75</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.hdc &gt; 4) and (c.time &lt; t.daily) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </HDC_DailyChampion>
                        <HDC_FullHouseGoldenBoy>
                            <id>32</id>
                            <group>HDC</group>
                            <bling>4</bling>
                            <text>HDC  golden boy</text>
                            <cr>50</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.hdc &gt; 4) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </HDC_FullHouseGoldenBoy>
                        <HDC_FullHouseExpertShowman>
                            <id>33</id>
                            <group>HDC</group>
                            <bling>3</bling>
                            <text>HDC Sponsored expert showman</text>
                            <cr>45</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.hdc &gt; 4) and (c.rating &gt; 0.3) and (c.finishBang &lt; 1000) and (lerpf( c.finishBang, 1000, 0, 0, 1 ) * c.rating)</Evaluate>
                        </HDC_FullHouseExpertShowman>
                        <HDC_FullHouseFavourite>
                            <id>34</id>
                            <group>HDC</group>
                            <bling>2</bling>
                            <text>HDC Sponsored favourite</text>
                            <cr>35</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.hdc &gt; 4) and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </HDC_FullHouseFavourite>
                        <HDC_FullHouseShowman>
                            <id>35</id>
                            <group>HDC</group>
                            <bling>2</bling>
                            <text>HDC Sponsored showman</text>
                            <cr>30</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.hdc &gt; 4) and (c.finishBangDist &lt; 1000) and lerpf( c.finishBang, 1000, 0, 0, 1 )</Evaluate>
                        </HDC_FullHouseShowman>
                        <HDC_Sponsored>
                            <id>36</id>
                            <group>HDC</group>
                            <bling>1</bling>
                            <text>HDC Sponsored</text>
                            <cr>5</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.hdc &gt; 0) and (c.rating+0.2 * lerpf( c.hdc, 0, 5, 0, 1 ))</Evaluate>
                        </HDC_Sponsored>
                        <RYLEE_Champion>
                            <id>37</id>
                            <group>RYLEE</group>
                            <bling>4</bling>
                            <text>RYLEE  world champion</text>
                            <cr>150</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.rylee &gt; 4) and (c.time &lt; t.world) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </RYLEE_Champion>
                        <RYLEE_DailyChampion>
                            <id>38</id>
                            <group>RYLEE</group>
                            <bling>4</bling>
                            <text>RYLEE daily world champion</text>
                            <cr>75</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.rylee &gt; 4) and (c.time &lt; t.daily) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </RYLEE_DailyChampion>
                        <RYLEE_FullHouseGoldenBoy>
                            <id>39</id>
                            <group>RYLEE</group>
                            <bling>4</bling>
                            <text>RYLEE  golden boy</text>
                            <cr>50</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.rylee &gt; 4) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </RYLEE_FullHouseGoldenBoy>
                        <RYLEE_FullHouseExpertShowman>
                            <id>40</id>
                            <group>RYLEE</group>
                            <bling>3</bling>
                            <text>RYLEE Sponsored expert showman</text>
                            <cr>45</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.rylee &gt; 4) and (c.rating &gt; 0.3) and (c.finishBang &lt; 1000) and (lerpf(c.finishBang, 1000, 0, 0, 1) * c.rating)</Evaluate>
                        </RYLEE_FullHouseExpertShowman>
                        <RYLEE_FullHouseFavourite>
                            <id>41</id>
                            <group>RYLEE</group>
                            <bling>2</bling>
                            <text>RYLEE Sponsored favourite</text>
                            <cr>35</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.rylee &gt; 4) and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </RYLEE_FullHouseFavourite>
                        <RYLEE_FullHouseShowman>
                            <id>42</id>
                            <group>RYLEE</group>
                            <bling>2</bling>
                            <text>RYLEE Sponsored showman</text>
                            <cr>30</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.rylee &gt; 4) and (c.finishBangDist &lt; 1000) and lerpf(c.finishBang, 1000, 0, 0, 1)</Evaluate>
                        </RYLEE_FullHouseShowman>
                        <RYLEE_Sponsored>
                            <id>43</id>
                            <group>RYLEE</group>
                            <bling>1</bling>
                            <text>RYLEE Sponsored</text>
                            <cr>5</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.rylee &gt; 0) and (c.rating+0.2 * lerpf(c.rylee, 0, 5, 0, 1))</Evaluate>
                        </RYLEE_Sponsored>
                        <JETBOX_Champion>
                            <id>44</id>
                            <group>JETBOX</group>
                            <bling>4</bling>
                            <text>JETBOX  world champion</text>
                            <cr>150</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.jetbox &gt; 4) and (c.time &lt; t.world) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </JETBOX_Champion>
                        <JETBOX_DailyChampion>
                            <id>45</id>
                            <group>JETBOX</group>
                            <bling>4</bling>
                            <text>JETBOX daily world champion</text>
                            <cr>75</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.jetbox &gt; 4) and (c.time &lt; t.daily) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </JETBOX_DailyChampion>
                        <JETBOX_FullHouseGoldenBoy>
                            <id>46</id>
                            <group>JETBOX</group>
                            <bling>4</bling>
                            <text>JETBOX  golden boy</text>
                            <cr>50</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.jetbox &gt; 4) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </JETBOX_FullHouseGoldenBoy>
                        <JETBOX_FullHouseExpertShowman>
                            <id>47</id>
                            <group>JETBOX</group>
                            <bling>3</bling>
                            <text>JETBOX Sponsored expert showman</text>
                            <cr>45</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.jetbox &gt; 4) and (c.rating &gt; 0.3) and (c.finishBang &lt; 1000) and (lerpf(c.finishBang, 1000, 0, 0, 1) * c.rating)</Evaluate>
                        </JETBOX_FullHouseExpertShowman>
                        <JETBOX_FullHouseFavourite>
                            <id>48</id>
                            <group>JETBOX</group>
                            <bling>2</bling>
                            <text>JETBOX Sponsored favourite</text>
                            <cr>35</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.jetbox &gt; 4) and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </JETBOX_FullHouseFavourite>
                        <JETBOX_FullHouseShowman>
                            <id>49</id>
                            <group>JETBOX</group>
                            <bling>2</bling>
                            <text>JETBOX Sponsored showman</text>
                            <cr>30</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.jetbox &gt; 4) and (c.finishBangDist &lt; 1000) and lerpf(c.finishBang, 1000, 0, 0, 1)</Evaluate>
                        </JETBOX_FullHouseShowman>
                        <JETBOX_Sponsored>
                            <id>50</id>
                            <group>JETBOX</group>
                            <bling>1</bling>
                            <text>JETBOX Sponsored</text>
                            <cr>5</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.jetbox &gt; 0) and (c.rating+0.2 * lerpf(c.jetbox, 0, 5, 0, 1))</Evaluate>
                        </JETBOX_Sponsored>
                        <LEXAN_Champion>
                            <id>51</id>
                            <group>LEXAN</group>
                            <bling>4</bling>
                            <text>LEXAN  world champion</text>
                            <cr>150</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.lexan &gt; 4) and (c.time &lt; t.world) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </LEXAN_Champion>
                        <LEXAN_DailyChampion>
                            <id>52</id>
                            <group>LEXAN</group>
                            <bling>4</bling>
                            <text>LEXAN daily world champion</text>
                            <cr>75</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.lexan &gt; 4) and (c.time &lt; t.daily) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </LEXAN_DailyChampion>
                        <LEXAN_FullHouseGoldenBoy>
                            <id>53</id>
                            <group>LEXAN</group>
                            <bling>4</bling>
                            <text>LEXAN  golden boy</text>
                            <cr>50</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.lexan &gt; 4) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </LEXAN_FullHouseGoldenBoy>
                        <LEXAN_FullHouseExpertShowman>
                            <id>54</id>
                            <group>LEXAN</group>
                            <bling>3</bling>
                            <text>LEXAN Sponsored expert showman</text>
                            <cr>45</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.lexan &gt; 4) and (c.rating &gt; 0.3) and (c.finishBang &lt; 1000) and (lerpf(c.finishBang, 1000, 0, 0, 1) * c.rating)</Evaluate>
                        </LEXAN_FullHouseExpertShowman>
                        <LEXAN_FullHouseFavourite>
                            <id>55</id>
                            <group>LEXAN</group>
                            <bling>2</bling>
                            <text>LEXAN Sponsored favourite</text>
                            <cr>35</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.lexan &gt; 4) and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </LEXAN_FullHouseFavourite>
                        <LEXAN_FullHouseShowman>
                            <id>56</id>
                            <group>LEXAN</group>
                            <bling>2</bling>
                            <text>LEXAN Sponsored showman</text>
                            <cr>30</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.lexan &gt; 4) and (c.finishBangDist &lt; 1000) and lerpf(c.finishBang, 1000, 0, 0, 1)</Evaluate>
                        </LEXAN_FullHouseShowman>
                        <LEXAN_Sponsored>
                            <id>57</id>
                            <group>LEXAN</group>
                            <bling>1</bling>
                            <text>LEXAN Sponsored</text>
                            <cr>5</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.lexan &gt; 0) and (c.rating+0.2 * lerpf(c.lexan, 0, 5, 0, 1))</Evaluate>
                        </LEXAN_Sponsored>
                        <ProPulse_Champion>
                            <id>58</id>
                            <group>ProPulse</group>
                            <bling>4</bling>
                            <text>ProPulse  world champion</text>
                            <cr>150</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.propulse &gt; 4) and (c.time &lt; t.world) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </ProPulse_Champion>
                        <ProPulse_DailyChampion>
                            <id>59</id>
                            <group>ProPulse</group>
                            <bling>4</bling>
                            <text>ProPulse daily world champion</text>
                            <cr>75</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.propulse &gt; 4) and (c.time &lt; t.daily) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </ProPulse_DailyChampion>
                        <ProPulse_FullHouseGoldenBoy>
                            <id>60</id>
                            <group>ProPulse</group>
                            <bling>4</bling>
                            <text>ProPulse  golden boy</text>
                            <cr>50</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.propulse &gt; 4) and c.onTrack and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </ProPulse_FullHouseGoldenBoy>
                        <ProPulse_FullHouseExpertShowman>
                            <id>61</id>
                            <group>ProPulse</group>
                            <bling>3</bling>
                            <text>ProPulse Sponsored expert showman</text>
                            <cr>45</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.propulse &gt; 4) and (c.rating &gt; 0.3) and (c.finishBang &lt; 1000) and (lerpf(c.finishBang, 1000, 0, 0, 1) * c.rating)</Evaluate>
                        </ProPulse_FullHouseExpertShowman>
                        <ProPulse_FullHouseFavourite>
                            <id>62</id>
                            <group>ProPulse</group>
                            <bling>2</bling>
                            <text>ProPulse Sponsored favourite</text>
                            <cr>35</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.propulse &gt; 4) and (c.rating &gt; 0.3) and c.rating </Evaluate>
                        </ProPulse_FullHouseFavourite>
                        <ProPulse_FullHouseShowman>
                            <id>63</id>
                            <group>ProPulse</group>
                            <bling>2</bling>
                            <text>ProPulse Sponsored showman</text>
                            <cr>30</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.propulse &gt; 4) and (c.finishBangDist &lt; 1000) and lerpf(c.finishBang, 1000, 0, 0, 1)</Evaluate>
                        </ProPulse_FullHouseShowman>
                        <ProPulse_Sponsored>
                            <id>64</id>
                            <group>ProPulse</group>
                            <bling>1</bling>
                            <text>ProPulse Sponsored</text>
                            <cr>5</cr>
                            <cr_curve>1</cr_curve>
                            <Evaluate>(c.propulse &gt; 0) and (c.rating+0.2 * lerpf(c.propulse, 0, 5, 0, 1))</Evaluate>
                        </ProPulse_Sponsored>
                    </data>");
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/webassets/Sodium/sodium2_shop/{version}/{project}/{lang}", async (ctx) =>
            {
                string? project = ctx.Request.Url.Parameters["project"];
                if (string.IsNullOrEmpty(project))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";

                string xmlPath = $"/webassets/Sodium/sodium2_shop/{ctx.Request.Url.Parameters["version"]}/{project}/{ctx.Request.Url.Parameters["lang"]}";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - sodium2_shop {project} definition data was not found, falling back to static file.");

                switch (project)
                {
                    case "lang":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(@"
                        <!--Archived from: https://web.archive.org/web/20110110184850/http://www.outso-srv1.com:80/webassets/Sodium/sodium2_shop/v0.2/lang/en-GB.xml-->
                        <lang>
                            <cloth>
                                <clothing_1>
                                    <name>Clothing Item 1</name>
                                    <desc>This is a clothing item</desc>
                                </clothing_1>
                                <clothing_2>
                                    <name>Clothing Item 2</name>
                                    <desc>This is another clothing item</desc>
                                </clothing_2>
                            </cloth>
                            <furn>
                                <furniture_1>
                                    <name>Furniture Item 1</name>
                                    <desc>Description furniture item 1</desc>
                                </furniture_1>
                                <furniture_2>
                                    <name>Furniture Item 2</name>
                                    <desc>Description furniture item 2</desc>
                                </furniture_2>
                                <furniture_3>
                                    <name>Furniture Item 3</name>
                                    <desc>Description furniture item 3</desc>
                                </furniture_3>
                                <furniture_4>
                                    <name>Furniture Item 4</name>
                                    <desc>Description furniture item 4</desc>
                                </furniture_4>
                                <furniture_5>
                                    <name>Furniture Item 5</name>
                                    <desc>Description furniture item 5</desc>
                                </furniture_5>
                                <furniture_6>
                                    <name>Furniture Item 6</name>
                                    <desc>Description furniture item 6</desc>
                                </furniture_6>
                                <furniture_7>
                                    <name>Furniture Item 7</name>
                                    <desc>Description furniture item 7</desc>
                                </furniture_7>
                                <furniture_8>
                                    <name>Furniture Item 8</name>
                                    <desc>Description furniture item 8</desc>
                                </furniture_8>
                                <furniture_9>
                                    <name>Furniture Item 9</name>
                                    <desc>Description furniture item 9</desc>
                                </furniture_9>
                                <furniture_10>
                                    <name>Furniture Item 10</name>
                                    <desc>Description furniture item 10</desc>
                                </furniture_10>
                                <furniture_11>
                                    <name>Furniture Item 11</name>
                                    <desc>Description furniture item 11</desc>
                                </furniture_11>
                                <furniture_12>
                                    <name>Furniture Item 12</name>
                                    <desc>Description furniture item 12</desc>
                                </furniture_12>
                                <furniture_13>
                                    <name>Furniture Item 13</name>
                                    <desc>Description furniture item 13</desc>
                                </furniture_13>
                                <furniture_14>
                                    <name>Furniture Item 14</name>
                                    <desc>Description furniture item 14</desc>
                                </furniture_14>
                                <furniture_4ab>
                                    <name>Furniture Item 4ab</name>
                                    <desc>Description furniture item 4ab</desc>
                                </furniture_4ab>
                                <furniture_5ab>
                                    <name>Furniture Item 5ab</name>
                                    <desc>Description furniture item 5ab</desc>
                                </furniture_5ab>
                                <furniture_6ab>
                                    <name>Furniture Item 6ab</name>
                                    <desc>Description furniture item 6ab</desc>
                                </furniture_6ab>
                                <furniture_7ab>
                                    <name>Furniture Item 7ab</name>
                                    <desc>Description furniture item 7ab</desc>
                                </furniture_7ab>
                                <furniture_8ab>
                                    <name>Furniture Item 8ab</name>
                                    <desc>Description furniture item 8ab</desc>
                                </furniture_8ab>
                                <furniture_9ab>
                                    <name>Furniture Item 9ab</name>
                                    <desc>Description furniture item 9ab</desc>
                                </furniture_9ab>
                                <furniture_10ab>
                                    <name>Furniture Item 10ab</name>
                                    <desc>Description furniture item 10ab</desc>
                                </furniture_10ab>
                                <furniture_11ab>
                                    <name>Furniture Item 11ab</name>
                                    <desc>Description furniture item 11ab</desc>
                                </furniture_11ab>
                                <furniture_12ab>
                                    <name>Furniture Item 12ab</name>
                                    <desc>Description furniture item 12ab</desc>
                                </furniture_12ab>
                            </furn>
                            <jato>
                                <jato_uber>
                                    <name>TURBULENCE Heavy Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. Massive acceleration bonuses available at the touch of a button are balanced by the heavy fuel load of an unused rocket.</desc>
                                </jato_uber>
                                <jato_ssrb_x>
                                    <name>HARE Fast Action Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. Instant acceleration for those times when you want to leave the pack behind.</desc>
                                </jato_ssrb_x>
                                <jato_basic>
                                    <name>TOUCHSTONE Rocket Boosters </name>
                                    <desc>Add these explosive boosters to Sodium2 racer to gain the advantage! Average burn time and thrust power make this the perfect all-purpose rocket. Experiment to find the best place to trigger.</desc>
                                </jato_basic>
                                <jato_ride>
                                    <name>FIRECRACKER Explosive Rocket Boosters</name>
                                    <desc>Add these boosters to your Sodium2 racer to gain the advantage. Best things come in small packages - unbelievable acceleration pushing your vehicle to the limit but with a short life so make it count.</desc>
                                </jato_ride>
                                <jato_pulsar>
                                    <name>AEON Long Life Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. A more steady boost effect but a much longer life will give a big advantage if you can maintain speed.</desc>
                                </jato_pulsar>
                                <jato_uber_sink>
                                    <name>ANTERIAL THRUST Heavy Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. Massive acceleration bonuses available at the touch of a button are balanced by the heavy fuel load of an unused rocket.</desc>
                                </jato_uber_sink>
                                <jato_ssrb_x_sink>
                                    <name>RABBIT Fast Action Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. Instant acceleration for those times when you want to leave the pack behind.</desc>
                                </jato_ssrb_x_sink>
                                <jato_basic_sink>
                                    <name>AGGRESSOR Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. Average burn time and thrust power make this the perfect all-purpose rocket. Experiment to find the best place to trigger.</desc>
                                </jato_basic_sink>
                                <jato_ride_sink>
                                    <name>ERUPTOR Explosive Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. Best things come in small packages - unbelievable acceleration pushing your vehicle to the limit but with a short life so make it count.</desc>
                                </jato_ride_sink>
                                <jato_pulsar_sink>
                                    <name>TERAPIN Long Life Rocket Booster</name>
                                    <desc>Add to your Sodium2 racer to gain the advantage. A more steady boost effect but a much longer life will give a big advantage if you can maintain speed.</desc>
                                </jato_pulsar_sink>
                            </jato>
                            <fuel>
                                <fuel_basic>
                                    <name>Stock Fuel</name>
                                    <desc>Don't play it safe with the standard fuel, try out our new experimental fuel variants to take those precious seconds off your lap time.</desc>
                                </fuel_basic>
                                <fuel_bionomic>
                                    <name>NOBEL 808 Fuel</name>
                                    <desc>A balanced fuel type for your Sodium2 racer that marginally increases both your top speed and acceleration.</desc>
                                </fuel_bionomic>
                                <fuel_jp2>
                                    <name>MOLOTOV Fuel</name>
                                    <desc>A highly unstable fuel mix for your Sodium2 racer that focuses on providing boosted acceleration.</desc>
                                </fuel_jp2>
                                <fuel_jp3>
                                    <name>DYNAMITE Fuel</name>
                                    <desc>An explosive fuel variant for your Sodium2 racer that increases your maximum top speed.</desc>
                                </fuel_jp3>
                                <fuel_bionomic_sink>
                                    <name>SCHONBEIN Fuel</name>
                                    <desc>A balanced fuel type for your Sodium2 racer that marginally increases both your top speed and acceleration.</desc>
                                </fuel_bionomic_sink>
                                <fuel_jp2_sink>
                                    <name>THERMOCHEM Fuel</name>
                                    <desc>A highly unstable fuel mix for your Sodium2 racer that focuses on providing boosted acceleration.</desc>
                                </fuel_jp2_sink>
                                <fuel_jp3_sink>
                                    <name>DETON8 Fuel</name>
                                    <desc>An explosive fuel variant for your Sodium2 racer that increases your maximum top speed.</desc>
                                </fuel_jp3_sink>
                            </fuel>
                            <burn>
                                <burn_basic>
                                    <name>Stock Afterburners</name>
                                    <desc>More thrust equals more acceleration but increased air pressure will destabilise your vehicle. Have you got the skill to take advantage of this advantage?</desc>
                                </burn_basic>
                                <burn_injectionmatrix>
                                    <name>IGNITION Afterburners</name>
                                    <desc>Adds a small level of acceleration to your Sodium2 racer. Your ride may become more unstable as a result.</desc>
                                </burn_injectionmatrix>
                                <burn_fdc>
                                    <name>BONESHAKER Afterburners</name>
                                    <desc>Hold your nerve through the increased instability and you will control a higher level of acceleration.</desc>
                                </burn_fdc>
                                <burn_ballistic>
                                    <name>BALLISTIC Afterburners</name>
                                    <desc>With great power comes great instability. Leave your opponents standing but be careful that the reduced stability doesn’t slam you into a wall. That would be embarrassing. </desc>
                                </burn_ballistic>
                                <burn_injectionmatrix_sink>
                                    <name>LIVE WIRE Afterburners</name>
                                    <desc>Adds a small level of acceleration to your Sodium2 racer. Your ride may become more unstable as a result.</desc>
                                </burn_injectionmatrix_sink>
                                <burn_fdc_sink>
                                    <name>FOREFRONT Afterburners</name>
                                    <desc>Hold your nerve through the increased instability and you will control a higher level of acceleration.</desc>
                                </burn_fdc_sink>
                                <burn_ballistic_sink>
                                    <name>EXPEL v82  Afterburners</name>
                                    <desc>With great power comes great instability. Leave your opponents standing but be careful that the reduced stability doesn’t slam you into a wall. That would be embarrassing. </desc>
                                </burn_ballistic_sink>
                            </burn>
                            <eng>
                                <eng_basic>
                                    <name>Stock Engine</name>
                                    <desc>Feel the need? The need for speed? It’s a fine balance between increasing your top speed and being weighed down on corners.</desc>
                                </eng_basic>
                                <eng_lexan_enyojar>
                                    <name>TITAN Engine</name>
                                    <desc>A noticeable jump in top speed to pull your Sodium2 racer away from its competition on the straights. An increase in weight will slow you down through the turns.</desc>
                                </eng_lexan_enyojar>
                                <eng_jetbox_shockdriver_jb22>
                                    <name>SHOCKDRIVER Engine</name>
                                    <desc>Control the lower turning speed into corners and your Sodium2 racer will hit those higher velocities.</desc>
                                </eng_jetbox_shockdriver_jb22>
                                <eng_hdc_aerospike_hf>
                                    <name>DREADNOUGHT Engine</name>
                                    <desc>A large and powerful engine to push your Sodium2 racer towards Mach 2. Don't forget to take account of the extra weight through the bends or a crash at that speed will leave nothing but a smear.</desc>
                                </eng_hdc_aerospike_hf>
                                <eng_lexan_enyojar_sink>
                                    <name>APOLLO Engine</name>
                                    <desc>A noticeable jump in top speed to pull your Sodium2 racer away from its competition on the straights. An increase in weight will slow you down through the turns.</desc>
                                </eng_lexan_enyojar_sink>
                                <eng_jetbox_shockdriver_jb22_sink>
                                    <name>THUNDERHORSE Engine</name>
                                    <desc>Control the lower turning speed into corners and your Sodium2 racer will hit those higher velocities.</desc>
                                </eng_jetbox_shockdriver_jb22_sink>
                                <eng_hdc_aerospike_hf_sink>
                                    <name>THOR Engine</name>
                                    <desc>A large and powerful engine to push your Sodium2 racer towards Mach 2. Don't forget to take account of the extra weight through the bends or a crash at that speed will leave nothing but a smear.</desc>
                                </eng_hdc_aerospike_hf_sink>
                            </eng>
                            <brks>
                                <brks_basic>
                                    <name>Stock Air Brakes</name>
                                    <desc>It's all in the reflexes. Speed is nothing if you haven't installed air brakes to prevent catastrophic crashes.</desc>
                                </brks_basic>
                                <brks_icarus>
                                    <name>DEADLINE Air Brakes</name>
                                    <desc>Sacrifice a small fraction of your Sodium2 racer's top speed and you'll turn more sharply into corners.</desc>
                                </brks_icarus>
                                <brks_wakebender>
                                    <name>TERMINUS Air Brakes</name>
                                    <desc>Improved hydraulics for your Sodium2 vehicle will give you a greater braking force so you spend less time on the brakes round the tight bends and more time on the POWER!</desc>
                                </brks_wakebender>
                                <brks_frictodraulic>
                                    <name>HYDROSTATIC Air Brakes</name>
                                    <desc>A brutally effective Air Brake system to get your Sodium2 racer through tighter corners at higher speeds but compliment them with other increased acceleration components or your rivals will make up lost ground as you struggle to pull away.</desc>
                                </brks_frictodraulic>
                                <brks_icarus_sink>
                                    <name>INERTIA Air Brakes</name>
                                    <desc>Sacrifice a small fraction of your Sodium2 racer's top speed and you'll turn more sharply into corners.</desc>
                                </brks_icarus_sink>
                                <brks_wakebender_sink>
                                    <name>TERMINAL FRICTION Air Brakes</name>
                                    <desc>Improved hydraulics for your Sodium2 vehicle will give you a greater braking force so you spend less time on the brakes round the tight bends and more time on the POWER!</desc>
                                </brks_wakebender_sink>
                                <brks_frictodraulic_sink>
                                    <name>CYBER-HALT Air Brakes</name>
                                    <desc>A brutally effective Air Brake system to get your Sodium2 racer through tighter corners at higher speeds but compliment them with other increased acceleration components or your rivals will make up lost ground as you struggle to pull away.</desc>
                                </brks_frictodraulic_sink>
                            </brks>
                            <frame>
                                <frame_basic>
                                    <name>Stock Frame</name>
                                    <desc>It's what's inside that counts. Customise the internal frame of your racer to tweak stability, weight and crash impact effects.</desc>
                                </frame_basic>
                                <frame_nanofiber>
                                    <name>NANOFIBER V.24 Lightweight Frame</name>
                                    <desc>A lighter frame will increase the acceleration of your Sodium2 racer at the cost of stability and crash impacts that reduce momentum by a greater amount.</desc>
                                </frame_nanofiber>
                                <frame_composite>
                                    <name>GALVONIUM Alpha Composite Frame</name>
                                    <desc>Heavier materials increase the stability of your Sodium2 racer as well as making it more resistant to loss of speed during impacts.</desc>
                                </frame_composite>
                                <frame_omniplex>
                                    <name>OMNIPLEX 48x Sturdy Frame</name>
                                    <desc>A heavy, sturdy frame for your Sodium2 racer will notably increase stability but the increased weight will create a greater loss of momentum following race impacts.</desc>
                                </frame_omniplex>
                                <frame_nanofiber_sink>
                                    <name>DIGIVAULT Frame</name>
                                    <desc>A lighter frame will increase the acceleration of your Sodium2 racer at the cost of stability and crash impacts that reduce momentum by a greater amount.</desc>
                                </frame_nanofiber_sink>
                                <frame_composite_sink>
                                    <name>STERLING Frame</name>
                                    <desc>Heavier materials increase the stability of your Sodium2 racer as well as making it more resistant to loss of speed during impacts.</desc>
                                </frame_composite_sink>
                                <frame_omniplex_sink>
                                    <name>SCHUMATTER Frame</name>
                                    <desc>A heavy, sturdy frame for your Sodium2 racer will notably increase stability but the increased weight will create a greater loss of momentum following race impacts.</desc>
                                </frame_omniplex_sink>
                            </frame>
                            <fcomp>
                                <fcomp_basic>
                                    <name>Stock Flight Computer</name>
                                    <desc>Flight Computer updates will make your vehicle much more responsive and increase stability at higher speeds.</desc>
                                </fcomp_basic>
                                <fcomp_autoadjustomator>
                                    <name>SYNAPSE Flight Computer</name>
                                    <desc>A minor stability increase is offset by a longer transonic period as you push through the speed of sound.</desc>
                                </fcomp_autoadjustomator>
                                <fcomp_synap_9000>
                                    <name>VOLTA Flight Computer</name>
                                    <desc>This Sodium2 flight computer is designed for optimum transonic transition up to Mach 1, but will also offer an increase in stability and improved turn response.</desc>
                                </fcomp_synap_9000>
                                <fcomp_ray_the_tube>
                                    <name>WHITE BOX Flight Computer</name>
                                    <desc>The latest in flight computer technology will constantly update the trajectory of your Sodium2 racer to greatly increase stability and turn response, but an extended time to break the Mach 1 barrier may let your opponents in.</desc>
                                </fcomp_ray_the_tube>
                                <fcomp_autoadjustomator_sink>
                                    <name>HALLI Flight Computer</name>
                                    <desc>A minor stability increase is offset by a longer transonic period as you push through the speed of sound.</desc>
                                </fcomp_autoadjustomator_sink>
                                <fcomp_synap_9000_sink>
                                    <name>VECTOR Flight Computer</name>
                                    <desc>This Sodium2 flight computer is designed for optimum transonic transition up to Mach 1, but will also offer an increase in stability and improved turn response.</desc>
                                </fcomp_synap_9000_sink>
                                <fcomp_ray_the_tube_sink>
                                    <name>PULSEDROID Flight Computer</name>
                                    <desc>The latest in flight computer technology will constantly update the trajectory of your Sodium2 racer to greatly increase stability and turn response, but an extended time to break the Mach 1 barrier may let your opponents in.</desc>
                                </fcomp_ray_the_tube_sink>
                            </fcomp>
                            <skin>
                                <skin_basic>
                                    <name>SILVER - Sodium 2 Racer Paint Scheme</name>
                                    <desc>New skins for your Sodium2 racer. </desc>
                                </skin_basic>
                                <skin_basic_01>
                                    <name>CRIMSON - Sodium 2 Racer Paint Scheme</name>
                                    <desc>Purchase this striking, deep red paint scheme for your Sodium2 vehicle and be the envy of the racing community.</desc>
                                </skin_basic_01>
                                <skin_basic_02>
                                    <name>RACING GREEN - Sodium 2 Racer Paint Scheme</name>
                                    <desc>Add a smooth green skin to your Sodium2 racer for a more natural look whilst racing to winning times.</desc>
                                </skin_basic_02>
                                <skin_basic_03>
                                    <name>BUMBLE BEE - Sodium 2 Racer Paint Scheme</name>
                                    <desc>A dazzling yellow skin for your Sodium2 racer. Pass over the finish line as no more than a blinding, golden streak!</desc>
                                </skin_basic_03>
                                <skin_basic_04>
                                    <name>HDC SALAMANDER - Sodium 2 Racer Paint Scheme</name>
                                    <desc>This radical HDC paint scheme features go-faster everything! </desc>
                                </skin_basic_04>
                                <skin_basic_05>
                                    <name>JETBOX SONIC CAMO - Sodium 2 Racer Paint Scheme</name>
                                    <desc>This futuristic evolution of the classic camo paint scheme will leave other racers wondering what just past them by.</desc>
                                </skin_basic_05>
                                <skin_basic_06>
                                    <name>Sodium 2 Racer Paint Scheme - RYLEE</name>
                                    <desc>New skins for your Sodium2 racer. </desc>
                                </skin_basic_06>
                                <skin_basic_07>
                                    <name>Sodium2 Vehicle Paint Scheme - [JETBOX]</name>
                                    <desc>An exciting new paint scheme for your Sodium 2 racer!</desc>
                                </skin_basic_07>
                                <skin_basic_08>
                                    <name>Sodium 2 Racer Paint Scheme - LEXAN</name>
                                    <desc>New skins for your Sodium2 racer. </desc>
                                </skin_basic_08>
                                <skin_basic_09>
                                    <name>Sodium 2 Racer Paint Scheme  - [ProPulse]</name>
                                    <desc>An exciting new paint scheme for you Sodium 2 racer!</desc>
                                </skin_basic_09>
                                <skin_basic_10>
                                    <name>FRUIT SALAD - Sodium 2 Racer Paint Scheme</name>
                                    <desc>New skins for your Sodium2 racer. </desc>
                                </skin_basic_10>
                            </skin>
                            <engh>
                                <engh_basic>
                                    <name>Stock Hover Engine</name>
                                    <desc>Hover Engines for your Sodium2 racer. Experiment with these for tighter, faster, smoother turns. Make sure you compensate for the extra weight added to your vehicle.</desc>
                                </engh_basic>
                                <engh_jetfan>
                                    <name>JETFAN Hover Engine</name>
                                    <desc>More responsive turning allows your Sodium2 racer to avoid surprise obstacles at high speed.</desc>
                                </engh_jetfan>
                                <engh_electrostatic>
                                    <name>ELECTROSTATIC Hover Engine</name>
                                    <desc>Adding the more powerful motor in this Hover Engine to your Sodium2 racer will increase turn response at the cost of weight.</desc>
                                </engh_electrostatic>
                                <engh_vortexthruster>
                                    <name>VORTEX Hover Engine</name>
                                    <desc>This Hover Engine benefits from increased blade size and less weight to produce faster turning, although loss of momentum after impacts will pronounced.</desc>
                                </engh_vortexthruster>
                                <engh_jetfan_sink>
                                    <name>HUMMING BIRD Hover Engine</name>
                                    <desc>More responsive turning allows your Sodium2 racer to avoid surprise obstacles at high speed.</desc>
                                </engh_jetfan_sink>
                                <engh_electrostatic_sink>
                                    <name>HORNET Hover Engine</name>
                                    <desc>Adding the more powerful motor in this Hover Engine to your Sodium2 racer will increase turn response at the cost of weight.</desc>
                                </engh_electrostatic_sink>
                                <engh_vortexthruster_sink>
                                    <name>VERTICES Hover Engine</name>
                                    <desc>This Hover Engine benefits from increased blade size and less weight to produce faster turning, although loss of momentum after impacts will pronounced.</desc>
                                </engh_vortexthruster_sink>
                            </engh>
                            <aero>
                                <aero_basic>
                                    <name>Stock Aerofoils</name>
                                    <desc>Change the aerodynamic profile of your Sodium2 racer to ride the turbulence hazards of a supersonic race.</desc>
                                </aero_basic>
                                <aero_aerobatic>
                                    <name>STEADFAST Aerofoils</name>
                                    <desc>Improves general handling of the Sodium2 racer and reduces drag through the sound barrier.</desc>
                                </aero_aerobatic>
                                <aero_scythe>
                                    <name>AEROBATIC Aerofoils</name>
                                    <desc>A minor improvement to the straight line speed of the Sodium2 racer also allows you to cut smoothly through the most awkward of turns.</desc>
                                </aero_scythe>
                                <aero_interceptor>
                                    <name>INTERCEPTOR Aerofoils</name>
                                    <desc>A more streamlined design for your Sodium2 racer to produce exceptional straight line speed and turning power. Turn too tightly though and you'll lose momentum.</desc>
                                </aero_interceptor>
                                <aero_aerobatic_sink>
                                    <name>FLIGHT SYS Aerofoils</name>
                                    <desc>Improves general handling of the Sodium2 racer and reduces drag through the sound barrier.</desc>
                                </aero_aerobatic_sink>
                                <aero_scythe_sink>
                                    <name>ALBATROS Aerofoils</name>
                                    <desc>A minor improvement to the straight line speed of the Sodium2 racer also allows you to cut smoothly through the most awkward of turns.</desc>
                                </aero_scythe_sink>
                                <aero_interceptor_sink>
                                    <name>ENTROPY Aerofoils </name>
                                    <desc>A more streamlined design for your Sodium2 racer to produce exceptional straight line speed and turning power. Turn too tightly though and you'll lose momentum.</desc>
                                </aero_interceptor_sink>
                            </aero>
                            <inta>
                                <inta_basic>
                                    <name>Stock Air Intakes</name>
                                    <desc>Stock Air Intakes</desc>
                                </inta_basic>
                            </inta>
                            <ctrl>
                                <ctrl_basic>
                                    <name>Stock Control Surfaces</name>
                                    <desc>Stock Control Surfaces</desc>
                                </ctrl_basic>
                            </ctrl>
                            <any>
                                <any_empty>
                                    <name>Nothing</name>
                                    <desc>Mount nothing here</desc>
                                </any_empty>
                            </any>
                        </lang>");
                        return;
                    case "mountLocks":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(@"
                        <!--Archived from: https://web.archive.org/web/20110110184918/http://www.outso-srv1.com:80/webassets/Sodium/sodium2_shop/v0.2/mountLocks/SCEE.xml-->
                        <mountlocks>
                            <jato1 type=""bool"">true</jato1>
                            <jato2 type=""bool"">true</jato2>
                            <fuel type=""bool"">false</fuel>
                            <tank type=""bool"">false</tank>
                            <burn type=""bool"">true</burn>
                            <eng type=""bool"">true</eng>
                            <brks type=""bool"">false</brks>
                            <frame type=""bool"">false</frame>
                            <fcomp type=""bool"">true</fcomp>
                            <inta type=""bool"">false</inta>
                            <skin type=""bool"">true</skin>
                            <engh type=""bool"">false</engh>
                            <stbl type=""bool"">false</stbl>
                            <ctrl type=""bool"">false</ctrl>
                            <aero type=""bool"">true</aero>
                        </mountlocks>");
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - sodium2_shop definition data was not found for project:{project}!");
                        break;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.Send();
            });
        }
    }
}
