using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Controls controls;

    private Vector2 move;

    private Rigidbody2D rigidBody;

    [SerializeField]
    private float speed = 1.0f;

    private void Awake()
    {
        controls = new Controls();

        controls.Master.Move.performed += ctx =>
        {
            move = ctx.ReadValue<Vector2>();
        };
        controls.Master.Move.canceled += ctx => move = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Master.Enable();
    }

    private void OnDisable()
    {
        controls.Master.Disable();
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody == null)
        {
            Debug.LogError("No rigidBody in Player");
        }
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidBody.position;

        position.x += move.x * speed * Time.fixedDeltaTime;
        position.y += move.y * speed * Time.fixedDeltaTime;

        rigidBody.MovePosition(position);
    }
}
