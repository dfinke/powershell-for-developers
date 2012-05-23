using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BeaverMusic.UI
{
    public class ViewElement : Control
    {
        static ViewElement()
        {
            var self = typeof(ViewElement);

            var template = new ControlTemplate(self);

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.ContentSourceProperty, "View");

            template.VisualTree = contentPresenter;

            template.Seal();

            TemplateProperty.OverrideMetadata(self, new FrameworkPropertyMetadata(template));
        }

        public ViewElement()
        {
            Loaded += OnLoaded;
        }

        #region internal IViewFactory ViewFactory { get; set; }

        internal ViewFactory ViewFactory
        {
            get { return (ViewFactory)GetValue(ViewFactoryProperty); }
            set { SetValue(ViewFactoryProperty, value); }
        }

        internal static ViewFactory GetViewFactory(DependencyObject obj)
        {
            return (ViewFactory)obj.GetValue(ViewFactoryProperty);
        }

        internal static void SetViewFactory(DependencyObject obj, ViewFactory value)
        {
            obj.SetValue(ViewFactoryProperty, value);
        }

        internal static readonly DependencyProperty ViewFactoryProperty =
            DependencyProperty.RegisterAttached("ViewFactory", typeof(ViewFactory), typeof(ViewElement), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        #endregion

        #region public Type ViewType { get; set; }

        public Type ViewType
        {
            get { return (Type)GetValue(ViewTypeProperty); }
            set { SetValue(ViewTypeProperty, value); }
        }

        public static readonly DependencyProperty ViewTypeProperty =
            DependencyProperty.Register("ViewType", typeof(Type), typeof(ViewElement), new UIPropertyMetadata(null));

        #endregion

        #region public object View { get; set; }

        public FrameworkElement View
        {
            get { return (FrameworkElement)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(FrameworkElement), typeof(ViewElement), new UIPropertyMetadata(null));

        #endregion

        #region public CreationStrategy ViewCreationStrategy { get; set; }

        public CreationStrategy ViewCreationStrategy
        {
            get { return (CreationStrategy)GetValue(ViewCreationStrategyProperty); }
            set { SetValue(ViewCreationStrategyProperty, value); }
        }

        public static readonly DependencyProperty ViewCreationStrategyProperty =
            DependencyProperty.Register("ViewCreationStrategy", typeof(CreationStrategy), typeof(ViewElement), new UIPropertyMetadata(CreationStrategy.Activate));

        #endregion

        #region public Type ViewModelType { get; set; }

        public Type ViewModelType
        {
            get { return (Type)GetValue(ViewModelTypeProperty); }
            set { SetValue(ViewModelTypeProperty, value); }
        }

        public static readonly DependencyProperty ViewModelTypeProperty =
            DependencyProperty.Register("ViewModelType", typeof(Type), typeof(ViewElement), new UIPropertyMetadata(null));

        #endregion

        #region public object ViewModel { get; set; }

        public object ViewModel
        {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(object), typeof(ViewElement), new UIPropertyMetadata(null));

        #endregion

        #region public CreationStrategy ViewModelCreationStrategy { get; set; }

        public CreationStrategy ViewModelCreationStrategy
        {
            get { return (CreationStrategy)GetValue(ViewModelCreationStrategyProperty); }
            set { SetValue(ViewModelCreationStrategyProperty, value); }
        }

        public static readonly DependencyProperty ViewModelCreationStrategyProperty =
            DependencyProperty.Register("ViewModelCreationStrategy", typeof(CreationStrategy), typeof(ViewElement), new UIPropertyMetadata(CreationStrategy.Resolve));

        #endregion

        private bool _isLoaded;

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((ViewElement)sender).OnLoaded(e);
        }

        private void OnLoaded(RoutedEventArgs e)
        {
            if (_isLoaded) return;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                if (ViewType != null)
                    View = Activator.CreateInstance(ViewType) as FrameworkElement;

                return;
            }

            var view = View;
            var viewModel = ViewModel;

            var viewType = ViewType != null || view == null ? ViewType : view.GetType();
            var viewModelType = ViewModelType != null || viewModel == null ? ViewModelType : viewModel.GetType();

            ViewFactory.CreateView(viewType, ref view, viewModelType, ref viewModel, ViewCreationStrategy, ViewModelCreationStrategy);

            View = view;
            ViewModel = viewModel;

            _isLoaded = true;
        }
    }
}
