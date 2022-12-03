using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//public static class ExtensionMethods
//{
//    public static void AddListener(this EventTrigger trigger, EventTriggerType eventType, System.Action<PointerEventData> listener)
//    {
//        EventTrigger.Entry entry = new EventTrigger.Entry();
//        entry.eventID = eventType;
//        entry.callback.AddListener(data => listener.Invoke((PointerEventData)data));
//        trigger.triggers.Add(entry);
//    }
//}

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
