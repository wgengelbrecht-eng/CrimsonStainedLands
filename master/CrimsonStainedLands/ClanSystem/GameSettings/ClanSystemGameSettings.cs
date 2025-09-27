using System.Reflection;

namespace CrimsonStainedLands.ClanSystem
{
    static partial class GameSettings
    {
        static private string _executableDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static public bool ClanSystemEnabled { get; set; } = true;
        static public bool PvpSystemEnabled { get; set; } = true;
        static public int MaxClansAllowed { get; } = 10;
        static public string ClanSystemDataFolder { get; } = Path.Combine(_executableDirectory, "data", "ClanSystem");
        static public string ClanSystemDataFile { get; } = "clans.xml";
        static public string ClanSystemPvpRoomFile { get; } = "pvp_rooms.xml";
        static public string ClanCreationRequestFile { get; } = "clan_creation_requests.xml";
        static public string ClanRoomsFile { get; } = "clan_rooms.xml";
        static public string PlayersDataFolder { get; } = Path.Combine(_executableDirectory, "data", "players");
        static public int MinLevelRequiredForClanCreation { get; } = 60; // admin|imm players level
        static public int MaxLengthOfClanName { get; } = 30;
        static public int MaxLengthOfClanTag { get; } = 30;
        static public int MinLevelToAskForClanCreation = 30; //player asking for a clan

    }
}