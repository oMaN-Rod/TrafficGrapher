namespace TrafficGrapher.Model.Messages
{
    public class CloseDialogMessage
    {
        public bool Close { get; }

        public CloseDialogMessage(bool close)
        {
            Close = close;
        }
    }
}
