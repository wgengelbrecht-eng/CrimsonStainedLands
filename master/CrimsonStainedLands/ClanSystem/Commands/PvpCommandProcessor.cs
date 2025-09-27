using CrimsonStainedLands.World;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Org.BouncyCastle.Security;
using System.Text.RegularExpressions;

namespace CrimsonStainedLands.ClanSystem
{
    public static class PvpDoFunction
    {
        public static void doPvp(Character ch, string arguments)
        {
            if (Helper.getNextArg(arguments, out string nextArg, out string remainingArgs))
            {
                switch (nextArg)
                {
                    case "help":
                        {
                            PvpService.CommandHelpPvp(ch, remainingArgs);
                            break;
                        }
                    case "list":
                        {
                            PvpService.CommandListPvpRooms(ch, remainingArgs);
                            break;
                        }
                    case "add_room":
                        {
                            PvpService.CommandAddPvpRoom(ch, remainingArgs);
                            break;
                        }
                    case "rem_room":
                        {
                            PvpService.CommandRemovePvpRoom(ch, remainingArgs);
                            break;
                        }
                    default:
                        {
                            ch.send("That is not a PvP command.");
                            break;
                        }
                }
            }
            else // Only 'pvp' was entered as a command, hench quick switch between pvp modes, 'On' and 'Off'
            {
                // If on, switch off, if off, switch on.
                PvpService.CommandPvpOnOff(ch, remainingArgs);   
            }
        }
    }
}