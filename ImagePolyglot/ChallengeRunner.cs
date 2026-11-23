
namespace ImagePolyglot
{
    public class ChallengeRunner
    {
        public bool CheckChallenge(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            // Simulated C# challenge syntax
            const string expectedCode =
                "Console.WriteLine(\"CHALLENGE_COMPLETED\");";

            return code.Trim() == expectedCode;
        }
    }
}
