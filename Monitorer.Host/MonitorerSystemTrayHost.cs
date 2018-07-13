using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using System.Windows.Forms;
using Monitorer.Host.Properties;
using Timer = System.Timers.Timer;

namespace Monitorer.Host
{
    public class MonitorerSystemTrayHost : IDisposable
    {
        private Dictionary<Guid, MonitorHost> _hosts;
        private Dictionary<Guid, MonitorUpdatedEventArgs> _monitorStatuses;
        private MonitorerTrayMenu _trayMenu;
        private NotifyIcon _trayIcon;
        private string _balloonMessage;
        private ToolTipIcon _balloonIcon;
        private Timer _balloonTimer;

        public MonitorerSystemTrayHost()
        {
            _hosts = new Dictionary<Guid, MonitorHost>();
            _monitorStatuses = new Dictionary<Guid, MonitorUpdatedEventArgs>();
            _trayIcon = new NotifyIcon();
            _trayMenu = new MonitorerTrayMenu(this);
            _balloonTimer = new Timer(10 * 1000);
            _balloonTimer.Elapsed += BalloonTimer_Elapsed;
            _balloonMessage = string.Empty;
            _balloonIcon = ToolTipIcon.None;
        }

        private void BalloonTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ResetBalloonTip();
            _balloonTimer.Stop();
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
        }

        public void Display()
        {
            _trayIcon.Icon = Resources.Magnify;
            _trayIcon.Text = "Monitorer";
            _trayIcon.Visible = true;
            _trayIcon.MouseClick += TrayIconOnMouseClick;
            _trayIcon.ContextMenuStrip = _trayMenu.Menu;
            
            TimerMonitorHost timerHost = new TimerMonitorHost("SQL Pings", 30 * 1000);
            _hosts.Add(timerHost.Guid, timerHost);
            timerHost.AddMonitor(CreatePingTimer("telvsqldev01", "telvsqldev01", 3 * 1000));
            timerHost.AddMonitor(CreatePingTimer("telvsqluat01", "telvsqluat01", 3 * 1000));
            timerHost.AddMonitor(CreatePingTimer("telvsqlao01", "telvsqlao01", 3 * 1000));

            timerHost.MonitorUpdated += MonitorUpdated;
            

        }

        private PingMonitor CreatePingTimer(string name, string path, int delay)
        {
            PingMonitor timerMonitor = new PingMonitor(name, path, delay);
            _trayMenu.AddTimerToMenu(timerMonitor);
            return timerMonitor;
        }

        private void MonitorUpdated(object sender, MonitorUpdatedEventArgs e)
        {
            if (_trayMenu.GetCheckedStatusByGuid(e.Guid))
            {
                _monitorStatuses[e.Guid] = e;
                ShowBalloonTip("Ping MonitorStatus");
            }
        }

        private void ShowBalloonTip(string title)
        {
            _balloonMessage = "";
            foreach (MonitorUpdatedEventArgs e in _monitorStatuses.Values.OrderBy(x => x.Name))
            {
                _balloonMessage += $"{e.Message}\r\n";
            }
            _trayIcon.ShowBalloonTip(0, title, !string.IsNullOrEmpty(_balloonMessage) ? _balloonMessage : "No monitors to report", _balloonIcon);
        }

        private void TrayIconOnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReportMonitorsStatus();
            }
        }

        public void ReportMonitorsStatus()
        {
            MonitorStatus status = MonitorStatus.Ok;

            _balloonIcon = ToolTipIcon.None;
            _balloonMessage = string.Empty;

            foreach (MonitorHost host in _hosts.Values.OrderBy(x => x.Name))
            {
                host.GetMonitorStatus();
            }

            _balloonIcon = ConvertTimerStatusToToolTipIcon(status);
            ShowBalloonTip("Monitorer MonitorStatus");
        }

        public static ToolTipIcon ConvertTimerStatusToToolTipIcon(MonitorStatus status)
        {
            switch (status)
            {
                case MonitorStatus.Warning:
                    return ToolTipIcon.Warning;
                case MonitorStatus.Error:
                    return ToolTipIcon.Error;
                case MonitorStatus.Down:
                    return ToolTipIcon.Error;
                default:
                    return ToolTipIcon.None;
            }
        }

        private void ResetBalloonTip()
        {
            _balloonIcon = ToolTipIcon.None;
        }
    }
}