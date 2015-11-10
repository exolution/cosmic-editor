using UnityEngine;
using System.Collections;
using System.Collections.Generic;
class MotionAgent : Action
{
    public NavMeshAgent navMeshAgent;
    public NavMeshObstacle navMeshObstacle;
    GameObject subject;
    internal Vector3 targetPoint;
    internal GameObject targetUnit;
    float followDistance;
    int count = 0;
    float lastRemain = 0;
    public bool isStop = true;
    public MotionAgent(GameObject subject, Vector3 targetPoint)
    {
        this.subject = subject;
        navMeshAgent = subject.GetComponent<NavMeshAgent>();
        navMeshObstacle = subject.GetComponent<NavMeshObstacle>();
        this.targetPoint = targetPoint;
    }
    public MotionAgent(GameObject subject, GameObject targetUnit, float followDistance)
    {
        this.subject = subject;
        navMeshAgent = subject.GetComponent<NavMeshAgent>();
        navMeshObstacle = subject.GetComponent<NavMeshObstacle>();
        this.targetUnit = targetUnit;
        this.targetPoint = targetUnit.transform.position;
        this.followDistance = followDistance;
    }

    public void abort()
    {

    }

    public void finish()
    {

    }

    public bool isDone()
    {

        if (!navMeshAgent.enabled)
        {
            return true;
        }
        if (navMeshAgent.remainingDistance == 0)
        {
            if (!isStop)
            {
                isStop = true;
                navMeshAgent.enabled = false;
                navMeshObstacle.enabled = true;

            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void pause()
    {

    }

    public void resumue()
    {

    }

    public void start()
    {

    }
}
public class MotionController : MonoBehaviour
{
    const float MIN_DISTANCE = 1F;
    Dictionary<int, MotionAgent> map = new Dictionary<int, MotionAgent>();

    

    List<MotionAgent> agentList = new List<MotionAgent>();
    public static MotionController Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("MotionController must be added to just only one gameobject");

        }
    }
    void LateUpdate()
    {
        for(int i = 0; i < agentList.Count; i++)
        {
            MotionAgent agent = agentList[i];
            if(agent.targetUnit!=null&&Vector3.SqrMagnitude(agent.targetPoint-agent.targetUnit.transform.position)> MIN_DISTANCE)
            {
                agent.targetPoint = agent.targetUnit.transform.position;
                agent.navMeshAgent.SetDestination(agent.targetPoint);
            }
        }
    }

    public static IEnumerator moveToPoint(GameObject subject, Vector3 targetPoint)
    {
        MotionAgent motionAgent;
        if (Instance.map.ContainsKey(subject.GetHashCode()))
        {
           motionAgent = Instance.map[subject.GetHashCode()];
            if (motionAgent.navMeshObstacle.enabled)
            {
                motionAgent.navMeshObstacle.enabled = false;
                yield return null;
                yield return null;
            }
            if (!motionAgent.navMeshAgent.enabled)
            {
                motionAgent.navMeshAgent.enabled = true;
            }
        }
        else
        {
            motionAgent = new MotionAgent(subject, targetPoint);
            motionAgent.navMeshObstacle.enabled = false;
           
           
            yield return null;
           
            yield return null;
           
            motionAgent.navMeshAgent.enabled = true;
            Instance.map.Add(subject.GetHashCode(), motionAgent);
        }
        
        //Instance.agentList.Add(motionAgent);
       

       
        motionAgent.isStop = false;
        motionAgent.navMeshAgent.SetDestination(targetPoint);
        yield return null;
        Debug.Log(motionAgent.navMeshAgent.remainingDistance);
        yield return motionAgent;
    }
    public static IEnumerator followUnit(GameObject subject, GameObject target,float distance=0)
    {
        MotionAgent motionAgent = new MotionAgent(subject, target, distance);
        Instance.agentList.Add(motionAgent);
        motionAgent.navMeshObstacle.enabled = false;
        yield return null;
        motionAgent.navMeshAgent.enabled = true;
        motionAgent.navMeshAgent.SetDestination(target.transform.position);
        yield return motionAgent;
    }
    
}