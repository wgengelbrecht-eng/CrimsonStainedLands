using System.Xml.Serialization;

namespace ClanSystemMod
{
    public class Clan
    {
        public string Name { get; set; } = "";
        public string Tag { get; set; } = "";
        public string LeaderPlayerName { get; set; } = "";
        public List<ClanMember> Members { get; set; } = new List<ClanMember>();
        
        public DateTime? VotingEnds { get; set; }

        [XmlIgnore]
        public bool IsVotingActive => VotingEnds.HasValue && VotingEnds.Value > DateTime.Now;

        public List<ClanVote> Votes { get; set; } = new List<ClanVote>();
    }
    

    public class ClanVote
    {
        public string VoterName { get; set; } = "";
        public string CandidateName { get; set; } = "";
    }

    
}