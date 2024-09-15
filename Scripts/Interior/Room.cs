using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Coherence;
using Coherence.Toolkit;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] connectedRooms;
    // public AnimationSequencerController animationSequencerController;
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            AnimateRooms();
        }
    }
    public void AnimateRooms()
    {

        var coherenceSync = GetComponentInParent<CoherenceSync>();
        if (!coherenceSync.HasStateAuthority)
        {
            bool requestSuccess = coherenceSync.RequestAuthority(AuthorityType.Full);
            Debug.Log(requestSuccess);
        }

        foreach (GameObject room in connectedRooms)
        {
            room.GetComponent<AnimationSequencerController>().Play();
        }
    }
}
