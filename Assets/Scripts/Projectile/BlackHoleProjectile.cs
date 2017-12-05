using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlackHoleProjectile : MonoBehaviour {

    public float Radius = 50f;
    public LayerMask Masks;

    public int Damage = 300;

    public float InitialSpeed = 2f;
    public float TimeBeforeImploding = 2f;
    public float TimeImploding = 5f;
    public float MinDistToTakeDamage = 0.5f;
    public float MaxForceAttraction = 100f;
    [SerializeField] GameObject Particles;

    [Header("Search Options")]
    public bool lookInParents = false;
    public bool lookInChildren = true;
    public bool lookFromRoot = true;

    private Rigidbody rb;

    bool isImploding = false;

    List<GameObject> ObjectToApplyForce = new List<GameObject>();

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * InitialSpeed, ForceMode.VelocityChange);
        Invoke("Implode", TimeBeforeImploding);
    }

    private void FixedUpdate()
    {
        if (isImploding)
            DoImplode();
    }

    public void Implode()
    {
        if (isImploding)
            return;
        else
        {
            isImploding = true;
        }

        rb.isKinematic = true;
        Particles.SetActive(true);
        Destroy(gameObject, TimeImploding);
    }

    private void OnDestroy()
    {
        foreach (GameObject obj in ObjectToApplyForce)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = Vector3.zero;
        }
    }

    void DoImplode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius, Masks);
        foreach (Collider hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.attachedRigidbody;
            if (rb != null)
            {
                if (!ObjectToApplyForce.Contains(rb.gameObject))
                    ObjectToApplyForce.Add(rb.gameObject);
            }
        }

        List<Health> damaged = new List<Health>();
        foreach (GameObject obj in ObjectToApplyForce)
        {
            Vector2 dir = transform.position - obj.transform.position;
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (dir.sqrMagnitude > MinDistToTakeDamage)
            {
                rb.AddForce(dir.normalized * MaxForceAttraction / dir.sqrMagnitude, ForceMode.Impulse);
            }
            else
            {
                var hitGameObject = obj.gameObject;
                if (lookFromRoot)
                    hitGameObject = obj.transform.root.gameObject;
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
            }
        }
        damaged.Clear();
    }
}
