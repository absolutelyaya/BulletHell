using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Current;

    public float BaseSpeed;
    public float FocusSpeedMultiplier;

    Rigidbody2D rb;
    float speed;

    void Start()
    {
        Current = this;
        rb = GetComponent<Rigidbody2D>();
        speed = BaseSpeed;
    }

    void Update()
    {
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
}
