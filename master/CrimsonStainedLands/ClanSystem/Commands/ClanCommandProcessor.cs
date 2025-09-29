using System.Collections;
using System.Text.RegularExpressions;
using CrimsonStainedLands.Extensions;

namespace CrimsonStainedLands.ClanSystem
{
    public static class ClanSystemDoFunction
    {

        //--- Main doClan
        public static void doClan(Character ch, string arguments)
        {
            if (Helper.getNextArg(arguments, out string nextArg, out string remainingArgs))
            {
                switch (nextArg.ToLower())
                {
                    case "help":
                        {
                            ClanService.CommandHelp(ch, remainingArgs);
                            break;
                        }
                    case "list":
                        {
                            ClanService.CommandGetClanList(ch, remainingArgs);
                            break;
                        }
                    case "members":
                        {
                            ClanService.CommandGetClanMemberList(ch, remainingArgs);
                            break;
                        }
                    case "create_clan":
                        {
                            ClanService.CommandCreateClan(ch, remainingArgs);
                            break;
                        }
                    case "delete_clan":
                        {
                            ClanService.CommandRemoveClan(ch, remainingArgs);
                            break;
                        }
                    case "set_tag":
                        {
                            ClanService.CommandSetClanTag(ch, remainingArgs);
                            break;
                        }
                    case "update_name":
                        {
                            ClanService.CommandUpdateClanName(ch, remainingArgs);
                            break;
                        }
                    case "add_member":
                        {
                            ClanService.CommandAddMember(ch, remainingArgs);
                            break;
                        }
                    case "rem_member":
                        {
                            ClanService.CommandRemoveMember(ch, remainingArgs);
                            break;
                        }
                    case "promote":
                        {
                            ClanService.CommandPromoteMember(ch, remainingArgs);
                            break;
                        }
                    case "demote":
                        {
                            ClanService.CommandDemoteMember(ch, remainingArgs);
                            break;
                        }
                    case "member_info":
                        {
                            ClanService.CommandGetMemberInfo(ch, remainingArgs);
                            break;
                        }
                    case "request":
                        {
                            ClanService.CommandClanRequestCreation(ch, remainingArgs);
                            break;
                        }
                    case "del_request":
                        {
                            ClanService.CommandDeleteClanRequest(ch, remainingArgs);
                            break;
                        }
                    case "clan_rooms":
                        {
                            ClanService.CommandListClanRooms(ch, remainingArgs);
                            break;
                        }
                    case "add_room":
                        {
                            ClanService.CommandCreateClanRoom(ch, remainingArgs);
                            break;
                        }
                    case "rem_room":
                        {
                            ClanService.CommandDeleteClanRoom(ch, remainingArgs);
                            break;
                        }
                    default:
                        {
                            ch.send("That is not a clan command. Type 'clan help'.");
                            break;
                        }
                }
            }
            else
            {
                ch.send("Do what with clan? Type 'clan help'.");
            }       
        }
    }
}