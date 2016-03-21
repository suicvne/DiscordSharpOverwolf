using System;

namespace OWObjectExample
{
    public class EntryPoint
    {
        public event Action<object> MyEvent;

        public void InvokeMyEvent(string myEventData)
        {
            OnMyEvent(myEventData);
        }

        public void OnMyEvent(string eventData)
        {
            if (MyEvent != null)
            {
                MyEvent(eventData);
            }
        }
    }
}
