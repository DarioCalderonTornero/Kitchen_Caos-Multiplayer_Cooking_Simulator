using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageControl : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image image;
    [SerializeField] private Image image2;
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private TextMeshProUGUI playerNameText;

    

    public static ImageControl Instance { get; private set; }

    private void Start()
    {
        HideImage1();
        HideImage2();
        player.OnSelectedCounterChange += Player_OnSelectedCounterChange;
        player.OnNotCounter += Player_OnNotCounter;
        //platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        stoveCounter.Onfrying += StoveCounter_Onfrying;
        stoveCounter.OnFried += StoveCounter_OnFried;
        stoveCounter.OnBurned += StoveCounter_OnBurned;
        TrashCounter.OnAnyObjectTrash += TrashCounter_OnAnyObjectTrash;

        Instance = this;
    }

    private void TrashCounter_OnAnyObjectTrash(object sender, EventArgs e)
    {
        HideImage1();
    }

    private void StoveCounter_OnBurned(object sender, EventArgs e)
    {
        ShowImage1();
    }

    private void StoveCounter_OnFried(object sender, EventArgs e)
    {
        ShowImage1();
    }

    private void StoveCounter_Onfrying(object sender, EventArgs e)
    {
        ShowImage1();
    }

    private void PlatesCounter_OnPlateSpawned(object sender, EventArgs e)
    {
        ShowImage1();
    }

    private void Player_OnNotCounter(object sender, EventArgs e)
    {
        HideImage1();
        HideImage2();
    }

    private void Player_OnSelectedCounterChange(object sender, Player.OnSelectedCounterChangeEventArgs e)
    {
        if (e.selectedCounter != null)
        {
            if (e.selectedCounter is CuttingCounter cuttingCounter)
            {
                cuttingCounter.OnCanCut += CuttingCounter_OnCanCut;
                cuttingCounter.OnCutFinished += CuttingCounter_OnCutFinished;

                if (player.HasKitchenObject())
                {
                    KitchenObjectSO playerKitchenObjectSO = player.GetKitchenObject().GetKitchenObjectSO();

                    foreach (CuttingObjectsSO cuttingObjectSO in cuttingCounter.cuttingObjectSOArray)
                    {
                        if (cuttingObjectSO.input == playerKitchenObjectSO)
                        {
                            ShowImage1();
                            break;
                        }
                    }
                }

                if (cuttingCounter.HasKitchenObject() && !player.HasKitchenObject())
                {
                    ShowImage2();
                    ShowImage1();
                }



                if (cuttingCounter.HasKitchenObject())
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    if (kitchenObject is PlateKitchenObject plateKitchenObject && player.HasKitchenObject())
                    {
                        ShowImage1();                           
                    }
                }
            }


            if (e.selectedCounter is ContainerCounter containerCounter)
            {
                if (!player.HasKitchenObject())
                {
                    ShowImage1();
                }
                containerCounter.OnObjectPicked += ContainerCounter_OnObjectPicked;
            }

            if (e.selectedCounter is ClearCounter clearCounter)

            {
                if (!clearCounter.HasKitchenObject())
                {
                    if (player.HasKitchenObject())
                    {
                        ShowImage1();
                    }
                }

                if (clearCounter.HasKitchenObject())
                {
                    if (!player.HasKitchenObject())
                    {
                        ShowImage1();
                    }
                }

                if (clearCounter.HasKitchenObject())
                {
                    KitchenObject kitchenObject = clearCounter.GetKitchenObject();

                    if (kitchenObject is PlateKitchenObject plateKitchenObject && player.HasKitchenObject())
                    {
                        KitchenObjectSO playerObjectSO = player.GetKitchenObject().GetKitchenObjectSO();

                        if (plateKitchenObject.IsValidIngredient(playerObjectSO))
                        {
                            ShowImage1();
                        }
                    }
                }

                if (player.HasKitchenObject() && player.GetKitchenObject() is PlateKitchenObject playerPlate)
                {
                    if (clearCounter.HasKitchenObject())
                    {
                        KitchenObjectSO counterObjectSO = clearCounter.GetKitchenObject().GetKitchenObjectSO();

                        if (playerPlate.IsValidIngredient(counterObjectSO))
                        {
                            ShowImage1();
                        }
                    }
                }
            }

            if (e.selectedCounter is PlatesCounter platesCounter)
            {
                //if (!player.HasKitchenObject() && platesCounter.spawnPlatesAmount >= 1)
                   // ShowImage1();
            }

            if (e.selectedCounter is StoveCounter stoveCounter)
            {
                if (player.HasKitchenObject())
                {
                    KitchenObjectSO playerKitchenObjectSO = player.GetKitchenObject().GetKitchenObjectSO();

                    bool hasValidRecipe = stoveCounter.HasRecipeWithInput(playerKitchenObjectSO);

                    if (hasValidRecipe)
                    {
                        ShowImage1();
                    }

                }

                if (!player.HasKitchenObject())
                {
                    ShowImage1();
                }

                if (!stoveCounter.HasKitchenObject() && !player.HasKitchenObject())
                {
                    HideImage1();
                }

                KitchenObject kitchenObject = player.GetKitchenObject();

                if (kitchenObject is PlateKitchenObject plateKitchenObject && stoveCounter.HasKitchenObject())
                {
                    ShowImage1();
                }


            }

            if (e.selectedCounter is DeliveryCounter deliveryCounter)
            {
                if (player.HasKitchenObject() && player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    ShowImage1();
                }
                else
                {
                    HideImage1();
                }
            }

            if (e.selectedCounter is TrashCounter trashCounter)
            {
                if (player.HasKitchenObject())
                {
                    ShowImage1();
                }
            }
        }

    }



    private void ContainerCounter_OnObjectPicked(object sender, EventArgs e)
    {
        HideImage1();

    }

    private void CuttingCounter_OnCutFinished(object sender, EventArgs e)
    {
        HideImage2();
    }

    private void CuttingCounter_OnCanCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;

        if (cuttingCounter.HasKitchenObject() && cuttingCounter.HasRecipeWithInput(cuttingCounter.GetKitchenObject().GetKitchenObjectSO()))
        {
            ShowImage2();
        }
        else
        {
            HideImage2();
        }
    }

    public void HideImage1()
    {
        image.gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(true);
    }

    public void HideImage2()
    {
        image2.gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(true);
    }

    public void ShowImage1()
    {
        image.gameObject.SetActive(true);
        playerNameText.gameObject.SetActive(false);
    }

    public void ShowImage2()
    {
        image2.gameObject.SetActive(true);
        playerNameText.gameObject.SetActive(false);
    }
}