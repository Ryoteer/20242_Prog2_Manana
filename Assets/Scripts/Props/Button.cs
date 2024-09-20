using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [Header("<color=blue>Behaviours</color>")]
    [SerializeField] private GameObject _affectedObj;

    private Material _aoMat;

    private void Start()
    {
        _aoMat = _affectedObj.GetComponent<Renderer>().material;
    }

    public void OnInteract()
    {
        _aoMat.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
    }
}
