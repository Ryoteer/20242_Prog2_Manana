using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    #region Singleton
    public static SceneLoadManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("<color=#F69AB1>UI</color>")]
    [SerializeField] private Image[] _bgImages;
    [SerializeField] private float _fadeTime = 0.25f;
    [SerializeField] private Image _loadBarBG;
    [SerializeField] private Image _loadBarFill;
    [SerializeField] private TextMeshProUGUI _stateText;

    private bool _isSceneLoading;

    private void Start()
    {
        for(int i = 0; i < _bgImages.Length; i++)
        {
            _bgImages[i].color = new Color(_bgImages[i].color.r, _bgImages[i].color.g, _bgImages[i].color.b, 0.0f);

            _bgImages[i].enabled = false;
        }

        _loadBarBG.enabled = false;
        _loadBarFill.enabled = false;
        _stateText.text = "";
        _stateText.enabled = false;
    }

    public void LoadSceneAsync(string scene)
    {
        if (_isSceneLoading) return;

        StartCoroutine(LoadAsync(scene));
    }

    private IEnumerator LoadAsync(string scene)
    {
        _isSceneLoading = true;

        float t = 0.0f;

        for(int i = 0; i < _bgImages.Length; i++)
        {
            _bgImages[i].enabled = true;
        }

        while (t < 1.0f)
        {
            t += Time.deltaTime / _fadeTime;

            for (int i = 0; i < _bgImages.Length; i++)
            {
                _bgImages[i].color = new Color(_bgImages[i].color.r, _bgImages[i].color.g, _bgImages[i].color.b, Mathf.Lerp(0.0f, 1.0f, t));
            }

            yield return null;
        }        

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(scene);

        asyncOp.allowSceneActivation = false;

        _loadBarBG.enabled = true;
        _loadBarFill.enabled = true;

        while (asyncOp.progress < 0.9f)
        {
            _loadBarFill.fillAmount = asyncOp.progress / 0.9f;
            yield return null;
        }

        _loadBarBG.enabled = false;
        _loadBarFill.enabled = false;

        _stateText.enabled = true;
        _stateText.text = "Press any button to continue.";

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        asyncOp.allowSceneActivation = true;

        _stateText.text = "";
        _stateText.enabled = false;

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / _fadeTime;

            for (int i = 0; i < _bgImages.Length; i++)
            {
                _bgImages[i].color = new Color(_bgImages[i].color.r, _bgImages[i].color.g, _bgImages[i].color.b, Mathf.Lerp(1.0f, 0.0f, t));
            }

            yield return null;
        }

        for (int i = 0; i < _bgImages.Length; i++)
        {
            _bgImages[i].enabled = false;
        }

        _isSceneLoading = false;
    }
}
