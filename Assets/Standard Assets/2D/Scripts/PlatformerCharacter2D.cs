using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        public Action<PlatformerCharacter2D> onWalk;
        public Action<PlatformerCharacter2D> onJump;
        public Action<PlatformerCharacter2D> onLand;
        public Action<PlatformerCharacter2D, bool> onHoldCrate;

        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private Transform m_RemainUnflipped;

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        [SerializeField]
        float k_GroundedRadius = 0.2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        private Vector2 intendedVelocity = Vector2.zero;
        private bool m_wasOnGround = true;  // For determining which way the player is currently facing.

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            m_wasOnGround = m_Grounded;
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_Grounded = true;
                    if((m_wasOnGround == false) && (onLand != null))
                    {
                        m_wasOnGround = true;
                        onLand(this);
                    }
                }
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
        }

        public bool IsGrounded
        {
            get
            {
                return m_Grounded;
            }
        }

        public bool IsFacingRight
        {
            get
            {
                return m_FacingRight;
            }
        }

        public Vector2 IntendedVelocity
        {
            get
            {
                return intendedVelocity;
            }
        }

        public void Move(float move, bool crouch, bool jump)
        {
            // If crouching, check to see if we're on the ground
            if ((crouch == true) && (m_Grounded == false))
            {
                crouch = false;
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            // Zero velocity
            intendedVelocity.x = 0;
            intendedVelocity.y = m_Rigidbody2D.velocity.y;

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                intendedVelocity.x = move * m_MaxSpeed;
                m_Rigidbody2D.velocity = intendedVelocity;

                // Make sure flipping is enabled
                if (crouch == false)
                {
                    UpdateFlip(move);
                }
                else
                {
                    UpdatePushPull(move);
                }
            }
            if (onHoldCrate != null)
            {
                onHoldCrate(this, crouch);
            }

            // If the player should jump...
            if (m_Grounded && (crouch == false) && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                if(onJump != null)
                {
                    onJump(this);
                }
            }
        }

        public void Step()
        {
            if(onWalk != null)
            {
                onWalk(this);
            }
        }

        private void UpdateFlip(float move)
        {
            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        private void UpdatePushPull(float move)
        {
            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                m_Anim.SetBool("Pulling", true);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                m_Anim.SetBool("Pulling", true);
            }
            else
            {
                m_Anim.SetBool("Pulling", false);
            }
        }

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;

            // Flip the unflipped transform as well
            theScale = m_RemainUnflipped.localScale;
            theScale.x *= -1;
            m_RemainUnflipped.localScale = theScale;
        }
    }
}
