using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticClass : MonoBehaviour
{
    private void Awake()
    {
        CuttingCounter.ResetStaticData();
        BaseCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        Player.ResetStaticData();
    }
}
