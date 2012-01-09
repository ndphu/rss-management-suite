using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreService
{
    public class TabDTO
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
        private int _ownerID;

        public int OwnerID
        {
            get { return _ownerID; }
            set { _ownerID = value; }
        }

        private string _ownerUsername;

        public string OwnerUsername
        {
            get { return _ownerUsername; }
            set { _ownerUsername = value; }
        }

        private int _childCount;

        public int ChildCount
        {
            get { return _childCount; }
            set { _childCount = value; }
        }
    }
}