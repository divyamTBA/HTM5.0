using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BrunoMikoski.AnimationSequencer;
public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }
    public GameObject loadingUI;
    public TMP_Text loadingText;
    public float defaultLoadDuration = 3f; // Default duration for the loading screen in seconds
    public AnimationSequencerController InAnimation;
    public AnimationSequencerController OutAnimation;

    private void Awake()
    {
        // Ensure only one instance of LoadingManager exists
        if (Instance != null)
        {
            Debug.Log("Found more than one Loading Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void StartLoad(float duration)
    {
        StartCoroutine(LoadingCoroutine(duration));
    }

    public void StartLoad()
    {
        StartCoroutine(LoadingCoroutine(defaultLoadDuration));
    }

    public void SetLoadingText(string text)
    {
        loadingText.text = text;
    }

    public void StopLoad()
    {
        // loadingUI.SetActive(false);
        OutAnimation.Play();
    }

    // Coroutine to handle the loading screen duration
    private IEnumerator LoadingCoroutine(float duration)
    {
        InAnimation.Play();
        yield return new WaitForSeconds(duration);
        StopLoad();
    }

}
