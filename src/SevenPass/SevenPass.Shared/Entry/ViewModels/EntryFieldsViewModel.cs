using System;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml;
using Caliburn.Micro;
using SevenPass.Messages;
using SevenPass.Models;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryFieldsViewModel : EntrySubViewModelBase,
        IHandle<EntryFieldExpandedMessage>
    {
        private readonly IEventAggregator _events;
        private readonly BindableCollection<EntryFieldViewModel> _items;
        private Visibility _listVisibility;

        /// <summary>
        /// Gets the field items.
        /// </summary>
        public IObservableCollection<EntryFieldViewModel> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets the visibility of the list.
        /// </summary>
        public Visibility ListVisibility
        {
            get { return _listVisibility; }
            private set
            {
                _listVisibility = value;
                NotifyOfPropertyChange(() => ListVisibility);
                NotifyOfPropertyChange(() => NoFieldVisibility);
            }
        }

        /// <summary>
        /// Gets the visibility of empty data prompt.
        /// </summary>
        public Visibility NoFieldVisibility
        {
            get
            {
                return ListVisibility == Visibility.Visible
                    ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public EntryFieldsViewModel(IEventAggregator events)
        {
            if (events == null)
                throw new ArgumentNullException("events");

            _events = events;
            _items = new BindableCollection<EntryFieldViewModel>();
            DisplayName = "Fields";
        }

        public void Handle(EntryFieldExpandedMessage message)
        {
            var expanded = message.Item;

            Items
                .Where(x => !ReferenceEquals(x, expanded))
                .Apply(x => x.IsExpanded = false);
        }

        protected override void Populate(IKeePassEntry element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            var fields = element.Fields.Select(x => new EntryFieldViewModel(this, _events)
            {
                Key = x.Name,
                Value = x.Value,
                IsProtected = x.IsProtected
            });


            ListVisibility = Items.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}