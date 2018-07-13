using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Management;

namespace Monitorer.Host
{
    public class ServiceMonitor : Monitor
    {
        // TODO: Create a ServiceMonitorHost then create ServiceMonitors for each service we want to know the status of
        
        private List<string> _services;
        private string _scopePath;
        private string _selectQueryPrefix;
        private ConnectionOptions _options;
        private ManagementScope _scope;
        public ServiceMonitor(string machine, List<string> services) : base(machine)
        {
            _services = services;
            _scopePath = @"\\" + machine + @"\root\cimv2";
            _options = new ConnectionOptions() {Impersonation =  ImpersonationLevel.Impersonate};
            _scope = new ManagementScope(_scopePath) {Options = _options};

            _selectQueryPrefix = "select * from Win32_Service where name in ({0})";

        }

        public override void Update()
        {
            string serviceString = string.Join(",", _services);
            string queryString = string.Format(_selectQueryPrefix, serviceString);
            SelectQuery query = new SelectQuery(queryString);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, query))
            {
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementBaseObject baseObject in collection)
                {
                    if (baseObject is ManagementObject service)
                    {
                        
                    }
                }
            }

        }
    }
}