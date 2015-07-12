using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Models
{
    public sealed class GroupItemModel
    {
        private IKeePassGroup _group;
        /// <summary>
        /// Gets or sets the group name.
        /// </summary>
        public string Name { get; set; }

        public KeePassId Id { get; set; }

        /// <summary>
        /// Gets or sets the group notes.
        /// </summary>
        public string Notes { get; set; }

        public GroupItemModel(IKeePassGroup group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            Name = group.Name;
            Id = group.Id;
            _group = group;
        }

        //public GroupItemModel() { }

        /// <summary>
        /// Lists the entries of this group.
        /// </summary>
        /// <returns>The entries.</returns>
        public List<EntryItemModel> ListEntries()
        {
            return _group.Entries
                .Select(x => new EntryItemModel(x))
                .ToList();
        }

        /// <summary>
        /// Lists the child groups of this group.
        /// </summary>
        /// <returns>The child groups.</returns>
        public List<GroupItemModel> ListGroups()
        {
            return _group.Groups
                .Select(x => new GroupItemModel(x))
                .ToList();
        }
    }
}