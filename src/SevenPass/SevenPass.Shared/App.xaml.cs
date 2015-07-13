using Autofac;
using Caliburn.Micro;
using SevenPass.Entry.ViewModels;
using SevenPass.Services.Cache;
using SevenPass.Services.Databases;
using SevenPass.Services.Picker;
using SevenPass.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace SevenPass
{
    public sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void BuildUp(object instance)
        {
            var handler = instance as IHandle;
            if (handler != null)
            {
                var events = Resolve<IEventAggregator>();
                events.Subscribe(instance);
            }

            base.BuildUp(instance);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            Resolve<IFilePickerService>().ContinueAsync(args);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.TileId.StartsWith("DB_"))
            {
                DisplayRootView<PasswordView>(args.Arguments);
                return;
            }

            if (Window.Current.Content == null)
            {
                DisplayRootView<MainView>();
            }
        }

        public override void RegisterServices(ContainerBuilder builder)
        {
            base.RegisterServices(builder);

            builder.Register(_ => SevenPass.Services.AutoMaps.Initialize())
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<CacheService>()
                .As<ICacheService>()
                .SingleInstance();
            builder.RegisterType<FilePickerService>()
                .As<IFilePickerService>()
                .SingleInstance();
            builder.RegisterType<RegisteredDbsService>()
                .As<IRegisteredDbsService>()
                .SingleInstance();

            //builder.RegisterType<ViewModels.MainViewModel>()
            //.AsSelf()
            //.InstancePerDependency();
            //builder.RegisterType<Views.MainView>()
            //.AsSelf()
            //.InstancePerDependency();

            builder.RegisterType<EntryDetailsViewModel>()
                .As<IEntrySubViewModel>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EntryNotesViewModel>()
                .As<IEntrySubViewModel>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EntryAttachmentsViewModel>()
                .As<IEntrySubViewModel>()
                .InstancePerLifetimeScope();
            builder.RegisterType<EntryFieldsViewModel>()
                .As<IEntrySubViewModel>()
                .InstancePerLifetimeScope();
        }
    }
}