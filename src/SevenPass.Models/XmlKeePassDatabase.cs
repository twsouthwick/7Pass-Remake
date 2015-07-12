using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Models
{
    public class XmlKeePassDatabase : IKeePassDatabase
    {
        private readonly IList<IKeePassGroup> _groups;
        private readonly KeePassId _id;
        private readonly string _name;

        public XmlKeePassDatabase(XDocument doc, KeePassId id, string name)
        {
            var groups = doc.Descendants("Group");

            _id = id;
            _name = name;
            _groups = doc.Descendants("Group")
                .Select(x => new XmlKeePassGroup(x))
                .Cast<IKeePassGroup>()
                .ToList();
        }

        public KeePassId Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IList<IKeePassIcon> Icons
        {
            get { return new List<IKeePassIcon>(); }
        }

        public IList<IKeePassGroup> Groups
        {
            get { return _groups; }
        }

        [DebuggerDisplay("Entry '{Title}'")]
        private class XmlKeePassEntry : IKeePassEntry
        {
            private readonly KeePassId _id;
            private readonly string _title;
            private readonly string _username;
            private readonly string _password;

            public XmlKeePassEntry(XElement entry)
            {
                var strings = entry.Elements("String")
                    .ToLookup(x => (string)x.Element("Key"), x => (string)x.Element("Value"));

                _id = entry.Element("UUID").Value;
                _title = strings["Title"].FirstOrDefault();
                _username = strings["UserName"].FirstOrDefault();
                _password = strings["Password"].FirstOrDefault();
            }

            public KeePassId Id
            {
                get { return _id; }
            }

            public string UserName
            {
                get { return _username; }
            }

            public string Password
            {
                get { return _password; }
            }

            public string Title
            {
                get { return _title; }
            }
        }

        [DebuggerDisplay("Group '{Name}'")]
        private class XmlKeePassGroup : IKeePassGroup
        {
            private readonly KeePassId _id;
            private readonly List<IKeePassEntry> _entries;
            private readonly List<IKeePassGroup> _groups;
            private readonly string _name;
            private readonly string _notes;

            public XmlKeePassGroup(XElement group)
            {
                _id = group.Element("UUID").Value;

                _entries = group.Elements("Entry")
                    .Select(x => new XmlKeePassEntry(x))
                    .Cast<IKeePassEntry>()
                    .ToList();

                _groups = group.Descendants("Group")
                    .Select(x => new XmlKeePassGroup(x))
                    .Cast<IKeePassGroup>()
                    .ToList();

                _name = (string)group.Element("Name");
                _notes = (string)group.Element("Notes");
            }

            public KeePassId Id
            {
                get { return _id; }
            }

            public IList<IKeePassEntry> Entries
            {
                get { return _entries; }
            }

            public IList<IKeePassGroup> Groups
            {
                get { return _groups; }
            }


            public string Name
            {
                get { return _name; }
            }

            public string Notes
            {
                get { return _notes; }
            }
        }
    }
}
