using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private float _distToChangeNode = 0.5f;
    [SerializeField] private float _distToChase = 6.0f;
    [SerializeField] private float _distToAttack = 2.0f;

    private Transform _actualNode, _targetTransform;
    private List<Transform> _pathfindingNodes = new();
    public Transform TargetTransform 
    { 
        get { return _targetTransform; } 
        set { _targetTransform = value; } 
    }
    public List<Transform> PathfindingNodes 
    { 
        get { return _pathfindingNodes; } 
        set { _pathfindingNodes = value; } 
    }

    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize()
    {
        _actualNode = GetNewNode();

        _agent.SetDestination(_actualNode.position);
    }

    private void Update()
    {
        if(Vector3.SqrMagnitude(transform.position - _targetTransform.position) <= Mathf.Pow(_distToChase, 2))
        {
            if(Vector3.SqrMagnitude(transform.position - _targetTransform.position) <= Mathf.Pow(_distToAttack, 2))
            {
                _agent.isStopped = true;

                Debug.Log($"<color=red>{name}</color>: Japish.");
            }
            else
            {
                if(_agent.isStopped) _agent.isStopped = false;

                _agent.SetDestination(_targetTransform.position);
            }
        }
        else
        {
            if (_agent.isStopped) _agent.isStopped = false;
            if (_agent.destination != _actualNode.position) _agent.SetDestination(_actualNode.position);

            if (Vector3.SqrMagnitude(transform.position - _actualNode.position) <= Mathf.Pow(_distToChangeNode, 2))
            {
                _actualNode = GetNewNode();

                _agent.SetDestination(_actualNode.position);
            }
        }
    }

    private Transform GetNewNode(Transform lastNode = null)
    {
        Transform newNode = _pathfindingNodes[Random.Range(0, _pathfindingNodes.Count)];

        while(lastNode == newNode)
        {
            newNode = _pathfindingNodes[Random.Range(0, _pathfindingNodes.Count)];
        }

        return newNode;
    }
}
