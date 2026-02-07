namespace JobOverview.Services
{
    public class ValidationRulesException : Exception
    {
        public Dictionary<string, string[]> Errors { get; } = new();
        public ValidationRulesException()
        {
        }
        public ValidationRulesException(string message, string v)
            : base(message)
        {
            
        }
        public ValidationRulesException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
    }
}