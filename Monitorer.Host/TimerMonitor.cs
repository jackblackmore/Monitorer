using System;
using System.Net.NetworkInformation;
using System.Timers;

namespace Monitorer.Host
{
    public abstract class TimerMonitor
    {
        protected int _delay;
        private MonitorStatus _monitorStatus;
        public Guid Guid { get; set; }
        public DateTime LastRun { get; set; }

        public virtual string Message =>
            $"{Name} @ {LastRun.ToString("T")} Status: {MonitorStatus.ToString()} For: {TimeSinceStatusChanged}";

        public MonitorStatus MonitorStatus
        {
            get => _monitorStatus;
            set
            {
                if (_monitorStatus != value)
                {
                    _monitorStatus = value;
                    StatusChangeTime = DateTime.Now;
                }

                OnStatusUpdated();
            }
        }

//        public virtual string Message => $"{Name} @ {LastRun.ToString("T")} MonitorStatus: {MonitorStatus.ToString()}";
        public string Name { get; set; }
        public DateTime StatusChangeTime { get; set; }

        public Timer Timer { get; set; }

        public string TimeSinceStatusChanged
        {
            get
            {
                TimeSpan span = DateTime.Now.Subtract(StatusChangeTime);
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

        public TimerMonitor(string name, int delay)
        {
            Guid = Guid.NewGuid();
            Name = name;
            _delay = delay;

            Timer = new Timer(delay);
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
//            MonitorStatus = MonitorStatus.Refreshing;
        }

        private void OnStatusUpdated()
        {
            StatusUpdatedEventArgs args = new StatusUpdatedEventArgs(this);
            _statusUpdated?.Invoke(this, args);
        }

        /// <summary>
        /// Calls the timers OnElapsed method when ever our status updated event is subscribed to
        /// </summary>
        private event StatusUpdatedEventHandler _statusUpdated;

        public event StatusUpdatedEventHandler StatusUpdated
        {
            add
            {
                _statusUpdated += value;
                OnElapsed();
            }
            remove => _statusUpdated -= value;
        }

        protected virtual void OnElapsed()
        {
            LastRun = DateTime.Now;
        }

        public override string ToString()
        {
            return Message;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnElapsed();
        }
    }
}