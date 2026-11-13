using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace CybersecurityAwarenessBotGUI.Services
{
    public class IntentResult
    {
        public UserIntent Intent { get; set; }
        public string TaskTitle { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
    }
}