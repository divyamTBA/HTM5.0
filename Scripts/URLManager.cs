using System;
using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;
using UnityEngine.SceneManagement;

public class URLManager : MonoBehaviour
{
    public TransitionSettings transition;
    public float transitionDelay;
    public static URLManager instance { get; private set; }
    public string tokenFetchUrl;
    public string tokenSendUrl;
    public string userSendUrl;
    public bool globalMode;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex > 0)
        {
            if (globalMode)
            {
                GameObject.Find("Room Connection Dialog").SetActive(true);
                Debug.Log("Multiplayer mode");
                FindObjectOfType<ApiFetch>().StartFetch(true);
            }
            else
            {
                GameObject.Find("Room Connection Dialog").SetActive(false);
                Debug.Log("Singleplayer mode");
                FindObjectOfType<ApiFetch>().StartFetch(true);

            }
        }
    }

    private void Awake()
    {
        // Ensure only one instance of DataPersistenceManager exists
        if (instance != null)
        {
            Debug.Log("Found more than one URL Manager in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void SetUrls(string tokenFetchUrl, string tokenSendUrl, string userSendUrl)
    {
        this.tokenFetchUrl = tokenFetchUrl;
        this.tokenSendUrl = tokenSendUrl;
        this.userSendUrl = userSendUrl;
    }
    public void SetTransition()
    {
        TransitionManager.Instance().Transition("Playground", transition, transitionDelay);
    }
}
