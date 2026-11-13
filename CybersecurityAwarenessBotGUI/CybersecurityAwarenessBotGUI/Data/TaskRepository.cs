using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CybersecurityAwarenessBotGUI.Models;
using MySql.Data.MySqlClient;

namespace CybersecurityAwarenessBotGUI.Data
{
    public class TaskRepository
    {
        public int AddTask(CyberTask task)
        {
            string sql = @"INSERT INTO cyber_tasks (title, description, reminder_date, is_completed)
                           VALUES (@title, @description, @reminder, @completed);
                           SELECT SCOPE_IDENTITY();"
            ;

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@title", task.Title);
            command.Parameters.AddWithValue("@description", task.Description);
            command.Parameters.AddWithValue("@reminder", task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value);
            command.Parameters.AddWithValue("@completed", task.IsCompleted);

            connection.Open();
            decimal newId = (decimal)command.ExecuteScalar();
            return (int)newId;
        }

        public List<CyberTask> GetAllTasks()
        {
            var tasks = new List<CyberTask>();
            string sql = @"SELECT task_id, title, description, reminder_date, is_completed
                           FROM cyber_tasks
                           ORDER BY is_completed, task_id DESC;"
            ;

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            using var command = new SqlCommand(sql, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new CyberTask
                {
                    TaskId = Convert.ToInt32(reader["task_id"]),
                    Title = Convert.ToString(reader["title"]) ?? "",
                    Description = Convert.ToString(reader["description"]) ?? "",
                    ReminderDate = reader["reminder_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["reminder_date"]),
                    IsCompleted = Convert.ToBoolean(reader["is_completed"])
                });
            }
            return tasks;
        }

        public void MarkTaskCompleted(int taskId)
        {
            string sql = "UPDATE cyber_tasks SET is_completed = 1 WHERE task_id = @id;";
            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", taskId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateReminder(int taskId, DateTime reminderDate)
        {
            string sql = "UPDATE cyber_tasks SET reminder_date = @reminder WHERE task_id = @id;";
            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@reminder", reminderDate);
            command.Parameters.AddWithValue("@id", taskId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public void DeleteTask(int taskId)
        {
            string sql = "DELETE FROM cyber_tasks WHERE task_id = @id;";
            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", taskId);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}