using SevenPass.Models;
using System.Collections.Generic;
using System.Globalization;

namespace SevenPass
{
    internal static class SevenPassExtensions
    {
        internal static IEnumerable<EntryItemModel> ExpandEntries(this GroupItemModel model)
        {
            foreach (var group in model.ListGroups())
            {
                foreach (var entry in group.ExpandEntries())
                {
                    yield return entry;
                }
            }

            foreach (var entry in model.ListEntries())
            {
                yield return entry;
            }
        }

        internal static bool ContainsIgnoreCase(this string text, string searchTerm)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(text, searchTerm, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
