using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void EventCB(params object[] parameter);

public class IEvent
{
    public EventCB EventCb { get; set; }
    /// <summary>
    /// 调用次数,0自动回收,-1无限
    /// </summary>
    public int Count { get; set; }
}

public static class EventMgr
{
    private static object tempTarget;
    private static Dictionary<object,Dictionary<string,List<IEvent>>> events;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        if (tempTarget == null || events == null)
        {
            
        tempTarget = new object();
        events=new Dictionary<object, Dictionary<string, List<IEvent>>>();
        }
    }
    
    
    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="target">对象，方便区分哪个对象注册的事件</param>
    /// <param name="key">事件名称</param>
    /// <param name="callback">事件回调，参数为object数组</param>
    public static void On(this object target,string key,EventCB callback)
    {
        Init();
        if(target==null || string.IsNullOrEmpty(key)|| callback==null) return;
        
        On(target,key,callback,-1);
        
    }

    /// <summary>
    /// 注册无对象事件(全局事件)
    /// </summary>
    /// <param name="key">事件名称</param>
    /// <param name="callback">事件回调，参数为object数组</param>
    public static void On(string key, EventCB callback)
    {
        Init();
        On(tempTarget,key,callback);
    }
    
    /// <summary>
    /// 注册一次性对象事件
    /// </summary>
    /// <param name="target">对象，方便区分哪个对象注册的事件</param>
    /// <param name="key">事件名称</param>
    /// <param name="callback">事件回调，参数为object数组</param>
    public static void Once(this object target, string key, EventCB callback)
    {
        Init();
        if(target==null || string.IsNullOrEmpty(key)|| callback==null) return;
        
        On(target,key,callback,1);
    }

    
    private static void On(this object target, string key, EventCB callback, int count)
    {
        Init();
        if (events.ContainsKey(target))
        {
            if (events[target].ContainsKey(key))
            {
                events[target][key].Add(new IEvent(){EventCb = callback,Count = count});
            }
            else
            {
                var tempList = new List<IEvent>();
                tempList.Add(new IEvent(){EventCb = callback,Count =count});
                events[target].Add(key,tempList);
            }
        }
        else
        {
            var temp = new Dictionary<string,List<IEvent>>();
            var tempList = new List<IEvent>();
            tempList.Add(new IEvent(){EventCb = callback,Count = count});
            temp.Add(key,tempList);
            events.Add(target,temp);
        }
    }

    
    

    /// <summary>
    /// 同步派发指定对象事件
    /// </summary>
    /// <param name="target">指定对象派发事件</param>
    /// <param name="key">指定派发事件名称</param>
    /// <param name="parameter">传参，元组</param>
    public static void Emit(this object target, string key, params object[] parameter)
    {
        Init();
        if(target==null || string.IsNullOrEmpty(key)) return;

        if (events.ContainsKey(target) && events[target].ContainsKey(key))
        {
            var temps = events[target][key];
            if (temps != null)
            {
                //Debug.Log(string.Format("{0}派发事件:{1},参数:{2}",target.ToString(),key,JsonUtility.ToJson(parameter)));
                Action action = delegate {  };
                foreach (var VARIABLE in temps)
                {
                    VARIABLE.EventCb?.Invoke(parameter);
                    VARIABLE.Count--;
                    if (VARIABLE.Count == 0)
                    {
                        action += () => { temps.Remove(VARIABLE); };
                    }
                }
                action.Invoke();
            }
        }
        
    }
    
    /// <summary>
    /// 同步派发全局事件
    /// </summary>
    /// <param name="key">指定派发事件名称</param>
    /// <param name="parameter">传参，元组</param>
    public static void Emit(string key, params object[] parameter)
    {
        Init();
        if(string.IsNullOrEmpty(key)) return;

        if (events.ContainsKey(tempTarget) && events[tempTarget].ContainsKey(key))
        {
            var temps = events[tempTarget][key];
            if (temps != null)
            {
                //Debug.Log(string.Format("{0}派发事件:{1},参数:{2}",target.ToString(),key,JsonUtility.ToJson(parameter)));
                Action action = delegate {  };
                foreach (var VARIABLE in temps)
                {
                    VARIABLE.EventCb?.Invoke(parameter);
                    VARIABLE.Count--;
                    if (VARIABLE.Count == 0)
                    {
                        action += () => { temps.Remove(VARIABLE); };
                    }
                }
                action.Invoke();
            }
        }
        
    }

    /// <summary>
    /// 异步派发指定对象事件
    /// </summary>
    /// <param name="target">指定对象派发事件</param>
    /// <param name="key">指定派发事件名称</param>
    /// <param name="parameter">传参，元组</param>
    public static void EmitForAsync(this object target, string key, params object[] parameter)
    {
        Init();
        if(target==null || string.IsNullOrEmpty(key)) return;

        if (events.ContainsKey(target) && events[target].ContainsKey(key))
        {
            var temps = events[target][key];
            if (temps != null)
            {
                foreach (var VARIABLE in temps)
                {
                    VARIABLE.Count--;
                    VARIABLE.EventCb?.BeginInvoke(parameter, (IAsyncResult ar) =>
                    {
                        if (VARIABLE.Count == 0)
                        {
                            temps.Remove(VARIABLE);
                        }
                    }, null);
                }
            }
        }
    }
    
    /// <summary>
    /// 异步派发全局事件
    /// </summary>
    /// <param name="key">指定派发事件名称</param>
    /// <param name="parameter">传参，元组</param>
    public static void EmitForAsync(string key, params object[] parameter)
    {
        Init();
        if(string.IsNullOrEmpty(key)) return;

        if (events.ContainsKey(tempTarget) && events[tempTarget].ContainsKey(key))
        {
            var temps = events[tempTarget][key];
            if (temps != null)
            {
                foreach (var VARIABLE in temps)
                {
                    VARIABLE.Count--;
                    VARIABLE.EventCb?.BeginInvoke(parameter, (IAsyncResult ar) =>
                    {
                        if (VARIABLE.Count == 0)
                        {
                            temps.Remove(VARIABLE);
                        }
                    }, null);
                }
            }
        }
    }
    

    /// <summary>
    /// 注销指定名称事件
    /// </summary>
    /// <param name="target">注销事件对象</param>
    /// <param name="key">注销事件名称</param>
    public static void Off(this object target, string key)
    {
        Init();
        if (target != null && !string.IsNullOrEmpty(key))
        {
            if (events.ContainsKey(target) && events[target].ContainsKey(key))
            {
                events[target].Remove(key);
            }
        }
    }
    /// <summary>
    /// 注销指定名称和回调事件
    /// </summary>
    /// <param name="target">注销事件对象</param>
    /// <param name="key">注销事件名称</param>
    /// <param name="callback">注销事件回调</param>
    public static void Off(this object target, string key, EventCB callback)
    {
        Init();
        if (target != null && !string.IsNullOrEmpty(key) && callback!=null)
        {
            if (events.ContainsKey(target) && events[target].ContainsKey(key))
            {
                var temps = events[target][key];
                if (temps != null)
                {
                    for (int i = 0; i < temps.Count; i++)
                    {
                        if (temps[i].EventCb == callback)
                        {
                            temps.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 注销对象所有事件
    /// </summary>
    /// <param name="target">注销对象</param>
    public static void TargetOff(this object target)
    {
        Init();
        if (target != null)
        {
            if (events.ContainsKey(target))
            {
                events.Remove(target);
            }
        }
    }
    /// <summary>
    /// 注销所有事件
    /// </summary>
    /// <param name="isClearGlobal">是否注销全局事件</param>
    public static void ClearEvents(bool isClearGlobal)
    {
        Init();
        if (isClearGlobal)
        {
            Dictionary<string,List<IEvent>> temp;
            events.TryGetValue(tempTarget,out temp);
            
            events.Clear();

            if (temp != null)
            {
                events.Add(tempTarget,temp);
            }
        }
        else
        {
            events.Clear();
        }
    }
    
}
