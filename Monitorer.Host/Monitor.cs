using System;

namespace Monitorer.Host
{
    public class Monitor
    {
        private MonitorHost _host;
        private MonitorStatus _monitorStatus;
        public Guid Guid { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual string Message =>
            $"{Name} @ {LastUpdated.ToString("T")} Status: {MonitorStatus.ToString()} For: {TimeSinceStatusChanged}";

        public MonitorStatus MonitorStatus
        {
            get => _monitorStatus;
            set
            {
                if (_monitorStatus != value)
                {
                    _monitorStatus = value;
                    StatusLastChanged = DateTime.Now;
                }
            }
        }

//        public virtual string Message => $"{Name} @ {LastUpdated.ToString("T")} MonitorStatus: {MonitorStatus.ToString()}";
        public string Name { get; set; }
        public DateTime StatusLastChanged { get; set; }

        public string TimeSinceStatusChanged
        {
            get
            {
                TimeSpan span = DateTime.Now.Subtract(StatusLastChanged);
                try
                {
                    string returnString = "";

                    if (span.Days >= 1)
                        returnString += $"{span.Days}d";
                    if (span.Hours >= 1)
                        returnString += $"{span.Hours}h";
                    if (span.Minutes >= 1)
                        returnString += $"{span.Minutes}m";
                    if (span.Seconds >= 1)
                        returnString += $"{span.Seconds}s";

                    if (string.IsNullOrEmpty(returnString))
                        returnString = "0s";

                    return returnString;
                }
                catch
                {
                    return span.ToString("g");
                }
            }
        }

        public Monitor(string name)
        {
            Guid = Guid.NewGuid();
            Name = name;
        }

        public override string ToString()
        {
            return Message;
        }


        public virtual void Update()
        {
            throw new NotImplementedException();
        }
    }
}