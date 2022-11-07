using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using THConfig.Extensions;

namespace TestTHConfig
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();

            THConfig.Manage.Load(Application.StartupPath, Application.ProductName);

            var rootPanel = new Panel();
            rootPanel.Dock = DockStyle.Fill;

            var rootFlowLayoutPanel = new FlowLayoutPanel();
            rootFlowLayoutPanel.Dock = DockStyle.Fill;
            rootFlowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            foreach(var p in THConfig.StaticSettings.Settings.GetType().GetProperties())
            {
                var t = p.GetMethod.ReturnType;
                if (t == typeof(bool))
                {
                    var cb = new CheckBox();
                    cb.AutoSize = true;

                    var attributes = p.GetCustomAttributes(true);

                    // set parameter display name
                    var displayName = attributes.FirstOrDefault(a => (a as DisplayNameAttribute) != null);
                    cb.Text = displayName != null ? (displayName as DisplayNameAttribute).DisplayName : p.Name;

                    // binding value
                    cb.DataBindings.Add(new Binding("Checked", THConfig.StaticSettings.Settings, p.Name));
                    rootFlowLayoutPanel.Controls.Add(cb);
                }
                break;
            }

            rootPanel.Controls.Add(rootFlowLayoutPanel);

            this.Controls.Add(rootPanel);
        }
    }
}
