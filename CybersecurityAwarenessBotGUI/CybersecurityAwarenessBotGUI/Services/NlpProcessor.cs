using System;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBotGUI.Services
{
    public class NlpProcessor
    {
        public IntentResult DetectIntent(string rawInput)
        {
            string input = rawInput.Trim().ToLower();

            if (ContainsAny(input, "show activity log", "show my activity", "recent actions", "what have you done for me", "activity log"))
                return new IntentResult { Intent = UserIntent.ShowActivityLog };

            if (ContainsAny(input, "start quiz", "play quiz", "cyber quiz", "test my knowledge", "cybersecurity game", "start the quiz"))
                return new IntentResult { Intent = UserIntent.StartQuiz };

            if (ContainsAny(input, "show tasks", "show my tasks", "view tasks", "what tasks do i have", "my tasks"))
                return new IntentResult { Intent = UserIntent.ShowTasks };

            if (input.Contains("remind me to") || input.Contains("set a reminder"))
                return new IntentResult
                {
                    Intent = UserIntent.AddTaskWithReminder,
                    TaskTitle = ExtractTaskTitle(input),
                    ReminderDate = ExtractReminderDate(input)
                };

            if (ContainsAny(input, "add task", "add a task", "create task", "create a task", "new task"))
                return new IntentResult
                {
                    Intent = UserIntent.AddTask,
                    TaskTitle = ExtractTaskTitle(input)
                };

            return new IntentResult { Intent = UserIntent.CybersecurityQuestion };
        }

        private bool ContainsAny(string input, params string[] phrases)
        {
            foreach (string p in phrases)
                if (input.Contains(p)) return true;
            return false;
        }

        public string ExtractTaskTitle(string input)
        {
            string title = Regex.Replace(
                input,
                @"^(please\s+)?(add\s+(a\s+)?task\s+to |create\s+(a\s+)?task\s+to? |new\s+task\s+to? |remind\s+me\s+to\s*)",
                "",
                RegexOptions.IgnoreCase);

            title = Regex.Replace(title, @"\s+(tomorrow|in\s+\d+\s+days?)\s*[\.\!\?]*$", "", RegexOptions.IgnoreCase);
            return title.Trim().TrimEnd('.', '!', '?');
        }

        public DateTime? ExtractReminderDate(string input)
        {
            if (input.Contains("tomorrow"))
                return DateTime.Today.AddDays(1);

            Match m = Regex.Match(input, @"in\s+(\d+)\s+days?", RegexOptions.IgnoreCase);
            if (m.Success && int.TryParse(m.Groups[1].Value, out int days))
                return DateTime.Today.AddDays(days);

            return null;
        }
    }
}