using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * 0.5f * Time.deltaTime;*/
    }

    /*public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.name == "Sphere") {
            Debug.Log("collided with a sphere");
        }

        if(collision.gameObject.name == "Cube") {
            Debug.Log("collided with a cube");
        }
    }*/

    public void OnTriggerEnter(Collider other) {
        Debug.Log("TRIGGERED BY: " + other.gameObject.name);
        Debug.Log("TRIGGERED BY: " + other.gameObject.tag);

        Debug.Log("PARENT: " + other.gameObject.transform.parent.name);
        Debug.Log("ROOT: " + other.gameObject.transform.root.gameObject.name);

        gameObject.SetActive(false);
    }
}
