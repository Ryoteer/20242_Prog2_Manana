using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class FadingPlatform : MonoBehaviour
{
    [Header("<color=#C3CDDB>Behaviours</color>")]
    [SerializeField] private float _fadeTime = 3.0f;
    [SerializeField] private float _interval = 5.0f;
    [SerializeField] private float _respawnTime = 3.0f;

    private Material _mat;
    private Collider _col;
    private NavMeshModifier _mod;
    private bool _isActive;
    private Color _ogColor;

    private void Start()
    {
        _col = GetComponent<Collider>();
        _mod = GetComponent<NavMeshModifier>();
        _mat = GetComponent<Renderer>().material;
        _ogColor = _mat.color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 6 && !_isActive)
        {
            StartCoroutine(PlatformInteraction());
        }
    }

    private IEnumerator PlatformInteraction()
    {
        _isActive = true;

        float t = 0.0f;

        while(t < 1.0f)
        {
            t += Time.deltaTime / _fadeTime;
            _mat.color = new Color(_ogColor.r, _ogColor.g, _ogColor.b, Mathf.Lerp(1.0f, 0.0f, t));
            yield return null;
        }

        _mat.color = new Color(_ogColor.r, _ogColor.g, _ogColor.b, 0.0f);
        _col.enabled = false;
        _mod.enabled = false;

        GameManager.Instance.BuildNavMesh();

        yield return new WaitForSeconds(_interval);

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / _respawnTime;
            _mat.color = new Color(_ogColor.r, _ogColor.g, _ogColor.b, Mathf.Lerp(0.0f, 1.0f, t));
            yield return null;
        }

        _mat.color = new Color(_ogColor.r, _ogColor.g, _ogColor.b, 1.0f);
        _col.enabled = true;
        _mod.enabled = true;

        GameManager.Instance.BuildNavMesh();

        _isActive = false;
    }
}
