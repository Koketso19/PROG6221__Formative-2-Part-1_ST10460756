using System;
using System.Media;

namespace CybersecurityAwarenessBot
{
    public class VoiceGreeting
    {
        public void PlayGreeting()
        {
            try
            {
                // PASTE YOUR COPIED PATH HERE (Inside the quotes)
                string audioPath = "ttsmaker-file-2026-9-11-9-56-30.wav";
                SoundPlayer player = new SoundPlayer(audioPath);
                player.PlaySync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Audio file not found. Error: " + ex.Message + "]");
                Console.WriteLine("Please check that the path in VoiceGreeting.cs is correct.");
            }
        }
    }
}