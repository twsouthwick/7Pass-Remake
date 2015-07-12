using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenPass.Models
{
    [DebuggerDisplay("KeePass ID: {Id}")]
    public class KeePassId
    {
        private readonly string _id;

        public KeePassId(string id)
        {
            _id = id;
        }

        public string Id { get { return _id; } }

        public static implicit operator KeePassId(string id)
        {
            return new KeePassId(id);
        }

        public static explicit operator string(KeePassId id)
        {
            return id.Id;
        }
    }

    public interface IKeePassDatabase
    {
        KeePassId Id { get; }

        string Name { get; }

        IList<IKeePassIcon> Icons { get; }

        IList<IKeePassGroup> Groups { get; }
    }

    public interface IKeePassIcon
    {

    }

    public interface IKeePassGroup
    {
        KeePassId Id { get; }
        string Name { get; }
        string Notes { get; }
        IList<IKeePassEntry> Entries { get; }
        IList<IKeePassGroup> Groups { get; }
    }

    public interface IKeePassEntry
    {
        KeePassId Id { get; }
        string UserName { get; }
        string Password { get; }
        string Title { get; }
    }
}
