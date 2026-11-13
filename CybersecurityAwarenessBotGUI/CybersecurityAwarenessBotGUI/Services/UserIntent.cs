using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CybersecurityAwarenessBotGUI.Services
{
    public enum UserIntent
    {
        Unknown,
        AddTask,
        AddTaskWithReminder,
        StartQuiz,
        ShowTasks,
        ShowActivityLog,
        CybersecurityQuestion
    }
}