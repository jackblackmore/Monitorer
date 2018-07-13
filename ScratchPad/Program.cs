using System;
using System.Management;


namespace ScratchPad
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;

            ManagementScope scope = new ManagementScope(@"\\telvmkdbuat01\root\cimv2");
            scope.Options = options;

            SelectQuery query = new SelectQuery($"select * from Win32_Service where name like 'MasterKEY%'");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementBaseObject baseObject in collection)
                {
                    if (baseObject is ManagementObject service)
                    {
                        Console.WriteLine(service["Name"]);
                    }
                }

            }






        }
    }
}
