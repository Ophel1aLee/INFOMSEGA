using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SceneLoader : MonoBehaviour
{
    public Button nextScene;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (nextScene != null)
        {
            nextScene.onClick.AddListener(() => MainManager.Instance.NextLevel(ProgressEnum.Unknown));
        }
    }

    public void FadeIn()
    {
        animator.SetBool("FadeIn", true);
        animator.SetBool("FadeOut", false);
    }

    public void FadeOut()
    {
        animator.SetBool("FadeIn", false);
        animator.SetBool("FadeOut", true);
    }

    public void SetNextSceneButton(Button button)
    {
        if (nextScene != null)
        {
            nextScene.onClick.RemoveAllListeners();
        }
        nextScene = button;
        if (nextScene != null)
        {
            nextScene.onClick.AddListener(() => MainManager.Instance.NextLevel(ProgressEnum.Unknown));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
