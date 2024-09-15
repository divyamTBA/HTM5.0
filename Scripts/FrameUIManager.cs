using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameUIManager : MonoBehaviour
{
    public GameObject framePanel;
    private void Start()
    {
        framePanel = GameObject.Find("FramePanel");
        framePanel.SetActive(false);
    }
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Frame"))
        {
            framePanel.SetActive(true);
            framePanel.GetComponent<FrameUI>().rawImage.texture = other.GetComponent<Frame>().texture;
            framePanel.GetComponent<FrameUI>().artDescription.text = other.GetComponent<Frame>().artDesc;
            framePanel.GetComponent<FrameUI>().artName.text = other.GetComponent<Frame>().artName;
            framePanel.GetComponent<FrameUI>().artPrice.text = "Price : Rs. " + other.GetComponent<Frame>().price;
            framePanel.GetComponent<FrameUI>().vtonButton.onClick.AddListener(() =>
            {
                LoadingManager.Instance.StartLoad(30);
                LoadingManager.Instance.SetLoadingText("Fetching your image and feeding it to our Virtual Tryon...");
            });
        }
    }
    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Frame"))
        {
            framePanel.GetComponent<FrameUI>().rawImage.texture = null;
            framePanel.SetActive(false);
        }
    }
}
