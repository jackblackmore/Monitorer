using System;
using System.IO;
using System.Net.Configuration;
using System.Net.NetworkInformation;
using System.Threading;
using System.Timers;
using System.Windows.Forms.VisualStyles;

namespace Monitorer.Host
{
    public class PingMonitor : Monitor
    {
        private string _path;
        private int _maxTimeout;
        private Ping _ping;
        private PingReply _pingReply;


        public PingMonitor(string name, string path, int maxTimeout) : base(name)
        {
            _path = path;
            _maxTimeout = maxTimeout;
            _ping = new Ping();
        }

        public override void Update()
        {
            _pingReply =  _ping.Send(_path, _maxTimeout < 5000 ? _maxTimeout : 1000);            
            MonitorStatus = _pingReply?.Status == IPStatus.Success ? MonitorStatus.Ok : MonitorStatus.Down;
        }
    }
}