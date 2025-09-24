using System;

namespace CrimsonStainedLands.ClanSystem
{
    public static class PvpNarratorMsgService
    {
        public static List<string> OffMsgs = new List<string>{  " [\\yPVP Narrator\\x] Scared mommy will see the bruises?",
                                                                " [\\yPVP Narrator\\x] You believe you are safe now.... but are you?",
                                                                " [\\yPVP Narrator\\x] Hmmm... So soon?" };

        public static List<string> OnMsgs = new List<string>{   " [\\yPVP Narrator\\x] Poke them with the pointy end!",
                                                                " [\\yPVP Narrator\\x] Always check your corners!",
                                                                " [\\yPVP Narrator\\x] Step right up and get your fresh meat right here!!!" };


        public static List<string> DiedMsgs = new List<string>{ " [\\yPVP Narrator\\x] Watch out! Oh..... sorry.",
                                                                " [\\yPVP Narrator\\x] Oof! Is that your new face now? ",
                                                                " [\\yPVP Narrator\\x] That did not go as planned. Was there even a plan?" };


        private static readonly Random random = new Random();

        public static string GetRandomPvpOffMsg()
        {
            int randomIndex = random.Next(OffMsgs.Count);
            return OffMsgs[randomIndex];
        }

        public static string GetRandomPvpOnMsg()
        {
            int randomIndex = random.Next(OnMsgs.Count);
            return OnMsgs[randomIndex];
        }

        public static string GetRandomPvpDiedMsg()
        {
            int randomIndex = random.Next(DiedMsgs.Count);
            return DiedMsgs[randomIndex];
        }
    }

}