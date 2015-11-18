using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class MainActorController : MonoBehaviour
{
    CharacterController m_characterCtrl;
    Animator m_animator;
    Vector3 m_targetPoint;
    
    //public WeaponTrail myTrail;
    public GameObject targetCircle;
    bool m_isIdle = true;
    private float t = 0.033f;
    private float tempT = 0;
    private float animationIncrement = 0.003f;
    public float distanceUp=2.0f;
    public float distanceAway=2f;
    public float smooth = 15f;
    int flag = 1;
    [Range(-10,10)]
    public float r1;
    [Range(-10, 10)]
    public float r2;
    [Range(-10, 10)]
    public float r3;
    GameObject target;
    DamageController damageController;
    NavMeshAgent navAgent;


   
    // Use this for initialization
    void Start()
    {
        m_characterCtrl = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        damageController = GetComponent<DamageController>();
        AttackBehaviour ab = m_animator.GetBehaviour<AttackBehaviour>();
        //ab.damageController = damageController;
        target = GameObject.Find("ashe");
        navAgent = GetComponent<NavMeshAgent>();
        Shader.WarmupAllShaders();

       
        
    }
    IEnumerator move(GameObject subject,Vector3 targetPoint)
    {
        m_animator.SetBool("isIdle", false);
        m_animator.SetBool("isWalk", true);
        m_animator.SetBool("isAttack", false);
       
        m_isIdle = false;
        yield return MotionController.moveToPoint(subject, targetPoint);
        m_isIdle = true;
        m_animator.SetBool("isIdle", true);
        m_animator.SetBool("isWalk", false);
    }
    // Update is called once per frame
    void Update()
    {
       
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //判断射线是否发生碰撞                
            if (Physics.Raycast(ray, out hit, 100))
            {
                if(hit.collider.gameObject.name == "Ground")
                {
                    m_targetPoint = Vector3.Scale(hit.point, new Vector3(1, 0, 1));

                    Destroy(Instantiate(targetCircle, new Vector3(m_targetPoint.x, 0.01f, m_targetPoint.z), new Quaternion(0, 0, 0, 0)), 1);
                    //transform.LookAt(m_targetPoint);
                   
                   // ActionManager.excuteAction(move(gameObject,hit.point));
                    //ActionManager.excuteAction(move(target,hit.point+ new Vector3(2, 0, 2)));




                }
                else if (hit.collider.gameObject.tag == "Enemy")
                {
                    //HUDRoot.NewText(UnityEngine.Random.Range(100, 1000).ToString(), target.transform, Color.red, -50, 20f, UnityEngine.Random.Range(-1f, -2f), UnityEngine.Random.Range(4f, 6f), UnityEngine.Random.Range(0, 2) == 0 ? bl_Guidance.LeftDown : bl_Guidance.RightDown);

                }




            }
        }
        if (Input.GetKey(KeyCode.A))
        {


            m_animator.SetBool("isIdle", true);
            m_animator.SetBool("isAttack", true);
            m_animator.SetBool("isWalk", false);
            m_targetPoint = transform.position + transform.forward * 0.5f;
            int n = 2;
            damageController.register(new Func<List<GameObject>> (() =>
            {
                Debug.Log(n);
                List<GameObject> targetList = new List<GameObject>();
                targetList.Add(target);
                return targetList;
            }));
                  
        }
        if (Input.GetKey(KeyCode.B))
        {
            ////navAgent.SetDestination(target.transform.position);
            //flag = 2;
            //Camera.main.transform.LookAt(transform);
            //// move(0.05f);
            ActionManager.excuteAction(follow());
        }
    }
    IEnumerator follow()
    {
        return null;
        //yield return MotionController.followUnit(target, this);
    }
    void LateUpdate()
    {
        Vector3 targetPositon;
        if (flag == 1)
        {
            targetPositon = transform.position + Vector3.up * distanceUp + Vector3.back * distanceAway;
        }
        else
        {
            targetPositon = transform.position + Vector3.up * distanceUp + Vector3.left * distanceAway;
        }
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPositon, Time.deltaTime*smooth);
        if (Camera.main.transform.position == targetPositon)
        {
            Camera.main.transform.LookAt(transform);
        }
        //Camera.main.transform.LookAt(transform);
        //t = Mathf.Clamp(Time.deltaTime, 0, 0.066f);

        //if (t > 0)
        //{
        //    while (tempT < t)
        //    {
        //        tempT += animationIncrement;

        //        if (myTrail.time > 0)
        //        {
        //            myTrail.Itterate(Time.time - t + tempT);
        //        }
        //        else
        //        {
        //            myTrail.ClearTrail();
        //        }
        //    }

        //    tempT -= t;

        //    if (myTrail.time > 0)
        //    {
        //        myTrail.UpdateTrail(Time.time, t);
        //    }
        //}
    }

    

    //void move(float speed)
    //{
    //    if (!m_isIdle)
    //    {
    //        Vector3 t = Vector3.Scale(m_targetPoint, new Vector3(1, 0, 1));
    //        Vector3 p = Vector3.Scale(transform.position, new Vector3(1, 0, 1));
    //        Vector3 direct = t - p;
    //        float distance = Mathf.Abs(Vector3.Distance(t, p));

    //        if (distance >= speed)
    //        {

    //            //注解3 限制移动
    //            Vector3 v = Vector3.ClampMagnitude(direct, speed);
    //            //可以理解为主角行走或奔跑了一步
               
    //            applyTurn(direct);
    //            m_characterCtrl.Move(v);

    //        }
    //        else
    //        {

    //            if (distance != 0)
    //            {
    //                transform.position =new Vector3( m_targetPoint.x,transform.position.y,m_targetPoint.z);
    //            }
    //            m_isIdle = true;
    //            m_animator.SetBool("isIdle", true);
    //            m_animator.SetBool("isWalk", false);

    //        }
    //    }
    //}
    void applyTurn(Vector3 direct)
    {
        if (direct.magnitude > 1f) direct.Normalize();
        direct = transform.InverseTransformDirection(direct);
        float turnAmount = Mathf.Atan2(direct.x, direct.z);

        float turnSpeed = Mathf.Lerp(180, 360, turnAmount * 180 / Mathf.PI);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

   
}
