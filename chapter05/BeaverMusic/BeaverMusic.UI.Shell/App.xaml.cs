using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;
using BeaverMusic.Composition;

namespace BeaverMusic.UI.Shell
{
    public partial class App : Application
    {
        private readonly CompositionContainer _container;

        public App()
        {
            var catalog = new AggregateCatalog(new DirectoryCatalog(Path.GetDirectoryName(typeof(App).Assembly.Location)), new AssemblyCatalog(typeof(App).Assembly));

            _container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddPart(ExistingComposablePart.Create<ExportProvider>(_container));
            batch.AddPart(ExistingComposablePart.Create(_container));

            _container.Compose(batch);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewFactory = _container.GetExportedValue<IViewFactory>();

            dynamic mainWindow = null;
            MainViewModel mainViewModel = null;

            viewFactory.CreateView<MainWindow, MainViewModel>(ref mainWindow, ref mainViewModel);

            mainWindow.Show();
        }
    }
}
