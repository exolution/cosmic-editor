using UnityEngine;
using System.Collections;
using System;

public enum InteractionEventType
{
    OnMouseEnter, OnMouseExit, RightClick, LeftClick
}
public class InteractionEvent
{
    public InteractionEventType type;
    public Unit targetUnit;
    public Vector3 targetPoint;
    public GameObject targetObject;

    public InteractionEvent(InteractionEventType type, Unit targetUnit, Vector3 targetPoint)
    {
        this.type = type;
        this.targetUnit = targetUnit;
        this.targetPoint = targetPoint;
    }
    public InteractionEvent(InteractionEventType type, Unit targetUnit)
    {
        this.type = type;
        this.targetUnit = targetUnit;
    }
    public InteractionEvent(InteractionEventType type, GameObject targetObject, Vector3 targetPoint)
    {
        this.type = type;
        this.targetObject = targetObject;
        this.targetPoint = targetPoint;
    }
    public InteractionEvent(InteractionEventType type, GameObject targetObject)
    {
        this.type = type;
        this.targetObject = targetObject;
    }
    public InteractionEvent(InteractionEventType type, Vector3 targetPoint)
    {
        this.type = type;
        this.targetPoint = targetPoint;
    }
    public override string ToString()
    {
        return this.type + "-" + this.targetUnit + "-" + this.targetObject + "-" + this.targetPoint;
    }
}

public delegate void PlayerInteractorHandler(InteractionEvent interactionEvent);
enum InteractionStatus
{
    NORMAL, TARGET_SELECT, AOE_SELECT
}

class SkillSelector : Action
{
    internal SkillSelectMode skillSelectMode;
    internal bool enabled;
    Skill skill;
    bool done = false;
    public void abort()
    {

    }

    public void select(Unit selectTargetUnit, Vector3 selectTargetPoint)
    {
        skill.setSkillTarget(selectTargetUnit, selectTargetPoint);
        enabled = false;
        done = true;
        Cursor.visible = true;
    }

    public bool isDone()
    {
        return done;
    }

    public void pause()
    {

    }

    public void resumue()
    {

    }
    public void init(Skill skill,SkillSelectMode skillSelectMode )
    {
        this.skillSelectMode = skillSelectMode;
        this.skill = skill;
        enabled = true;
        done = false;
        if (skillSelectMode == SkillSelectMode.Ground || skillSelectMode == SkillSelectMode.Unit)
        {
            Cursor.SetCursor(PlayerInteractor.Instance.m_select, Vector2.zero, CursorMode.ForceSoftware);
        }

    }

}

/// <summary>
/// 管理玩家的鼠标与各种物体的交互
/// </summary>
public class PlayerInteractor : MonoBehaviour
{

    public Texture2D m_pointer;
    public Texture2D m_attack;
    public Texture2D m_select;
    PlayerInfo currentPlayerInfo;
    public float smooth = 4f;
    Shader rimLight;
    SkillSelector skillSelector=new SkillSelector();

    InteractionStatus interactionStatus = InteractionStatus.NORMAL;

    public event PlayerInteractorHandler MouseEnterUnit;
    public event PlayerInteractorHandler MouseExitUnit;
    public event PlayerInteractorHandler ClickUnit;
    public event PlayerInteractorHandler MouseEnterObject;
    public event PlayerInteractorHandler MouseExitObject;
    public event PlayerInteractorHandler ClickObject;
   
    public static PlayerInteractor Instance
    {
        get;
        private set;
    }
    // Use this for initialization
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            PlayerManager.init();
            currentPlayerInfo = PlayerManager.getCurrentPlayerInfo();
            ClickObject += currentPlayerInfo.selectUnit.onclickGround;
            //PlayerInteractor.Instance.ClickUnit += currentPlayerInfo.selectUnit.onclickGround;
        }
        else
        {
            Debug.LogError("ActionManager must be added to just only one gameobject");
        }
    }
    void Start()
    {

        Cursor.SetCursor(m_pointer, Vector2.zero, CursorMode.ForceSoftware);
        rimLight = Shader.Find("Custom/RimLight");
        //MouseOverUnit += onMouseOverUnit;
        //MouseOutUnit += new PlayerInteractorHandler(onMouseOutUnit);
        MouseEnterUnit += onMouseEnterUnit;
        MouseEnterObject += onMouseEnterObject;

    }

    Skill skill=new Skill();
    void Update()
    {
        if (Input.GetKey("e"))
        {
            ActionManager.excuteAction(test(skill, SkillSelectMode.AOE));
        }
        else if (Input.GetKey("a"))
        {
            ActionManager.excuteAction(test(skill, SkillSelectMode.Unit));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // mc_animator.SetInteger("idle", 1);
            //currentPlayerInfo.selectUnit.animator.SetInteger("attack",1);
            currentPlayerInfo.selectUnit.animationManager.playAnimation("Run");

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //mc_animator.SetInteger("idle", 0);
            //animationManager.playAnimation("idle1");
        }
    }
    IEnumerator test(Skill skill, SkillSelectMode skillSelectMode)
    {
        yield return PlayerInteractor.SkillSelect(skill, skillSelectMode);
        Debug.Log("select target is" + skill.skillTarget);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        
        RaycastHit hit = new RaycastHit();
        Ray ray;
       
        if (skillSelector.enabled && skillSelector.skillSelectMode == SkillSelectMode.AOE)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Terrain"));
            Cursor.visible = false;
            Vector3 targetPositon = new Vector3(hit.point.x, gameObject.transform.position.y, hit.point.z);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPositon, Time.deltaTime * smooth);
        }

        bool leftMouseButtonDown = Input.GetMouseButtonDown(0);
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);
        if (leftMouseButtonDown || rightMouseButtonDown)
        {
            InteractionEventType iet = leftMouseButtonDown ? InteractionEventType.LeftClick : InteractionEventType.RightClick;
            if (!skillSelector.enabled || skillSelector.skillSelectMode != SkillSelectMode.AOE)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Terrain", "Unit"));
                
                Unit hitUnit = UnitManager.getUnit(hit.collider.gameObject);
                if (hitUnit)
                {
                    if (ClickUnit != null) ClickUnit(new InteractionEvent(iet, hitUnit, hit.point));

                }
                else
                {
                    if (ClickObject != null) ClickObject(new InteractionEvent(iet, hit.collider.gameObject, hit.point));

                }
            }
            else
            {
                skillSelector.select(null, hit.point);
            }


        }

    }


    public static void doMouseEnterUnit(Unit targetUnit)
    {
        if (Instance.MouseEnterUnit != null) Instance.MouseEnterUnit(new InteractionEvent(InteractionEventType.OnMouseEnter, targetUnit));
    }

    public static void doMouseExitUnit(Unit targetUnit)
    {
        if (Instance.MouseExitUnit != null) Instance.MouseExitUnit(new InteractionEvent(InteractionEventType.OnMouseExit, targetUnit));
    }
    public static void doClickUnit(InteractionEventType iet, Unit targetUnit, Vector3 clickPoint)
    {
        if (Instance.ClickUnit != null) Instance.ClickUnit(new InteractionEvent(iet, targetUnit, clickPoint));
    }

    public static void doMouseEnterObject(GameObject targetObject)
    {
        if (Instance.MouseEnterObject != null) Instance.MouseEnterObject(new InteractionEvent(InteractionEventType.OnMouseEnter, targetObject));
    }
    public static void doMouseExitObject(GameObject targetObject)
    {
        if (Instance.MouseExitObject != null) Instance.MouseExitObject(new InteractionEvent(InteractionEventType.OnMouseExit, targetObject));
    }
    public static void doClickObject(InteractionEventType iet, GameObject targetUnit, Vector3 clickPoint)
    {
        if (Instance.MouseExitObject != null) Instance.MouseExitObject(new InteractionEvent(iet, targetUnit, clickPoint));
    }


    void onMouseEnterUnit(InteractionEvent e)
    {
        if (skillSelector.skillSelectMode == SkillSelectMode.Ground || skillSelector.skillSelectMode == SkillSelectMode.Unit)
        {
            Cursor.SetCursor(m_select, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            if (PlayerManager.isOpponent(PlayerManager.currentPlayer, e.targetUnit.player))
            {
                Cursor.SetCursor(m_attack, Vector2.zero, CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.SetCursor(m_pointer, Vector2.zero, CursorMode.ForceSoftware);
            }
        }
        //    Renderer renderer = e.target.GetComponentInChildren<Renderer>();
        //    Debug.Log("over:"+renderer.material.shader.name);
        //    if (renderer)
        //    {
        //        hoveredTargetShader = renderer.material.shader;
        //        renderer.material.shader = rimLight;
        //    }
    }
    void onMouseEnterObject(InteractionEvent e)
    {

        if (skillSelector.skillSelectMode == SkillSelectMode.Ground || skillSelector.skillSelectMode == SkillSelectMode.Unit)
        {
            Cursor.SetCursor(m_select, Vector2.zero, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(m_pointer, Vector2.zero, CursorMode.ForceSoftware);
        }
        

        //    Renderer renderer = e.target.GetComponentInChildren<Renderer>();
        //    Debug.Log("over:"+renderer.material.shader.name);
        //    if (renderer)
        //    {
        //        hoveredTargetShader = renderer.material.shader;
        //        renderer.material.shader = rimLight;
        //    }
    }
    //void onMouseOutUnit(InteractionEvent e)
    //{
    //    Renderer renderer = e.target.GetComponentInChildren<Renderer>();
    //    //debug.log(renderer.material.shader.name);
    //    Debug.Log("out:"+hoveredTargetShader.name);
    //    if (renderer)
    //    {

    //        renderer.material.shader = hoveredTargetShader;
    //    }
    //}
    public static Action SkillSelect(Skill skill,SkillSelectMode skillSelectMode)
    {
        Instance.skillSelector.init(skill, skillSelectMode);
        return Instance.skillSelector;
    }
}
