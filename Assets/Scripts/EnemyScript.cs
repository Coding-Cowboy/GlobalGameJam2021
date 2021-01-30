using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    Rigidbody body;

    public GameObject player;
    public float chaseSpeed = 2.2f;
    public float detectionRange = 15.0f;
    public bool keepChasing = false;
    private bool chasing = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 distanceToPlayer = player.transform.position - transform.position;

        // Check if player is in range
        if (distanceToPlayer.magnitude <= detectionRange)
        {
            RaycastHit lineOfSightHit;
            if (Physics.Raycast(transform.position, distanceToPlayer.normalized, out lineOfSightHit, detectionRange))
            {
                if (lineOfSightHit.collider.gameObject.CompareTag("Player"))
                    chasing = true;
                else if (!keepChasing)
                    chasing = false;
            }
        }
        else if (!keepChasing)
            chasing = false;

        // Chase the player
        if (chasing)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, distanceToPlayer.normalized, 0.035f, 1f), Vector3.up);

            Vector3 moveVelocity = new Vector3(distanceToPlayer.normalized.x * chaseSpeed, 0, distanceToPlayer.normalized.z * chaseSpeed);
            if (moveVelocity.magnitude > chaseSpeed)
                moveVelocity = moveVelocity.normalized * chaseSpeed;
            moveVelocity.y = body.velocity.y;
            if (body.velocity.magnitude > chaseSpeed)
                moveVelocity = body.velocity;

            transform.position += moveVelocity * Time.deltaTime;
        }
    }
}
