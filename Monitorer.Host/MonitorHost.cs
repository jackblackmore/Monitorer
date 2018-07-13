using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Monitorer.Host
{
    public class MonitorHost
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        private Dictionary<Guid, Monitor> _monitors;
        public DateTime LastUpdated { get; set; }

        public MonitorHost(string name)
        {
            Guid = Guid.NewGuid();
            Name = name;
            _monitors = new Dictionary<Guid, Monitor>();
        }

        public virtual void AddMonitor(Monitor monitor)
        {
            _monitors.Add(monitor.Guid, monitor);
        }

        public virtual void GetMonitorUpdates()
        {
            
            LastUpdated = DateTime.Now;
            foreach (Monitor monitor in _monitors.Values)
            {
                monitor.Update(); // TODO: Create Async Update event
                OnMonitorUpdate(monitor);
            }
        }

        public virtual void GetMonitorStatus()
        {
            foreach (Monitor monitor in _monitors.Values.OrderBy(x => x.Name))
            {
                OnMonitorUpdate(monitor);
            }
        }

        private void OnMonitorUpdate(Monitor monitor)
        {
            MonitorUpdatedEventArgs args = new MonitorUpdatedEventArgs(monitor);
            _monitorUpdated?.Invoke(this, args);
        }

        /// <summary>
        /// Calls the timers OnElapsed method when ever our status updated event is subscribed to
        /// </summary>
        private event MonitorUpdatedEventHandler _monitorUpdated;

        public event MonitorUpdatedEventHandler MonitorUpdated
        {
            add
            {
                _monitorUpdated += value;
                GetMonitorUpdates();
            }
            remove => _monitorUpdated -= value;
        }

    }

    public class TimerMonitorHost : MonitorHost
    {
        protected int _delay;
        public Timer Timer { get; set; }

        public TimerMonitorHost(string name, int delay) : base(name)
        {
            _delay = delay;

            Timer = new Timer(delay);
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetMonitorUpdates();
        }
    }
}