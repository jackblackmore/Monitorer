using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using Monitorer.Host.Properties;
using Timer = System.Timers.Timer;

namespace Monitorer.Host
{
    public class MonitorerSystemTrayHost : IDisposable
    {
        private Dictionary<Guid, TimerMonitor> _timers;
        private MonitorerTrayMenu _trayMenu;
        private NotifyIcon _trayIcon;
        private string _balloonMessage;
        private ToolTipIcon _balloonIcon;
        private Timer _balloonTimer;

        public MonitorerSystemTrayHost()
        {
            _timers = new Dictionary<Guid, TimerMonitor>();
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

            CreatePingTimer("telvsqldev01", "telvsqldev01", 30 * 1000);
            CreatePingTimer("telvsqluat01", "telvsqluat01", 30 * 1000);
            CreatePingTimer("telvsqlao01", "telvsqlao01", 30 * 1000);
        }

        private PingMonitor CreatePingTimer(string name, string path, int delay)
        {
            PingMonitor timerMonitor = new PingMonitor(name, path, delay);
            _timers.Add(timerMonitor.Guid, timerMonitor);
            _trayMenu.AddTimerToMenu(timerMonitor);

            timerMonitor.StatusUpdated += TimerStatusUpdated;
            return timerMonitor;
        }

        private void TimerStatusUpdated(object sender, StatusUpdatedEventArgs e)
        {
            if (_trayMenu.GetCheckedStatusByGuid(e.Guid))
            {
                if ((int) e.ToolTipIcon > (int) _balloonIcon)
                    _balloonIcon = e.ToolTipIcon;
            
                _balloonMessage += $"{e.Message}\r\n";
                ShowBalloonTip("Ping MonitorStatus");
            }
        }

        private void ShowBalloonTip(string title)
        {
            _balloonTimer.Start();
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

            foreach (TimerMonitor timer in _timers.Values.OrderBy(x => x.Name))
            {
                if (!_trayMenu.GetCheckedStatusByGuid(timer.Guid))
                    continue;

                if ((int) timer.MonitorStatus > (int) status)
                    status = timer.MonitorStatus;

                _balloonMessage += $"{timer.Message}\r\n";
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
            _balloonMessage = string.Empty;
            _balloonIcon = ToolTipIcon.None;
        }
    }
}