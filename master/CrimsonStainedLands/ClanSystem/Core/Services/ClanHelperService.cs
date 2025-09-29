using System.Text.RegularExpressions;
using CrimsonStainedLands.Extensions;

namespace CrimsonStainedLands.ClanSystem
{
    public static class Helper
    {
        /// <summary>
        /// Gets the next word in a string of words seperated by the delimiter ' ' and multiple words if quoted, ' or ".
        /// This function uses the Utilty.OneArgument for it's core but returns a bool depending on if a next word 
        /// was available or not.
        /// </summary>
        /// <param name="arguments">String with multiple words of which the first word needs to be removed.</param>
        /// <param name="nextArg">Returns the next word from |arguments|, returns "" if none. </param>
        /// <param name="remainingArgs">Returns the remaining words after the first word has been removed.</param>
        /// <returns>True if a next word was availble, False if no more words where available.</returns>
        public static bool getNextArg(string arguments, out string nextArg, out string remainingArgs)
        {
            nextArg = "";
            remainingArgs = Utility.OneArgument(arguments, ref nextArg);
            if (nextArg != "")
                return true;

            return false;

            /*
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
            */
        }   
    }
}