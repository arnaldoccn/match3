using UnityEngine;
namespace MatchThree
{
    /// <summary>
    /// Creates the singleton pattern
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _shuttingDown = false;
        private static object _lock = new object();
        private static T _instance;

        /// <summary>
        /// Access the instance and guarantees that this is thread safely
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_shuttingDown)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (_instance == null)
                        {
                            var singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return _instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _shuttingDown = true;
        }


        private void OnDestroy()
        {
            _shuttingDown = true;
        }
    }
}