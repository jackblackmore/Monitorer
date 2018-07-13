using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using System.Net.NetworkInformation;
using System.Timers;
using System.Windows.Forms.VisualStyles;

namespace Monitorer.Host
{
    public class PingMonitor : TimerMonitor
    {
        private string _path;
        private Ping _ping;
        private PingReply _pingReply;


        public PingMonitor(string name, string path, int delay) : base(name, delay)
        {
            _path = path;
            _ping = new Ping();
        }

        protected override void OnElapsed()
        {
            base.OnElapsed();
            _pingReply = _ping.Send(_path, 1000 > _delay ? _delay : 1000);
            MonitorStatus = _pingReply?.Status == IPStatus.Success ? MonitorStatus.Ok : MonitorStatus.Down;
        }

    }

    public class ServiceMonitor : TimerMonitor
    {
        public ServiceMonitor(string machine, List<string> services, int delay) : base(machine, delay)
        {

        }
        protected override void OnElapsed()
        {
            base.OnElapsed();
        }
    }
}