using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTHConfig
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();

            THConfig.Manage.Load(Application.StartupPath, Application.ProductName);

            var rootPanel = new Panel();

            var rootFlowLayoutPanel = new FlowLayoutPanel();
            rootFlowLayoutPanel.Dock = DockStyle.Fill;
            rootFlowLayoutPanel.FlowDirection = FlowDirection.TopDown;

            rootPanel.Container.Add(rootFlowLayoutPanel);
        }
    }
}
