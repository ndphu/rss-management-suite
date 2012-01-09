using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSSMSPluginInterface
{
    public interface IRSSPlugin
    {
        string GetRSSResult();

        string GetPluginName();

        string GetRSSName();

        string GetRSSDescription();

        string GetRSSWebsiteLink();
    }
}
