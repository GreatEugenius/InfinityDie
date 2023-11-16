using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLinkWhite : MonoBehaviour
{
    public GameObject White;
    Vector3 distance;
    // Start is called before the first frame update
    void Start()
    {
        distance = White.GetComponent<Transform>().position - gameObject.GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<Transform>().position = White.GetComponent<Transform>().position - distance;
    }
}
