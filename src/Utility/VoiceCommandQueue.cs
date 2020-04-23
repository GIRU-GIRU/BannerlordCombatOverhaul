using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace GCO.Utility
{
    internal static class VoiceCommandQueue
    {
        private static readonly Queue<VoiceCommandQueueItem> queue = new Queue<VoiceCommandQueueItem>();
        private static MissionTime VoiceCommandTimer = MissionTime.Now;

        internal static void ResetVoiceCommandTimer(float delay = 2000f)
        {
            VoiceCommandTimer = MissionTime.MillisecondsFromNow(delay);
        }

        internal static MissionTime GetVoiceCommandTimer()
        {
            return VoiceCommandTimer;
        }

        internal static void QueueItem(string voiceTypeString, float delayAfter = 2000f)
        {
            if (queue.Count > 7)
            {
                _ = queue.Dequeue();
            }

            queue.Enqueue(new VoiceCommandQueueItem(voiceTypeString, delayAfter));
        }

        internal static Queue<VoiceCommandQueueItem> GetVoiceCommandQueue()
        {
            return queue;
        }

        internal static VoiceCommandQueueItem GetNextQueueItem()
        {
            var item = queue.Dequeue();
            while (queue.Count > 0 && queue.Peek() == item)
                return GetNextQueueItem();
            return item;
        }
    }
}

