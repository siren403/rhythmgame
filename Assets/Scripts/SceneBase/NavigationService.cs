using UnityEngine;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine.SceneManagement;

namespace ScenePresenter
{
    public static class NavigationService
    {
        /// <summary>
        ///                                              -> PrepareAsync ->   Subscribe : 스트림 처리 시간에 따라 Start보다 뒤에 호출 될 수 있음 ->
        /// NavigateAsync -> SceneLoad -> behavior.Awake ->                 behavior.Start -> BeforeInit -> Init
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="argument"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IObservable<Unit> NavigateAsync(string sceneName, object argument = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            return Observable.FromCoroutine<Unit>(observer => HyperOptimizedFastAsyncOperationLoad(SceneManager.LoadSceneAsync(sceneName, mode), observer))
                .SelectMany(_ =>
                {
                    var scenes = GameObject.FindObjectsOfType<SceneBase>();
                    var loadedScene = scenes.Single(x => !x.IsLoaded);

                    loadedScene.IsLoaded = true;
                    loadedScene.Argument = argument;

                    loadedScene.gameObject.SetActive(false);

                    return loadedScene.PrepareAsync()
                    .Do(__ =>
                    {
                        loadedScene.gameObject.SetActive(true);
                    });
                });
        }
        public static IEnumerator HyperOptimizedFastAsyncOperationLoad(AsyncOperation operation, IObserver<Unit> observer)
        {
            if (!operation.isDone) yield return operation;

            observer.OnNext(Unit.Default);
            observer.OnCompleted();
        }
    }

}