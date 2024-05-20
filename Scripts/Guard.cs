using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Guard : MonoBehaviour
{
    public static event System.Action OnGuardSpottedPlayer;

    public float speed = 5.0f;
    public float waitTime = 1.0f;
    public float turnSpeed = 90;
    public float detectionTime = 0.5f;

    public Light spotLight;
    float viewAngle;
    public float viewDist; 
    public LayerMask viewMask;

    float playerVisibleTimer;

    public Transform pathHolder;

    Transform player;

    Color OriginalSpotlightColour;

    void Start()
    {
        OriginalSpotlightColour = spotLight.color;

        viewAngle = spotLight.spotAngle;
        player = GameObject.FindGameObjectWithTag("Player").transform; 

        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, detectionTime);
        spotLight.color = Color.Lerp(OriginalSpotlightColour, Color.red, playerVisibleTimer/detectionTime);

        if (playerVisibleTimer >= detectionTime)
        {
            if (OnGuardSpottedPlayer != null)
            {
                OnGuardSpottedPlayer();
            }
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDist) 
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized; 
            float angleBetween = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetween < viewAngle / 2f) 
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];

                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirLookTarget.z, dirLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).transform.position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDist);
    }
}
