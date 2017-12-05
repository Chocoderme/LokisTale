using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PreventVoid : MonoBehaviour {

    [Serializable]
    public enum VoidAction
    {
        Teleport,
        Kill,
        DamageAndTeleport,
        DoNothing
    }

    [Serializable]
    public struct VoidLogic
    {
        public string Tag;
        public VoidAction Action;
    }

    public Transform TeleportPosition;
    public int DamageCount;
    public VoidLogic[] Logics;
    public VoidAction DefaultAction = VoidAction.Kill;

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject.transform.root.gameObject;
        bool found = false;
        foreach (var logic in Logics)
        {
            if (logic.Tag == obj.tag)
            {
                ExecuteAction(logic.Action, obj);
                found = true;
                break;
            }
        }
        if (!found)
            ExecuteAction(DefaultAction, obj);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject.transform.root.gameObject;
        bool found = false;
        foreach (var logic in Logics)
        {
            if (logic.Tag == obj.tag)
            {
                ExecuteAction(logic.Action, obj);
                found = true;
                break;
            }
        }
        if (!found)
            ExecuteAction(DefaultAction, obj);
    }

    private void ExecuteAction(VoidAction action, GameObject obj)
    {
        switch (action)
        {
            case VoidAction.Kill:
                KillTarget(obj);
                break;
            case VoidAction.Teleport:
                TeleportTarget(obj);
                break;
            case VoidAction.DamageAndTeleport:
                DamageAndTeleport(obj);
                break;
        }
    }

    private void KillTarget(GameObject obj)
    {
        Health hp = obj.GetComponent<Health>();
        if (hp)
            hp.Kill();
        else
            Destroy(obj);
    }

    private void TeleportTarget(GameObject obj)
    {
        obj.transform.position = TeleportPosition.position;
    }

    private void DamageAndTeleport(GameObject obj)
    {
        Health hp = obj.GetComponent<Health>();
        if (hp)
            hp.Damage(DamageCount);
        TeleportTarget(obj);
    }
}
