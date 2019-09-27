namespace TrafficGrapher.Model
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
