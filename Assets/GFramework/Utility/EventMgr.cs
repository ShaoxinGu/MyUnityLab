using System.Collections.Generic;
using UnityEngine.Events;

namespace GFramework
{
    public class EventMgr : Singleton<EventMgr>
    {
        private Dictionary<string, UnityAction<object>> eventDic = new Dictionary<string, UnityAction<object>>();

        public void AddEventListener(string eventName, UnityAction<object> action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                eventDic[eventName] += action;
            }
            else
            {
                eventDic.Add(eventName, action);
            }
        }

        public void RemoveEventListener(string eventName, UnityAction<object> action)
        {
            if (eventDic.ContainsKey(eventName))
                eventDic[eventName] -= action;
        }

        public void DispatchEvent(string eventName, object param)
        {
            if (eventDic.ContainsKey(eventName))
            {
                eventDic[eventName].Invoke(param);
            }
        }

        public void Clear()
        {
            eventDic.Clear();
        }
    }
}
