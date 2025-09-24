using Microsoft.AspNetCore.SignalR;

namespace CrimsonStainedLands.ClanSystem
{

    public class PvpEnabledMember
    {
        public string PlayerName { get; set; } = "";
        public string ClanName { get; set; } = "";
    }
    
    public enum PvpFlag
    {
        Off,
        On,
        None
    }
}