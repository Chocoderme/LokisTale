using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionDisplay : MonoBehaviour {

    public UnityEngine.UI.Image[] holder;
    WaveManager wm;
    Shoot playerShoot;

    public Color visibleColor;
    public Color HiddenColor;

    public UnityEngine.UI.Image[] choices;
    public Color normalColor;
    public Color choiceColor;

    public float waitBlink = 0.05f;

    // Use this for initialization
    void Start()
    {
        if (holder == null)
            Debug.LogError("Couldn't find canvas image holder");
        foreach (var img in holder)
        {
            var toc = img.color;
            toc.a = 0;
            img.color = toc;
        }
    }
	
	public void ShowDisplay(int choice)
    {
        StartCoroutine(FadeTo(HiddenColor, visibleColor, 1f));
        StartCoroutine(YoloWeapon(3f, choice));
        Invoke("HideDisplay", 6f);
    }

    IEnumerator YoloWeapon(float time, int choice)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += waitBlink;
            int index = Random.Range(0, choices.Length);
            for (int i = 0; i < choices.Length; ++i)
            {
                if (i == index)
                    choices[i].color = choiceColor;
                else
                    choices[i].color = normalColor;
            }
            yield return new WaitForSeconds(waitBlink);
        }
        for (int i = 0; i < choices.Length; ++i)
        {
            if (i == choice)
                choices[i].color = choiceColor;
            else
                choices[i].color = normalColor;
        }
    }

    public void HideDisplay()
    {
        StartCoroutine(FadeTo(visibleColor, HiddenColor, 1f));
    }

    IEnumerator FadeTo(Color from, Color to, float time)
    {
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            if (elapsed > time)
                elapsed = time;
            foreach (var img in holder)
            {
                var fromc = img.color;
                fromc.a = from.a;
                var toc = img.color;
                toc.a = to.a;
                img.color = Color.Lerp(fromc, toc, elapsed / time);
            }
            yield return null;
        }
    }
}
