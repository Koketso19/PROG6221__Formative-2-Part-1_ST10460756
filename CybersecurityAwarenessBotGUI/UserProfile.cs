namespace CybersecurityAwarenessBotGUI
{
    public class UserProfile
    {
        // Automatic property for Name
        public string Name { get; set; }

        // Memory: Store user's favourite topic
        public string FavouriteTopic { get; set; } = "";

        // Memory: Store the last topic discussed (for "tell me more")
        public string LastTopic { get; set; } = "";
    }
}