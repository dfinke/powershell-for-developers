using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Windows;

namespace BeaverMusic.UI
{
    public enum CreationStrategy
    {
        Activate,
        Resolve,
        Inject
    }

    [Export(typeof(IViewFactory))]
    internal class ViewFactory : IViewFactory
    {
        private readonly ExportProvider _exportProvider;

        [ImportingConstructor]
        public ViewFactory(CompositionContainer exportProvider)
        {
            _exportProvider = exportProvider;
        }

        public void Attach(dynamic view)
        {
            ViewElement.SetViewFactory(view, this);
        }

        public void CreateView<TView, TViewModel>(
            ref dynamic view,
            ref TViewModel viewModel,
            CreationStrategy viewCreationStrategy = CreationStrategy.Activate,
            CreationStrategy viewModelCreationStrategy = CreationStrategy.Resolve)
        {
            CreateComponent<TView>(ref view, viewCreationStrategy);
            CreateComponent<TViewModel>(ref viewModel, viewModelCreationStrategy);

            view.DataContext = viewModel;
            Attach(view);
        }

        public void CreateView(
            Type viewType,
            ref FrameworkElement view,
            Type viewModelType,
            ref object viewModel,
            CreationStrategy viewCreationStrategy = CreationStrategy.Activate,
            CreationStrategy viewModelCreationStrategy = CreationStrategy.Resolve)
        {
            object weakView = view;
            CreateComponent(viewType, ref weakView, viewCreationStrategy);
            view = (FrameworkElement)weakView;

            CreateComponent(viewModelType, ref viewModel, viewModelCreationStrategy);

            view.DataContext = viewModel;
            Attach(view);
        }

        private void CreateComponent<T>(ref dynamic component, CreationStrategy creationStrategy)
        {
            switch (creationStrategy)
            {
                case CreationStrategy.Activate: component = Activator.CreateInstance<T>(); break;
                case CreationStrategy.Resolve: component = _exportProvider.GetExportedValue<T>(); break;
                case CreationStrategy.Inject: if (component == null) throw new ArgumentNullException("component"); break;
                default: throw new NotSupportedException();
            }
        }

        private void CreateComponent<T>(ref T component, CreationStrategy creationStrategy)
        {
            switch (creationStrategy)
            {
                case CreationStrategy.Activate: component = Activator.CreateInstance<T>(); break;
                case CreationStrategy.Resolve: component = _exportProvider.GetExportedValue<T>(); break;
                case CreationStrategy.Inject: if (component == null) throw new ArgumentNullException("component"); break;
                default: throw new NotSupportedException();
            }
        }

        private void CreateComponent(Type componentType, ref object component, CreationStrategy creationStrategy)
        {
            switch (creationStrategy)
            {
                case CreationStrategy.Activate: component = Activator.CreateInstance(componentType); break;
                case CreationStrategy.Resolve: component = _exportProvider.GetExportedValue<object>(componentType.FullName); break;
                case CreationStrategy.Inject: if (component == null) throw new ArgumentNullException("component"); break;
                default: throw new NotSupportedException();
            }
        }
    }
}
