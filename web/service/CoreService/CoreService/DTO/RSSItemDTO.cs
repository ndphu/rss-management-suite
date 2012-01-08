using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreService
{
    public class RSSItemDTO
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _rSSLink;

        public string RSSLink
        {
            get { return _rSSLink; }
            set { _rSSLink = value; }
        }
        private int _tabID;

        public int TabID
        {
            get { return _tabID; }
            set { _tabID = value; }
        }
    }
}