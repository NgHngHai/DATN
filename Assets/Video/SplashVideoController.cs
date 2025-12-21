using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashVideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    AsyncOperation loadSceneOperation;

    void Start()
    {
        loadSceneOperation = SceneManager.LoadSceneAsync(1);
        loadSceneOperation.allowSceneActivation = false;
        _videoPlayer.loopPointReached += OnVideoFinished;
    }

    
    void OnVideoFinished(VideoPlayer source)
    {
        source.Stop();
        _videoPlayer.loopPointReached -= OnVideoFinished;
        loadSceneOperation.allowSceneActivation = true;
    }

    void OnDestroy()
    {
        _videoPlayer.loopPointReached -= OnVideoFinished;
    }
}