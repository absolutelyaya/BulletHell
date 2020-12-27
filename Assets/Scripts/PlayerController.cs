using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Current;

    public int Health;
    public float BaseSpeed;
    public float FocusSpeedMultiplier;
    public float FireRate;
    public Transform FireLocation;
    public GameObject Bullet;

    Rigidbody2D rb;
    float speed;
    float nextShotTime;

    void Start()
    {
        Current = this;
        rb = GetComponent<Rigidbody2D>();
        speed = BaseSpeed;
    }

    void Update()
    {
        if(InputManager.GetKey("Shoot") && Time.time > nextShotTime)
        {
            PoolManager.current.Activate(Bullet, FireLocation.position);
            nextShotTime = Time.time + FireRate;
        }

        if(InputManager.GetKeyDown("Focus"))
        {
            speed *= FocusSpeedMultiplier;
        }
        if(InputManager.GetKeyUp("Focus"))
        {
            speed /= FocusSpeedMultiplier;
        }
    }

    private void FixedUpdate()
    {
        var direction = new Vector2(InputManager.GetAxisRaw("MoveRight"), InputManager.GetAxisRaw("MoveUp"));
        rb.MovePosition((Vector2)transform.position + direction * speed * Time.deltaTime);
        //transform.rotation = Quaternion.Euler(new Vector3(0, InputManager.GetAxisRaw("MoveRight") * 30, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            Health--;
            if(Health <= 0) SceneManager.LoadScene("SampleScene");
            PoolManager.current.Deactivate(collision.gameObject);
        }
    }
}
