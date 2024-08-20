using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    [Header("Character Name")]
    public string characterName = "";

    [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Current State")]
    [SerializeField] protected AIState currentState;

    [Header("States")]
    public IdleState idleState;
    public PursueTargetState pursueTargetState;
    public CombatStanceState combatStanceState;
    public AttackState attackState;

    protected override void Awake()
    {
        base.Awake();

        aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>();
        aiCharacterLocomotionManager = GetComponent <AICharacterLocomotionManager>();

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            // Use a copy so the original is not modified
            idleState = Instantiate(idleState);
            pursueTargetState = Instantiate(pursueTargetState);
            combatStanceState = Instantiate(combatStanceState);
            attackState = Instantiate(attackState);

            currentState = idleState;
        }

        aiCharacterNetworkManager.currentHealth.OnValueChanged += aiCharacterNetworkManager.CheckHealth;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        aiCharacterNetworkManager.currentHealth.OnValueChanged -= aiCharacterNetworkManager.CheckHealth;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (characterUIManager.hasFloatingHealthBar)
        {
            characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHealthChanged;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (characterUIManager.hasFloatingHealthBar)
        {
            characterNetworkManager.currentHealth.OnValueChanged -= characterUIManager.OnHealthChanged;
        }
    }

    protected override void Update()
    {
        base.Update();

        aiCharacterCombatManager.HandleActionRecovery(this);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsOwner)
        {
            ProcessStateMachine();
        }
    }

    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);

        /* Alternative that checks for null first
        AIState nextState = null;

        if (currentState != null)
        {
            nextState = currentState.Tick(this);
        }
        */

        if (nextState != null)
        {
            currentState = nextState;
        }

        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (aiCharacterCombatManager.currentTarget != null)
        {
            aiCharacterCombatManager.targetDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetDirection);

            aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            // if (remainingDistance > combatStanceState.maximumEngagementDistance)
            if (remainingDistance > navMeshAgent.stoppingDistance && currentState != attackState && currentState != combatStanceState)
            {
                aiCharacterNetworkManager.isMoving.Value = true;
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false;
            }
        }
        else
        {
            aiCharacterNetworkManager.isMoving.Value = false;
        }
    }
}
