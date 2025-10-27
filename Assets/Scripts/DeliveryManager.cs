using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class DelyveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFail;

    [SerializeField] private ReciveListSO reciveListSO;
    private List<RecipeSO> waitingRecipeSOList;
    public static DelyveryManager Instance { get; private set; }

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;

    private Dictionary<ulong, int> playerSuccessfulRecipes = new Dictionary<ulong, int>();

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            SpawnRecipe();
        }
    }

    public void SpawnRecipe()
    {
        if (KitchenGameManager.Instance.isPlayingGame() && waitingRecipeSOList.Count < waitingRecipesMax)
        {
            int waitingRecipeIndex = UnityEngine.Random.Range(0, reciveListSO.recipeSOList.Count);
            SpawnNewWaitingRecipeClientRpc(waitingRecipeIndex);
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeIndex)
    {
        RecipeSO waitingRecipeSO = reciveListSO.recipeSOList[waitingRecipeIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenSOList())
                    {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    HandleRecipeDeliverySuccessServerRpc(i);
                    return;
                }
            }
        }

        MatchDeliveryManagerIncorrectUIServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleRecipeDeliverySuccessServerRpc(int waitingRecipeSOListIndex, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        if (!playerSuccessfulRecipes.ContainsKey(clientId))
        {
            playerSuccessfulRecipes[clientId] = 0;
        }

        playerSuccessfulRecipes[clientId]++;

        // Notificar al cliente correspondiente de su progreso
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        };

        UpdateClientSuccessfulRecipesClientRpc(clientId, playerSuccessfulRecipes[clientId], clientRpcParams);

        // Eliminar receta completada del servidor
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        // Sincronizar índices actualizados con todos los clientes
        SyncWaitingRecipesClientRpc(GetRecipeIndices().ToArray());
    }

    [ClientRpc]
    private void UpdateClientSuccessfulRecipesClientRpc(ulong clientId, int successfulRecipes, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            // Actualiza el número de recetas exitosas solo para el cliente correspondiente
            playerSuccessfulRecipes[clientId] = successfulRecipes;
            OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MatchDeliveryManagerIncorrectUIServerRpc()
    {
        MatchDeliveryManagerIncorrectUIClientRpc();
    }

    [ClientRpc]
    private void MatchDeliveryManagerIncorrectUIClientRpc()
    {
        OnRecipeFail?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void SyncWaitingRecipesClientRpc(int[] recipeIndices)
    {
        // Reconstruir la lista local en los clientes
        waitingRecipeSOList.Clear();
        foreach (int index in recipeIndices)
        {
            waitingRecipeSOList.Add(reciveListSO.recipeSOList[index]);
        }

        // Disparar el evento para actualizar la interfaz en los clientes
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
    }

    private List<int> GetRecipeIndices()
    {
        // Devuelve los índices de las recetas actuales en la lista
        List<int> recipeIndices = new List<int>();
        foreach (RecipeSO recipe in waitingRecipeSOList)
        {
            recipeIndices.Add(reciveListSO.recipeSOList.IndexOf(recipe));
        }
        return recipeIndices;
    }

    public List<RecipeSO> WaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetPlayerSuccessfulRecipes(ulong clientId)
    {
        return playerSuccessfulRecipes.ContainsKey(clientId) ? playerSuccessfulRecipes[clientId] : 0;
    }
}
