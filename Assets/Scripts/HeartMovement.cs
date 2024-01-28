using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartMovement : MonoBehaviour
{
    private Transform heartTarget;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        heartTarget = GameObject.Find("HeartTarget").transform;
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, heartTarget.position, ref velocity, smoothTime);

        if (Vector3.Distance(transform.position, heartTarget.position) <= 0.5f)
        {
            Scorer.Instance.AddMoney();
            Destroy(gameObject);
        }
    }
}