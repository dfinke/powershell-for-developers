using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace BeaverMusic.UI.Shell
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class AlbumEditDialogViewModel
    {
        private readonly IAlbumRepository _albumRepository;

        public event EventHandler Completed = delegate { };

        [ImportingConstructor]
        public AlbumEditDialogViewModel(IAlbumRepository albumRepository, AlbumEditViewModel albumEditViewModel)
        {
            _albumRepository = albumRepository;
            AlbumEditViewModel = albumEditViewModel;

            InitializeCommands();
        }

        #region Commands

        public ICommand CommitCommand { get; private set; }

        public ICommand CancelCommand { get; private set; }

        private void InitializeCommands()
        {
            CommitCommand = new DelegateCommand(Commit);
            CancelCommand = new DelegateCommand(Cancel);
        }

        #endregion

        public AlbumEditViewModel AlbumEditViewModel { get; private set; }

        public void Initialize(Album album)
        {
            AlbumEditViewModel.AlbumId = album.Id;
            AlbumEditViewModel.AlbumName = album.Name;
            AlbumEditViewModel.Artist = album.Artist;

            if (album.Songs != null)
                foreach (var song in album.Songs)
                    AlbumEditViewModel.Songs.Add(new AlbumEditViewModel.SongViewModel() { SongName = song.Name, Length = song.Length });
        }

        public void Commit()
        {
            var album = AlbumEditViewModel.AlbumId == null ? new Album() : new Album(AlbumEditViewModel.AlbumId.Value);

            album.Name = AlbumEditViewModel.AlbumName;
            album.Artist = AlbumEditViewModel.Artist;
            album.Songs = AlbumEditViewModel.Songs.Select(s => new Song { Name = s.SongName, Length = s.Length }).ToArray();

            _albumRepository.SaveAlbum(album);

            Completed(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            Completed(this, EventArgs.Empty);
        }
    }
}
