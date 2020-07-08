using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    Vector2 jumpVector;

    Rigidbody2D rb;
    PhysicsMaterial2D physicsMaterial;
    State state;
    Animator anim;

    public event EventHandler<PlaySoundEventArgs> PlaySoundEvent;

    public class PlaySoundEventArgs : EventArgs
    {
        public string sfx;
    }

    public enum State
    {
        Grounded,
        InAir
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsMaterial = GetComponent<Collider2D>().sharedMaterial;
        anim = GetComponent<Animator>();
        PlayerControls.Instance.OnJumpCommand += Instance_OnJumpCommand;
        PlayerControls.Instance.OnButtonPressed += Instance_OnButtonPressed;
    }

    private void Instance_OnJumpCommand(object sender, PlayerControls.OnJumpCommandArgs args)
    {
        if (state == State.Grounded)
        {
            jumpVector = args.force;
        }
    }

    private void Instance_OnButtonPressed(object sender, EventArgs args)
    {
        anim.SetTrigger("crouch");
    }

    void FixedUpdate()
    {
        if (jumpVector.magnitude > 0)
        {
            rb.AddForce(jumpVector, ForceMode2D.Impulse);

            if (jumpVector.x > 0)
            {
                anim.SetTrigger("jumpRight");
            }
            else if (jumpVector.x < 0)
            {
                anim.SetTrigger("jumpLeft");
            }
            else
            {
                // Jump central
            }

            jumpVector = Vector2.zero;
            PlaySoundEvent?.Invoke(this, new PlaySoundEventArgs { sfx = "jump" });
            SetState(State.InAir);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Needed for consistency: sometimes a collision is detected as entering even if its staying
        if (state == State.Grounded)
            return;

        if (CheckIfJumpableSurface(other))
        {
            SetState(State.Grounded);
            PlaySoundEvent?.Invoke(this, new PlaySoundEventArgs { sfx = "hitJumpableSurface" });
            anim.SetTrigger("hurt");
        }
        else
        {
            PlaySoundEvent?.Invoke(this, new PlaySoundEventArgs { sfx = "hitWall" });
        }
    }

    // private void OnCollisionStay2D(Collision2D other)
    // {
    //     // Check collisions while sliding on the same chunk of tiles
    //     if (state != State.Grounded)
    //     {
    //         if (CheckIfJumpableSurface(other))
    //         {
    //             SetState(State.Grounded);
    //             PlaySoundEvent?.Invoke(this, new PlaySoundEventArgs { sfx = "hitJumpableSurface" });
    //         }
    //     }
    // }

    bool CheckIfJumpableSurface(Collision2D other)
    {
        if (other.GetContact(0).normal == Vector2.up)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetState(State requiredState)
    {
        switch (requiredState)
        {
            case State.Grounded:
                state = State.Grounded;
                rb.velocity = Vector3.zero;
                PlayerControls.Instance.enabled = true;
                break;
            case State.InAir:
                state = State.InAir;
                PlayerControls.Instance.enabled = false;
                break;
        }
    }
}
