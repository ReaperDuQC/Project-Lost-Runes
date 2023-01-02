using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LostRunes
{
    public class SceneLoaderManager : MonoBehaviour
    {
        [SerializeField] string _sceneToLoad;

        bool _readyToLoad = false;

        public void LoadNextScene()
        {
           StartCoroutine(LoadNextSceneCoroutine());
        }
        IEnumerator LoadNextSceneCoroutine()
        {
            yield return new WaitUntil(() => _readyToLoad == true);

            SceneManager.LoadScene(_sceneToLoad);
        }
        public void ReadyToLoad()
        {
            _readyToLoad = true;
        }
        public void SetSceneToLoad(string sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
        }
    }
}