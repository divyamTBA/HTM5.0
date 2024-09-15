using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFramerate : MonoBehaviour
{
    public int TargetFrames;
    private void Start()
    {
        Application.targetFrameRate = TargetFrames;
    }
}
