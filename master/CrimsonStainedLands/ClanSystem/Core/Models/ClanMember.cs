namespace CrimsonStainedLands.ClanSystem
{
    public class ClanMember
    {
        public string playerName { get; set; }
        public ClanRank Rank { get; set; }
    }

    public enum ClanRank
    {
        None,       // Only used if the player is not a part of any clan, error return
        GreenHorn,
        Member,
        Lieutenant,
        Captain,
        Leader
    }
}