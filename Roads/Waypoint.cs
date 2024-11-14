using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] int _wayPointIndex;
    public int WayPointIndex
    { get { return _wayPointIndex; } set { _wayPointIndex = value; } }

    public List<Transform> _possibleNextWaypoints;

    public Transform interSection;

    public float angleToNextPoint;

    public bool isBridge;

    [SerializeField] LayerMask _wayPointMask;
    [SerializeField] float _wayPointRadius;

    [SerializeField] List<Transform> validOptions = new();

    private void Start()
    {
        if (WayPointIndex == 0 && _possibleNextWaypoints.Count == 0)
        {
            Intersection();
        }
        else if (_possibleNextWaypoints.Count == 0)
        {
            _possibleNextWaypoints = GetNextWayPoints();
            if (_possibleNextWaypoints.Count != 0)
            {
                angleToNextPoint = Vector3.Angle(transform.position, _possibleNextWaypoints[0].position);
            }
        }
        else
        {
            Vector3 targetDir = _possibleNextWaypoints[0].position - transform.position;

            angleToNextPoint = Vector3.Angle(targetDir, transform.forward);
        }

    }

    /// <summary>
    /// Handles the logic for teh intersection
    /// </summary>
    void Intersection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _wayPointRadius, _wayPointMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Waypoint waypoint = colliders[i].GetComponent<Waypoint>();
            if (waypoint != this && waypoint._possibleNextWaypoints.Count == 0)
            {
                waypoint._wayPointIndex = 3;
            }
            else
            {
                if (waypoint != this)
                {
                    validOptions.Add(waypoint.transform);
                }
            }

        }

        for (int i = 0; i < colliders.Length; i++)
        {
            Waypoint waypoint = colliders[i].GetComponent<Waypoint>();
            if (waypoint.WayPointIndex != 3)
            {
                continue;
            }

            float closestRange = Mathf.Infinity;
            Transform closestTransform = null;

            for (int w = 0; w < validOptions.Count; w++)
            {
                if (closestTransform == null)
                {
                    closestTransform = validOptions[w].transform;
                    closestRange = Vector3.Distance(colliders[i].transform.position, closestTransform.position);
                }
                else if (Vector3.Distance(validOptions[w].transform.position, colliders[i].transform.position) < closestRange)
                {
                    closestRange = Vector3.Distance(validOptions[w].transform.position, colliders[i].transform.position);
                    closestTransform = validOptions[w].transform;
                }
            }
            if (closestTransform != null && validOptions.Contains(closestTransform))
            {
                for (int w = 0; w < validOptions.Count; w++)
                {
                    waypoint._possibleNextWaypoints.Add(validOptions[w]);
                }
                waypoint._possibleNextWaypoints.Remove(closestTransform);
            }
            else
            {
                continue;
            }
        }
    }

    /// <summary>
    /// Returns the next waypoint from a list of possible waypoints in range
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetNextWayPoints()
    {
        List<Transform> transforms = new();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _wayPointRadius, _wayPointMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].transform != this.transform)
            {
                transforms.Add(colliders[i].transform);
            }
        }
        return transforms;
    }

    /// <summary>
    /// Returns a list of waypoint connections from this waypoint using the wayPointIndex and excludedTransform
    /// </summary>
    /// <param name="wayPointIndex"></param>
    /// <param name="excludedTransform"></param>
    /// <returns></returns>
    public List<Transform> GetWayPointConnections(int wayPointIndex, Transform excludedTransform = null)
    {
        List<Transform> transforms = new();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _wayPointRadius, _wayPointMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Waypoint>()._wayPointIndex == wayPointIndex && colliders[i].transform != this.transform)
            {
                if (excludedTransform != null)
                {
                    if (colliders[i].transform == excludedTransform)
                    {
                        continue;
                    }
                }

                transforms.Add(colliders[i].transform);

            }
        }
        return transforms;
    }

    private void OnDrawGizmos()
    {
        if (_wayPointIndex == 1)
        {
            Gizmos.color = Color.green;
        }
        else if (_wayPointIndex == 2)
        {
            Gizmos.color = Color.red;
        }
        else if (_wayPointIndex == 3)
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawWireSphere(transform.position, _wayPointRadius);
    }
}
