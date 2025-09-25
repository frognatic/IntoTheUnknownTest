using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            Instance = InstanceCheck(Instance) as T;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private Singleton<T> InstanceCheck(Singleton<T> instanceCheck)
        {
            if (instanceCheck != null && instanceCheck != this)
            {
                DestroyImmediate(gameObject);
                return instanceCheck;
            }

            return this;
        }
    }
}
