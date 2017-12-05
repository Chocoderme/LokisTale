using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour {

    public Action<int, HealthInfo> OnDamaged;
    public Action OnDie;

    [SerializeField]
    private HealthInfo Infos;
    private EntityAnimation entityAnimation;

    public UnityEngine.UI.Slider HealthUI;

    void Start()
    {
        Infos.currentHealth = Infos.StartHealth;
        if (HealthUI)
        {
            HealthUI.maxValue = Infos.MaxHealth;
            HealthUI.minValue = 0;
        }

        entityAnimation = gameObject.GetComponent<EntityAnimation>();
    }

    private void Update()
    {
        if (HealthUI)
            HealthUI.value = Infos.currentHealth;
    }

    public void Kill()
    {
        if (entityAnimation)
        {
            entityAnimation.FadeOut();
            Destroy(gameObject, 0.6f);
        }
        else
        {
            Destroy(gameObject);
        }
        if (OnDie != null)
            OnDie();
    }

    public void Damage(int damageCount)
    {
        if (entityAnimation)
        {
            entityAnimation.TakeDamage();
        }

        Infos.currentHealth -= damageCount;
        if (Infos.currentHealth <= 0)
            Kill();
        if (OnDamaged != null)
            OnDamaged(damageCount, Infos);
    }

	public HealthInfo GetHealthInfos() {
		return Infos;
	}

    [Serializable]
    public struct HealthInfo
    {
        public int MaxHealth;
        public int StartHealth;
        
        public int currentHealth;
    }
}
