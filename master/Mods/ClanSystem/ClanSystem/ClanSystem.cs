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
        Module.Character.OnEnterRoomEvent += OnCharacterEnterRoom;
    }


    public static void DoClan(CrimsonStainedLands.Character ch, string arguments)
    {
        //--- All the clan commands are located here
        ClanSystemDoFunction.doClan(ch, arguments);
    }

    public static void OnCharacterEnterRoom(CrimsonStainedLands.Character character, RoomData oldRoom, RoomData newRoom)
    {
        ClanService.OnCharacterEnterRoom(character, oldRoom, newRoom);

        /*
        //--- For testing, remove at later stage
        ClanMember member = character.GetVariable<ClanMember>("ClanMember");
        ClanRoom room = newRoom.GetVariable<ClanRoom>("ClanRoom");

        if (member != null)
        {
            character.send($"ClanMember object : {member.playerName} | {member.Rank} | {member.ClanName}\n");
        }

        if (room != null)
        {
            character.send($"ClanRoom object : {room.RoomVnum} | {room.ClanName}");
        }
        */
    }



    private void OnDataLoaded()
    {
        ClanService.OnDataLoaded();
    }


    private void OnCharacterLoading(CrimsonStainedLands.Character character, XElement element)
    {
        ClanService.OnCharacterLoading(character, element);
    }
}
