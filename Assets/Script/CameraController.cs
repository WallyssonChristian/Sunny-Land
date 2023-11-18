using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform fox;
    // public Rigidbody2d player;

    void Start()
    {
        fox = GameObject.Find("Player").GetComponent(typeof(Transform)) as Transform;
        // Obtendo variavel de outro script
        // float foxVelocity = GetComponent<PlayerController>().jumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(fox.position.x, fox.position.y, transform.position.z);
    }
}
