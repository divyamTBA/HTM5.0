using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class TransitionControllerGame : MonoBehaviour
{
    public TransitionSettings transition;
    public float startDelay;
    public GameObject objectToChange;
    TransitionManager transitionManager;
    // Start is called before the first frame update
    public void OpenCanvas(bool activate)
    {
        transitionManager = TransitionManager.Instance();
        transitionManager.onTransitionCutPointReached += () =>
        {
            ActivatePanel(activate);
        };
        transitionManager.Transition(transition, startDelay);
    }


    public void ActivatePanel(bool activate)
    {
        objectToChange.SetActive(activate);
        transitionManager.onTransitionCutPointReached -= () =>
        {
            ActivatePanel(activate);
        };
    }
}
