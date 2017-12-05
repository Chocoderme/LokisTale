using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour {

    [SerializeField] Color ColorDamage;
    [SerializeField] float FadeTime = 0.5f;

    Animator mAnimator;
    Renderer[] mRenderer;
    List<Material> mMaterial = new List<Material>();
    List<Color> OriginalColor = new List<Color>();

    // Use this for initialization
    void Awake()
    {
        mAnimator = GetComponentInChildren<Animator>();
        mRenderer = GetComponentsInChildren<Renderer>();
        foreach (Renderer red in mRenderer)
        {
            mMaterial.Add(red.material);
            OriginalColor.Add(red.material.color);
        }
    }

    private void Start()
    {
        FadeIn();
    }

    private void Update()
    {
       /* if (Input.GetKey(KeyCode.A))
             TakeDamage();
        else if (Input.GetKey(KeyCode.Z))
            FadeOut();*/
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCoroutine(0, 1, FadeTime));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCoroutine(1, 0, FadeTime));
    }

    public void Run()
    {
        if (mAnimator)
        {
            mAnimator.SetBool("Running", true);
			mAnimator.SetBool("Jumping", false);
			mAnimator.SetBool("Attacking", false);
        }
    }

	public void Attack()
	{
		if (mAnimator)
		{
			mAnimator.SetBool("Jumping", false);
			mAnimator.SetBool("Running", false);
			mAnimator.SetBool("Attacking", true);
		}
	}

	public void Jumping()
	{
		if (mAnimator)
		{
			mAnimator.SetBool("Running", false);
			mAnimator.SetBool("Attacking", false);
			mAnimator.SetBool("Jumping", true);
			//mAnimator.SetBool("Running", true);
		}
	}

	public void Landing()
	{
		if (mAnimator)
		{
			mAnimator.SetBool("Jumping", false);
			//mAnimator.SetBool("Running", true);
		}
	}

    public void Idle()
    {
        if (mAnimator)
        {
            mAnimator.SetBool("Running", false);
			mAnimator.SetBool("Jumping", false);
			mAnimator.SetBool("Attacking", false);
        }
    }

    IEnumerator FadeCoroutine(float Start, float End, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            foreach (Material mat in mMaterial)
            {
                Color start = new Color(mat.color.r, mat.color.g, mat.color.b, Start);
                Color end = new Color(mat.color.r, mat.color.g, mat.color.b, End);
                mat.color = Color.Lerp(start, end, elapsedTime / time);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void TakeDamage()
    {
        if (mAnimator)
            mAnimator.SetTrigger("DamageTaken");
        foreach (Material mat in mMaterial)
        {
            mat.color = ColorDamage;
        }

        Invoke("NormalColor", 0.2f);
    }

    void NormalColor()
    {
        for (int i = 0; i < OriginalColor.Count; i++)
        {
            mMaterial[i].color = OriginalColor[i];
        }
    }
}
