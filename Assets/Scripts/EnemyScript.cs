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

        // Find spawned player
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        Vector3 distanceToPlayer = player.transform.position - transform.position;

        // Check if player is in range
        if (distanceToPlayer.magnitude <= detectionRange)
        {
            RaycastHit lineOfSightHit;
            Debug.DrawRay(transform.position + Vector3.up * 0.4f, distanceToPlayer.normalized, new Color(1, 0, 0), detectionRange); // debug ray
            if (Physics.Raycast(transform.position + Vector3.up * 0.4f, distanceToPlayer.normalized, out lineOfSightHit, detectionRange)) // apply offset so ray doesn't cast from inside floor
            {
                Debug.Log("Something hit! tag="+lineOfSightHit.collider.gameObject.tag);
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
