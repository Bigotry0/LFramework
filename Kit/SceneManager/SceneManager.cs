using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LFramework.Kit.SceneManager
{
    public class SceneManager : MonoBehaviour
    {
        public UnityEvent OnLoadScene = new UnityEvent();
        public UnityEvent OnLoadFinish = new UnityEvent();
        private AsyncOperation asyncLoad;
        
        public bool AutoActiveScene = false;
        
        private float Progress = 0;

        public void LoadSceneByAsync(string sceneName)
        {
            StartCoroutine(LoadAsyncScene(sceneName));
            OnLoadScene?.Invoke();
        }

        public float GetProgress()
        {
            return Progress;
        }

        public void ActiveScene()
        {
            if (!AutoActiveScene)
            {
                asyncLoad.allowSceneActivation = true;
            }
        }

        IEnumerator LoadAsyncScene(string sceneName)
        { 
            asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (AutoActiveScene)
            {
                asyncLoad.allowSceneActivation = true;
            }
            else
            {
                asyncLoad.allowSceneActivation = false;
            }
            while (!asyncLoad.isDone)
            {
                Progress = asyncLoad.progress;
                Debug.Log(Progress);
                if (Progress >= 0.9f)
                {
                    OnLoadFinish?.Invoke();
                }
                yield return null;
            }
        }
    }
}