using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement.Normalize();

        anim.SetFloat("MoveX", movement.x);
        anim.SetFloat("MoveY", movement.y);
        anim.SetBool("IsMoving", movement != Vector2.zero);

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void UpdateAnimation()
    {
        if (movement == Vector2.zero)
            return;

        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            if (movement.x > 0)
                anim.Play("WalkRight");
            else
                anim.Play("WalkLeft");
        }
        else
        {
            if (movement.y > 0)
                anim.Play("WalkUp");
            else
                anim.Play("WalkDown");
        }
    }
}