using System;

namespace CybersecurityAwarenessBotGUI.Models
{
    public class CyberTask
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}