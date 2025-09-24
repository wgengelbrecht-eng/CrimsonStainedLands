namespace CrimsonStainedLands.ClanSystem
{
    public class Clan
    {
        public string Name { get; set; } = "";
        public string Tag { get; set; } = "";
        public string LeaderPlayerName { get; set; } = "";
        public List<ClanMember> Members { get; set; } = new List<ClanMember>();
    }

    
}