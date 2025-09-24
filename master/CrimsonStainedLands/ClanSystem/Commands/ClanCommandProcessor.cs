using System.Collections;
using System.Text.RegularExpressions;

namespace CrimsonStainedLands.ClanSystem
{
    public static class ClanSystemDoFunction
    {

        //--- Main doClan
        public static void doClan(Character ch, string arguments)
        {

            if (getNextArg(arguments, out string nextArg, out string remainingArgs))
            {
                switch (nextArg)
                {
                    case "help":
                        {
                            ClanHelp(ch, remainingArgs);
                            break;
                        }
                    case "list":
                        {
                            ClanList(ch, remainingArgs);
                            break;
                        }
                    case "members":
                        {
                            ClanMembersList(ch, remainingArgs);
                            break;
                        }
                    case "create_clan":
                        {
                            // only imm|admins can create|delete clans
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanCreate(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "delete_clan":
                        {
                            // only imm|admins can create|delete clans
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanDelete(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "set_tag":
                        {
                            // only imm|admins|clan leaders can set tag
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanSetTagAsAdmin(ch, remainingArgs);
                            }
                            else if (ClanService.IsAClanLeader(ch.Name))
                            {
                                ClanSetTagAsLeader(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "update_name":
                        {
                            // only imm|admins|clan leaders can update clan name
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanUpdateNameAsAdmin(ch, remainingArgs);
                            }
                            else if (ClanService.IsAClanLeader(ch.Name))
                            {
                                ClanUpdateNameAsLeader(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "add_member":
                        {
                            // only imm|admins|clan leaders|captains can add members
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanAddPlayerAsAdmin(ch, remainingArgs);
                            }
                            else if (ClanService.GetPlayerRank(ch.Name) >= ClanRank.Captain)
                            {
                                ClanAddPlayerAsLeader(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "remove_member":
                        {
                            // only imm|admins|clan leaders|captains can add members
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanRemovePlayerAsAdmin(ch, remainingArgs);
                            }
                            else if (ClanService.GetPlayerRank(ch.Name) >= ClanRank.Captain)
                            {
                                ClanRemovePlayerAsLeader(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "promote":
                        {
                            // only imm|admins|clan leaders|captains can promote members
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanPromotePlayerAsAdmin(ch, remainingArgs);
                            }
                            else if (ClanService.GetPlayerRank(ch.Name) >= ClanRank.Captain)
                            {
                                ClanPromotePlayerAsLeader(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "demote":
                        {
                            // only imm|admins|clan leaders|captains can demote members
                            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
                            {
                                ClanDemotePlayerAsAdmin(ch, remainingArgs);
                            }
                            else if(ClanService.GetPlayerRank(ch.Name) >= ClanRank.Captain)
                            {
                                ClanDemotePlayerAsLeader(ch, remainingArgs);
                            }
                            else
                            {
                                ch.send("You cannot use that command. Type 'clan help'.");
                            }
                            break;
                        }
                    case "member_info":
                        {
                            ClanGetPlayerInfo(ch, remainingArgs);
                            break;
                        }
                    default:
                        {
                            ch.send("That is not a clan command. Type 'clan help'.");
                            break;
                        }
                }

                // Send any error msgs generated by ClanService and ClanDBService to user 
                // to explain why certain actions did not work or failed
                string msgsToUser = "";
                msgsToUser += ClanService.GetErrorMsgsAndClearBuffer();//Calling this forces the buffer to clear for next loop
                msgsToUser += ClanDBService.GetErrorMsgsAndClearBuffer();//Calling this forces the buffer to clear for next loop
                if (msgsToUser != "")
                {
                    ch.send(msgsToUser);
                }
            }
            else
            {
                ch.send("Do what with clan? Type 'clan help'.");
            }
        }


        //----------------------------------- doClan helper functions
        private static void ClanHelp(Character ch, string arguments)
        {
            string clanHelpAdminPlayer = "";
            string clanHelpNonAdminPlayer = "";
            string clanHelpLeaderPlayer = "";

            // Only send this help list to 'Imm|Admins'
            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
            {
                clanHelpAdminPlayer =   $"\\rHere follows the clan commands for Admin players.\\x\n" +
                                        $"\\g{"list",-30}\\x| {"List all clans.",-100}\n" +
                                        $"\\g{"members",-30}\\x| {"List the members with their ranks of a specified clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan members 'clan name'",-100}\n" +
                                        $"\\g{"create_clan",-30}\\x| {"Create a new clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan create_clan 'clan name' 'clan leader' 'clan tag'",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan create_clan 'clan name'",-100}\n" +
                                        $"\\g{"delete_clan",-30}\\x| {"Delete a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan delete_clan 'clan name'",-100}\n" +
                                        $"\\g{"set_tag",-30}\\x| {"Set or update a clan tag.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan set_tag 'clan name' 'clan tag'",-100}\n" +
                                        $"\\g{"update_name",-30}\\x| {"Update|Change the name of a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan update_name 'clan name' 'new clan name'",-100}\n" +
                                        $"\\g{"add_member",-30}\\x| {"Add a new player to a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan add_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"remove_member",-30}\\x| {"Remove a player from a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan remove_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"promote",-30}\\x| {"Promote a player in a clan.",-100}\n" +
                                        $"{"",-30}| {"Repeat this call untill the player reaches the required rank.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan promote_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"demote",-30}\\x| {"Demote a player in a clan.",-100}\n" +
                                        $"{"",-30}| {"Repeat this call untill the player reaches the required rank.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan demote_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"member_info",-30}\\x| {"Get the rank of a player.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan member_info 'player name'",-100}\n";

                ch.send(clanHelpAdminPlayer);
            }
            // Only send this help list to 'clan leader' ranked players
            else if (ClanService.GetPlayerRank(ch.Name) == ClanRank.Leader)
            {
                clanHelpLeaderPlayer = $"\\rHere follows the clan commands for players of rank Leader.\\x\n" +
                                        $"\\g{"list",-30}\\x| {"List all clans.",-100}\n" +
                                        $"\\g{"members",-30}\\x| {"List the members with their ranks of a specified clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan members 'clan name'",-100}\n" +
                                        $"\\g{"set_tag",-30}\\x| {"Set or update a clan tag.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan set_tag 'clan name' 'clan tag'",-100}\n" +
                                        $"\\g{"update_name",-30}\\x| {"Update|Change the name of a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan update_name 'clan name' 'new clan name'",-100}\n" +
                                        $"\\g{"add_member",-30}\\x| {"Add a new player to a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan add_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"remove_member",-30}\\x| {"Remove a player from a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan remove_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"promote",-30}\\x| {"Promote a player in a clan.",-100}\n" +
                                        $"{"",-30}| {"Repeat this call untill the player reaches the required rank.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan promote_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"demote",-30}\\x| {"Demote a player in a clan.",-100}\n" +
                                        $"{"",-30}| {"Repeat this call untill the player reaches the required rank.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan demote_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"member_info",-30}\\x| {"Get the rank of a player.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan member_info 'player name'",-100}\n";

                ch.send(clanHelpLeaderPlayer);
            }
            // Only send this help list to clan members of rank 'captain'
            else if (ClanService.GetPlayerRank(ch.Name) == ClanRank.Leader - 1)
            {
                clanHelpLeaderPlayer = $"\\rHere follows the clan commands for players of rank Captain.\\x\n" +
                                        $"\\g{"list",-30}\\x| {"List all clans.",-100}\n" +
                                        $"\\g{"members",-30}\\x| {"List the members with their ranks of a specified clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan members 'clan name'",-100}\n" +
                                        $"\\g{"add_member",-30}\\x| {"Add a new player to a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan add_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"remove_member",-30}\\x| {"Remove a player from a clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan remove_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"promote",-30}\\x| {"Promote a player in a clan.",-100}\n" +
                                        $"{"",-30}| {"Repeat this call untill the player reaches the required rank.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan promote_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"demote",-30}\\x| {"Demote a player in a clan.",-100}\n" +
                                        $"{"",-30}| {"Repeat this call untill the player reaches the required rank.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan demote_player 'clan name' 'player name'",-100}\n" +
                                        $"\\g{"member_info",-30}\\x| {"Get the rank of a player.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan member_info 'player name'",-100}\n";

                ch.send(clanHelpLeaderPlayer);
            }
            else
            // Send this help list to any remaining 'ranked' players and 'non clan' members
            {
                clanHelpNonAdminPlayer = $"\\rClan Help.\\x\n" +
                                        $"\\g{"list",-30}\\x| {"List all clans.",-100}\n" +
                                        $"\\g{"members",-30}\\x| {"List the members with their ranks of a specified clan.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan members 'clan name'",-100}\n" +
                                        $"\\g{"member_info",-30}\\x| {"Get the rank of a player.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x clan member_info 'player name'",-100}\n";

                ch.send(clanHelpNonAdminPlayer);
            }
        }

        private static void ClanList(Character ch, string arguments)
        {
            string list = "";
            List<Clan> clans = ClanDBService.GetAllClans();

            if (clans != null)
            {
                if (clans.Count == 0)
                {
                    ch.send("There are no clans as yet.");
                    return;
                }

                list += $"{"\\rClan Name",-32}|{"Clan Tag",-30}|Clan Leader\\x\n";
                foreach (Clan clan in clans)
                {
                    list += $"{clan.Name,-30}|{clan.Tag,-30}|{clan.LeaderPlayerName}\n";
                }
                ch.send(list);
            }  
        }


        private static void ClanMembersList(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs))
            {
                string membesrList = ClanService.GetMemberInfoOfClan(clanName);
                ch.send(membesrList);   
            }
            else
            {
                ch.send("No clan name was given. Type 'clan help'.");
            }  
        }


        private static void ClanCreate(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs_1))
            {
                // we've got a clan name, if more args then continue, else create clan with name only
                if (getNextArg(remainingArgs_1, out string clanLeader, out string remainingArgs_2))
                {
                    //Have clanLeader name
                    if (getNextArg(remainingArgs_2, out string clanTag, out string remainingArgs_3))
                    {
                        //have clantag string
                        if (ClanService.CreateClan(clanName, clanLeader, clanTag))
                        {
                            ch.send("Clan created.");
                        }
                    }
                }
                else
                {
                    // create clan with name only              
                    if (ClanService.CreateClan(clanName))
                    {
                        ch.send("Clan created...");
                    }
                }
            }
            else
            {
                ch.send("No clan name was given. Type 'clan help'.");
            }
        }


        private static void ClanDelete(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs_1))
            {
                if (ClanDBService.removeClan(clanName))
                {
                    ch.send("Clan removed.");
                }
            }
            else
            {
                ch.send("No clan name was specified. Type 'clan help'.");
            }
        }
        

        private static void ClanSetTagAsAdmin(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs_1))
            {
                if (getNextArg(remainingArgs_1, out string clanTag, out string remainingArgs_2))
                {
                    if (ClanService.SetClanTag(clanName, clanTag))
                    {
                        ch.send("Clan tag has been updated.");
                    }
                }
                else
                {
                    ch.send("No clan tag was specified. Type 'clan help'.");
                }
            }
            else
            {
                ch.send("No clan name was specified. Type 'clan help'.");
            }
        }


        private static void ClanSetTagAsLeader(Character ch, string arguments)
        {
            string clanName = ClanService.GetClanName(ch.Name);
            
            if (getNextArg(arguments, out string clanTag, out string remainingArgs))
            {
                if (ClanService.SetClanTag(clanName, clanTag))
                {
                    ch.send("Clan tag has been updated.");
                }
            }
            else
            {
                ch.send("No clan tag was specified. Type 'clan help'.");
            }
        }


        private static void ClanUpdateNameAsAdmin(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs_1))
            {
                if (getNextArg(remainingArgs_1, out string newClanName, out string remainingArgs_2))
                {
                    if (ClanService.UpdateClanName(clanName, newClanName))
                    {
                        ch.send("Clan name has been updated.");
                    }
                }
                else
                {
                    ch.send("No new clan name was given. Type 'clan help'.");
                }
            }
            else
            {
                ch.send("No clan name was given. Type 'clan help'.");
            }
        }


        private static void ClanUpdateNameAsLeader(Character ch, string arguments)
        {
            string clanName = ClanService.GetClanName(ch.Name);
            if (clanName != "")
            {
                if (getNextArg(arguments, out string newClanName, out string remainingArgs))
                {
                    if (ClanService.UpdateClanName(clanName, newClanName))
                    {
                        ch.send("Clan name has been updated.");
                    }
                }
                else
                {
                    ch.send("No new clan name was given. Type 'clan help'.");
                }
            }
        }


        private static void ClanAddPlayerAsAdmin(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs_1))
            {
                if (getNextArg(remainingArgs_1, out string playerName, out string remainingArgs_2))
                {
                    if (ClanService.AddMember(clanName, playerName))
                    {
                        ch.send("Player added to the clan");
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            }
            else
            {
                ch.send("No clan name was specfied. Type 'clan help'.");
            }
        }


        private static void ClanAddPlayerAsLeader(Character ch, string arguments)
        {
            string clanName = ClanService.GetClanName(ch.Name);

            if (clanName != "")
            {
                if (getNextArg(arguments, out string playerName, out string remainingArgs))
                {
                    if (ClanService.AddMember(clanName, playerName))
                    {
                        ch.send("Player added to the clan");
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            } 
        }


        private static void ClanRemovePlayerAsAdmin(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs))
            {
                if (getNextArg(remainingArgs, out string playerName, out string remainingArgs_1))
                {
                    if (ClanService.RemoveMember(clanName, playerName))
                    {
                        ch.send("Player removed from the clan.");
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            }
            else
            {
                ch.send("No clan name was specfied. Type 'clan help'.");
            }
        }
        

        private static void ClanRemovePlayerAsLeader(Character ch, string arguments)
        {
            string clanName = ClanService.GetClanName(ch.Name);

            if (clanName != "")
            {
                if (getNextArg(arguments, out string playerName, out string remainingArgs))
                {
                    // if trying to remove a clan leader
                    if (ClanService.IsAClanLeader(playerName))
                    {
                        if (!ClanService.IsAClanLeader(ch.Name))
                        {
                            ch.send("You cannot remove a clan leader from the clan.");
                            return;
                        }
                    }

                    if (ClanService.RemoveMember(clanName, playerName))
                    {
                        ch.send("Player removed from the clan.");
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            }
            
        }


        private static void ClanPromotePlayerAsAdmin(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs_1))
            {
                if (getNextArg(remainingArgs_1, out string playerName, out string remainingArgs_2))
                {
                    if (ClanService.PromoteMember(clanName, playerName))
                    {
                        string currentRank = ClanService.GetPlayerRankAsString(playerName);
                        ch.send($"Player now has the rank : {currentRank}");   
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            }
            else
            {
                ch.send("No clan name was specfied. Type 'clan help'.");
            }
        }
        

        private static void ClanPromotePlayerAsLeader(Character ch, string arguments)
        {
            string clanName = ClanService.GetClanName(ch.Name);

            if (clanName != "")
            {
                if (getNextArg(arguments, out string playerName, out string remainingArgs))
                {
                    // if trying to promote a clan leader
                    if (ClanService.IsAClanLeader(playerName))
                    {
                        if (!ClanService.IsAClanLeader(ch.Name))
                        {
                            ch.send("You cannot promote a clan leader.");
                            return;
                        }
                    }

                    if (ClanService.PromoteMember(clanName, playerName))
                    {
                        string currentRank = ClanService.GetPlayerRankAsString(playerName);
                        ch.send($"Player now has the rank : {currentRank}");     
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            }
            
        }


        private static void ClanDemotePlayerAsAdmin(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string clanName, out string remainingArgs_1))
            {
                if (getNextArg(remainingArgs_1, out string playerName, out string remainingArgs_2))
                {
                    if (ClanService.DemoteMember(clanName, playerName))
                    {
                        string currentRank = ClanService.GetPlayerRankAsString(playerName);
                        ch.send($"Player now has the rank : {currentRank}");
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            }
            else
            {
                ch.send("No clan name was specfied. Type 'clan help'.");
            }
        }
        

        private static void ClanDemotePlayerAsLeader(Character ch, string arguments)
        {
            string clanName = ClanService.GetClanName(ch.Name);

            if (clanName != "")
            {
                if (getNextArg(arguments, out string playerName, out string remainingArgs))
                {
                    // if trying to demote a clan leader
                    if (ClanService.IsAClanLeader(playerName))
                    {
                        if (!ClanService.IsAClanLeader(ch.Name))
                        {
                            ch.send("You cannot demote a clan leader.");
                            return;
                        }
                    }

                    if (ClanService.DemoteMember(clanName, playerName))
                    {
                        string currentRank = ClanService.GetPlayerRankAsString(playerName);
                        ch.send($"Player now has the rank : {currentRank}");  
                    }
                }
                else
                {
                    ch.send("No player name was specfied. Type 'clan help'.");
                }
            } 
        }


        private static void ClanGetPlayerInfo(Character ch, string arguments)
        {
            if (getNextArg(arguments, out string playerName, out string remainingArgs))
            {
                if (ClanService.IsPlayerInAnyClan(playerName, out string clanName))
                {
                    string rank = ClanService.GetPlayerRankAsString(playerName);
                    ch.send($"{playerName} has rank {rank} within clan {clanName}");
                }
                else
                {
                    ch.send("Player is not a member of any clan.");
                }
            }
            else
            {
                ch.send("No player name was given. Type clan help.");
            }
        }


        

        //--------------------- Class ClanCommandProcessor helper functions
        public static bool getNextArg(string arguments, out string nextArg, out string remainingArgs)
        {
            nextArg = "";
            remainingArgs = "";

            string pattern = @"'[^']+'|[^ ]+";
            var matches = Regex.Matches(arguments, pattern);

            List<string> args = matches.Cast<Match>().Select(m => m.Value).ToList();

            if (args.Count > 0)
            {
                nextArg = args[0].Replace("'", "");
                if (args.Count >= 1)
                {
                    args.RemoveAt(0);
                    remainingArgs = string.Join(" ", args);
                }
                return true;
            }
            return false;
        }
    }
}