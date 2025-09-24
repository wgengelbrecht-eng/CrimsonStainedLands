using System.Xml;
using System.Xml.Serialization;
using FxSsh.Messages;

namespace CrimsonStainedLands.ClanSystem
{
    public static class ClanDBService
    {
        private static readonly List<Clan> _clans = [];
        //Universal Msg buffer for error messages and feedback. 
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


        public static bool addClan(Clan clan)
        {
            if (!ClanExists(clan.Name))
            {
                _clans.Add(clan);
                if (!BackUpToFile())
                {
                    return false;
                }
                return true;
            }
            else
            {
                addErrorMsg("This clan already exists.");
                return false;
            }
        }

        public static bool removeClan(string clanName)
        {

            int placement = 0;
            foreach (Clan clan in _clans)
            {
                if (clan.Name.Equals(clanName, StringComparison.CurrentCultureIgnoreCase))
                {
                    _clans.RemoveAt(placement);
                    if (!BackUpToFile())
                    {
                        return false;
                    }
                    return true;
                }
                placement++;
            }
            addErrorMsg("No clan by that name was found.");
            return false;
        }

        public static bool UpdateClanRecord(Clan clan)
        {
            foreach (Clan clanEntry in _clans)
            {
                if (clanEntry.Name.Equals(clan.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    _clans.Remove(clanEntry);
                    _clans.Add(clan);
                    if (!BackUpToFile())
                    {
                        //undo
                        _clans.Remove(clan);
                        _clans.Add(clanEntry);
                        return false;
                    }
                    return true;
                }
            }
            addErrorMsg("That clan does not exist.");
            return false;
        }


        public static Clan GetClan(string clanName)
        {
            foreach (Clan clan in _clans)
            {
                if (clan.Name.Equals(clanName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return clan;
                }
            }
            addErrorMsg("No Clan by that name found.");
            Clan retClan = null;
            return retClan;
        }


        public static Clan GetClanByPlayerName(string playerName)
        {
            Clan retClan = null;
            foreach (Clan clan in _clans)
            {
                foreach (ClanMember member in clan.Members)
                {
                    if (member.playerName == playerName)
                    {
                        return clan;
                    }
                }
            }
            // Player  not found
            addErrorMsg("No clans have that player as a member.");
            return retClan;
        }


        public static bool ClanExists(string clanName)
        {
            foreach (Clan clan in _clans)
            {
                if (clan.Name.Equals(clanName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        public static List<Clan> GetAllClans()
        {
            return _clans;
        }


        public static bool BackUpToFile()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<Clan>));

                using (var writer = new StreamWriter(Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)))
                {
                    serializer.Serialize(writer, new List<Clan>(_clans));
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                addErrorMsg($"Error: Access to the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)}' is denied.\n{ex.Message}");

            }
            catch (DirectoryNotFoundException ex)
            {
                addErrorMsg($"Error: The directory for the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)}' was not found.{ex.Message}");
            }
            catch (IOException ex)
            {
                addErrorMsg($"Error: A file I/O error occurred while writing to '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)}'.\n{ex.Message}");
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


        public static bool LoadFromFile()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<Clan>));
                List<Clan> readList = new List<Clan>();
                using (var reader = new StreamReader(Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)))
                {
                    readList = (List<Clan>)serializer.Deserialize(reader);
                }

                _clans.Clear();
                foreach (Clan clan in readList)
                {
                    addClan(clan);
                }
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                addErrorMsg($"Error: Access to the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)}' is denied.\n{ex.Message}");

            }
            catch (DirectoryNotFoundException ex)
            {
                addErrorMsg($"Error: The directory for the path '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)}' was not found.{ex.Message}");
            }
            catch (IOException ex)
            {
                addErrorMsg($"Error: A file I/O error occurred while writing to '{Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)}'.\n{ex.Message}");
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


        public static int GetNumberOfClans()
        {
            return _clans.Count;
        }


        public static void EnsureFileExists()
        {

            try
            {
                Directory.CreateDirectory(GameSettings.ClanSystemDataFolder);

                if (!File.Exists(Path.Combine(GameSettings.ClanSystemDataFolder, GameSettings.ClanSystemDataFile)))
                {
                    //Create a clean|empty xml document.
                    BackUpToFile();

                }

            }
            catch (Exception ex)
            {
                addErrorMsg($"Exception occured in trying to ensure that folder : {GameSettings.ClanSystemDataFolder}" +
                            $" and file : {GameSettings.ClanSystemDataFile} exists. Exception: {ex.Message}");
            }
        }
        
        
    }
}