using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class SurfaceAvatar : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.Surface = GetComponent<NavMeshSurface>();
    }
}
