using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CybersecurityAwarenessBotGUI.Data;
using CybersecurityAwarenessBotGUI.Models;
using CybersecurityAwarenessBotGUI.Services;

namespace CybersecurityAwarenessBotGUI
{
    public partial class MainWindow : Window
    {
        private readonly UserProfile user;
        private readonly ResponseHandler responder;
        private readonly SentimentAnalyzer analyzer;

        private readonly TaskRepository taskRepository = new TaskRepository();
        private readonly QuizManager quizManager = new QuizManager();
        private readonly NlpProcessor nlpProcessor = new NlpProcessor();
        private readonly ActivityLogger activityLogger = new ActivityLogger();

        public MainWindow()
        {
            InitializeComponent();
            user = new UserProfile();
            responder = new ResponseHandler();
            analyzer = new SentimentAnalyzer();

            AddBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot.");
            AddBotMessage("Before we begin, what is your name?");

            PlayVoiceGreeting();

            activityLogger.ActivityAdded += () => Dispatcher.Invoke(() => DisplayRecentActivity(5));
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTasks();
            DisplayRecentActivity(5);
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                var player = new SoundPlayer("ttsmaker-file-2026-9-11-9-56-30.wav");
                player.PlaySync();
            }
            catch
            {
                AddBotMessage("(Audio greeting unavailable — continuing in text mode)");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessInput();

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessInput();
        }

        private void ProcessInput()
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AddUserMessage(input);
            UserInputBox.Clear();

            if (string.IsNullOrEmpty(user.Name))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^[A-Za-z\s\-']{2,50}$"))
                {
                    AddBotMessage("Invalid input. Please enter a name using letters only (2–50 characters).");
                    return;
                }
                user.Name = input;
                AddBotMessage($"Welcome, {user.Name}! I am your cybersecurity awareness assistant.");
                AddBotMessage("You can ask me about: Password, Phishing, or Privacy.");
                AddBotMessage("You can also say: 'Add a task to …', 'Remind me to … tomorrow', 'Start quiz', 'Show my tasks', 'What have you done for me?'");
                return;
            }

            IntentResult intent = nlpProcessor.DetectIntent(input);
            string botResponse;

            switch (intent.Intent)
            {
                case UserIntent.StartQuiz:
                    tabMain.SelectedIndex = 2;
                    StartQuizInternal();
                    activityLogger.Log("Quiz started via chatbot");
                    botResponse = "Sure. I've opened the cybersecurity quiz.";
                    break;

                case UserIntent.ShowTasks:
                    tabMain.SelectedIndex = 1;
                    LoadTasks();
                    activityLogger.Log("Tasks viewed via chatbot");
                    botResponse = "Here are your saved cybersecurity tasks.";
                    break;

                case UserIntent.ShowActivityLog:
                    tabMain.SelectedIndex = 3;
                    DisplayRecentActivity(5);
                    activityLogger.Log("Activity log requested via chatbot");
                    botResponse = "Here are your most recent actions.";
                    break;

                case UserIntent.AddTask:
                    botResponse = AddTaskFromChat(intent.TaskTitle, null);
                    break;

                case UserIntent.AddTaskWithReminder:
                    botResponse = AddTaskFromChat(intent.TaskTitle, intent.ReminderDate);
                    break;

                default:
                    string sentiment = analyzer.DetectSentiment(input);
                    botResponse = responder.GetResponse(input, user, sentiment);
                    if (botResponse == "EXIT")
                    {
                        AddBotMessage($"Goodbye, {user.Name}! Stay safe online.");
                        SendButton.IsEnabled = false;
                        UserInputBox.IsEnabled = false;
                        return;
                    }
                    break;
            }

            AddBotMessage(botResponse);
            DisplayRecentActivity(5);
        }

        private string AddTaskFromChat(string taskTitle, DateTime? reminderDate)
        {
            if (string.IsNullOrWhiteSpace(taskTitle))
                return "I recognised you want to add a task, but I need the task details. Try: 'Add a task to review my privacy settings'.";

            var task = new CyberTask
            {
                Title = taskTitle,
                Description = "Task created from chatbot conversation.",
                ReminderDate = reminderDate,
                IsCompleted = false
            };

            try
            {
                int id = taskRepository.AddTask(task);
                activityLogger.Log("Task added via chatbot: " + taskTitle);
                if (reminderDate.HasValue)
                    activityLogger.Log("Reminder set for " + taskTitle + " on " + reminderDate.Value.ToShortDateString());
                LoadTasks();

                return reminderDate.HasValue
                    ? $"Task added: {taskTitle}. Reminder set for {reminderDate.Value:yyyy-MM-dd}."
                    : $"Task added: {taskTitle}. Open the Tasks tab to add a reminder.";
            }
            catch (Exception ex)
            {
                return "I could not save that task: " + ex.Message;
            }
        }

        private void LoadTasks()
        {
            try
            {
                dgvTasks.ItemsSource = null;
                dgvTasks.ItemsSource = taskRepository.GetAllTasks();
                lblTaskStatus.Text = "";
            }
            catch (Exception ex)
            {
                lblTaskStatus.Text = "Database Error: Could not load tasks. " + ex.Message;
            }
        }

        private CyberTask GetSelectedTask() => dgvTasks.SelectedItem as CyberTask;

        private void chkUseReminder_Changed(object sender, RoutedEventArgs e)
        {
            if (dtpReminder != null)
                dtpReminder.IsEnabled = chkUseReminder.IsChecked == true;
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string description = txtTaskDescription.Text.Trim();

            if (title == "" || description == "")
            {
                lblTaskStatus.Text = "Please enter both a task title and description.";
                return;
            }

            DateTime? reminder = null;
            if (chkUseReminder.IsChecked == true)
            {
                if (dtpReminder.SelectedDate == null)
                {
                    lblTaskStatus.Text = "Please choose a reminder date, or uncheck the reminder box.";
                    return;
                }
                reminder = dtpReminder.SelectedDate.Value.Date;
            }

            var task = new CyberTask
            {
                Title = title,
                Description = description,
                ReminderDate = reminder,
                IsCompleted = false
            };

            try
            {
                int id = taskRepository.AddTask(task);
                lblTaskStatus.Text = "Task added successfully. ID: " + id;
                activityLogger.Log("Task added: " + task.Title);
                if (reminder.HasValue)
                    activityLogger.Log("Reminder set: " + task.Title + " — " + reminder.Value.ToShortDateString());

                txtTaskTitle.Clear();
                txtTaskDescription.Clear();
                chkUseReminder.IsChecked = false;
                LoadTasks();
            }
            catch (Exception ex)
            {
                lblTaskStatus.Text = "Task could not be saved: " + ex.Message;
            }
        }

        private void btnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask task = GetSelectedTask();
            if (task == null)
            {
                lblTaskStatus.Text = "Please select a task first.";
                return;
            }

            try
            {
                taskRepository.MarkTaskCompleted(task.TaskId);
                activityLogger.Log("Task completed: " + task.Title);
                lblTaskStatus.Text = "Task marked as completed.";
                LoadTasks();
            }
            catch (Exception ex)
            {
                lblTaskStatus.Text = "Could not complete task: " + ex.Message;
            }
        }

        private void btnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            CyberTask task = GetSelectedTask();
            if (task == null)
            {
                lblTaskStatus.Text = "Please select a task first.";
                return;
            }

            var answer = MessageBox.Show($"Delete this task?\n\n{task.Title}", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer != MessageBoxResult.Yes) return;

            try
            {
                taskRepository.DeleteTask(task.TaskId);
                activityLogger.Log("Task deleted: " + task.Title);
                lblTaskStatus.Text = "Task deleted.";
                LoadTasks();
            }
            catch (Exception ex)
            {
                lblTaskStatus.Text = "Could not delete task: " + ex.Message;
            }
        }

        private void btnRefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
            lblTaskStatus.Text = "Task list refreshed from database.";
        }

        private void StartQuizInternal()
        {
            quizManager.StartQuiz();
            lblQuizFeedback.Text = "";
            btnSubmitAnswer.IsEnabled = true;
            btnNextQuestion.IsEnabled = false;
            DisplayCurrentQuestion();
        }

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            StartQuizInternal();
            activityLogger.Log("Cybersecurity quiz started");
        }

        private void DisplayCurrentQuestion()
        {
            QuizQuestion q = quizManager.CurrentQuestion;
            lblQuestionNumber.Text = $"Question {quizManager.CurrentIndex + 1} of {quizManager.TotalQuestions}";
            lblQuestion.Text = q.QuestionText;

            RadioButton[] buttons = { rdoA, rdoB, rdoC, rdoD };
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].IsChecked = false;
                if (i < q.Options.Count)
                {
                    buttons[i].Content = q.Options[i];
                    buttons[i].Visibility = Visibility.Visible;
                }
                else
                {
                    buttons[i].Visibility = Visibility.Collapsed;
                }
            }

            lblQuizScore.Text = $"Score: {quizManager.Score}/{quizManager.TotalQuestions}";
        }

        private int GetSelectedQuizOption()
        {
            if (rdoA.IsChecked == true) return 0;
            if (rdoB.IsChecked == true) return 1;
            if (rdoC.IsChecked == true) return 2;
            if (rdoD.IsChecked == true) return 3;
            return -1;
        }

        private void btnSubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            int selected = GetSelectedQuizOption();
            if (selected == -1)
            {
                lblQuizFeedback.Text = "Please select an answer.";
                return;
            }

            QuizAnswerResult result = quizManager.CheckAnswer(selected);
            lblQuizFeedback.Text = result.Feedback;
            lblQuizScore.Text = $"Score: {quizManager.Score}/{quizManager.TotalQuestions}";

            btnSubmitAnswer.IsEnabled = false;
            btnNextQuestion.IsEnabled = true;
        }

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            bool hasNext = quizManager.MoveNext();
            if (hasNext)
            {
                lblQuizFeedback.Text = "";
                btnSubmitAnswer.IsEnabled = true;
                btnNextQuestion.IsEnabled = false;
                DisplayCurrentQuestion();
            }
            else
            {
                string comment = GetFinalQuizComment();
                lblQuizFeedback.Text = $"Quiz complete! Final score: {quizManager.Score}/{quizManager.TotalQuestions}. {comment}";
                activityLogger.Log($"Quiz completed — score {quizManager.Score}/{quizManager.TotalQuestions}");
                btnSubmitAnswer.IsEnabled = false;
                btnNextQuestion.IsEnabled = false;
            }
        }

        private string GetFinalQuizComment()
        {
            double percentage = (double)quizManager.Score / quizManager.TotalQuestions * 100;
            if (percentage >= 80) return "Excellent! You have strong cybersecurity awareness.";
            if (percentage >= 60) return "Good work. Review the topics you missed.";
            return "Keep learning. Revisit the cybersecurity tips and try again.";
        }

        private void DisplayRecentActivity(int count)
        {
            if (lstActivity == null) return;
            lstActivity.Items.Clear();

            List<ActivityEntry> recent = activityLogger.GetRecent(count);
            if (recent.Count == 0)
            {
                lstActivity.Items.Add("No activity recorded yet.");
                return;
            }

            foreach (ActivityEntry entry in recent)
                lstActivity.Items.Add(entry.ToString());
        }

        private void btnRefreshActivity_Click(object sender, RoutedEventArgs e) => DisplayRecentActivity(5);
        private void btnShowMore_Click(object sender, RoutedEventArgs e) => DisplayRecentActivity(10);

        private void AddUserMessage(string message)
        {
            var bubble = new Border { Background = new SolidColorBrush(Color.FromRgb(0, 150, 136)), CornerRadius = new CornerRadius(10), Padding = new Thickness(10), Margin = new Thickness(50, 5, 0, 5), HorizontalAlignment = HorizontalAlignment.Right };
            bubble.Child = new TextBlock { Text = message, Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap, FontSize = 14 };
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToEnd();
        }

        private void AddBotMessage(string message)
        {
            var bubble = new Border { Background = new SolidColorBrush(Color.FromRgb(60, 60, 80)), CornerRadius = new CornerRadius(10), Padding = new Thickness(10), Margin = new Thickness(0, 5, 50, 5), HorizontalAlignment = HorizontalAlignment.Left };
            bubble.Child = new TextBlock { Text = "Bot: " + message, Foreground = Brushes.White, TextWrapping = TextWrapping.Wrap, FontSize = 14 };
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToEnd();
        }
    }
}