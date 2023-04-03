using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public Vector2 startPosition;
    public Vector2 endPosition;
    public float orientation;
    public float length;


    public void SetUpBranch(float orientation, float length)
    {
        this.transform.position = new Vector3(startPosition.x, startPosition.y, 0f);
        this.transform.Rotate(Vector3.forward * orientation);
        this.transform.Translate(Vector3.up * length);

    }


}