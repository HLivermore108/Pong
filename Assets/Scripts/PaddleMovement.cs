using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    public float speed = 5.0f;

    void Start()
    {
        
    }

    void Update()
    {

        float vertical = Input.GetAxis("Vertical"); // Gets input. If holding W returns a 1, if holding S returns -1, no input returns 0.

        transform.position += new Vector3(0, vertical * speed * Time.deltaTime, 0); //The 0's are for the X and Z which stays the same.
                                                                                    //The vert speed time.deltatime is for the Y. Vertical is the input from float vertical = Input... and uses that to move up and down.
    }
}
