using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveCounterVisual;
    [SerializeField] private GameObject stoveCounterParticles;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnchangeStateEventArgs e)
    {
        bool onVisual = e.state == StoveCounter.State.frying || e.state == StoveCounter.State.fried;
        stoveCounterVisual.SetActive(onVisual);
        stoveCounterParticles.SetActive(onVisual);
    }
}
