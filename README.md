# PSHome MultiServer is a powerful, open-source unified server software designed for hosting custom online lobbies, with a primary focus on reviving and running PlayStation®Home.

<img width="1024" height="1024" alt="pshmslogo" src="https://github.com/user-attachments/assets/0c15b147-7673-46c5-84f2-38ea113d6b92" />


It enables users to set up local or public servers for this iconic 3D social gaming community as well as a variety of other, supported titles.

supporting both Retail (consumer) and QA (developer) builds, you can build the Home experience that you remember. Want to share to the world? All of this requires minimal setup efforts as long as you have basic knowledge of server hosting.

Supported titles are playable on PS3 consoles, the RPCS3 emulator and PC, PSHome MultiServer being part of the Home Laboratory preservation project, it allows glitching, custom content creation, in-game modding, and full CDN (Content Delivery Network) functionality using archived PlayStation®Home assets. It also serves as a basis for contribution to various other projects.

This tool provides a versatile developer-oriented environment to restore online functionalities even outside of the gaming hobby, integrating features like a web server, a dns proxy and more. Plugins and specififc tools such as Nautilus (for asset management) are here to go beyond what's possible and improve uppon the basis provided, the only limit, is your imagination!

# Features

Local and public server hosting,
Support for custom content and modding,
Seemsless integration with the supported platforms,
Tools for databases management,
Up to date security standards and backward compatible with a wide range of configurations,
Open-source and community-driven preservation efforts

# Compatible Games

PSHome MultiServer is specialized for PlayStation®Home and related PS3 online experiences. While primarily built around Home, it integrates with broader PS3 and PC custom server projects for various projects:

# Home Laboratory (https://discord.gg/dwme8TRJgz) – Full PlayStation®Home online revival with spaces, avatars, mini-games, events, social features and functionalities surpassing what was possible back then.

# PSORG (https://discord.gg/psorg-756702841804030052) - PS Online Returnal Gaming, group focused on working and hosting several titles:

PSP:

ATV Offroad Fury Blazin' Trials:  Considered the "enhanced version" of ATV Offroad Fury 3, developed by Climax Racing and not by Rainbow Studios.

Gretzky NHL 2005 : Developed by Page 44 Studios and releasing under the 989 Sports label, considered to be the successor to the NHL FaceOff series, and has minimal online to vs another player.

Gretzky NHL 06: Direct sequel to Gretzky NHL 2005.

Lemmings: The PSP port of Lemmings the developed by Team17, and identical to that of PS2 version.

NBA 07: Developed by San Diego Studios, the game features 1v1 matches online. Being the second installment to the series.

NBA 08: Direct sequel to NBA 07

NBA 09 - The Inside: Direct sequel to NBA 08

Twisted Metal - Head-On: Releasing in 2005 and developed by Incognito Entertainment, Head-On is the first entry that is online-enabled and is a direct sequel to TM2, ignoring TM3 & TM4 events in those.

SOCOM - U.S. Navy SEALs - Fireteam Bravo 1 Pubeta: Public Beta of FTB1

SOCOM - U.S. Navy SEALs - Fireteam Bravo 2: Sequel to FTB2, 

Syphon Filter: Dark Mirror: The first entry in the Syphon Filter series to be developed on PSP, developed by Bend Studios, this sees follows after the events of The Omega Strain, having the player continue as Gabe Logan.

PS2:

Arc the Lad: End of Darkness:  Developed in 2005 by Japanese developers Cattle Call, AtL:EoD is a action role-playing JPRG being the second and last entry of the series to be on console. Using a Real-Time Battle System instead of a tactics-based one, the game features up to a 8 player online, capable of doing various quests, and earn exclusive online-only rewards. This game had a short lived online, however it had so much potential and support.

Destruction Derby: Arenas:  Developed by Studio 33 in 2004, and the fifth & final entry in the series, this featured car combat in close quarter arenas, with minimal budget provided, and basic car physics is enough to appease the players.

Gran Turismo 4 (Online Public Beta): The Online Pubeta version of GT4, which was cut from the final game due to time constraints, includes 721 cars. 

Hardware Online Arena: Developed by London Studio in 2003, H:OA was a online multiplayer vehicular combat game, releasing alongside the launch of online play for the PS2.

Jak X - Combat Racing Public Beta Trial: Pubeta version of Jak X - Combat Racing, having some online variations than the Release.
Jak X - Combat Racing:  (NTSC) Developed by none-other-than Naughty Dog, this was the fourth installment in the Jak series, introudcing a story based on a championship set in the Jak universe. (Supported By DNS Server)

Lemmings:  Developed by Team17, this was a direct remake of Lemmings from the original released in 1991, partial online is available with a level editor, leaderboards for specific levels, and the ability to up/download User submitted levels to share globally!

Twisted Metal Black Online: The first entry that was developed by Incognito Inc. and with online in the TM series.
Twisted Metal Harbor City (Prototypes): Cancelled entries in the TM series, capable of 12 player online with battles on unreleased maps.

PS3:

Sony -

Calling All Cars - RPCS3 Cross-play 🟢

Resistance: Fall of Man (base game)

Ratchet & Clank: Up Your Arsenal HD - RPCS3 Cross-play 🟢

Ratchet & Clank: Deadlocked HD/Gladiator HD- - RPCS3 Cross-play 🟢

Motorstorm - RPCS3 Cross-play 🟢 US/EU on v3.1, missing JP update

Motorstorm: Pacific Rift - RPCS3 Cross-play 🟢

NBA 07

SOCOM: Confrontation - RPCS3 Cross-play 🟢   Server OPEN BETA STATUS

Starhawk Public Beta ver - RPCS3 Cross-play ❌ unstable on RPCS3
NPUA70136 Beta build

PlayStation Home - Limited RPCS3 Cross-play 🟡 
NPIA00005 Release/QA, NPIA00010 DEV, NPEA00013DEV/BETA

Warhawk -  RPCS3 Cross-play 🟢 Collaborative efforts with Score from Warhawk Revived

EA -
Burnout Paradise v1.9 - RPCS3 Cross-play 🟢 

Ubisoft/Quazal -
I am Alive - Game Stat Tracking + UPlay 
Assassin's Creed II - Game Stat Tracking + UPlay
Ghostbusters: The VideoGame - Beta
Driver: San Francisco - Beta + UPlay

# Warhawk Revived - Full blown Warhawk revival group focused on accuracy and experience driven improvements.

# Multiserver Server Stack Overview

These compatibilities stem from extended server emulation projects integrated and adapted into the PSHome MultiServer suite. For the latest updates and setup guides, check the repository and associated Home Laboratory resources. Contributions are welcome to expand support!

# Overview

If you feel that any of the information provided here is inaccurate, please reach out to have it corrected with more accurate details. Subject to revisions! 

Important clarifications : 
Only ApacheNet, MitmDNS, Horizon, SSFW ( v1.70 and onward ), and SVO are needed for PlayStation®Home.
ApacheNet can be used as a stand alone web server for hosting.
MitmDNS can be used as a stand alone DNS.

# 1) ApacheNet -->

A minimalistic C# reimplementation of the infamous Apache web server, with support for both HTTP and HTTPS ( SSL ). It utilizes parts of WatsonWebServer and Spacewizard's HTTPListener.

For HTTPS, it can generate self-signed certificates for a wide array of dead domains, or optionally import your own. This includes CA-validated Edge certificates to enable verified padlock icons in modern browsers, as well as Origin certificates for authenticating game traffic.

It also includes some bonus features such as:

- Built-in PHP support via an integrated PHP-CGI handler, allowing it to execute PHP scripts using latest versions of PHP,

- Dynamic .DLL-based C# plugin system, allowing additional services to be loaded at runtime,

- Flexible redirect engine that mimics the Apache-style redirect system, with support for HTTP status redirects, regex matching, and conditional rewrites,

- Optional built-in upscaler for web-served images using top of the line technologies (ImageMagick/ffmpeg/FSR1.0).

# 2) MitmDNS -->

Man-In-The-Middle DNS Server. UDP Server intended to redirect dead domains to a IPv4 or IPv6 address typically. It utilizes parts of Kapetan's DNSLibrary.

# 3 ) Horizon -->

A reimplementation of Sony Medius network protocol reengineered in c#.

Horizon has evolved over time through multiple forked GitHub repositories. Its origin traces back to the Ratchet & Clank: Deadlocked PS2 restoration project. The specific fork used for Multiserver was Jumpsuit’s GitHub repository, "Horizon-Server-Public-Extended".

For simplicity, Horizon is Medius. Medius is the original server software written in C++, consisting of several interconnected components that worked together to provide online functionality.

The following fall under the Horizon component within Multiserver :

MUM --> Medius Universe Manager : This handles the "Universes" for a given Medius game, housing the players, channels, etc. Player never sees this server.

MUIS --> Medius Universe Information Server : Fetches the list of universes for a given Medius game from a database ( DB ), this is the entry point to MAS.

MAS--> Medius Authentication Server : Rather self-explanatory; It is the entry point of any Medius game, with/without MUIS. Authenticates the player to Medius, then moves them to MLS.

MLS--> Medius Lobby Server : The Lobby Server is the office house, where players are put in and can view clans, player stats, make game sessions, join different lobby "channels", and player with others.

DME --> Distributed Memory Engine : Dedicated Game Server for sharing game developer specific information over the wire between clients, this is absent for Peer to Peer Medius games.

NAT --> Proprietary port hole-punch through for Medius.

BWPS --> Bandwidth Probe Server for measuring client metrics to ensure stable connections as peer to peer host on Medius.
 
# 4) SVO -->

Sports View Online, or SCE-RT View Online as it was rebranded under Sony after the 2002 buyout.

Below are two quotes from a Warhawk HTML sample DEMO file that describe SVO :

1)
"The logic and formatting of the SVO features are generated on the server and sent down as SVML ( SCE-RT View Markup Language )."

2)
"Architecture Overview : The SVO system is designed to foster online community building so it works in the game's front-end only. It does not address in-game communication issues. The exception to this is that scores are posted to the server from the in-game module. Server: The server architecture is a three-tier design: 1) Presentation layer - The jsp web servers (we use Apache Tomcat) this layer handles all communication with the PS2's, via HTTP, and formats the data in SVML 2) Enterprise layer - The enterprise servers run activatable objects via Java RMI. It does all the computations and database communications. 3) Database layer - Where the data is stored. We use Oracle. This three-tier design is very common for large websites and is a proven, scalable, architecture."

Used heavily in older versions of PlayStation®Home from 1.65 and below; The transition from SVO to SSFW migration started in version 1.65.
Used heavily in Buzz Quiz Player, Starhawk, Socom: Confrontation, Warhawk, Killzone series

# 5) SSFW --> 

JSON HTTP / HTTPS web server that handles PlayStation®Home's save data services including user settings, avatar layouts, wardrobe saves, clans, apartment layouts, rewards, and save data generated from the save data LUA libraries. This replaced SVO entirely from 1.70 and onward.

# Misc Servers

# 6) QuazalServer ->

Ubisoft Revival Server; Based on ReHamster’s AlcatrazServer and co-developed with the Quazal research discord group (Halvors). Network protocol is simply referred to as "Quazal". Quazal has an internal protocol called Spark that some games utilize. Quazal Technologies was an independent company before Ubisoft bought it.

Games such as Driver: San Francisco, Ghost Recon Future Soldier, Dirt 2, Assassin's Creed (2, 3, Brotherhood, Revelations)
Games such as Blacksite Area 51, Overlord II, Operation Flashpoint: Dragon Rising, Ghostbusters TVG, Top Gun use Quazal + Spark

# 7) MultiSpy -->

GameSpy Revival Server; Based on civ4-mp's fork of PRMasterServer. Network protocol is simply referred to as "GameSpy". Developed by the IGN dev team. It was often integrated with other network protocols such as Aries. 

Games such as Star Wars Battlefront, Test Drive Unlimited (Integrated with EdNet)

# 8) MultiSocks -->

EA Revival Server consisting of two network protocols : Blaze (EA), DirtySocks (EA; The Aries protocol was built on top of DirtySocks; Same tech). m4kill’s BlazeSDK is utilized for Blaze.

Games such as Mass Effect 3 (Blaze), Crysis 3 Open Beta (Blaze), Army of Two: The Devil's Cartel (Blaze), Need For Speed Underground (Aries), Tiger Woods PGA Tour 06 (Aries), Burnout Paradise (Aries), Lord of the Rings: Conquest (Aries)

# 9) EdenServer --> 

Eden Games Revival Server; Network protocol is called EdNet. Specifically for Test Drive Unlimited ( TDU ). 

# How to compile and run (Winows only for now, Linux compatible very soon.)

For deployment, use the Publish.bat script.

For testing and rapid-usage, use the Build.bat (without Win32 GUI) or build_win32.bat .

Once compiled, go to ~Build-Output to see the artifacts generated, they will run out of the box (Note: some CDN or game-specific data will have to be given to the server depending the use-case (IE: wwwwroot for http data), these details will be communicated to you via the tty if needed).

Don't hesitate to double check json config files located in the static folder at the root of the application working directory before deployment.

If issues happen, it is recommended to check the server IPs generated (should be automatic, but some configurations can be particular).

The software is docker compatible, image comming soon!

# games,custom server,online,pshome,playstation-home,playstation-revival,hl,psorg,play,peer to peer,p2p,xlink kai
