using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Shoot))]
public class ShootEditor : Editor {

    private void OnSceneGUI()
    {
        Shoot selection = (Shoot)target;

        if (selection.Bullets.Length > 0)
        {
            Shoot.BulletInfos bullet = selection.Bullets[selection.showBulletId % selection.Bullets.Length];
            if (bullet.Projectile != null && selection.showGUI)
            {
                for (int i = 0; i < selection.EditorGUIPrecision; i++)
                {
                    Vector3 p1 = new Vector3(i, selection.EditorGUIPrecision - i);
                    Vector3 p2 = new Vector3(selection.EditorGUIPrecision - i, -i);
                    Vector3 p3 = new Vector3(-i, -selection.EditorGUIPrecision + i);
                    Vector3 p4 = new Vector3(-selection.EditorGUIPrecision + i, i);

                    p1.Normalize();
                    p2.Normalize();
                    p3.Normalize();
                    p4.Normalize();

                    p1.Scale(bullet.SpawnScale);
                    p2.Scale(bullet.SpawnScale);
                    p3.Scale(bullet.SpawnScale);
                    p4.Scale(bullet.SpawnScale);

                    Matrix4x4 m1 = Matrix4x4.TRS(selection.transform.position + p1 + bullet.SpawnOffset, Quaternion.identity, bullet.Projectile.transform.localScale);
                    Matrix4x4 m2 = Matrix4x4.TRS(selection.transform.position + p2 + bullet.SpawnOffset, Quaternion.identity, bullet.Projectile.transform.localScale);
                    Matrix4x4 m3 = Matrix4x4.TRS(selection.transform.position + p3 + bullet.SpawnOffset, Quaternion.identity, bullet.Projectile.transform.localScale);
                    Matrix4x4 m4 = Matrix4x4.TRS(selection.transform.position + p4 + bullet.SpawnOffset, Quaternion.identity, bullet.Projectile.transform.localScale);

                    Graphics.DrawMesh(bullet.Projectile.GetComponentInChildren<MeshFilter>().sharedMesh, m1, bullet.Projectile.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0);
                    Graphics.DrawMesh(bullet.Projectile.GetComponentInChildren<MeshFilter>().sharedMesh, m2, bullet.Projectile.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0);
                    Graphics.DrawMesh(bullet.Projectile.GetComponentInChildren<MeshFilter>().sharedMesh, m3, bullet.Projectile.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0);
                    Graphics.DrawMesh(bullet.Projectile.GetComponentInChildren<MeshFilter>().sharedMesh, m4, bullet.Projectile.GetComponentInChildren<MeshRenderer>().sharedMaterial, 0);
                }
            }
        }
    }
}
