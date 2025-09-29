using System.Data.SqlTypes;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CrimsonStainedLands.World;

namespace CrimsonStainedLands.ClanSystem
{
    public static class PvpService
    {
        private static readonly List<PvpEnabledMember> _pvpEnabledPlayers = []; //--- Quick access list (Does not save to file)
        //private static readonly List<PvpEnabledRoom> _pvpEnabledRooms = [];//--- Moved to ClanDbService since it saves to file
        private static readonly List<Character> _activePvpFightersOriginalState = []; //--- (Does not save to file)
        private static readonly List<string> _isInActivePvpFightList = []; //--- (Does not save to file)


        // Switches between on and off, if player is on this list it is on
        // This allows for a quick search on 'pvp on' players instead of iterating 
        // through the whole player base.

        public static void CommandPvpOnOff(Character ch, string arguments)
        {
            // Only clan members can partake in pvp
            if (ClanService.IsPlayerInAnyClan(ch.Name, out string clanName))
            {
                // Search for member in active list, remove if found, (switch off)
                foreach (PvpEnabledMember member in _pvpEnabledPlayers)
                {
                    if (member.PlayerName.Equals(ch.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        _pvpEnabledPlayers.Remove(member);
                        ch.send($"PvP flag is now \\GOFF\\x {PvpNarratorMsgService.GetRandomPvpOffMsg()}");
                        return;
                    }
                }

                //Player was not found so add to list (switch on)
                PvpEnabledMember newMember = new PvpEnabledMember
                {
                    PlayerName = ch.Name,
                    ClanName = clanName
                };
                _pvpEnabledPlayers.Add(newMember);
                ch.send($"PvP flag is now \\RON\\x {PvpNarratorMsgService.GetRandomPvpOnMsg()}");
            }
            else
            {
                ch.send("Only clan members can partake in PvP.");
            }
        }


        public static bool IsPlayerPvpFlagOn(string playerName, out string clanName)
        {
            clanName = "";
            foreach (PvpEnabledMember member in _pvpEnabledPlayers)
            {
                if (member.PlayerName.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    clanName = member.ClanName;
                    return true;
                }
            }
            return false;
        }


        public static void CommandAddPvpRoom(Character ch, string arguments)
        {
            if (!(ch.Level >= GameSettings.MinLevelRequiredForClanCreation))
            {
                ch.send("You cannot add PvP rooms.");
            }

            if (Helper.getNextArg(arguments, out string roomVnum, out string remainingArgs))
            {
                bool success = int.TryParse(roomVnum, out int roomVnumber);
                if (success)
                {
                    if (ClanDBService.IsRoomInPvpEnabledRoomList(roomVnumber))
                    {
                        ch.send("That room is already in the PvP enabled room list.");
                        return;
                    }

                    RoomData curRoom;
                    if (RoomData.Rooms.TryGetValue(roomVnumber, out curRoom))
                    {
                        var room = new PvpEnabledRoom
                        {
                            RoomVnum = roomVnumber,
                        };

                        ClanDBService.AddPvpEnabledRoom(room);
                        ClanDBService.WriteToFilePvpRooms(out string errMsg);// Backup changes
                        if (errMsg != "")
                        {
                            ch.send($"Something went wrong : {errMsg}");
                            return;
                        }
                        ch.send($"The following room vnum was added: \\r{curRoom.Name}\\x located in the vacinity of \\b{curRoom.Area.Name}\\x");
                    }
                    else
                    {
                        ch.send("That room does not exist.");
                    }
                }
                else
                {
                    ch.send("That is not a number.");
                }
            }
            else
            {
                ch.send("No Vnum was given.");
            }
        }



        public static void CommandRemovePvpRoom(Character ch, string arguments)
        {
            if (!(ch.Level >= GameSettings.MinLevelRequiredForClanCreation))
            {
                ch.send("You cannot remove PvP rooms.");
            }

            if (Helper.getNextArg(arguments, out string roomVnum, out string remainingArgs))
            {
                bool success = int.TryParse(roomVnum, out int roomVnumber);
                if (success)
                {
                    if (ClanDBService.IsRoomInPvpEnabledRoomList(roomVnumber))
                    {
                        ClanDBService.RemovePvpEnabledRoom(roomVnumber);
                    }
                    else
                    {
                        ch.send("That room is not in the PvP enabled rooms list.");
                    }
                }
                else
                {
                    ch.send("That is not a number.");
                }
            }
            else
            {
                ch.send("No Vnum was given.");
            }
        }



        public static void CommandListPvpRooms(Character ch, string arguments)
        {
            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)// List for admin player
            {
                string retString = "";
                foreach (PvpEnabledRoom room in ClanDBService.GetAllPvpEnabledRooms())
                {
                    RoomData curRoom;
                    if (RoomData.Rooms.TryGetValue(room.RoomVnum, out curRoom))
                    {
                        retString += $"\\r{room.RoomVnum}\\x|\\g{curRoom.Name}\\x located in the vacinity of \\b{curRoom.Area.Name}\\x\n";
                    }
                    else
                    {
                        retString += $"No description for room with vNum \\r{room.RoomVnum}\\x\n";
                    }
                }

                if (retString != "")
                {
                    string firstLine = "Here is the list of currently available PvP enabled rooms.\n";
                    firstLine += retString;
                    ch.send(firstLine);
                }
                else
                {
                    ch.send("There are currently no PvP enabled rooms.");
                }
            }
            else // List for non admin players
            {
                string retString = "";
                foreach (PvpEnabledRoom room in ClanDBService.GetAllPvpEnabledRooms())
                {
                    RoomData curRoom;
                    if (RoomData.Rooms.TryGetValue(room.RoomVnum, out curRoom))
                    {
                        retString += $"\\r{curRoom.Name}\\x located in the vacinity of \\b{curRoom.Area.Name}\\x\n";
                    }
                    else
                    {
                        retString += $"No description for room with vNum \\r{room.RoomVnum}\\x\n";
                    }
                }

                if (retString != "")
                {
                    string firstLine = "Here is the list of currently available PvP enabled rooms.\n";
                    firstLine += retString;
                    ch.send(firstLine);
                }
                else
                {
                    ch.send("There are currently no PvP enabled rooms.");
                }
            }
        }


        public static void CommandHelpPvp(Character ch, string arguments)
        {
            string helpMsgForAdmins = $"Here follows the pvp commands for admin players.\n" +
                                        $"\\g{"list",-30}\\x| {"List all pvp rooms.",-100}\n" +
                                        $"\\g{"pvp",-30}\\x| {"Quick switch between PvP modes.",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x pvp",-100}\n" +
                                        $"\\g{"add_room",-30}\\x| {"Add a room to the quick access PvP enabled rooms",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x pvp add_room 'room vNum'",-100}\n" +
                                        $"\\g{"rem_room",-30}\\x| {"Remove a room from the quick access Pvp enabled rooms",-100}\n" +
                                        $"{"",-30}| {"\\rUsage :\\x pvp rem_room 'room vNum'",-100}\n";

            string helpMsgForAll = $"Here follows the pvp commands for admin players.\n" +
                                        $"\\g{"list",-30}\\x| {"List all pvp rooms.",-100}\n" +
                                        $"\\g{"pvp",-30}\\x| {"Quick switch between PvP modes.",-100}\n";

            if (ch.Level >= GameSettings.MinLevelRequiredForClanCreation)
            {
                ch.send(helpMsgForAdmins);
            }
            else
            {
                ch.send(helpMsgForAll);
            }
        }




        public static void savePlayerState(Character ch)
        {
            _activePvpFightersOriginalState.Add(ch);
        }


        public static bool hasPlayerSavedState(string playerName)
        {
            foreach (Character ch in _activePvpFightersOriginalState)
            {
                if (ch.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool getPlayerOriginalState(string playerName, out Character chReturn)
        {
            chReturn = new Character();
            foreach (Character ch in _activePvpFightersOriginalState)
            {
                if (ch.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    chReturn = ch;
                    return true;
                }
            }
            return false;
        }

        public static void addPlayerToIsInActivePvpFightList(string playerName)
        {
            _isInActivePvpFightList.Add(playerName);
        }


        public static bool IsPlayerInActivePvpFight(string playerName)
        {
            foreach (string name in _isInActivePvpFightList)
            {
                if (name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static void RemovePlayerFromIsInActivePvpFightList(string playerName)
        {
            _isInActivePvpFightList.Remove(playerName);
        }

        public static void PvpAttack(Character ch, Character victim)
        {
            /*
            if (IsSaveToAttack(ch, victim))
            {
                if (!hasPlayerSavedState(ch.Name))
                    savePlayerState(ch);

                if (!hasPlayerSavedState(victim.Name))
                    savePlayerState(victim);

                if (!IsPlayerInActivePvpFight(ch.Name))
                    setPlayerAsActivelyPvpFighting(ch.Name);

                if (!IsPlayerInActivePvpFight(victim.Name))
                    setPlayerAsActivelyPvpFighting(victim.Name);

                if (ch != null && victim != null)
                {
                    ItemData weapon;
                    ch.Position = Positions.Fighting;
                    ch.Fighting = victim;
                    if (ch.Form == null)
                        ch.Equipment.TryGetValue(WearSlotIDs.Wield, out weapon);
                    else
                        weapon = null;

                    Combat.oneHit(ch, victim, weapon);
                    ch.WaitState(Game.PULSE_VIOLENCE);
                }
            }
            */
        }

        public static bool IsSaveToAttack(Character ch, Character victim)
        {
            /*
            if (!PvpService.IsRoomInPvpEnabledRoomList(ch.Room.Vnum))
            {
                ch.send("This room is not flagged for PvP combat!");
                return false;
            }

            if (!PvpService.IsPlayerPvpFlagOn(ch.Name, out string clanNameCh))
            {
                ch.send("You don't have your PvP flag on. You cannot attack.");
                return false;
            }

            if (!PvpService.IsPlayerPvpFlagOn(victim.Name, out string clanNameVictim))
            {
                ch.send($"{victim.Name} does not have their PvP flag set to on, you cannot attack them!");
                return false;
            }

            if (ch == victim)// This is not 'Fight club', :)
            {
                ch.send("This is not 'Fight club', you cannot attack yourself!");
                return false;
            }

            */
            return true;
        }
    }
}