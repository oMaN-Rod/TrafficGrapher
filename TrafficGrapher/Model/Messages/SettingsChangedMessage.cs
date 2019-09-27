namespace TrafficGrapher.Model.Messages
{
    public class SettingsChangedMessage
    {
        public bool Changed { get; }

        public SettingsChangedMessage(bool changed)
        {
            Changed = changed;
        }
    }
}
