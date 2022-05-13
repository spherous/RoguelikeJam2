using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanOnLoss : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private Transform moveToTransform;
    [SerializeField] CameraMovementController cameraMovementController;
    [SerializeField] CameraFollow cameraFollow;


    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        gameManager.onHealthChanged += OnHealthChanged;
        moveToTransform = GameObject.FindObjectOfType<Kingdom>().transform;
    }
    private void OnHealthChanged(IHealth changed, float oldHP, float newHP, float percent)
    {
        if(newHP == 0)
        {
            cameraMovementController.enabled = false;
            cameraFollow.followSpeed = 1;
            gameObject.transform.position = moveToTransform.position;
            return;
        }
    }
}
