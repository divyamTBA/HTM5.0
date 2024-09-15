using System.Collections;
using System.Collections.Generic;
using Coherence.Toolkit;
using UnityEngine;

public class Frame : MonoBehaviour
{
    public string frameNumber;
    public string contentUrl;
    public Texture2D texture;
    public string artName;
    public string artDesc;
    public string price;
    public void SetFrame(int frameNumber, string contentUrl, Texture2D texture, string desc, string name, string price)
    {
        this.price = price;
        this.artDesc = desc;
        this.artName = name;
        this.frameNumber = "F" + frameNumber.ToString("00");
        this.contentUrl = contentUrl;
        this.gameObject.name = this.frameNumber;
        this.texture = texture;
    }
}
