using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerControls : MonoBehaviour
{
    public event EventHandler OnButtonPressed;
    public event EventHandler<OnJumpCommandArgs> OnJumpCommand;

    public class OnJumpCommandArgs : EventArgs
    {
        public Vector2 force;
    }

    public static PlayerControls Instance { get; private set; }

    float jumpingAmount;
    Vector2 jumpVector;
    KeyCode pressedKey;
    [SerializeField] float maxJumpAmount = 10;
    [SerializeField] float jumpForce = 10;
    [SerializeField] float jumpMultiplier = 1;
    
    [SerializeField] float jumpWidth = 1.5f;
    [SerializeField] float jumpHeight = 2;

    TrajectoryController trajectory;

    void Awake()
    {
        Instance = this;
        jumpVector = Vector2.zero;
        trajectory = GetComponent<TrajectoryController>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow)) pressedKey = KeyCode.RightArrow;
        if (Input.GetKey(KeyCode.LeftArrow)) pressedKey = KeyCode.LeftArrow;
        if (Input.GetKey(KeyCode.UpArrow)) pressedKey = KeyCode.UpArrow;

        if (pressedKey != KeyCode.None && jumpingAmount < maxJumpAmount)
        {
            // if its the first press, fire an event to signal it
            if(jumpingAmount == 0)
            {
                OnButtonPressed?.Invoke(this, new EventArgs());
            }

            jumpingAmount += Time.deltaTime * jumpForce * jumpMultiplier;
            trajectory.show = true;

            switch (pressedKey)
            {
                case KeyCode.RightArrow:
                    trajectory.velocity = new Vector2(jumpWidth, jumpHeight) * jumpingAmount;
                    break;
                case KeyCode.LeftArrow:
                    trajectory.velocity = new Vector2(-jumpWidth, jumpHeight) * jumpingAmount;
                    break;
                case KeyCode.UpArrow:
                    trajectory.velocity = new Vector2(0, jumpHeight) * jumpingAmount;
                    break;
            }
        }

        // reset if exceeeded for consistency
        if (jumpingAmount > maxJumpAmount)
            jumpingAmount = maxJumpAmount;

        if (Input.GetKeyUp(pressedKey))
        {
            switch (pressedKey)
            {
                case KeyCode.RightArrow:
                    jumpVector = new Vector2(jumpWidth, jumpHeight) * jumpingAmount;
                    break;
                case KeyCode.LeftArrow:
                    jumpVector = new Vector2(-jumpWidth, jumpHeight) * jumpingAmount;
                    break;
                case KeyCode.UpArrow:
                    jumpVector = new Vector2(0, jumpHeight) * jumpingAmount;
                    break;
            }

            Jump();
        }

    }

    private void Jump()
    {
        OnJumpCommand?.Invoke(this, new OnJumpCommandArgs { force = jumpVector });
        jumpingAmount = 0;
        jumpVector = Vector2.zero;
        pressedKey = KeyCode.None;
        trajectory.show = false;
    }
}
