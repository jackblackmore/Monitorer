using System;
using System.Windows.Forms;

namespace Monitorer.Host
{
    public class MonitorUpdatedEventArgs
    {
        public string Message { get; set; }
        public Guid Guid { get; set; }
        public MonitorStatus Status { get; set; }
        public string Name { get; set; }

        public MonitorUpdatedEventArgs(Monitor monitor)
        {
            Guid = monitor.Guid;
            Status = monitor.MonitorStatus;
            Message = monitor.Message;
            Name = monitor.Name;

        }
    }

    public delegate void MonitorUpdatedEventHandler(object sender, MonitorUpdatedEventArgs args);
}