using System;
using System.Windows.Forms;

namespace Monitorer.Host
{
    public class StatusUpdatedEventArgs
    {
        public string Message { get; set; }
        public Guid Guid { get; set; }
        public MonitorStatus Status { get; set; }
        public ToolTipIcon ToolTipIcon { get; set; }

        public StatusUpdatedEventArgs(TimerMonitor timer)
        {
            Guid = timer.Guid;
            Status = timer.MonitorStatus;
            Message = timer.Message;

            ToolTipIcon = MonitorerSystemTrayHost.ConvertTimerStatusToToolTipIcon(Status);
        }
    }

    public delegate void StatusUpdatedEventHandler(object sender, StatusUpdatedEventArgs args);
}