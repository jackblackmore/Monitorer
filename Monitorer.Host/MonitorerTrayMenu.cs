using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Monitorer.Host
{
    public class MonitorerTrayMenu
    {
        private MonitorerSystemTrayHost _host;
        private Dictionary<string, ToolStripMenuItem> _monitorItemGroups;
        private Dictionary<Guid, MonitorMenuItem> _monitorItems;
        public ContextMenuStrip Menu { get; set; }

        public MonitorerTrayMenu(MonitorerSystemTrayHost host)
        {
            _monitorItemGroups = new Dictionary<string, ToolStripMenuItem>();
            _monitorItems = new Dictionary<Guid, MonitorMenuItem>();
            _host = host;
            CreateMenu();
        }

        public void CreateMenu()
        {
            Menu = new ContextMenuStrip();
            ToolStripMenuItem item = new ToolStripMenuItem("Exit", null, Exit_Click);
            Menu.Items.Add(new ToolStripSeparator());
            Menu.Items.Add(item);
        }

        public bool GetCheckedStatusByGuid(Guid guid)
        {
            if (_monitorItems.Count == 0) // Needed if event fires before items are put into dictionary
                return true;

            if (_monitorItems.TryGetValue(guid, out MonitorMenuItem item))
                return item.Checked;
            else
                return false;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void AddTimerToMenu(TimerMonitor timer)
        {
            ToolStripMenuItem group = GetCreateMonitorGroup(timer);
            MonitorMenuItem item = new MonitorMenuItem(timer, timer.Name) {Checked = true};
            _monitorItems.Add(timer.Guid, item);
            // TODO: Add item alphabetically
            group.DropDownItems.Add(item);
            item.Click += Item_Click;
        }

        private ToolStripMenuItem GetCreateMonitorGroup(TimerMonitor timer)
        {
            string groupName = timer.GetType().Name;
            if (_monitorItemGroups.TryGetValue(groupName, out ToolStripMenuItem group))
                return group;

            ToolStripMenuItem newGroup = new ToolStripMenuItem(groupName);
            _monitorItemGroups.Add(groupName, newGroup);
            // TODO: Add item alphabetically
            Menu.Items.Insert(0, newGroup);

            return newGroup;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            if (sender is MonitorMenuItem item)
            {
                item.Checked = !item.Checked;
                _host.ReportMonitorsStatus();
            }
        }
    }

    public class MonitorMenuItem : ToolStripMenuItem
    {
        public TimerMonitor Timer { get; set; }

        public MonitorMenuItem(TimerMonitor timer, string text) : base(text)
        {
            Timer = timer;
        }
    }
}