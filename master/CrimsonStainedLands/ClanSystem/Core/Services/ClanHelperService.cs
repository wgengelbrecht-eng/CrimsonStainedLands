using System.Text.RegularExpressions;

namespace CrimsonStainedLands.ClanSystem
{
    public static class Helper
    {
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