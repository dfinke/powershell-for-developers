namespace BeaverMusic.UI
{
    public interface IViewFactory
    {
        void Attach(dynamic view);

        void CreateView<TView, TViewModel>(
            ref dynamic view,
            ref TViewModel viewModel,
            CreationStrategy viewCreationStrategy = CreationStrategy.Activate,
            CreationStrategy viewModelCreationStrategy = CreationStrategy.Resolve);
    }
}
