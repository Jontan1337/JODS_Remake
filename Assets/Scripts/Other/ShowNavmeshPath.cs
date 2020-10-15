using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class ShowNavmeshPath : MonoBehaviour
{

    private LineRenderer line;
    private NavMeshAgent agent;
    public string status;
    public bool hasPath;
    public bool calculatePath;
    public Vector3 destination;
    public bool raycast;
    public NavMeshHit hit;

    public bool showEdge;
    [Range(0f,1f)] public float thicc = 0.3f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        line = GetComponent<LineRenderer>();
        line.numCapVertices = 5;
        line.widthCurve = AnimationCurve.Linear(0, thicc, 1, thicc);
    }
    private void Update()
    {
        hasPath = agent.hasPath;
        status = agent.pathStatus.ToString();
        calculatePath = agent.CalculatePath(agent.destination, agent.path);
        destination = agent.destination;
        raycast = NavMesh.Raycast(transform.position, agent.destination, out hit, 1);
        if (agent.hasPath)
        {
            line.SetPosition(0, transform.position);
            DrawPath(agent.path);
        }
    }
    private void DrawPath(NavMeshPath path)
    {
        if (showEdge)
        {
            NavMeshHit hit;
            if (NavMesh.FindClosestEdge(new Vector3(agent.destination.x, agent.destination.y - 1, agent.destination.z), out hit, NavMesh.AllAreas))
            {
                line.SetPosition(1, hit.position);
            }
        }
        else
        {
        if (path.corners.Length < 2)
            return;

        line.SetPositions(path.corners);

        line.positionCount = path.corners.Length;
        for (var i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }
        }
    }
}
