using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Text;

namespace BridgeTimer
{
    public class SoundThresholdNotificator : Viewmodel.IThresholdNotification
    {

        public bool IsMuted { get; set; }
        public void Notify(CountDownTimer.ThresholdReached threshold)
        {
            if (IsMuted) return;

            var player = new SoundPlayer();
            switch(threshold)
            {
                case CountDownTimer.ThresholdReached.RoundStarted:
                    player.SoundLocation=(@"Sounds\roundstarted.wav");
                    player.Play();
                    break;
                case CountDownTimer.ThresholdReached.RoundEnded:
                    player.SoundLocation = (@"Sounds\roundended.wav");
                    player.Play();
                    break;
                case CountDownTimer.ThresholdReached.EndOfRoundWarning:
                    player.SoundLocation = (@"Sounds\warning.wav");
                    player.Play();
                    break;
            }
        }
    }
}
