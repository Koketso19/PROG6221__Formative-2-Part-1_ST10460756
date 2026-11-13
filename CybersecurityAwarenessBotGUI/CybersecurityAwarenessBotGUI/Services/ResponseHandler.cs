using System;
using System.Collections.Generic;
using CybersecurityAwarenessBotGUI.Models;

namespace CybersecurityAwarenessBotGUI.Services
{
    public class ResponseHandler
    {
        private readonly Random random = new Random();
        private readonly Dictionary<string, int> lastTipIndex = new Dictionary<string, int>();

        private readonly List<string> passwordTips = new List<string>
        {
            "Use a mix of uppercase, lowercase, numbers, and symbols.",
            "Never reuse passwords across different sites.",
            "Consider using a password manager to keep track of complex passwords.",
            "Enable Two-Factor Authentication (2FA) wherever possible."
        };

        private readonly List<string> phishingTips = new List<string>
        {
            "Never click on suspicious links in emails or SMS.",
            "Always check the sender's email address carefully.",
            "Look out for urgent language asking for personal information.",
            "If in doubt, contact the organisation directly using their official website."
        };

        private readonly List<string> privacyTips = new List<string>
        {
            "Review your privacy settings on social media regularly.",
            "Be careful about what personal information you share online.",
            "Use a VPN when connecting to public Wi-Fi.",
            "Read privacy policies before signing up for new services."
        };

        public string GetResponse(string input, UserProfile user, string sentiment)
        {
            string cleanedInput = input.ToLower().Trim();

            if (sentiment == "worried" || sentiment == "frustrated")
            {
                string prefix = sentiment == "worried"
                    ? $"It's completely understandable to feel that way, {user.Name}. Scammers can be very convincing."
                    : $"I hear you, {user.Name}. Let's slow down and take it one step at a time.";
                return $"{prefix} Here's a tip: {TipForTopic(user.LastTopic)}";
            }

            if (cleanedInput.Contains("i'm interested in") || cleanedInput.Contains("i am interested in"))
            {
                int index = cleanedInput.IndexOf("interested in") + 14;
                string topic = cleanedInput.Substring(index).Trim();
                user.FavouriteTopic = topic;
                return $"Great! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online.";
            }

            if (cleanedInput.Contains("another tip") || cleanedInput.Contains("tell me more") || cleanedInput.Contains("explain more"))
            {
                if (user.LastTopic == "password") return GetRandomTip(passwordTips, "password");
                if (user.LastTopic == "phishing") return GetRandomTip(phishingTips, "phishing");
                if (user.LastTopic == "privacy") return GetRandomTip(privacyTips, "privacy");
                return "What topic would you like to learn more about? (Password, Phishing, or Privacy)";
            }

            if (cleanedInput.Contains("how are you"))
                return $"I'm functioning perfectly, {user.Name}! I'm here to help you stay safe online.";

            if (cleanedInput.Contains("purpose"))
                return "My purpose is to educate South African citizens about cybersecurity threats like phishing, malware, and safe browsing.";

            if (cleanedInput.Contains("password") || cleanedInput.Contains("login") || cleanedInput.Contains("2fa"))
            {
                user.LastTopic = "password";
                return WrapWithFavourite(user, "password", GetRandomTip(passwordTips, "password"));
            }
            if (cleanedInput.Contains("phishing") || cleanedInput.Contains("scam") || cleanedInput.Contains("suspicious link"))
            {
                user.LastTopic = "phishing";
                return WrapWithFavourite(user, "phishing", GetRandomTip(phishingTips, "phishing"));
            }
            if (cleanedInput.Contains("privacy") || cleanedInput.Contains("safe browsing") || cleanedInput.Contains("vpn"))
            {
                user.LastTopic = "privacy";
                return WrapWithFavourite(user, "privacy", GetRandomTip(privacyTips, "privacy"));
            }

            if (cleanedInput.Contains("exit") || cleanedInput.Contains("bye"))
                return "EXIT";

            return "I didn't quite understand that. Could you please rephrase? Try asking about 'Password', 'Phishing', or 'Privacy'.";
        }

        private string WrapWithFavourite(UserProfile user, string topic, string tip)
        {
            if (!string.IsNullOrEmpty(user.FavouriteTopic) && user.FavouriteTopic.Contains(topic))
                return $"Since you're interested in {user.FavouriteTopic}: {tip}";
            return tip;
        }

        private string TipForTopic(string topic)
        {
            if (topic == "password") return GetRandomTip(passwordTips, "password");
            if (topic == "phishing") return GetRandomTip(phishingTips, "phishing");
            if (topic == "privacy") return GetRandomTip(privacyTips, "privacy");
            return GetRandomTip(passwordTips, "password");
        }

        private string GetRandomTip(List<string> tips, string topicKey)
        {
            int idx = random.Next(tips.Count);
            if (tips.Count > 1 && lastTipIndex.TryGetValue(topicKey, out int previous) && idx == previous)
                idx = (idx + 1) % tips.Count;
            lastTipIndex[topicKey] = idx;
            return "Tip: " + tips[idx];
        }
    }
}