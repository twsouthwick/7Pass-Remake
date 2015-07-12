using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using SevenPass.Models;
using SevenPass.Services.Cache;
using SevenPass.ViewModels;
using SevenPass.Entry.ViewModels;
using Xunit;
using System.Collections.Generic;

namespace SevenPass.Tests.ViewModels
{
    public class GroupViewModelTests
    {
        private readonly MockNavigationService _navigation;
        private readonly GroupViewModel _viewModel;

        public GroupViewModelTests()
        {
            _navigation = new MockNavigationService();
            _viewModel = new GroupViewModel(
                new MockCacheService(), _navigation)
            {
                Id = MockCacheService.GROUP_ID,
            };
        }

        [Fact]
        public void Should_open_child_group_on_select()
        {
            var group = new TestGroup(MockCacheService.CHILD_GROUP_ID, string.Empty);

            _viewModel.SelectedItem = new GroupItemViewModel(new GroupItemModel(group));

            Assert.Equal(typeof(GroupViewModel), _navigation.Target);
        }

        [Fact]
        public void Should_open_entry_on_select()
        {
            _viewModel.SelectedItem = new EntryItemViewModel(new EntryItemModel(new TestEntry { Id = MockCacheService.ENTRY_ID, }));

            Assert.Equal(typeof(EntryViewModel), _navigation.Target);
        }

        [Fact]
        public void Should_populate_items_on_initialize()
        {
            _viewModel.Initialize();

            var group = Assert.Single(_viewModel.Items
                .OfType<GroupItemViewModel>());
            Assert.Equal("Child Group", group.Name);

            var entry = Assert.Single(_viewModel.Items
                .OfType<EntryItemViewModel>());
            Assert.Equal("Demo Entry", entry.Title);
        }

        [Fact]
        public void Should_populate_names_on_initialize()
        {
            _viewModel.Initialize();
            Assert.Equal("Demo DB", _viewModel.DatabaseName);
            Assert.Equal("Root Group", _viewModel.DisplayName);
        }

        public class MockCacheService : ICacheService
        {
            public const string CHILD_GROUP_ID = "kaLNzo6afkKGD1dJiKTXFA==";
            public const string ENTRY_ID = "1gwdeQjEhUeTV4/Ihg4c3g==";
            public const string GROUP_ID = "SnMTc/hDbkKKeEIv3n1qwA==";

            public IKeePassDatabase Database
            {
                get
                {
                    return null;
                }
            }

            public XElement Root
            {
                get { throw new NotSupportedException(); }
            }

            public void Cache(IKeePassDatabase database)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public XElement GetEntry(string uuid)
            {
                throw new NotSupportedException();
            }

            public IKeePassGroup GetGroup(string uuid)
            {
                Assert.Equal(GROUP_ID, uuid);

                var root = new TestGroup(GROUP_ID, "Root Group");
                root.Groups.Add(new TestGroup(CHILD_GROUP_ID, "Child group"));
                root.Entries.Add(new TestEntry { Id = ENTRY_ID, Title = "Demo Entry" });

                return root;
            }
        }
        public class TestEntry : IKeePassEntry
        {
            public KeePassId Id { get; set; }

            public string UserName { get; set; }

            public string Password { get; set; }

            public string Title { get; set; }
        }

        public class TestGroup : IKeePassGroup
        {
            public TestGroup(KeePassId id, string name)
            {
                Id = id;
                Name = name;
                Entries = new List<IKeePassEntry>();
                Groups = new List<IKeePassGroup>();
            }

            public KeePassId Id { get; private set; }

            public string Name { get; private set; }

            public string Notes { get; private set; }

            public IList<IKeePassEntry> Entries { get; private set; }

            public IList<IKeePassGroup> Groups { get; private set; }
        }
    }
}