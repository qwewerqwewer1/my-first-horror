using UnityEngine;

public class Player1 : MonoBehaviour {
    public float speed = 5f;
    public float strength = 5f;
    private Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {   
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.forward * (speed * Time.deltaTime), Space.World);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.back * (speed * Time.deltaTime), Space.World);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * (speed * Time.deltaTime), Space.World);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * (speed * Time.deltaTime), Space.World);
        
        if (Input.GetKeyDown(KeyCode.Space)) _rb.AddForce(Vector3.up * strength);
    }
}
