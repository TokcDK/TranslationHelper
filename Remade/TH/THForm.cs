using GetListOfSubClasses;
using Menus;
using System.IO;
using System.Windows.Forms;

namespace TH
{
    public partial class UICoreMainForm : Form
    {
        public UICoreMainForm()
        {
            InitializeComponent();

            LoadMenus();
        }

        private void LoadMenus()
        {
            //Load Menus
            var Plugins = Inherited.GetListOfInterfaceImplimentations<IMenu>(Path.Combine(Application.StartupPath, "Modules", "Menus"));

            foreach (var plugin in Plugins)
            {
                //Create new menu
                var menu = new ToolStripMenuItem
                {
                    Text = plugin.Text,
                    ToolTipText = plugin.ToolTipText
                };

                //Register click event
                menu.Click += plugin.OnClick;

                //check parent menu
                ToolStripMenuItem parent = null;
                bool HasParent;
                if (HasParent = !string.IsNullOrWhiteSpace(plugin.Parent))
                    if (!MenusCoreMenuStrip.Contains(plugin.Parent, out parent))
                    {
                        parent = new ToolStripMenuItem
                        {
                            Text = plugin.Parent
                        };
                    }

                //check parent category
                ToolStripMenuItem category = null;
                bool HasCategory = !string.IsNullOrWhiteSpace(plugin.Category);
                if (!string.IsNullOrWhiteSpace(plugin.Parent) && HasCategory)
                    if (!parent.Contains(plugin.Category, out category))
                    {
                        category = new ToolStripMenuItem
                        {
                            Text = plugin.Category
                        };
                    }

                //add category and make it current if exist
                if (HasCategory)
                {
                    category.DropDownItems.Add(menu);
                    menu = category;
                }

                //add parent and make it current if exist
                if (HasParent)
                {
                    parent.DropDownItems.Add(menu);
                    menu = parent;
                }

                //add result menu
                MenusCoreMenuStrip.Items.Add(menu);
            }
        }
    }
}
