namespace CybersecurityAwarenessBotGUI
{
    public class SentimentAnalyzer
    {
        public string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();

            // Detect Worried / Frustrated
            if (lowerInput.Contains("worried") || lowerInput.Contains("scared") ||
                lowerInput.Contains("frustrated") || lowerInput.Contains("anxious"))
            {
                return "worried";
            }
            // Detect Curious
            else if (lowerInput.Contains("curious") || lowerInput.Contains("wondering") ||
                     lowerInput.Contains("interested"))
            {
                return "curious";
            }
            // Neutral / Normal
            return "neutral";
        }
    }
}