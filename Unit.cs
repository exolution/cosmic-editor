using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType
{
    Normal,Missle, Civilian
}
public class Unit : MonoBehaviour
{

    public Animator animator;

    public AnimationManager animationManager;
    public Dictionary<int, Skill> skillDictionary = new Dictionary<int, Skill>();

    public UnitType type;
    public Player player;
    public UnitAttribute unitAttribute;
    void Awake()
    {
        UnitManager.add(gameObject, this);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animationManager = new AnimationManager(animator);
       

    }
    void Update()
    {

        Debug.Log("update"+Time.realtimeSinceStartup);
    }
    void LateUpdate()
    {

        Debug.Log("late update" + Time.realtimeSinceStartup);
    }
    public void onclickGround(InteractionEvent interactionEvent)
    {
        move(interactionEvent.targetPoint);
    }
    void OnMouseEnter()
    {
         PlayerInteractor.doMouseEnterUnit(this);
      
    }
    public void animationEvent(AnimationEvent ae)
    {
        //Debug.Log(a);
        //Debug.Log(b);
        //Debug.Log(s);
        //Debug.Log(sk);
       
    }
    void OnMouseExit()
    {
        
         PlayerInteractor.doMouseExitUnit(this);
    }
    public Action move(Vector3 targetPoint)
    {
        return ActionManager.excuteAction(moveAction(targetPoint));
    }
    public IEnumerator moveAction(Vector3 targetPoint)
    {
        animationManager.playAnimation("Run");
        yield return MotionController.moveToPoint(gameObject, targetPoint);
        animationManager.playAnimation("Idle.idle1");
    }
    public IEnumerator followUnitAction(Unit target,float distace = 2f)
    {
        yield return MotionController.followUnit(gameObject, target.gameObject, distace);
    }
    

}
