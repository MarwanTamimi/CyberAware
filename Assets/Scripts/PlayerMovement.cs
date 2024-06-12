using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
[SerializeField] float speed;
[SerializeField] private new Rigidbody2D rigidbody2D;
[SerializeField] private TrailRenderer tr;
[SerializeField] private Transform groundCheck;
[SerializeField] private LayerMask groundLayer;

//player movement 
private float vertical, horizontal;
public float jumpingPower;
private bool isFacingRight = true;

//player dash
private bool canDash = true;
private bool isDashing;
private float dashingPower = 40f;
private float dashingTime = 0.2f;
private float dashingCooldown = 1f;

void Start()
{
    rigidbody2D = GetComponent<Rigidbody2D>();
}

void Update()
{

    if (isDashing)
    {
        return;
    }

    horizontal = Input.GetAxisRaw("Horizontal");

    if (Input.GetButtonDown("Jump") && IsGrounded())
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpingPower);
    }
    if (Input.GetButtonUp("Jump") && rigidbody2D.velocity.y > 0f)
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y * 0.5f);
    }
        //MoveUser();

        Flip();
    if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
    { 
        StartCoroutine(Dash());
    }
}

private void FixedUpdate()
{
    if (isDashing)
    {
        return;
    }
    rigidbody2D.velocity = new Vector2(horizontal * speed, rigidbody2D.velocity.y);
}


private bool IsGrounded()
{
    return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
}

    private void Flip()
{
    if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

}

//void MoveUser()
//{
//    horizontal = Input.GetAxis("Horizontal");
//    vertical = Input.GetAxis("Vertical");
//    rigidbody2D.velocity = new Vector2(horizontal * speed, vertical * speed);
//}


private IEnumerator Dash()
{
    canDash = false;
    isDashing = true;
    float originalGravity = rigidbody2D.gravityScale;
    rigidbody2D.gravityScale = 0f;
    rigidbody2D.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
    tr.emitting = true;
    yield return new WaitForSeconds(dashingTime);
    tr.emitting = false;
    rigidbody2D.gravityScale = originalGravity;
    isDashing = false;
    yield return new WaitForSeconds(dashingCooldown);
    canDash = true;

}
}
