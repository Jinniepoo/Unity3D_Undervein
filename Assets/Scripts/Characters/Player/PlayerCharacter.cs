using Diablo.Characters;
using Diablo.Core;
using Diablo.InventorySystem.Inventory;
using Diablo.InventorySystem.Items;
using Diablo.SceneUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Diablo.Characters
{
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour, IAttackable, IDamageable
    {
        [SerializeField]
        private InventoryObject equipment;

        [SerializeField]
        private InventoryObject inventory;

        #region Variables
        public PlaceTargetWithMouse picker;

        private CharacterController controller;
        [SerializeField]
        private LayerMask groundLayerMask;

        internal void TakeDamage(int v)
        {
            throw new NotImplementedException();
        }

        private NavMeshAgent agent;
        private Camera camCamera;

        [SerializeField]
        private Animator animator;

        readonly int moveHash = Animator.StringToHash("Move");
        readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
        readonly int attackTriggerHash = Animator.StringToHash("AttackTrigger");
        readonly int attackIndexHash = Animator.StringToHash("AttackIndex");
        readonly int hitTriggerHash = Animator.StringToHash("Hit");
        readonly int isAliveHash = Animator.StringToHash("Alive");

        [SerializeField]
        private LayerMask targetMask;
        public Transform target;

        public bool IsInAttackState => GetComponent<AttackStateController>()?.IsInAttackState ?? false;

        [SerializeField]
        private Transform hitPoint;

        public PlayerInputActions inputActions;

        [SerializeField]
        public StatsObject playerStats;

        #endregion

        #region Main Methods

        // Start is called before the first frame update
        void Start()
        {
            inventory.OnUseItem += OnUseItem;

            inputActions = new PlayerInputActions();
            inputActions.Enable();

            controller = GetComponent<CharacterController>();

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = true;

            camCamera = Camera.main;

            InitAttackBehaviour();
        }

        void Update()
        {
            if (!IsAlive)
            {
                return;
            }

            CheckAttackBehaviour();


            bool isOnUI = EventSystem.current.IsPointerOverGameObject();

            if (!isOnUI && inputActions.Player.LeftClick.WasPressedThisFrame() && !IsInAttackState)
            {
                Ray ray = camCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, groundLayerMask))
                {
                    Debug.Log("Raycast Detected: " + hit.collider.name + " " + hit.point);

                    RemoveTarget();

                    agent.SetDestination(hit.point);

                    SetPicker(hit.point);
                }
            }
            else if (!isOnUI && inputActions.Player.RightClick.WasPressedThisFrame())
            {
                Ray ray = camCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    Debug.Log("Raycast Detected: " + hit.collider.name + " " + hit.point);

                    IDamageable damagable = hit.collider.GetComponent<IDamageable>();
                    if (damagable != null && damagable.IsAlive)
                    {
                        SetTarget(hit.collider.transform, CurrentAttackBehaviour?.range ?? 0);
                        SetPicker(hit.collider.transform.position);
                    }

                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        SetTarget(hit.collider.transform, interactable.Distance);
                    }
                }
            }

            if (target != null)
            {
                if (target.GetComponent<IInteractable>() != null)
                {
                    float calcDistance = Vector3.Distance(target.position, transform.position);
                    float range = target.GetComponent<IInteractable>().Distance;
                    if (calcDistance > range)
                    {
                        SetTarget(target, range);
                    }

                    FaceToTarget();
                }
                else if (!(target.GetComponent<IDamageable>()?.IsAlive ?? false))
                {
                    RemoveTarget();
                }
                else
                {
                    float calcDistance = Vector3.Distance(target.position, transform.position);
                    float range = CurrentAttackBehaviour?.range ?? 2.0f;
                    if (calcDistance > range)
                    {
                        SetTarget(target, range);
                    }

                    FaceToTarget();
                }
            }

            if (agent.pathPending || (agent.remainingDistance > agent.stoppingDistance))
            {
                controller.Move(agent.velocity * Time.deltaTime);
                animator.SetFloat(moveSpeedHash, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
                animator.SetBool(moveHash, true);
            }
            else
            {
                controller.Move(Vector3.zero);

                if (!agent.pathPending)
                {
                    animator.SetFloat(moveSpeedHash, 0);
                    animator.SetBool(moveHash, false);
                    agent.ResetPath();
                }

                if (target != null)
                {
                    if (target.GetComponent<IInteractable>() != null)
                    {
                        IInteractable interactable = target.GetComponent<IInteractable>();
                        interactable.Interact(this.gameObject);
                        target = null;
                    }
                    else if (target.GetComponent<IDamageable>() != null)
                    {
                        AttackTarget();
                    }
                }
                else
                {
                    RemovePicker();
                }
            }
        }

        private void OnAnimatorMove()
        {
            // Follow CharacterController
            Vector3 position = transform.position;
            position.y = agent.nextPosition.y;

            animator.rootPosition = position;
            agent.nextPosition = position;
        }
        #endregion Main Methods

        #region Helper Methods

        private void InitAttackBehaviour()
        {
            foreach (AttackBehaviour behaviour in attackBehaviours)
            {
                behaviour.targetMask = targetMask;
            }

            GetComponent<AttackStateController>().enterAttackHandler += OnEnterAttackState;
            GetComponent<AttackStateController>().exitAttackHandler += OnEnterAttackState;
        }

        private void CheckAttackBehaviour()
        {
            if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
            {
                CurrentAttackBehaviour = null;

                foreach (AttackBehaviour behaviour in attackBehaviours)
                {
                    if (behaviour.IsAvailable)
                    {
                        if ((CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.priority < behaviour.priority))
                        {
                            CurrentAttackBehaviour = behaviour;
                        }
                    }
                }
            }
        }

        void SetTarget(Transform newTarget, float stoppingDistance)
        {
            target = newTarget;

            agent.stoppingDistance = stoppingDistance - 0.05f;
            agent.updateRotation = false;
            agent.SetDestination(newTarget.transform.position);

            SetPicker(newTarget.transform.position);
        }

        public void RemoveTarget()
        {
            target = null;
            agent.stoppingDistance = 0.00f;
            agent.updateRotation = true;

            agent.ResetPath();

            RemovePicker();
        }

        private void SetPicker(Vector3 position)
        {
            if (!picker)
            {
                return;
            }

            Vector3 calcPosition = position;
            calcPosition.y += picker.surfaceOffset;
            picker.transform.position = calcPosition;
        }

        private void RemovePicker()
        {
            if (!picker)
            {
                return;
            }

            Vector3 calcPosition = picker.transform.position;
            calcPosition.y = -10;
            picker.transform.position = calcPosition;

        }

        void AttackTarget()
        {
            if (CurrentAttackBehaviour == null)
            {
                return;
            }

            if (target != null && !IsInAttackState && CurrentAttackBehaviour.IsAvailable)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= CurrentAttackBehaviour?.range)
                {
                    animator?.SetTrigger(attackTriggerHash);
                    animator.SetInteger(attackIndexHash, CurrentAttackBehaviour.animationIndex);
                }
            }
        }

        void FaceToTarget()
        {
            if (target)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
            }
        }

        public void OnEnterAttackState()
        {
            UnityEngine.Debug.Log("[Player] OnEnterAttackState()");
            playerStats.AddMana(-3);
        }

        public void OnExitAttackState()
        {
            UnityEngine.Debug.Log("[Player] OnExitAttackState()");
        }

        #endregion Helper Methods

        #region IAttackable Interfaces
        [SerializeField]
        private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {
            if (CurrentAttackBehaviour != null)
            {
                CurrentAttackBehaviour.ExecuteAttack(target.gameObject);
            }
        }

        #endregion IAttackable Interfaces

        #region IDamagable Interfaces

        public bool IsAlive => playerStats.Health > 0;

        public void TakeDamage(int damage, GameObject damageEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            playerStats.AddHealth(-damage);

            if (damageEffectPrefab)
            {
                Instantiate<GameObject>(damageEffectPrefab, hitPoint);
            }

            if (IsAlive)
            {
                animator?.SetTrigger(hitTriggerHash);
            }
            else
            {
                animator?.SetBool(isAliveHash, false);
            }
        }

        #endregion IDamagable Interfaces


        #region Inventory
        private void OnUseItem(ItemObject itemObject)
        {
            foreach (ItemBuff buff in itemObject.data.buffs)
            {
                if (buff.stat == AttributeType.Health)
                {
                    playerStats.AddHealth(buff.value);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponent<GroundItem>();
            if (item)
            {
                if (inventory.AddItem(new Item(item.itemObject), 1))
                    Destroy(other.gameObject);
            }
        }

        public bool PickupItem(ItemObject itemObject, int amount = 1)
        {
            if (itemObject != null)
            {
                return inventory.AddItem(new Item(itemObject), amount);
            }

            return false;
        }
        #endregion Inventory
    }
}