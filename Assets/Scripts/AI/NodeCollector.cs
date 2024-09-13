using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCollector : MonoBehaviour
{
    private Transform[] _nodes;

    private void Start()
    {
        _nodes = GetComponentsInChildren<Transform>();

        foreach(Enemy enemy in GameManager.Instance.Enemies)
        {
            enemy.PathfindingNodes.AddRange(_nodes);
            enemy.Initialize();
        }
    }
}
