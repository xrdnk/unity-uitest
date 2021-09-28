#if USE_UNITASK

using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnityUITest
{
    public abstract partial class UITest
    {
        /// <summary>
        /// 条件満足時まで待機する
        /// </summary>
        /// <param name="condition">条件</param>
        protected async UniTask WaitForAsync(Condition condition)
        {
            await StartAsyncCoroutine(WaitForAsyncInternal(condition, Environment.StackTrace));
        }

        /// <summary>
        /// シーンロード完了まで待機する
        /// </summary>
        /// <param name="name">シーン名</param>
        protected async UniTask LoadSceneAsync(string name)
        {
            await StartAsyncCoroutine(LoadSceneAsyncInternal(name));
        }

        /// <summary>
        /// テキストの表示が一致しているか検証が完了するまで待機する
        /// </summary>
        /// <param name="id">Text Component オブジェクトの名前</param>
        /// <param name="text">テキストの期待値</param>
        protected async UniTask AssertLabelAsync(string id, string text)
        {
            await StartAsyncCoroutine(AssertLabelAsyncInternal(id, text));
        }

        /// <summary>
        /// ボタン押下完了まで待機
        /// </summary>
        /// <param name="buttonName">Button Component オブジェクトの名前</param>
        protected async UniTask PressAsync(string buttonName)
        {
            await StartAsyncCoroutine(PressAsyncInternal(buttonName));
        }

        /// <summary>
        /// ボタン押下完了まで待機
        /// </summary>
        /// <param name="o">Button Component オブジェクト</param>
        protected async UniTask PressAsync(GameObject o)
        {
            await StartAsyncCoroutine(PressAsyncInternal(o));
        }

        async UniTask StartAsyncCoroutine(UniTask asyncCoroutine)
        {
            CreateMonoBehaviour();
            await mb.StartAsyncCoroutine(token => asyncCoroutine);
        }

        async UniTask WaitForAsyncInternal(Condition condition, string stackTrace)
        {
            var time = 0f;
            while (!condition.Satisfied())
            {
                if (time > WaitTimeout)
                {
                    throw new Exception($"Operation timed out: {condition}\n {stackTrace}");
                }
                else
                {
                    for (var i = 0; i < WaitIntervalFrames; i++)
                    {
                        time += Time.unscaledDeltaTime;
                        await UniTask.Yield();
                    }
                }
            }
        }

        async UniTask LoadSceneAsyncInternal(string name)
        {
#if UNITY_EDITOR
            if (name.Contains(".unity"))
            {
                EditorSceneManager.LoadSceneInPlayMode(name, new LoadSceneParameters(LoadSceneMode.Single));
                await WaitForAsync(new SceneLoaded(Path.GetFileNameWithoutExtension(name)));
                return;
            }
#endif
            SceneManager.LoadScene(name);
            await WaitForAsync(new SceneLoaded(name));
        }

        async UniTask PressAsyncInternal(string buttonName)
        {
            var buttonAppeared = new ObjectAppeared(buttonName);
            await WaitForAsync(buttonAppeared);
            await PressAsync(buttonAppeared.o);
        }

        async UniTask PressAsyncInternal(GameObject o)
        {
            await WaitForAsync(new ButtonAccessible(o));
            Debug.Log("Button pressed: " + o);
            ExecuteEvents.Execute(o, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            await UniTask.Yield();
        }

        async UniTask AssertLabelAsyncInternal(string id, string text)
        {
            await WaitForAsync(new LabelTextAppeared(id, text));
        }
    }
}

#endif