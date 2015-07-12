using System;
using System.Linq;
using System.Xml.Linq;

namespace SevenPass.Models
{
    public sealed class EntryItemModel
    {
        private readonly IKeePassEntry _entry;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get { return _entry.Password; } }

        /// <summary>
        /// Gets or sets the entry title.
        /// </summary>
        public string Title { get { return _entry.Title; } }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get { return _entry.UserName; } }

        public KeePassId Id { get { return _entry.Id; } }

        public EntryItemModel() { }

        public EntryItemModel(IKeePassEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            _entry = entry;
        }
    }
}