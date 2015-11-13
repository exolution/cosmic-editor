using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UnitType
{
    Normal,Missle, Civilian
}
public class Unit : MonoBehaviour
{

    Animator mc_animator;

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
        mc_animator = GetComponent<Animator>();
        animationManager = new AnimationManager(mc_animator);
        PlayerInteractor.Instance.ClickObject += onclickGround;
        PlayerInteractor.Instance.ClickUnit += onclickGround;

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
           // mc_animator.SetInteger("idle", 1);
            animationManager.playAnimation("attack");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            mc_animator.SetInteger("idle", 0);
            //animationManager.playAnimation("idle1");
        }

    }
    void onclickGround(InteractionEvent interactionEvent)
    {
        Debug.Log(interactionEvent);
    }
    void OnMouseEnter()
    {
         PlayerInteractor.doMouseEnterUnit(this);
      
    }

    void OnMouseExit()
    {
        
         PlayerInteractor.doMouseExitUnit(this);
    }
    public IEnumerator followUnit(Unit target,float distace = 2f)
    {
        yield return MotionController.followUnit(gameObject, target.gameObject, distace);
    }
    

}
