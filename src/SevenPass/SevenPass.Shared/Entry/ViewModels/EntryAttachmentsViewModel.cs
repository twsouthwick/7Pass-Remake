using System;
using System.Linq;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Caliburn.Micro;
using SevenPass.Services.Picker;
using SevenPass.Models;

namespace SevenPass.Entry.ViewModels
{
    public sealed class EntryAttachmentsViewModel : EntrySubViewModelBase
    {
        private readonly BindableCollection<EntryAttachmentViewModel> _items;
        private readonly IFilePickerService _picker;
        private Visibility _listVisibility;
        private DataTransferManager _transferManager;

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        public IObservableCollection<EntryAttachmentViewModel> Items
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
                NotifyOfPropertyChange(() => NoAttachmentVisibility);
            }
        }

        /// <summary>
        /// Gets the visibility of empty data prompt.
        /// </summary>
        public Visibility NoAttachmentVisibility
        {
            get
            {
                return ListVisibility == Visibility.Visible
                    ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public EntryAttachmentsViewModel(IFilePickerService picker)
        {
            if (picker == null)
                throw new ArgumentNullException("picker");

            _picker = picker;
            DisplayName = "Attachments";
            _items = new BindableCollection<EntryAttachmentViewModel>();
        }

        protected override void OnActivate()
        {
            _transferManager = DataTransferManager.GetForCurrentView();
            _transferManager.DataRequested += OnDataRequested;
        }

        protected override void OnDeactivate(bool close)
        {
            _transferManager.DataRequested -= OnDataRequested;
        }

        protected override void Populate(IKeePassEntry element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            Items.AddRange(element.Attachment.Select(o => new EntryAttachmentViewModel(o, _picker)));

            ListVisibility = Items.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var sharing = _items.FirstOrDefault(x => x.IsSharing);
            if (sharing == null)
                return;

            sharing.IsSharing = false;
            var data = args.Request.Data;
            var defer = args.Request.GetDeferral();

            var file = await sharing.SaveToFile();
            data.Properties.Title = file.Name;
            data.SetStorageItems(new[] { file }, true);
            defer.Complete();
        }
    }
}