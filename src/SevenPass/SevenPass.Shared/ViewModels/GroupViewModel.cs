using Caliburn.Micro;
using SevenPass.Entry.ViewModels;
using SevenPass.Models;
using SevenPass.Services.Cache;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenPass.ViewModels
{
    /// <summary>
    /// ViewModel to display database data.
    /// </summary>
    public sealed class GroupViewModel : Screen
    {
        private readonly ICacheService _cache;
        private readonly INavigationService _navigation;
        private readonly BindableCollection<IItemViewModel> _items = new BindableCollection<IItemViewModel>();

        private object _selectedItem;
        private string _searchText;

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets or sets the UUID of the group to be displayed.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Search text for the view
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                FilterItems(_searchText);
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        private void FilterItems(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                Initialize();
            }
            else
            {

                var root = GetGroup();
                var result = root.ExpandEntries()
                    .Where(e => e.Title.ContainsIgnoreCase(searchText))
                    .Select(e => new EntryItemViewModel(e))
                    .OrderBy(e => e.Title);

                _items.Clear();
                _items.AddRange(result);
            }
        }

        /// <summary>
        /// Gets or sets the group items.
        /// </summary>
        public BindableCollection<IItemViewModel> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);

                if (value == null)
                    return;

                Open(value);
                SelectedItem = null;
            }
        }

        public GroupViewModel(ICacheService cache,
            INavigationService navigation)
        {
            if (cache == null) throw new ArgumentNullException("cache");
            if (navigation == null) throw new ArgumentNullException("navigation");

            _cache = cache;
            _navigation = navigation;
            DatabaseName = _cache.Database.Name;
        }

        private GroupItemModel GetGroup()
        {
            var element = !string.IsNullOrEmpty(Id)
                ? _cache.GetGroup(Id)
                : _cache.Root;

            // TODO: handle group not found
            return new GroupItemModel(element);
        }

        /// <summary>
        /// Initializes the page.
        /// </summary>
        /// <returns></returns>
        public void Initialize()
        {
            var group = GetGroup();
            DisplayName = group.Name;

            var groups = group
                .ListGroups()
                .Select(x => new GroupItemViewModel(x))
                .Cast<IItemViewModel>();

            var entries = group
                .ListEntries()
                .Select(x => new EntryItemViewModel(x))
                .Cast<IItemViewModel>();

            Items.Clear();
            Items.AddRange(groups.Concat(entries));
        }

        protected override void OnInitialize()
        {
            Initialize();
        }

        /// <summary>
        /// Opens the specified item.
        /// </summary>
        /// <param name="item">The item to open.</param>
        private void Open(object item)
        {
            if (!OpenGroup(item))
                OpenEntry(item);
        }

        private void OpenEntry(object item)
        {
            var entry = item as EntryItemViewModel;
            if (entry == null)
                return;

            _navigation
                .UriFor<EntryViewModel>()
                .WithParam(x => x.Id, entry.Id)
                .Navigate();
        }

        private bool OpenGroup(object item)
        {
            var group = item as GroupItemViewModel;
            if (group == null)
                return false;

            _navigation
                .UriFor<GroupViewModel>()
                .WithParam(x => x.Id, group.Id)
                .Navigate();

            return true;
        }
    }
}