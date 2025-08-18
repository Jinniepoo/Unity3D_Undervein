using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace Diablo.Characters
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class ControllerCharacter : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float fSpeed = 5f;
        [SerializeField] private float fJumpHeight = 2f;
        [SerializeField] private float fDashDistance = 5f;
        [SerializeField] private float fGravity = -9.81f;
        [SerializeField] public Vector3 vDrags;
        [SerializeField] private LayerMask lGroundLayerMask;

        private CharacterController characterController;
        // 저항값 계산
        private Vector3 vCalcVelocity;
        private bool bOnGround = true;

        private PlayerInputActions inputActions;
        private Vector2 vMoveInput;
        private bool bJumpPressed = false;
        private bool bDashPressed = false;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            inputActions = new PlayerInputActions();

            inputActions.Player.Move.performed += ctx => vMoveInput = ctx.ReadValue<Vector2>();
            inputActions.Player.Move.canceled += ctx => vMoveInput = Vector2.zero;

            inputActions.Player.Jump.performed += ctx => bJumpPressed = true;
            inputActions.Player.Dash.performed += ctx => bDashPressed = true;
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
        }

        void Update()
        {
            bOnGround = characterController.isGrounded;
            if (bOnGround && vCalcVelocity.y < 0)
                vCalcVelocity.y = 0f;

            Vector3 move = new Vector3(vMoveInput.x, 0, vMoveInput.y);
            if (move != Vector3.zero)
                transform.forward = move;

            characterController.Move(move * fSpeed * Time.deltaTime);

            if (bJumpPressed && bOnGround)
            {
                vCalcVelocity.y += Mathf.Sqrt(fJumpHeight * -2f * fGravity);
                bJumpPressed = false;
            }

            if (bDashPressed)
            {
                float logTermX = Mathf.Log(1f / (Time.deltaTime * vDrags.x + 1)) / -Time.deltaTime;
                float logTermZ = Mathf.Log(1f / (Time.deltaTime * vDrags.z + 1)) / -Time.deltaTime;

                Vector3 dashVelocity = Vector3.Scale(transform.forward, new Vector3(logTermX, 0, logTermZ)) * fDashDistance;
                vCalcVelocity += dashVelocity;

                bDashPressed = false;
            }

            // 중력 및 감속 처리 
            vCalcVelocity.y += fGravity * Time.deltaTime;
            vCalcVelocity.x /= 1 + vDrags.x * Time.deltaTime;
            vCalcVelocity.y /= 1 + vDrags.y * Time.deltaTime;
            vCalcVelocity.z /= 1 + vDrags.z * Time.deltaTime;

            characterController.Move(vCalcVelocity * Time.deltaTime);
        }
        #endregion
    }
}
