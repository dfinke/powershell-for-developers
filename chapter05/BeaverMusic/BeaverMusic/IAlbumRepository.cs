using System.Collections.ObjectModel;

namespace BeaverMusic
{
    public interface IAlbumRepository
    {
        ReadOnlyObservableCollection<Album> GetAlbums();

        Album SaveAlbum(Album album);

        void RemoveAlbum(int id);

        void Clear();
    }
}