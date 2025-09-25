using System.Collections.Generic;
using IntoTheUnknownTest.Pooling;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public enum PoolObjectType
    {
        MapTiles,
    }
    
    public class PoolingManager : Singleton<PoolingManager>
    {
        [SerializeField] private MapTile _mapTilePrefab;
        
        private readonly Dictionary<PoolObjectType, object> _poolsDictionary = new();

        public void PrepareMapTiles(Transform parent, int poolSize)
        {
            CreatePool(PoolObjectType.MapTiles, _mapTilePrefab, poolSize, parent.transform);
        }
        
        public void CleanupAllPoolObjects()
        {
            _poolsDictionary.Clear();
        }
        
        public void CreatePool<T>(PoolObjectType key, T prefab, int initialSize, Transform parent = null) where T : Component
        {
            if (_poolsDictionary.ContainsKey(key))
            {
                return;
            }

            var pool = new ObjectPool<T>(prefab, initialSize, parent);
            _poolsDictionary[key] = pool;
        }

        public T Get<T>(PoolObjectType key) where T : Component
        {
            return _poolsDictionary.TryGetValue(key, out var pool) ? ((ObjectPool<T>)pool).Get() : null;
        }

        public void ReturnToPool<T>(PoolObjectType key, T obj) where T : Component
        {
            if (_poolsDictionary.TryGetValue(key, out var pool))
            {
                ((ObjectPool<T>)pool).ReturnToPool(obj);
            }
        }
    }
}
