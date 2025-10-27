using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarLookAt : MonoBehaviour
{
    private enum Mode { LookAt,LookAtInverted, CameraForward}

    [SerializeField] private Mode mode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                break;
            case Mode.LookAtInverted:
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward; 
                break;

        }
    }
}
