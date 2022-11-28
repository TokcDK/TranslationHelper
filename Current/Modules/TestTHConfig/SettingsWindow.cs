using System.ComponentModel;
using System.Linq;
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

            rootPanel.Controls.Add(rootFlowLayoutPanel);
            this.Controls.Add(rootPanel);

            var settingsType = THConfig.StaticSettings.Settings.GetType();

            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // https://stackoverflow.com/a/39336890 , get attributes for interface properties
            var displayNameInfos = CustomAttributeExtractorExtensions.GetPropertyAttributesFromType<DisplayNameAttribute>(settingsType);
            foreach (var p in THConfig.StaticSettings.Settings.GetType().GetProperties())
            {
                var t = p.GetMethod.ReturnType;
                if (t == typeof(bool))
                {
                    var cb = new CheckBox();
                    cb.AutoSize = true;

                    // set parameter display name
                    var displayNameInfo = displayNameInfos.FirstOrDefault(i => i.Property.Name == p.Name && i.Property.GetMethod.ReturnType == t);
                    cb.Text = displayNameInfo != null ? displayNameInfo.Attribute.DisplayName : p.Name;

                    // binding value
                    cb.DataBindings.Add(new Binding("Checked", THConfig.StaticSettings.Settings, p.Name));
                    rootFlowLayoutPanel.Controls.Add(cb);
                }
                else if (t == typeof(int) || t == typeof(string))
                {
                    var flp = new FlowLayoutPanel();
                    flp.AutoSize = true;
                    flp.Dock = DockStyle.Fill;

                    var lbl = new Label();
                    lbl.AutoSize = true;
                    var tbx = new TextBox();
                    tbx.AutoSize = true;

                    // set parameter display name
                    var displayNameInfo = displayNameInfos.FirstOrDefault(i => i.Property.Name == p.Name && i.Property.GetMethod.ReturnType == t);
                    lbl.Text = displayNameInfo != null ? displayNameInfo.Attribute.DisplayName : p.Name;

                    // binding value
                    tbx.DataBindings.Add(new Binding("Text", THConfig.StaticSettings.Settings, p.Name));

                    flp.Controls.Add(lbl);
                    flp.Controls.Add(tbx);

                    rootFlowLayoutPanel.Controls.Add(flp);
                }
            }
        }
    }
}
