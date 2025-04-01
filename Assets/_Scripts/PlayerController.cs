using Unity.Netcode;
using UnityEngine;
using System;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 3f;
    private Camera _mainCamera;
    private Vector3 _mouseInput;

    private void Initialize()
    {
        _mainCamera = Camera.main;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }
    private void Update()
    {
        if (!Application.isFocused) return;
        //Movement
        _mouseInput.x = Input.mousePosition.x;
        _mouseInput.y = Input.mousePosition.y;
        _mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint((Vector3)_mouseInput);
        mouseWorldCoordinates.z = 0f;
        transform.position = Vector3.MoveTowards(current: transform.position,
            target: mouseWorldCoordinates,
            maxDistanceDelta: Time.deltaTime * speed);

        //Rotate
        if(mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDirection = mouseWorldCoordinates - transform.position;
            targetDirection.z = 0;
            transform.up = targetDirection;
        }
    }
}
