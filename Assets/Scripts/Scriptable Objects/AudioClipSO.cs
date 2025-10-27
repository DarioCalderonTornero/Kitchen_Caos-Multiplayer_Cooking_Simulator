using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipSO : ScriptableObject
{
    public AudioClip[] chop;
    public AudioClip[] deliverySuccess;
    public AudioClip[] deliveryFail;
    public AudioClip[] footsteps;
    public AudioClip[] dropObject;
    public AudioClip[] pickUpObject;
    public AudioClip stoveCounter;
    public AudioClip[] trash;
    public AudioClip[] warning;
}
