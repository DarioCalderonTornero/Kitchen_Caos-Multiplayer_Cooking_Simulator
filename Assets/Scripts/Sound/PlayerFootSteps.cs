using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    private Player player;
    private float footStepsTimer;
    private float footStepsTimerMax = 0.1f;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        footStepsTimer -= Time.deltaTime;
        if (footStepsTimer < 0f)
        {
            footStepsTimer = footStepsTimerMax;

            if (player.isMoving)
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootStepsSound(player.transform.position, volume);
            }
        }
    }
}
