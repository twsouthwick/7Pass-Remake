using Autofac;
using Caliburn.Micro;
using SevenPass.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace SevenPass
{
    public abstract class CaliburnAutofacApplication : CaliburnApplication
    {
        private IContainer _container;

        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            // Ensure base.RootFrame is available
            PrepareViewFirst();

            builder.RegisterType<GlobalMessagesService>()
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .OnActivated(e => e.Instance.Subscribe(e.Context.Resolve<GlobalMessagesService>()))
                .SingleInstance();
            builder.RegisterType<FrameAdapter>()
                .As<INavigationService>()
                .SingleInstance();
            builder.RegisterInstance(RootFrame)
                .As<Frame>();

            RegisterServices(builder);

            _container = builder.Build();

        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrEmpty(key))
            {
                if (_container.TryResolve(service, out instance))
                    return instance;
            }
            else
            {
                if (_container.TryResolveNamed(key, service, out instance))
                    return instance;
            }

            throw new Exception(string.Format("Could not locate any instances of service {0}.", service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            _container.InjectProperties(instance);
        }

        public virtual void RegisterServices(ContainerBuilder builder)
        {
            LogManager.GetLog = type => new DebugLogger(type);

            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                .AsSelf()
                .InstancePerDependency();
        }

        protected T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
        public class DebugLogger : ILog
        {
            private readonly Type _type;

            public DebugLogger(Type type)
            {
                _type = type;
            }

            public void Info(string format, params object[] args)
            {
                if (format.StartsWith("No bindable"))
                    return;
                if (format.StartsWith("Action Convention Not Applied"))
                    return;
                Debug.WriteLine("INFO: " + format, args);
            }

            public void Warn(string format, params object[] args)
            {
                Debug.WriteLine("WARN: " + format, args);
            }

            public void Error(Exception exception)
            {
                Debug.WriteLine("ERROR: {0}\n{1}", _type.Name, exception);
            }
        }
    }
}

