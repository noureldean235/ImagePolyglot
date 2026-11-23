
using System.Text.RegularExpressions;

namespace ImagePolyglot.Runner
{
    public class ChallengeRunner
    {
        public string Run(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "No code received.";

            var pattern =
                @"^\s*Console\.WriteLine\(""([^""\r\n]*)""\);\s*$";

            var lines = code.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.None
            );

            var output = new List<string>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = Regex.Match(line, pattern);

                if (!match.Success)
                    return "CHALLENGE_FAILED: Unsupported instruction.";

                output.Add(match.Groups[1].Value);
            }

            return string.Join(Environment.NewLine, output);
        }
    }
}
