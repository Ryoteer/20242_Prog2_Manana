using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCollector : MonoBehaviour
{
    [Header("<color=red>AI</color>")]
    [SerializeField] private Player _player;
    [SerializeField] private List<Enemy> _enemies = new();

    private Transform[] _nodes;

    private void Start()
    {
        _nodes = GetComponentsInChildren<Transform>();

        foreach(Enemy enemy in _enemies)
        {
            enemy.TargetTransform = _player.transform;
            enemy.PathfindingNodes.AddRange(_nodes);
            enemy.Initialize();
        }
    }
}
