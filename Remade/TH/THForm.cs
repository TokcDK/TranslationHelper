﻿using GetListOfSubClasses;
using Menus;
using System;
using System.IO;
using System.Windows.Forms;
using THCore;

namespace TH
{
    public partial class UICoreMainForm : Form
    {
        public UICoreMainForm()
        {
            InitializeComponent();

            SetVars();

            LoadMenus();
        }

        private void SetVars()
        {
            SharedData.StartupPath = Application.StartupPath;
            SharedData.MenuModulesDirPath = Path.Combine(SharedData.StartupPath, "Modules", "Menus");
        }

        private void LoadMenus()
        {
            //Load Menus
            var Plugins = Inherited.GetListOfInterfaceImplimentations<IMenu>(SharedData.MenuModulesDirPath);

            foreach (var plugin in Plugins)
            {
                //Create new menu
                var menu = new ToolStripMenuItem
                {
                    Text = plugin.Text,
                    ToolTipText = plugin.Description
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
