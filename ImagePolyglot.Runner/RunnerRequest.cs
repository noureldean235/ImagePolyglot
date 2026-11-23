namespace ImagePolyglot.Runner
{
    public class RunnerRequest
    {
        public string Code { get; set; } = string.Empty;
    }

    public class RunnerResponse
    {
        public bool Success { get; set; }
        public string Output { get; set; } = string.Empty;
    }
}