using System.Collections.Generic;
using UnityEngine;

namespace IntoTheUnknownTest.Pooling
{
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> _objects = new Queue<T>();
        private readonly T _prefab;
        private readonly Transform _parent;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                var obj = Object.Instantiate(_prefab, _parent);
                obj.gameObject.SetActive(false);
                _objects.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj = _objects.Count > 0 ? _objects.Dequeue() : Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(true);
            
            return obj;
        }

        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            _objects.Enqueue(obj);
        }
    }
}
