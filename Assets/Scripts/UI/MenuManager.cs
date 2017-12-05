using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public Animator mAnimatorBoat;

    public void LoadScene(string scene)
    {
        mAnimatorBoat.SetTrigger("Go");
        StartCoroutine(LaunchNewScene(scene));
    }

    IEnumerator LaunchNewScene(string scene)
    {
        yield return new WaitForSeconds(2.3f);
        SceneManager.LoadScene(scene);
    }
}
