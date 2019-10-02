using System;
using TrafficGrapher.Model.Enums;

namespace TrafficGrapher.Model
{
    public class PollData
    {
        public TimeSpan PrevUpTime { get; set; }
        public TimeSpan UpTime { get; set; }
        public double Previous { get; set; }
        public double Current { get; set; }
        public double Diff { get; set; }
        public DateTime PollTime { get; set; }
        public DateTime StartTime { get; }
        public double ElapsedTime => UpTime.TotalMilliseconds - PrevUpTime.TotalMilliseconds;
        public PollData(DateTime dateTime)
        {
            StartTime = dateTime;
        }

        public double CalcDiff(CounterUnit unit)
        {
            double octets;

            if (Previous > Current)
            {
                if (Previous < Math.Pow(2, 32))
                {
                    octets = Math.Pow(2, 32) - Previous + Current;
                }
                else if (Previous < Math.Pow(2, 64))
                {
                    octets = Math.Pow(2, 64) - Previous + Current;
                }
                else
                {
                    octets = 0;
                }
            }
            else
            {
                octets = Current - Previous;
            }

            if (unit == CounterUnit.Bits)
            {
                Diff = octets * 8 / ElapsedTime / 1000;
            }
            else { Diff = octets / ElapsedTime / 1000; }
            return Diff;
        }

        public void Next()
        {
            PrevUpTime = UpTime;
            Previous = Current;
        }
    }
}