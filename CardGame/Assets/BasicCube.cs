using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class BasicCube : BasicCubeBehavior
{

    public float speed = 5;

    // Update is called once per frame
    void Update()
    {
        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
            return;
        }

        transform.position += new Vector3(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"), 0.0f) 
                              * speed * Time.deltaTime;

        networkObject.position = transform.position;
        networkObject.rotation = transform.rotation;
    }
}
