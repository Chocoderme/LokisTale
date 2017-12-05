using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LowRangeProjectile : MonoBehaviour {

    public float Radius = 1f;
    public LayerMask Masks;

    public int Damage;

    public float InitialSpeed = 1f;
    public float TimeToStop = 0.5f;
    public int SelfDamage = 5;
    private ParticleSystem[] Particles;

    [Header("Search Options")]
    public bool lookInParents = false;
    public bool lookInChildren = true;
    public bool lookFromRoot = true;

    private Rigidbody rb;
    private Health life;
    bool StartSlow = false;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        life = GetComponent<Health>();
        Particles = GetComponentsInChildren<ParticleSystem>();

        rb.AddForce(transform.forward * InitialSpeed, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        if (!StartSlow)
        {
            StartCoroutine(SlowToStop());
            StartSlow = true;
        }
    }

    IEnumerator SlowToStop()
    {
        yield return new WaitForFixedUpdate();
        StartCoroutine(Slow(rb.velocity, Vector2.zero, TimeToStop));
    }

    IEnumerator Slow(Vector2 Start, Vector2 End, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            //Debug.Log("Elapsed time : "+elapsedTime+" Time : " + time);
            rb.velocity = Vector2.Lerp(Start, End, elapsedTime / time);
            foreach (ParticleSystem part in Particles)
            {
                var main = part.main;
                main.simulationSpeed = rb.velocity.magnitude * rb.velocity.magnitude + 0.1f;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider hitCollider = collision.collider;
        List<Health> damaged = new List<Health>();
        var hitGameObject = hitCollider.gameObject;
        if (lookFromRoot)
            hitGameObject = hitCollider.transform.root.gameObject;
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

        life.Damage(SelfDamage);
    }
}
