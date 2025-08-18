using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // ? NEW Input System namespace

namespace Diablo.Characters
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
    public class RigidBodyCharacter : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private float fSpeed = 5f;

        [SerializeField]
        private float fJumpHeight = 2f;

        [SerializeField]
        private float fCheckDistanceToGround = 0.2f;

        [SerializeField]
        private LayerMask lGroundLayerMask;

        [SerializeField]
        private float fDashDistance = 5f;

        private Rigidbody rRigidbody;
        private Vector3 vInputDirection = Vector3.zero;
        private bool bOnGround = true;

        private PlayerInputActions inputActions;
        private Vector2 vMoveInput;
        private bool bJumpPressed = false;
        private bool bDashPressed = false;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            inputActions = new PlayerInputActions();

            // Move input
            inputActions.Player.Move.performed += context => vMoveInput = context.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += context => vMoveInput = Vector2.zero;

            // Jump input
            inputActions.Player.Jump.performed += context => bJumpPressed = true;

            // Dash input
            inputActions.Player.Dash.performed += context => bDashPressed = true;
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        void Start()
        {
            rRigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            CheckGroundStatus();

            vInputDirection = new Vector3(vMoveInput.x, 0, vMoveInput.y);
            if (vInputDirection != Vector3.zero)
            {
                transform.forward = vInputDirection;
            }

            if (bJumpPressed && bOnGround)
            {
                rRigidbody.AddForce(Vector3.up * Mathf.Sqrt(fJumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
                bJumpPressed = false;
            }

            if (bDashPressed)
            {
                float fDamping = rRigidbody.linearDamping; // or rRigidbody.linearDamping if using ECS Physics
                float fLogTerm = Mathf.Log(1f / (Time.deltaTime * fDamping + 1)) / -Time.deltaTime;
                Vector3 vDashVelocity = Vector3.Scale(transform.forward, new Vector3(fLogTerm, 0, fLogTerm)) * fDashDistance;
                rRigidbody.AddForce(vDashVelocity, ForceMode.VelocityChange);
                bDashPressed = false;
            }
        }

        private void FixedUpdate()
        {
            rRigidbody.MovePosition(rRigidbody.position + vInputDirection * fSpeed * Time.fixedDeltaTime);
        }

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.up * 0.1f + Vector3.down * fCheckDistanceToGround, Color.red);
#endif
            bOnGround = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hitInfo, fCheckDistanceToGround, lGroundLayerMask);
        }

        #endregion
    }
}
