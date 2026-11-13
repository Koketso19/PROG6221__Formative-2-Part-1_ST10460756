using System;

namespace CybersecurityAwarenessBotGUI.Models
{
    public class ActivityEntry
    {
        public DateTime Time { get; set; }
        public string Description { get; set; } = "";
        public override string ToString() => $"{Time:HH:mm} - {Description}";
    }
}