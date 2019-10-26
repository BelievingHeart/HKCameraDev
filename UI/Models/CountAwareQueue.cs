using System;
using System.Collections.Generic;

namespace UI.Models
{
    /// <summary>
    /// This class will fire size changed event when its size is changed'
    /// </summary>
    public class CountAwareQueue<T> : Queue<T>
    {
        public event Action<int> CountChange;
        
        public new void Enqueue(T element)
        {
            base.Enqueue(element);
            OnCountChange(Count);
        }

        public new T Dequeue()
        {
            if(Count == 0) throw new InvalidOperationException("The queue is empty");
            
            OnCountChange(Count-1);
            return base.Dequeue();
        }

        protected virtual void OnCountChange(int obj)
        {
            CountChange?.Invoke(obj);
        }

        public CountAwareQueue(IEnumerable<T> source) : base(source)
        {
            
        }

        public CountAwareQueue() 
        {
            
        }
    }
}