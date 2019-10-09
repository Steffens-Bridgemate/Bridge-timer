using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                    player.SoundLocation=(App.GetFullAppDataPath(App.RoundStartedSoundFile));
                    player.Play();
                    break;
                case CountDownTimer.ThresholdReached.RoundEnded:
                    player.SoundLocation = (App.GetFullAppDataPath(App.RoundEndedSoundFile));
                    player.Play();
                    break;
                case CountDownTimer.ThresholdReached.EndOfRoundWarning:
                    player.SoundLocation = (App.GetFullAppDataPath(App.WarningSoundFile));
                    player.Play();
                    break;
            }
        }
    }
}
