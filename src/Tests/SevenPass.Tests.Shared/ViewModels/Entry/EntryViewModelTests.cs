using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Caliburn.Micro;
using SevenPass.Entry.ViewModels;
using SevenPass.Services.Cache;
using SevenPass.ViewModels;
using Xunit;
using SevenPass.Models;

namespace SevenPass.Tests.ViewModels.Entry
{
    public class EntryViewModelTests
    {
        private readonly XElement _entry;
        private readonly MockEntrySubViewModel _subModel;
        private readonly EntryViewModel _viewModel;

        public EntryViewModelTests()
        {
            _entry = new XElement("Entry");
            _subModel = new MockEntrySubViewModel();

            _viewModel = new EntryViewModel(
                new MockCacheService(_entry),
                new IEntrySubViewModel[] { _subModel })
            {
                Id = MockCacheService.ID,
            };
        }

        [Fact]
        public void Initialize_should_populate_sub_view_models()
        {
            ScreenExtensions.TryActivate(_viewModel);
            Assert.Same(_entry, _subModel.Element);
        }

        public class MockCacheService : ICacheService
        {
            public const string ID = "NK4XTExcnk+wrek5ojwJfQ==";

            public IKeePassDatabase Database
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            public XElement Root
            {
                get { throw new NotSupportedException(); }
            }

            public MockCacheService()
            {
            }

            public void Cache(IKeePassDatabase database)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public IKeePassEntry GetEntry(KeePassId uuid)
            {
                Assert.Equal(ID, uuid);
                return null;
            }

            public IKeePassGroup GetGroup(KeePassId uuid)
            {
                throw new NotSupportedException();
            }
        }

        public class MockEntrySubViewModel : IEntrySubViewModel
        {
            public string DisplayName { get; set; }

            public IKeePassEntry Element { get; set; }

            public string Id { get; set; }

            public IEnumerable<AppBarCommandViewModel> GetCommands()
            {
                yield break;
            }

            public void Loads(IKeePassEntry element)
            {
                Element = element;
            }
        }
    }
}