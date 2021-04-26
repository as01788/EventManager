using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDemo : MonoBehaviour
{
    public Tuple<string[],int> temps = new Tuple<string[], int>(new string[10]{"111","222","333","444","555","666","777","888","999","000"},6666);

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            EventMgr.On("e", Test);
            EventMgr.On("e", Test);
            EventMgr.On("e", Test);
            EventMgr.On("e", Test);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            EventMgr.Once(this,"e", (object[] temps) =>
            {
                foreach (var VARIABLE in temps)
                {
                    Debug.Log(VARIABLE.ToString());
                }
            });
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            EventMgr.EmitForAsync("e",new Tuple<int>(UnityEngine.Random.Range(-100,100))); 
        }else if (Input.GetKeyDown(KeyCode.C))
        {
            EventMgr.ClearEvents(true);
        }
    }

    void Test(object[] temps)
    {
        foreach (var VARIABLE in temps)
        {
            Debug.Log(VARIABLE.ToString());
        }
    }
    
}
