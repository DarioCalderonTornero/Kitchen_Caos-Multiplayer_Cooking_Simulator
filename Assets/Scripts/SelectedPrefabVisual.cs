using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedPrefabVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] GameObject[] visualGameObjectArray;

    void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }
        else
        {
            Player.OnSpawnAnyPlayer += Player_OnSpawnAnyPlayer;
        }
    }

    private void Player_OnSpawnAnyPlayer(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChange -= Player_OnSelectedCounterChange;
            Player.LocalInstance.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        }
    }

    private void Player_OnSelectedCounterChange(object sender, Player.OnSelectedCounterChangeEventArgs e)
    {
       if (e.selectedCounter == baseCounter)
       {
            Show();
       }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
        
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
