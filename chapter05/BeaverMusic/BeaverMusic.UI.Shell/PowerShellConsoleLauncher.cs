using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using EmbeddedPSConsole;
using System.Windows;

namespace BeaverMusic.UI.Shell
{
    public interface IPowerShellConsoleLauncher
    {
        void Launch();
    }

    [Export(typeof(IPowerShellConsoleLauncher))]
    internal class PowerShellConsoleLauncher : IPowerShellConsoleLauncher
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly MefHelper _mefHelper;

        private PSConsole _console;

        [ImportingConstructor]
        public PowerShellConsoleLauncher(IAlbumRepository albumRepository, MefHelper mefHelper)
        {
            _albumRepository = albumRepository;
            _mefHelper = mefHelper;
        }

        public void Launch()
        {
            if (_console != null)
            {
                _console.Activate();
                return;
            }

            PSConfig.AddVariable("MEFHelper", _mefHelper);
            PSConfig.AddVariable("AlbumRepository", _albumRepository);
            PSConfig.AddVariable("MainWindow", Application.Current.MainWindow);

            PSConfig.Profile = "BeaverMusicProfile.ps1";
            _console = new PSConsole();
            _console.Closing += (s, e) => _console = null;

            _console.Show();
        }

        [Export]
        public class MefHelper
        {
            [Import]
            public ExportProvider ExportProvider { get; set; }

            public object GetExport(string contractName)
            {
                return ExportProvider.GetExport<object>(contractName).Value;
            }

            public ComposablePartCatalog GetMEFCatalog
            {
                get { return ((CompositionContainer)ExportProvider).Catalog; }
            }

            public IEnumerable<ComposablePartDefinition> GetMEFCatalogParts
            {
                get { return GetMEFCatalog.Parts; }
            }
        }
    }
}
