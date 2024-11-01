using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void LoadScenAsync(string scene)
    {
        SceneLoadManager.Instance.LoadSceneAsync(scene);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
