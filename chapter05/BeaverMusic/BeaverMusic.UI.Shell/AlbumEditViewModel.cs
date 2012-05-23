using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace BeaverMusic.UI.Shell
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class AlbumEditViewModel : BindableBase
    {
        [ImportingConstructor]
        public AlbumEditViewModel()
        {
            Songs = new ObservableCollection<SongViewModel>();

            InitializeCommands();
        }

        #region Commands

        public ICommand AddSongCommand { get; private set; }
        public ICommand RemoveSongCommand { get; private set; }

        private void InitializeCommands()
        {
            AddSongCommand = new DelegateCommand(AddSong);
            RemoveSongCommand = new DelegateCommand<SongViewModel>(RemoveSong);
        }

        #endregion

        private int? _albumId;
        public int? AlbumId
        {
            get { return _albumId; }
            set { SetProperty(ref _albumId, value, "AlbumId"); }
        }

        private string _albumName;
        public string AlbumName
        {
            get { return _albumName; }
            set { SetProperty(ref _albumName, value, "AlbumName"); }
        }

        private string _artist;
        public string Artist
        {
            get { return _artist; }
            set { SetProperty(ref _artist, value, "Artist"); }
        }

        public ObservableCollection<SongViewModel> Songs { get; private set; }

        private SongViewModel _songToAdd = new SongViewModel();
        public SongViewModel SongToAdd
        {
            get { return _songToAdd; }
            set { SetProperty(ref _songToAdd, value, "SongToAdd"); }
        }

        public void AddSong()
        {
            Songs.Add(_songToAdd);
            SongToAdd = new SongViewModel();
        }

        public void RemoveSong(SongViewModel song)
        {
            Songs.Remove(song);
        }

        public class SongViewModel : BindableBase
        {
            private string _songName;
            public string SongName
            {
                get { return _songName; }
                set { SetProperty(ref _songName, value, "SongName"); }
            }

            private TimeSpan _length;
            public TimeSpan Length
            {
                get { return _length; }
                set { SetProperty(ref _length, value, "Length"); }
            }
        }
    }
}
