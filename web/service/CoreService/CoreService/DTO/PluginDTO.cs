using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreService
{
    public class PluginDTO
    {
        int _pluginID;

        public int PluginID
        {
            get { return _pluginID; }
            set { _pluginID = value; }
        }

        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        string _websiteLink;

        public string WebsiteLink
        {
            get { return _websiteLink; }
            set { _websiteLink = value; }
        }
        
    }
}