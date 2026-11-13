using System.Collections.Generic;
using System.Linq;
using CybersecurityAwarenessBotGUI.Models;

namespace CybersecurityAwarenessBotGUI.Services
{
    public class ActivityLogger
    {
        private readonly List<ActivityEntry> entries = new List<ActivityEntry>();
        public event System.Action? ActivityAdded;

        public void Log(string description)
        {
            entries.Add(new ActivityEntry { Time = System.DateTime.Now, Description = description });
            ActivityAdded?.Invoke();
        }

        public List<ActivityEntry> GetRecent(int count) =>
            entries.OrderByDescending(x => x.Time).Take(count).ToList();
    }
}