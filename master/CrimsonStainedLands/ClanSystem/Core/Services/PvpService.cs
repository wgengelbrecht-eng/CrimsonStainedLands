using System.Data.SqlTypes;
using System.Xml.Serialization;
using CrimsonStainedLands.World;

namespace CrimsonStainedLands.ClanSystem
{
    public static class PvpService
    {
        private static readonly List<PvpEnabledMember> _pvpEnabledPlayers = []; //Quick access list
        private static readonly List<PvpEnabledRoom> _pvpEnabledRooms = [];//Quick access list
        private static readonly List<Character> _activePvpFightersOriginalState = [];
        private static readonly List<string> _isInActivePvpFightList = [];
        private static readonly List<String> _errorMsgBuffer = [];


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

        // Switches between on and off, if player is on this list it is on
        // This allows for a quick search on 'pvp on' players instead of iterating 
        // through the whole player base.
        public static bool SetPlayerPvpFlag(string playerName)
        {
            // Only clan members can partake in pvp
            if (ClanService.IsPlayerInAnyClan(playerName, out string clanName))
            {
                // Search for member in active list, remove if found, (switch off)
                foreach (PvpEnabledMember member in _pvpEnabledPlayers)
                {
                    if (member.PlayerName == playerName)
                    {
                        _pvpEnabledPlayers.Remove(member);
                        return true;
                    }
                }

                //Player was not found so add to list (switch on)
                PvpEnabledMember newMember = new PvpEnabledMember
                {
                    PlayerName = playerName,
                    ClanName = clanName
                };
                _pvpEnabledPlayers.Add(newMember);

                return true;
            }
            else
            {
                addErrorMsg("Only clan members can partake in PvP.");
                return false;
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


        public static void addPvpRoom(PvpEnabledRoom room)
        {
            _pvpEnabledRooms.Add(room);
            BackUpToFile();
        }


        public static void removePvpRoom(int vnum)
        {
            int count = 0;
            foreach (PvpEnabledRoom room in _pvpEnabledRooms)
            {
                if (room.RoomVnum == vnum)
                {
                    _pvpEnabledRooms.RemoveAt(count);
                    BackUpToFile();
                    return;
                }
                count++;
            }
        }


        public static int GetNumberOfRooms()
        {
            return _pvpEnabledRooms.Count;
        }

        public static bool IsRoomInPvpEnabledRoomList(int vNum)
        {
            foreach (PvpEnabledRoom room in _pvpEnabledRooms)
            {
                if (room.RoomVnum == vNum)
                {
                    return true;
                }
            }
            return false;
        }


        public static string GetListPvpEnabledRooms()
        {
            string retString = "";

            foreach (PvpEnabledRoom room in _pvpEnabledRooms)
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
            return retString;
        }
        

         public static bool LoadFromFile()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<PvpEnabledRoom>));
                List<PvpEnabledRoom> readList = new List<PvpEnabledRoom>();
                using (var reader = new StreamReader(Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)))
                {
                    readList = (List<PvpEnabledRoom>)serializer.Deserialize(reader);
                }

                _pvpEnabledRooms.Clear();
                foreach (PvpEnabledRoom room in readList)
                {
                    _pvpEnabledRooms.Add(room);
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                addErrorMsg($"Error: Access to the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)}' is denied.\n{ex.Message}");

            }
            catch (DirectoryNotFoundException ex)
            {
                addErrorMsg($"Error: The directory for the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)}' was not found.{ex.Message}");
            }
            catch (IOException ex)
            {
                addErrorMsg($"Error: A file I/O error occurred while writing to '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)}'.\n{ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                addErrorMsg($"Error: An XML serialization error occurred.\n");
                // Check the inner exception for the real cause
                if (ex.InnerException != null)
                {
                    addErrorMsg($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                addErrorMsg($"An unexpected error occurred during serialization: {ex.Message}");
            }
            return false;
        }


        public static bool BackUpToFile()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<PvpEnabledRoom>));

                using (var writer = new StreamWriter(Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)))
                {
                    serializer.Serialize(writer, new List<PvpEnabledRoom>(_pvpEnabledRooms));
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                addErrorMsg($"Error: Access to the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)}' is denied.\n{ex.Message}");

            }
            catch (DirectoryNotFoundException ex)
            {
                addErrorMsg($"Error: The directory for the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)}' was not found.{ex.Message}");
            }
            catch (IOException ex)
            {
                addErrorMsg($"Error: A file I/O error occurred while writing to '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)}'.\n{ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                addErrorMsg($"Error: An XML serialization error occurred.\n");
                // Check the inner exception for the real cause
                if (ex.InnerException != null)
                {
                    addErrorMsg($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex) // Catch any other unexpected exceptions
            {
                addErrorMsg($"An unexpected error occurred during serialization: {ex.Message}");
            }
            return false;
        }


        public static void EnsureFileExists()
        {

            try
            {
                Directory.CreateDirectory(GameSettings.ClanSystemDataFolder);

                if (!File.Exists(Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemPvpRoomFile)))
                {
                    //Create a clean|empty xml document.
                    BackUpToFile();

                }

            }
            catch (Exception ex)
            {
                addErrorMsg($"Exception occured in trying to ensure that folder : {GameSettings.ClanSystemDataFolder}" +
                            $" and file : {GameSettings.ClanSystemPvpRoomFile} exists. Exception: {ex.Message}");
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