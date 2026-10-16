using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBotGUI
{
    public class ResponseHandler
    {
        private Random random = new Random();

        // Lists for Random Responses (Part 2 requirement)
        private List<string> passwordTips = new List<string>
        {
            "Use a mix of uppercase, lowercase, numbers, and symbols.",
            "Never reuse passwords across different sites.",
            "Consider using a password manager to keep track of complex passwords.",
            "Enable Two-Factor Authentication (2FA) wherever possible."
        };

        private List<string> phishingTips = new List<string>
        {
            "Never click on suspicious links in emails or SMS.",
            "Always check the sender's email address carefully.",
            "Look out for urgent language asking for personal information.",
            "If in doubt, contact the organisation directly using their official website."
        };

        private List<string> privacyTips = new List<string>
        {
            "Review your privacy settings on social media regularly.",
            "Be careful about what personal information you share online.",
            "Use a VPN when connecting to public Wi-Fi.",
            "Read privacy policies before signing up for new services."
        };

        public string GetResponse(string input, UserProfile user, string sentiment)
        {
            string cleanedInput = input.ToLower().Trim();

            // --- 1. HANDLE SENTIMENT FIRST ---
            if (sentiment == "worried")
            {
                return $"It's completely understandable to feel that way, {user.Name}. Scammers can be very convincing. Let me share a tip to help you stay safe: {passwordTips[random.Next(passwordTips.Count)]}";
            }

            // --- 2. HANDLE MEMORY (Favourite Topic) ---
            if (cleanedInput.Contains("i'm interested in") || cleanedInput.Contains("i am interested in"))
            {
                int index = cleanedInput.IndexOf("interested in") + 14;
                string topic = cleanedInput.Substring(index).Trim();
                user.FavouriteTopic = topic;
                return $"Great! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online.";
            }

            // --- 3. HANDLE FOLLOW-UP (Conversation Flow) ---
            if (cleanedInput.Contains("another tip") || cleanedInput.Contains("tell me more") || cleanedInput.Contains("explain more"))
            {
                if (user.LastTopic == "password") return GetRandomTip(passwordTips);
                if (user.LastTopic == "phishing") return GetRandomTip(phishingTips);
                if (user.LastTopic == "privacy") return GetRandomTip(privacyTips);
                return "What topic would you like to learn more about? (Password, Phishing, or Privacy)";
            }

            // --- 4. KEYWORD RECOGNITION & BASIC RESPONSES ---
            if (cleanedInput.Contains("how are you"))
            {
                return $"I'm functioning perfectly, {user.Name}! I'm here to help you stay safe online.";
            }
            else if (cleanedInput.Contains("purpose"))
            {
                return "My purpose is to educate South African citizens about cybersecurity threats like phishing, malware, and safe browsing.";
            }
            else if (cleanedInput.Contains("password"))
            {
                user.LastTopic = "password";
                return GetRandomTip(passwordTips);
            }
            else if (cleanedInput.Contains("phishing"))
            {
                user.LastTopic = "phishing";
                return GetRandomTip(phishingTips);
            }
            else if (cleanedInput.Contains("privacy"))
            {
                user.LastTopic = "privacy";
                return GetRandomTip(privacyTips);
            }
            else if (cleanedInput.Contains("exit") || cleanedInput.Contains("bye"))
            {
                return "EXIT";
            }
            else
            {
                return "I didn't quite understand that. Could you please rephrase? Try asking about 'Password', 'Phishing', or 'Privacy'.";
            }
        }

        private string GetRandomTip(List<string> tips)
        {
            return "Tip: " + tips[random.Next(tips.Count)];
        }
    }
}