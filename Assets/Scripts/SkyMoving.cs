using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SkyMoving : MonoBehaviour {

    SpriteRenderer rdr;

    protected Vector2 uvOffset = Vector2.zero;
    public Vector2 AnimateRate = new Vector3(.05f, 0);
    public Vector2 LeftRightLimit = new Vector3(-.05f, .05f);
    public Vector2 UpDownLimit = new Vector2(-.05f, .05f);
    public float UpDownRate = .0001f;
    public float LeftRightRate = .0001f;
    public string textureName = "_MainTex";

    private int UpDownSign = 1;
    private float randomUpDown = 0f;
    private float randomLeftRight = 0f;
    // Use this for initialization
    void Start () {
        rdr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
    private float currentOffset = 0f;
	void Update () {
        Material textureToAnimate = rdr.material;
        if (textureToAnimate != null)
        {
            uvOffset += AnimateRate * Time.deltaTime;

            randomUpDown += UpDownRate * UpDownSign;
            if (randomUpDown < UpDownLimit.x || randomUpDown > UpDownLimit.y)
                UpDownSign *= -1;

            if (uvOffset.x >= 1.0f)
            {
                uvOffset.x = 0.0f;
            }

            if (uvOffset.y >= 1.0f)
            {
                uvOffset.y = 0.0f;
            }
            textureToAnimate.mainTextureOffset = uvOffset + new Vector2(0, randomUpDown);
        }
    }
}
