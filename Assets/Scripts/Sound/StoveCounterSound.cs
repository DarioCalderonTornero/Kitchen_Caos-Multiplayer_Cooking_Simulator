using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private StoveCounter stoveCounter;

    private float warningTimer;
    private bool playWarningSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventsArgs e)
    {
        float burnShowProgress = .5f;
        playWarningSound = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgress;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnchangeStateEventArgs e)
    {
        bool audioBool = e.state == StoveCounter.State.frying || e.state == StoveCounter.State.fried;
        if (audioBool)
        {
            audioSource.Play();
        }

        else
        {
            audioSource.Pause();
        }
    }

    private void Update()
    {
        if (playWarningSound)
        {
            warningTimer -= Time.deltaTime;
            if (warningTimer <= 0)
            {
                float warningTimerMax = .2f;

                warningTimer = warningTimerMax;
                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }
        }
       
    }
}
