using System;
using System.Linq;
using Caliburn.Micro;
using SevenPass.Services.Cache;

namespace SevenPass.Entry.ViewModels
{
    public sealed class BrowserViewModel : Screen
    {
        private readonly ICacheService _cache;

        private readonly BindableCollection<FieldViewModel> _strings;
        private string _url;

        /// <summary>
        /// Gets or sets the entry UUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets the strings.
        /// </summary>
        public IObservableCollection<FieldViewModel> Strings
        {
            get { return _strings; }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }

        public BrowserViewModel(ICacheService cache)
        {
            if (cache == null)
                throw new ArgumentNullException("cache");

            _cache = cache;
            _strings = new BindableCollection<FieldViewModel>();
        }

        protected override void OnInitialize()
        {
            var entry = _cache.GetEntry(Id);

            _strings.AddRange(entry.Fields.Select(x => new FieldViewModel { Key = x.Name, Value = x.Value }));

            UserName = entry.UserName;
            Password = entry.Password;
        }

        public class FieldViewModel
        {
            /// <summary>
            /// Gets or sets the key.
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { get; set; }
        }
    }
}