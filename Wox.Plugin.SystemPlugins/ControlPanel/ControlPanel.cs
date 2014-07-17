﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Infrastructure;
using Wox.Infrastructure.Storage.UserSettings;
using WindowsControlPanelItems;
using System.Diagnostics;
using System.IO;

namespace Wox.Plugin.SystemPlugins.ControlPanel
{
    public class ControlPanel : BaseSystemPlugin
    {
        #region Properties

        private PluginInitContext context;

        public override string Description
        {
            get
            {
                return "Search within the Control Panel.";
            }
        }

        public override string ID
        {
            get { return "209621585B9B4D81813913C507C058C6"; }
        }

        public override string Name { get { return "Control Panel"; } }

        public override string IcoPath { get { return @"Images\ControlPanel.png"; } }

        private List<ControlPanelItem> controlPanelItems;
        private string iconFolder;

        #endregion Properties

        protected override void InitInternal(PluginInitContext context)
        {
            this.context = context;
            controlPanelItems = WindowsControlPanelItems.List.Create(48);
            iconFolder = @"Images\ControlPanelIcons\";

            foreach (ControlPanelItem item in controlPanelItems)
            {
                if (!File.Exists(iconFolder + item.ApplicationName + ".ico"))
                {
                    item.Icon.ToBitmap().Save(iconFolder + item.ApplicationName + ".ico"); //Wierd hack to not lose quality when saving as .ico
                }
            }
        }

        protected override List<Result> QueryInternal(Query query)
        {
            if (query.RawQuery.EndsWith(" ") || query.RawQuery.Length <= 1) return new List<Result>();
            string myQuery = query.RawQuery.Trim();

            List<Result> results = new List<Result>();
            List<Result> filteredResults = new List<Result>();

            foreach (var item in controlPanelItems)
            {
                if (item.LocalizedString.IndexOf(myQuery, StringComparison.OrdinalIgnoreCase) >= 0 || item.InfoTip.IndexOf(myQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                {

                    if (item.LocalizedString.IndexOf(myQuery, StringComparison.OrdinalIgnoreCase) >= 0) //Items with Query in title have higher score.
                    {
                        results.Insert(0, new Result()
                        {
                            Title = item.LocalizedString,
                            SubTitle = item.InfoTip,
                            IcoPath = "Images\\ControlPanelIcons\\" + item.ApplicationName + ".ico",
                            Action = e =>
                            {
                                try
                                {
                                    Process.Start(item.ExecutablePath);
                                }
                                catch (Exception)
                                {
                                    //Silently Fail for now..
                                }
                                return true;
                            }
                        });
                    }
                    else
                    {
                        results.Add(new Result()
                        {
                            Title = item.LocalizedString,
                            SubTitle = item.InfoTip,
                            IcoPath = "Images\\ControlPanelIcons\\" + item.ApplicationName + ".ico",
                            Action = e =>
                            {
                                try
                                {
                                    Process.Start(item.ExecutablePath);
                                }
                                catch (Exception)
                                {
                                    //Silently Fail for now..
                                }
                                return true;
                            }
                        });
                    }
                }
            }
            for (int i = 0; i < 2 && i < results.Count; i++)
            {
                filteredResults.Add(results[i]);
            }

            return filteredResults;
        }
    }
}
