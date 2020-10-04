using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string _openAnimation = "Open", _closeAnimation = "Close";

    public Animator _animator;

    private void Awake()
    {
        Open();
    }

    [ContextMenu("Reload Scene")]
    public void ReloadScene(float delay = 0f)
    {
        var manager = new SceneController();
        Close(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex), delay);
    }

    public void LoadScene(string name, float delay = 0f)
    {
        var manager = new SceneController();
        Close(() => SceneManager.LoadScene(name), delay);
    }

    public void LoadScene(string name)
    {
        var manager = new SceneController();
        Close(() => SceneManager.LoadScene(name));
    }

    public void Open(Action callback = null, float delay = 0f)
    {
        StartCoroutine(OpenCoroutine(callback, delay));
    }

    public void Close(Action callback = null, float delay = 0f)
    {
        StartCoroutine(CloseCoroutine(callback, delay));
    }

    IEnumerator CloseCoroutine(Action callback = null, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        _animator.Play(_closeAnimation);

        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length * 1.5f);

        callback?.Invoke();
    }

    IEnumerator OpenCoroutine(Action callback = null, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        _animator.Play(_openAnimation);

        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length * 1.5f);

        callback?.Invoke();
    }

    public void Quit()
    {
        Close(() => Application.Quit());
    }
}
