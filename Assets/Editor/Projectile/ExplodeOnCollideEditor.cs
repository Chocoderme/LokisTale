using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ExplodeOnCollide))]
public class ExplodeOnCollideEditor : UnityEditor.Editor {

    public void OnSceneGUI()
    {
        ExplodeOnCollide selection = (ExplodeOnCollide)target;

        Handles.color = Color.red;
        Handles.DrawWireDisc(selection.transform.position, -Vector3.forward, selection.Radius);
        //Gizmos.DrawWireSphere(selection.gameObject.transform.position, selection.Radius);
    }
}
