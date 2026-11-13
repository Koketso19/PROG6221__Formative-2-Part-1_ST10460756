using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CybersecurityAwarenessBotGUI.Models;

namespace CybersecurityAwarenessBotGUI.Data
{
    public class TaskRepository
    {
        // TEMPORARY: Fake database so the app doesn't crash without MySQL
        private static List<CyberTask> fakeDatabase = new List<CyberTask>();
        private static int nextId = 1;

        public int AddTask(CyberTask task)
        {
            task.TaskId = nextId++;
            fakeDatabase.Add(task);
            return task.TaskId;
        }

        public List<CyberTask> GetAllTasks()
        {
            return fakeDatabase;
        }

        public void MarkTaskCompleted(int taskId)
        {
            var task = fakeDatabase.Find(t => t.TaskId == taskId);
            if (task != null) task.IsCompleted = true;
        }

        public void UpdateReminder(int taskId, DateTime reminderDate)
        {
            var task = fakeDatabase.Find(t => t.TaskId == taskId);
            if (task != null) task.ReminderDate = reminderDate;
        }

        public void DeleteTask(int taskId)
        {
            var task = fakeDatabase.Find(t => t.TaskId == taskId);
            if (task != null) fakeDatabase.Remove(task);
        }
    }
}