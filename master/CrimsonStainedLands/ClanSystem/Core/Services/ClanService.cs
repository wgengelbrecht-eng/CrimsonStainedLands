using K4os.Compression.LZ4.Internal;
using Microsoft.AspNetCore.Builder;
using Mysqlx;

namespace CrimsonStainedLands.ClanSystem
{
    public static class ClanService
    {
        //Universal Msg buffer for error messages. 
        private static readonly List<string> _errorMsgBuffer = [];

        public static string GetErrorMsgsAndClearBuffer()
        {
            string msgs = "";
            foreach (String msg in _errorMsgBuffer)
            {
                msgs += msg + "\n";
            }
            _errorMsgBuffer.Clear();
            return msgs;
        }


        public static string GetErrorMsgsBuffer()
        {
            string msgs = "";
            foreach (String msg in _errorMsgBuffer)
            {
                msgs += msg + "\n";
            }
            return msgs;
        }
        public static bool hasErrorMsgs()
        {
            if (_errorMsgBuffer.Count > 0)
                return true;

            return false;
        }

        private static void addErrorMsg(string errorMsg)
        {
            _errorMsgBuffer.Add(errorMsg);
        }

        public static bool CreateClan(string clanName, string leaderName, string clanTag)
        {
            if (clanName.Length > GameSettings.MaxLengthOfClanName)
            {
                addErrorMsg($"The clan name cannot be more than {GameSettings.MaxLengthOfClanName} characters long.");
                return false;
            }
            
            if (clanTag.Length > GameSettings.MaxLengthOfClanTag)
            {
                addErrorMsg($"The clan tag cannot be more than {GameSettings.MaxLengthOfClanTag} characters long.");
                return false;
            }

            if (ClanDBService.GetNumberOfClans() >= GameSettings.MaxClansAllowed)
            {
                addErrorMsg("The maximum ammount of clans have been reached. You cannot create more.");
                return false;
            }

            if (IsPlayerInAnyClan(leaderName, out string outClanName))
            {
                addErrorMsg("This player is part of a clan already.");
                return false;
            }

            if (ClanDBService.ClanExists(clanName))
            {
                addErrorMsg("This clan already exists.");
                return false;
            }


            List<string> mudPlayers = getAllExistingPlayerNames();
            bool mudPlayerFound = false;
            foreach (string mudPlayer in mudPlayers)
            {
                if (leaderName.Equals(mudPlayer, StringComparison.CurrentCultureIgnoreCase))
                {
                    mudPlayerFound = true;
                    break;
                }
            }

            if (mudPlayerFound)
            {
                var newClan = new Clan
                {
                    Name = clanName,
                    Tag = clanTag,
                    LeaderPlayerName = leaderName,
                    Members = new List<ClanMember>
                    {
                        new ClanMember { playerName = leaderName, Rank = ClanRank.Leader}
                    }
                };

                if (ClanDBService.addClan(newClan))
                {
                    if (ClanDBService.BackUpToFile())
                    {
                        return true;
                    }  
                    return false;
                }
                return false;
            }
            else
            {
                addErrorMsg("The player name was not found. Active or Inactive.");
                return false;
            }
        }


        public static bool CreateClan(string clanName)
        {
          
            if (clanName.Length > GameSettings.MaxLengthOfClanName)
            {
                addErrorMsg($"The clan name cannot be more than {GameSettings.MaxLengthOfClanName} characters long.");
                return false;
            }

            if (ClanDBService.GetNumberOfClans() >= GameSettings.MaxClansAllowed)
            {
                addErrorMsg("The maximum ammount of clans have been reached. You cannot create more.");
                return false;
            }

            if (ClanDBService.ClanExists(clanName))
            {
                addErrorMsg("This clan already exists.");
                return false;
            }

            var newClan = new Clan
            {
                Name = clanName,
                Tag = "",
                LeaderPlayerName = "",
            };

            if (ClanDBService.addClan(newClan))
            {
                
                return true;
            }
            return false;
        }

        public static bool SetClanTag(string clanName, string clanTag)
        {
            
            if (clanTag.Length > GameSettings.MaxLengthOfClanTag)
            {
                addErrorMsg($"The clan tag cannot be more than {GameSettings.MaxLengthOfClanTag} characters long.");
                return false;
            }

            Clan clan = ClanDBService.GetClan(clanName);

            if (clan != null)
            {
                clan.Tag = clanTag;
                if (ClanDBService.UpdateClanRecord(clan))
                {
                    return true;
                }
            }
            return false;
        }


        public static bool UpdateClanName(string clanName, string newClanName)
        {

            if (clanName.Length > GameSettings.MaxLengthOfClanName)
            {
                addErrorMsg($"The clan name cannot be more than {GameSettings.MaxLengthOfClanName} characters long.");
                return false;
            }

            Clan clan = ClanDBService.GetClan(clanName);
            if (clan != null)
            {
                if (ClanService.isClanNameUsed(clanName))
                {
                    addErrorMsg("This clan name is already in use.");
                    return false;
                }
                clan.Name = newClanName;
                if (ClanDBService.UpdateClanRecord(clan))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AddMember(string clanName, string playerName)
        {

            if(IsPlayerInAnyClan(playerName,out string outClanName))
            {
                addErrorMsg($"This player is part of a clan already. Clan member of : {outClanName}");
                return false;
            }


            List<string> mudPlayers = getAllExistingPlayerNames();
            bool mudPlayerFound = false;
            foreach (string mudPlayer in mudPlayers)
            {
                if (playerName.Equals(mudPlayer, StringComparison.CurrentCultureIgnoreCase))
                {
                    mudPlayerFound = true;
                    break;
                }
            }

            if (!mudPlayerFound)
            {
                addErrorMsg("The player was not found. Active or inactive.");
                return false;    
            }

            Clan clan = ClanDBService.GetClan(clanName);
            if (clan != null)
            {
                clan.Members.Add(new ClanMember { playerName = playerName, Rank = ClanRank.GreenHorn });
                if (ClanDBService.UpdateClanRecord(clan))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveMember(string clanName, string playerName)
        {
            if (IsPlayerInClan(clanName, playerName))
            {
                Clan clan = ClanDBService.GetClan(clanName);
                if (clan != null)
                {
                    try
                    {
                        clan.Members.RemoveAll(member => member.playerName == playerName);
                        if (IsAClanLeader(playerName))
                        {
                            clan.LeaderPlayerName = "";
                        }
                        if (ClanDBService.UpdateClanRecord(clan))
                        {
                             return true;
                        }
                    }
                    catch (ArgumentNullException Ex)
                    {
                        addErrorMsg($"No member removed. Exception: {Ex}");
                        return false;
                    }
                }
                return false;
            }
            else
            {
                addErrorMsg("This player is not part of this clan.");
                return false;
            }
            
        }

        public static bool PromoteMember(string clanName, string playerName)
        {
            if (IsPlayerInClan(clanName, playerName))
            {
                Clan clan = ClanDBService.GetClan(clanName);
                if (clan != null)
                {
                    foreach (ClanMember member in clan.Members)
                    {
                        if (member.playerName == playerName)
                        {
                            if (member.Rank == ClanRank.Leader)
                            {
                                addErrorMsg("Cannot promote a member that is a leader. This is the Highest level.");
                                return false;
                            }

                            if (member.Rank + 1 == ClanRank.Leader)
                            {
                                string leaderName = getClanLeader(clanName);
                                if (leaderName != "")
                                {
                                    addErrorMsg("Cannot promote this member to leader. This clan already has a leader.");
                                    return false;
                                }
                                else
                                {
                                    clan.LeaderPlayerName = playerName;
                                }
                            }

                            member.Rank += 1;
                            if (ClanDBService.UpdateClanRecord(clan))
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
                return false;
            }
            else
            {
                addErrorMsg("Player is not part of this clan.");
                return false;
            }
        }

        public static bool DemoteMember(string clanName, string playerName)
        {
            if (IsPlayerInClan(clanName, playerName))
            {
                Clan clan = ClanDBService.GetClan(clanName);
                if (clan != null)
                {
                    foreach (ClanMember member in clan.Members)
                    {
                        if (member.playerName == playerName)
                        {
                            if (member.Rank == ClanRank.GreenHorn)
                            {
                                addErrorMsg("Cannot demote a member that is a Green Horn. This is the lowest level.");
                                return false;
                            }

                            if (member.Rank == ClanRank.Leader)
                            {
                                clan.LeaderPlayerName = "";
                            }

                            member.Rank -= 1;
                            if (ClanDBService.UpdateClanRecord(clan))
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
                return false;
            }
            else
            {
                addErrorMsg("Player is not part of this clan.");
                return false;
            }
        }

        public static string GetPlayerRankAsString(string playerName)
        {
            List<Clan> clans = ClanDBService.GetAllClans();
            if (clans != null)
            {
                foreach (Clan clan in clans)
                {
                    foreach (ClanMember member in clan.Members)
                    {
                        if (member.playerName == playerName)
                        {
                            return member.Rank.ToString();
                        }
                    }
                }
                addErrorMsg("No player by that name was found in any of the clans");
                return "";
            }
            return "";
        }

        public static ClanRank GetPlayerRank(string playerName)
        {
            List<Clan> clans = ClanDBService.GetAllClans();
            if (clans != null)
            {
                foreach (Clan clan in clans)
                {
                    foreach (ClanMember member in clan.Members)
                    {
                        if (member.playerName.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return member.Rank;
                        }
                    }
                }
                return ClanRank.None;
            }
            return ClanRank.None;
        }

        public static bool IsPlayerInClan(string clanName, string playerName)
        {
            Clan clan = ClanDBService.GetClan(clanName);
            if (clan != null)
            {
                foreach (ClanMember member in clan.Members)
                {
                    if (member.playerName.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }


        public static bool IsPlayerInAnyClan(string playerName, out string clanName)
        {
            clanName = "";

            List<Clan> clans = ClanDBService.GetAllClans();
            if (clans != null)
            {
                foreach (Clan clan in clans)
                {
                    foreach (ClanMember member in clan.Members)
                    {
                        if (member.playerName.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            clanName = clan.Name;
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }


        public static string GetClanName(string playerName)
        {

            List<Clan> clans = ClanDBService.GetAllClans();
            if (clans != null)
            {
                foreach (Clan clan in clans)
                {
                    foreach (ClanMember member in clan.Members)
                    {
                        if (member.playerName.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return clan.Name;
                        }
                    }
                }
                addErrorMsg("No Clan with that player in it found.");
                return "";
            }
            return "";
        }


        public static string GetMemberInfoOfClan(string clanName)
        {
            string returnInfo = "";

            Clan clan = ClanDBService.GetClan(clanName);
            if (clan != null)
            {
                returnInfo = $"\\rClan\\x : {clan.Name} [{clan.Tag}]\n";
                string leaderInfo = "";
                string captainInfo = "";
                string lieutenantInfo = "";
                string memberInfo = "";
                string greenHornInfo = "";

                foreach (ClanMember member in clan.Members)
                {
                    if (member.Rank == ClanRank.Leader)
                        leaderInfo += $"{member.playerName,-30} | \\y(*>>>)\\xLeader\\y(<<<*)\\x\n";

                    if (member.Rank == ClanRank.Captain)
                        captainInfo += $"{member.playerName,-30} | \\r(>>>)\\xCaptain\\r(<<<)\\x\n";

                    if (member.Rank == ClanRank.Lieutenant)
                        lieutenantInfo += $"{member.playerName,-30} | \\c(>>)\\xLieutenant\\c(<<)\\x\n";

                    if (member.Rank == ClanRank.Member)
                        memberInfo += $"{member.playerName,-30} | \\b(+)\\xMember\\b(+)\\x\n";

                    if (member.Rank == ClanRank.GreenHorn)
                        greenHornInfo += $"{member.playerName,-30} | \\g(-)\\xGreen-horn\\g(-)\\x\n";

                }

                returnInfo += leaderInfo + captainInfo + lieutenantInfo + memberInfo + greenHornInfo;
                return returnInfo;
            }
            return "";
        }


        public static string getClanLeader(string clanName)
        {
            foreach (Clan clan in ClanDBService.GetAllClans())
            {
                if (clan.Name.Equals(clanName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return clan.LeaderPlayerName;
                }
            }
            addErrorMsg("No leader found for this clan");
            return "";
        }


        public static bool IsAClanLeader(string playerName)
        {
          
            foreach (Clan clan in ClanDBService.GetAllClans())
            {
                if (clan.LeaderPlayerName.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        public static bool isClanNameUsed(string clanName)
        {
            List<Clan> clans = ClanDBService.GetAllClans();

            foreach (Clan clan in clans)
            {
                if (clan.Name.Equals(clanName, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }


        public static List<string> getAllExistingPlayerNames()
        {
            List<string> players = new List<string>();
            foreach (string filePath in Directory.EnumerateFiles(GameSettings.PlayersDataFolder))
            {
                string fileName = Path.GetFileName(filePath);
                string name = GetSubstringBeforeDot(fileName);
                players.Add(name);
            }
            return players;
        }

        public static string GetSubstringBeforeDot(string input)
        {
            int dotIndex = input.IndexOf('.');

            if (dotIndex > -1)
            {
                return input.Substring(0, dotIndex);
            }
            else
            {
                return input;
            }
        }
    }
}