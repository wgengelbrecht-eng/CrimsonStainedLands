using CrimsonStainedLands;
using System.Collections;
using System.Text.RegularExpressions;
using CrimsonStainedLands.Extensions;

namespace ClanSystemMod
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
                    case "add-clan":
                        {
                            ClanService.CommandCreateClan(ch, remainingArgs);
                            break;
                        }
                    case "rem-clan":
                        {
                            ClanService.CommandRemoveClan(ch, remainingArgs);
                            break;
                        }
                    case "set-tag":
                        {
                            ClanService.CommandSetClanTag(ch, remainingArgs);
                            break;
                        }
                    case "update-name":
                        {
                            ClanService.CommandUpdateClanName(ch, remainingArgs);
                            break;
                        }
                    case "add-member":
                        {
                            ClanService.CommandAddMember(ch, remainingArgs);
                            break;
                        }
                    case "rem-member":
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
                    case "member-info":
                        {
                            ClanService.CommandGetMemberInfo(ch, remainingArgs);
                            break;
                        }
                    case "request":
                        {
                            ClanService.CommandClanRequestCreation(ch, remainingArgs);
                            break;
                        }
                    case "del-request":
                        {
                            ClanService.CommandDeleteClanRequest(ch, remainingArgs);
                            break;
                        }
                    case "clan-rooms":
                        {
                            ClanService.CommandListClanRooms(ch, remainingArgs);
                            break;
                        }
                    case "add-room":
                        {
                            ClanService.CommandCreateClanRoom(ch, remainingArgs);
                            break;
                        }
                    case "rem-room":
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