using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CybersecurityAwarenessBotGUI
{
    public partial class MainWindow : Window
    {
        private UserProfile user;
        private ResponseHandler responder;
        private SentimentAnalyzer analyzer;

        public MainWindow()
        {
            InitializeComponent();
            user = new UserProfile();
            responder = new ResponseHandler();
            analyzer = new SentimentAnalyzer();

            // Start the conversation
            AddBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot.");
            AddBotMessage("Before we begin, what is your name?");

            // Play Voice Greeting on startup
            PlayVoiceGreeting();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                // Look for the WAV file in the same folder as the app
                SoundPlayer player = new SoundPlayer("ttsmaker-file-2026-9-11-9-56-30.wav");
                player.PlaySync();
            }
            catch
            {
                // If the WAV file is missing, just continue silently
                AddBotMessage("(Audio greeting unavailable - continuing in text mode)");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
            }
        }

        private void ProcessInput()
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            // 1. Display User Message
            AddUserMessage(input);
            UserInputBox.Clear();

            // 2. Handle Name Input
            if (string.IsNullOrEmpty(user.Name))
            {
                if (int.TryParse(input, out _))
                {
                    AddBotMessage("Invalid input. Please enter a valid name (letters only).");
                }
                else
                {
                    user.Name = input;
                    AddBotMessage($"Welcome, {user.Name}! I am your cybersecurity awareness assistant.");
                    AddBotMessage("You can ask me about: Password, Phishing, or Privacy. Or type 'I'm interested in [topic]'.");
                }
                return;
            }

            // 3. Detect Sentiment
            string sentiment = analyzer.DetectSentiment(input);

            // 4. Get Response
            string response = responder.GetResponse(input, user, sentiment);

            // 5. Handle Exit
            if (response == "EXIT")
            {
                AddBotMessage($"Goodbye, {user.Name}! Stay safe online.");
                SendButton.IsEnabled = false;
                UserInputBox.IsEnabled = false;
            }
            else
            {
                AddBotMessage(response);
            }
        }

        private void AddUserMessage(string message)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 150, 136)),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(50, 5, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            TextBlock text = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };
            bubble.Child = text;
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToEnd();
        }

        private void AddBotMessage(string message)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(60, 60, 80)),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 5, 50, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            TextBlock text = new TextBlock
            {
                Text = "Bot: " + message,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };
            bubble.Child = text;
            ChatPanel.Children.Add(bubble);
            ChatScrollViewer.ScrollToEnd();
        }
    }
}