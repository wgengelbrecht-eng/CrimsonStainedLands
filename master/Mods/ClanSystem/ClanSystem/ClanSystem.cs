using CrimsonStainedLands;

using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using ClanSystemMod;
using CrimsonStainedLands.World;

public class ClanSystemModule : CrimsonStainedLands.Module
{
    public ClanSystemModule(string dllPath, System.Reflection.Assembly assembly) : base(dllPath, assembly)
    {
        this.Name = "ClanSystemModule";
        this.Description = "Mod to add clan functionality.";


        CrimsonStainedLands.Command.Commands.Add(new CrimsonStainedLands.Command()
        {
            Name = "clan",
            Info = "Command for clan functionality.",
            Action = DoClan,
            MinimumLevel = 0,
            MinimumPosition = CrimsonStainedLands.Positions.Dead,
            NPCCommand = false,
            Skill = null
        });

        Module.OnDataLoadedEvent += OnDataLoaded;
        Module.Character.LoadingEvent += OnCharacterLoading;
    }


    public static void DoClan(CrimsonStainedLands.Character ch, string arguments)
    {
        //--- All the clan commands are located here
        ClanSystemDoFunction.doClan(ch, arguments);
    }


    private void OnDataLoaded()
    {
        // A helper function to handle loading errors consistently.
        bool HandleLoadError(string errorMessage, string systemName)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine(errorMessage);
                Console.WriteLine($"[{systemName}] has been disabled due to a loading error.");
                return false;
            }
            return true;
        }


        //--- Initialize Clan System
        ClanDBService.EnsureFileExists(out string errMsgEnsureFile);
        GameSettings.ClanSystemEnabled = HandleLoadError(errMsgEnsureFile, "Clan System");

        if (GameSettings.ClanSystemEnabled)
        {
            using (new LoadTimer("ClanSystem Service loaded {0} clans", () => ClanDBService.GetNumberOfClans()))
            {
                ClanDBService.ReadFromFileClans(out string errMsgReadClans);
                GameSettings.ClanSystemEnabled = HandleLoadError(errMsgReadClans, "Clan System");
            }
        }

        if (GameSettings.ClanSystemEnabled)
        {
            using (new LoadTimer("ClanSystem Service loaded {0} clan rooms", () => ClanDBService.getNumberOfClanRooms()))
            {
                ClanDBService.ReadFromFileClanRooms(out string errMsgReadClanRooms);
                GameSettings.ClanSystemEnabled = HandleLoadError(errMsgReadClanRooms, "Clan System");
            }
        }
    }
    
    private void OnCharacterLoading(CrimsonStainedLands.Character character, XElement element)
    {
        ClanMember member = new ClanMember();
        if (ClanService.IsPlayerInAnyClan(character.Name, out string clanName))
        {
            member.playerName = character.Name;
            member.ClanName = clanName;
            member.Rank = ClanService.GetPlayerRank(character.Name);
            character.Variables["ClanMember"] = member;
        }
        else
        {
            character.Variables["ClanMember"] = null;
        }
    }

}
