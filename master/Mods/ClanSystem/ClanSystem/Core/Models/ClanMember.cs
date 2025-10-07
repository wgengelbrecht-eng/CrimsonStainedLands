namespace ClanSystemMod
{
    public class ClanMember
    {
        public string playerName { get; set; } = "NULL";
        public ClanRank Rank { get; set; } = ClanRank.None;
    
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