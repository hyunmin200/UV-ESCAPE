using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform PlayerPos;

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerPos.position;
    }
}
