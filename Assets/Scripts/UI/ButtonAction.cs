using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    public void LoadScenAsync(string scene)
    {
        SceneLoadManager.Instance.LoadSceneAsync(scene);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
