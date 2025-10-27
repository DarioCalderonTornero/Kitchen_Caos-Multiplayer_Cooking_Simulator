using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleIconTemplate : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetKitchenObject(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.sprite;
    }
}
