using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplodeOnCollide : MonoBehaviour {

    [Range(0.1f, 10f)]
    public float Radius = 1f;
    public LayerMask Masks;

    public int Damage;

    public float InitialSpeed = 1f;
    public GameObject Particles;

    [Header("Search Options")]
    public bool lookInParents = false;
    public bool lookInChildren = true;
    public bool lookFromRoot = true;

    private Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * InitialSpeed, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, Masks);
        int i = 0;
        if (Particles)
        {
            var particleGO = Instantiate(Particles, transform.position, Quaternion.identity);
            particleGO.transform.localScale *= Radius;
            for (var j = 0; j < particleGO.transform.childCount; ++j)
            {
                particleGO.transform.GetChild(j).localScale *= Radius;
            }
        }
        while (i < hitColliders.Length)
        {
            List<Health> damaged = new List<Health>();
            var hitGameObject = hitColliders[i].gameObject;
            if (lookFromRoot)
                hitGameObject = hitColliders[i].transform.root.gameObject;
            var healthComps = hitGameObject.GetComponents<Health>();
            if (healthComps != null)
            {
                foreach (var healthComp in healthComps)
                {
                    healthComp.Damage(Damage);
                    damaged.Add(healthComp);
                }
            }
            if (lookInParents)
            {
                healthComps = hitGameObject.GetComponentsInParent<Health>();
                if (healthComps != null)
                {
                    foreach (var healthComp in healthComps)
                    {
                        if (!damaged.Contains(healthComp))
                        {
                            healthComp.Damage(Damage);
                            damaged.Add(healthComp);
                        }
                    }
                }
            }
            if (lookInParents)
            {
                healthComps = hitGameObject.GetComponentsInChildren<Health>();
                if (healthComps != null)
                {
                    foreach (var healthComp in healthComps)
                    {
                        if (!damaged.Contains(healthComp))
                        {
                            healthComp.Damage(Damage);
                            damaged.Add(healthComp);
                        }
                    }
                }
            }
            damaged.Clear();
            ++i;
        }
    }
}
