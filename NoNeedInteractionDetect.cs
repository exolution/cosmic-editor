using UnityEngine;
using System.Collections;

public class NoNeedInteractionDetect : MonoBehaviour
{
   
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }

   
   
    public void OnMouseExit()
    {
        Debug.Log("exit");
        //PlayerInteractor.EnableDetect();
    }
    public void OnMouseEnter()
    {

        //PlayerInteractor.DisableDetect();
    }
}
