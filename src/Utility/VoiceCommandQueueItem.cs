using System.Collections.Generic;

namespace GCO.Utility
{
    internal class VoiceCommandQueueItem
    {
        public string VoiceTypeString { get; }

        public float DelayAfter { get; }

        public VoiceCommandQueueItem(string voiceTypeString, float delayAfter)
        {
            VoiceTypeString = voiceTypeString;
            DelayAfter = delayAfter;
        }

        public static bool operator ==(VoiceCommandQueueItem left, VoiceCommandQueueItem right)
        {
            return left.VoiceTypeString == right.VoiceTypeString && left.DelayAfter == right.DelayAfter;
        }

        public static bool operator !=(VoiceCommandQueueItem left, VoiceCommandQueueItem right)
        {
            return left.VoiceTypeString != right.VoiceTypeString || left.DelayAfter != right.DelayAfter;
        }

        public override bool Equals(object obj)
        {
            return obj is VoiceCommandQueueItem item &&
                   VoiceTypeString == item.VoiceTypeString &&
                   DelayAfter == item.DelayAfter;
        }

        public override int GetHashCode()
        {
            int hashCode = 1798943918;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(VoiceTypeString);
            hashCode = hashCode * -1521134295 + DelayAfter.GetHashCode();
            return hashCode;
        }
    }
}
