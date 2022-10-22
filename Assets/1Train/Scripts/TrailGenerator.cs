using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//주인공 움직이기, Trail관리 

public class TrailGenerator : MonoBehaviour
{
    NavMeshAgent nav;
    RecommendAgent agent;
    void OnEnable()
    {
        nav = GetComponent<NavMeshAgent>();
        agent = transform.parent.GetComponent<RecommendAgent>();
        trail = traces.GetComponent<TrailEnergyDecrease>();
    }

    public void goTo(int id)
    {
        if (id < 0)
        {
            if (!agent.warp)
                nav.SetDestination(new Vector3(0, 0, 0));
        }
        else
        {
            Vector3 pos = agent.flagpos[id] + agent.transform.position;
            makeTrace(5);
            if (!agent.warp)
                nav.SetDestination(pos);
            else
            {
                warpTo(pos);
            }
        }
        agent.updateFlags(id);
    }
    private Vector3 makeNoisePoint(Vector3 prevPos, Vector3 nextPos)
    {
        while (true)
        {
            float length = Vector3.Magnitude(nextPos - prevPos);
            float ratio = Random.Range(0.2f, 0.8f);
            Vector2 noisePoint = Random.insideUnitCircle.normalized * ratio * length / 2;
            Vector3 noisePointV3 = new Vector3(noisePoint.x, 0, noisePoint.y);

            Vector3 returnPoint = Vector3.Lerp(prevPos, nextPos, 0.5f) + noisePointV3;
            if (returnPoint.x > agent.worldSize || returnPoint.x < -agent.worldSize
            || returnPoint.z < -agent.worldSize || returnPoint.z > agent.worldSize)
            {
                continue;
            }

            return returnPoint;
        }


    }
    public void warpTo(Vector3 pos)
    {
        NavMeshPath path = new NavMeshPath();
        nav.CalculatePath(pos, path);

        Vector3 prevPos = transform.position;

        List<Vector3> corners = new List<Vector3>(path.corners);
        // for (int i = 1; i < corners.Count; i += 2)
        // {
        //     corners.Insert(i, makeNoisePoint(corners[i - 1], corners[i]));
        // }
        foreach (Vector3 corner in corners)
        {
            traceLine(prevPos, corner);
            prevPos = corner;
        }
        traceLine(prevPos, pos);
        transform.position = pos;
    }

    public void randomWarp(int n)
    {
        if (!agent.warp)
        {
            Debug.Log("cannot warp :(");
            return;
        }

        for (int i = 0; i < n; i++)
        {
            goTo(Random.Range(0, agent.flagCount));
        }
    }

    void traceLine(Vector3 src, Vector3 dst)
    {
        makeTrace(src);
        float dist = Vector3.Distance(src, dst);
        if (dist < traceSpacing) return;
        for (int i = 1; i <= dist / traceSpacing; i++)
        {
            makeTrace(Vector3.Lerp(src, dst, traceSpacing * i / dist));
        }
    }

    public GameObject tracePrefab;
    public Transform traces;
    TrailEnergyDecrease trail;
    public float traceSpacing;

    Vector3 lastpos;

    public void makeTrace(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(tracePrefab, new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity, traces);
            trail.trailEvap();
        }
    }

    public void makeTrace(Vector3 pos)
    {
        if (agent.warp) trail.trailEvap();
        Instantiate(tracePrefab, pos, Quaternion.identity, traces);

    }

    private void FixedUpdate()
    {
        if (!agent.warp && (lastpos == null || Vector3.SqrMagnitude(lastpos - transform.position) > traceSpacing))
        {
            makeTrace(1);
            lastpos = transform.position;
        }
    }


}



// if (queueFilled > 0)
//     agent.AddReward(Vector3.SqrMagnitude(waypoints.Peek() - transform.position) / 10000);
// agent.AddReward(agent.energy / 10);
