# PROG6221 Cybersecurity Awareness Chatbot — Final POE

## Student Information
- **Name:** [Your Name Here]
- **Student Number:** [Your Student Number Here]
- **Module:** PROG6221 Programming 2A

## Project Overview
This is a GUI-based Cybersecurity Awareness Assistant built in WPF. It combines the functionality developed in Parts 1, 2 and 3 into one cohesive application.

## Final Features

### Part 1
- Voice greeting on startup (WAV file).
- ASCII branding on the chat header.
- Basic cybersecurity interaction.
- Input validation (name and message).

### Part 2
- GUI interaction (WPF, dark theme, chat bubbles).
- Keyword recognition (password, phishing, privacy + synonyms).
- Random responses (no immediate repeats).
- Conversation flow ("tell me more").
- Memory and recall (name, favourite topic, last topic).
- Sentiment detection (worried, frustrated, curious).

### Part 3
- **Task Assistant:** Add, view, complete, and delete tasks with optional reminders.
- **Database:** Tasks are saved to a database and persist across restarts.
- **Cybersecurity Quiz:** 12 questions (Multiple Choice + True/False) with immediate feedback and final score.
- **NLP Simulation:** Recognizes different phrasings using string matching and regex to trigger actions (e.g., "remind me to update my password tomorrow").
- **Activity Log:** Records meaningful actions (tasks, quiz, etc.) and displays the most recent 5-10 activities.

## Database Setup
*(Note: Depending on whether you use MySQL or SQL Server, adjust these steps)*
1. Ensure your database server (MySQL or SQL Server) is running.
2. Open the database management tool and run the `database_setup.sql` script provided in the project folder.
3. Open `Data/DatabaseConfig.cs` and ensure the connection string matches your local setup (e.g., your root password).
4. Build and run the application.

## How to Use
- **Chat tab:** Ask about `password`, `phishing`, `privacy`, or say `I'm interested in phishing`.
- **Tasks tab:** Fill in the title and description, optionally add a reminder date, and click Add Task. Select a row to Mark Complete or Delete.
- **Cyber Quiz tab:** Click Start Quiz, submit your answer, and click Next. Your final score is shown at the end.
- **Activity Log tab:** Shows the most recent 5 actions. Use Show More to see 10.

### Example Chatbot Commands
- `Add a task to enable two-factor authentication`
- `Remind me to update my password tomorrow`
- `Start quiz`
- `Show my tasks`
- `What have you done for me?`

## Releases
- `v3.0-final`

## Video Presentation
[Paste your unlisted YouTube video link here]