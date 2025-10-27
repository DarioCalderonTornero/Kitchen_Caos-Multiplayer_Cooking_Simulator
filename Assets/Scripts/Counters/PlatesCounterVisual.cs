using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform visualSpawnPrefab;
    [SerializeField] private Transform counterTopPoint;

    private List<GameObject> gameobjectPlatesList = new List<GameObject>();

    private void Awake()
    {
        gameobjectPlatesList = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateRemove += PlatesCounter_OnPlateRemove;
    }

    private void PlatesCounter_OnPlateRemove(object sender, System.EventArgs e)
    {
        GameObject plateGameObject = gameobjectPlatesList[gameobjectPlatesList.Count - 1];
        gameobjectPlatesList.Remove(plateGameObject);
        Destroy(plateGameObject);
    }

    private void PlatesCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
       Transform plateVisualTransform = Instantiate(visualSpawnPrefab, counterTopPoint);

        float plateYOffset = .1f;

        plateVisualTransform.localPosition = new Vector3(0, plateYOffset * gameobjectPlatesList.Count, 0);
        gameobjectPlatesList.Add(plateVisualTransform.gameObject);
    }
}
