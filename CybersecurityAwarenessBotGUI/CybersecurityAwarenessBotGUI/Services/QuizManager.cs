using System.Collections.Generic;
using CybersecurityAwarenessBotGUI.Models;
using CybersecurityAwarenessBotGUI.Models.CybersecurityAwarenessBotGUI.Models;

namespace CybersecurityAwarenessBotGUI.Services
{
    public class QuizManager
    {
        private readonly List<QuizQuestion> questions;

        public int CurrentIndex { get; private set; }
        public int Score { get; private set; }
        public bool QuizStarted { get; private set; }
        public int TotalQuestions => questions.Count;
        public QuizQuestion CurrentQuestion => questions[CurrentIndex];

        public QuizManager() { questions = BuildQuestions(); }

        public void StartQuiz() { CurrentIndex = 0; Score = 0; QuizStarted = true; }

        public QuizAnswerResult CheckAnswer(int selectedIndex)
        {
            bool correct = selectedIndex == CurrentQuestion.CorrectOptionIndex;
            if (correct) Score++;
            return new QuizAnswerResult
            {
                IsCorrect = correct,
                Feedback = (correct ? "Correct! " : "Not quite. ") + CurrentQuestion.Explanation
            };
        }

        public bool MoveNext()
        {
            if (CurrentIndex < questions.Count - 1) { CurrentIndex++; return true; }
            QuizStarted = false;
            return false;
        }

        private List<QuizQuestion> BuildQuestions() => new List<QuizQuestion>
        {
            new QuizQuestion { QuestionText = "An unexpected email asks for your password. What should you do?", Options = new List<string> { "Reply with the password", "Click the link immediately", "Report it as phishing", "Forward it to friends" }, CorrectOptionIndex = 2, Explanation = "Legitimate organisations should not unexpectedly ask for your password by email." },
            new QuizQuestion { QuestionText = "Which password practice is safer?", Options = new List<string> { "Reuse one password everywhere", "Use unique passwords for important accounts", "Use your birthday as the password", "Share passwords with friends" }, CorrectOptionIndex = 1, Explanation = "Unique passwords reduce the damage if one account is compromised." },
            new QuizQuestion { QuestionText = "True or False: Public Wi-Fi is always safe for online banking.", Options = new List<string> { "True", "False" }, CorrectOptionIndex = 1, Explanation = "Public Wi-Fi can be risky; sensitive activity should use a trusted secure connection." },
            new QuizQuestion { QuestionText = "What is the main purpose of two-factor authentication (2FA)?", Options = new List<string> { "Make passwords shorter", "Add another verification step", "Remove the need for account security", "Share login details faster" }, CorrectOptionIndex = 1, Explanation = "2FA adds another verification step beyond the password." },
            new QuizQuestion { QuestionText = "What is social engineering?", Options = new List<string> { "Manipulating people into revealing information", "Designing social media logos", "Repairing computer hardware", "Compressing files" }, CorrectOptionIndex = 0, Explanation = "Social engineering targets people through manipulation rather than only attacking technology." },
            new QuizQuestion { QuestionText = "Before opening a link in a suspicious message, what should you do?", Options = new List<string> { "Open it quickly", "Verify the sender and inspect the link", "Disable security software", "Send personal information first" }, CorrectOptionIndex = 1, Explanation = "Checking the sender and destination helps identify suspicious links." },
            new QuizQuestion { QuestionText = "True or False: Reusing one very strong password on every account is recommended.", Options = new List<string> { "True", "False" }, CorrectOptionIndex = 1, Explanation = "Password reuse allows one breach to endanger several accounts." },
            new QuizQuestion { QuestionText = "Which attachment should make you most cautious?", Options = new List<string> { "An expected file from your lecturer", "An unexpected executable attachment", "A file you created yourself", "A known document from your own drive" }, CorrectOptionIndex = 1, Explanation = "Unexpected executable files can contain malicious software." },
            new QuizQuestion { QuestionText = "What can a password manager help you do?", Options = new List<string> { "Store and generate unique passwords", "Publish passwords online", "Disable account security", "Avoid using passwords completely" }, CorrectOptionIndex = 0, Explanation = "A trusted password manager can help create and store unique passwords securely." },
            new QuizQuestion { QuestionText = "Why should privacy settings be reviewed?", Options = new List<string> { "To control who can access personal information", "To make every account public", "To remove all passwords", "To accept every tracking request" }, CorrectOptionIndex = 0, Explanation = "Privacy settings help control how personal information is shared and exposed." },
            new QuizQuestion { QuestionText = "Why are software security updates important?", Options = new List<string> { "They only change colours", "They can fix known security weaknesses", "They always delete files", "They make phishing impossible" }, CorrectOptionIndex = 1, Explanation = "Updates often patch known vulnerabilities that attackers could exploit." },
            new QuizQuestion { QuestionText = "True or False: A message can be suspicious even if it uses a familiar company logo.", Options = new List<string> { "True", "False" }, CorrectOptionIndex = 0, Explanation = "Attackers can copy logos and branding, so the sender and link still need verification." }
        };
    }
}