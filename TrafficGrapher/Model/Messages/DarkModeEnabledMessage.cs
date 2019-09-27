namespace TrafficGrapher.Model.Messages
{
    public class DarkModeEnabledMessage
    {
        public bool DarkModeEnabled { get; }

        public DarkModeEnabledMessage(bool darkMode)
        {
            DarkModeEnabled = darkMode;
        }
    }
}
