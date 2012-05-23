using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace BeaverMusic
{
    [Export(typeof(IAlbumRepository))]
    internal class AlbumRepository : IAlbumRepository
    {
        private int _idSeed;
        private readonly Dictionary<int, int> _indexMap = new Dictionary<int, int>();
        private readonly ObservableCollection<Album> _albums = new ObservableCollection<Album>();

        public ReadOnlyObservableCollection<Album> GetAlbums()
        {
            return new ReadOnlyObservableCollection<Album>(_albums);
        }

        public Album SaveAlbum(Album album)
        {
            if (album.Id == null)
            {
                album.Id = _idSeed++;
                _indexMap[album.Id.Value] = _albums.Count;
                _albums.Add(album);
            }
            else
            {
                _albums[_indexMap[album.Id.Value]] = album;
            }

            return album;
        }

        public void RemoveAlbum(int[] ids)
        {
            foreach (var index in ids)
                RemoveAlbum(index);
        }

        public void RemoveAlbum(int id)
        {
            int index;
            if (_indexMap.TryGetValue(id, out index))
            {
                _indexMap.Remove(id);
                _albums.RemoveAt(index);

                for (int i = index; i < _albums.Count; i++)
                {
                    var album = _albums[i];
                    _indexMap[album.Id.Value] = i;
                }
            }
        }

        public void Clear()
        {
            _albums.Clear();
            _indexMap.Clear();
        }
    }
}