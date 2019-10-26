using System;
using System.Collections;
using System.Collections.Generic;
using UI.Helpers;

namespace UI.ViewModels
{
    /// <summary>
    /// A list that is able to loop backward and forward infinitely
    /// </summary>
    public class RecyclableMegaList<T> : ViewModelBase, ICollection<List<T>>
    {
        private int _innerIndex;
        private int _currentIndex;

        /// <summary>
        /// Current index of the internal list
        /// </summary>
        private int InnerIndex
        {
            get { return _innerIndex; }
             set
            {
                _innerIndex = value;
            }
        }

        /// <summary>
        /// Current index of the internal list
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            private set
            {
                _currentIndex = value; 
                OnCurrentIndexChanged(value);
            }
        }


        /// <summary>
        /// Internal list that stores the data
        /// </summary>
        private List<List<T>> MegaList { get; set; }

        /// <summary>
        /// This event will fire when <see cref="CurrentIndex"/> is changed
        /// </summary>
        public event Action<int> CurrentIndexChanged;


        /// <summary>
        /// Fires when the current index is about to go equal or above <see cref="Count"/>
        /// </summary>
        public event Action UpperIndexExceeded;

        /// <summary>
        /// Fires when the current index is about to go below 0
        /// </summary>
        public event Action LowerIndexExceeded;

        /// <summary>
        /// Set current index to dstIndex and return that element
        /// </summary>
        /// <param name="dstIndex"></param>
        /// <returns></returns>
        public List<T> JumpTo(int dstIndex)
        {
            if (dstIndex >= Count || dstIndex < 0)
                throw new IndexOutOfRangeException("The index to jump to is invalid");
            InnerIndex = dstIndex;

            CurrentIndex = InnerIndex;
            return this[dstIndex];
        }

        /// <summary>
        /// Return next element in the internal list
        /// If the new index exceeds Count - 1, it will go back to 0
        /// and continue to provide data
        /// </summary>
        /// <returns></returns>
        public List<T> NextUnbounded()
        {
            InnerIndex++;
            if (InnerIndex >= MegaList[0].Count)
            {
                InnerIndex = 0;
                OnUpperIndexExceeded();
            }

            CurrentIndex = InnerIndex;
            return this[InnerIndex];
        }

        /// <summary>
        /// Return next element in the internal list
        /// If the new index exceeds Count - 1, it will go back to 0
        /// and provide null
        /// </summary>
        /// <returns></returns>
        public List<T> NextBounded()
        {
            InnerIndex++;
            if (InnerIndex >= MegaList[0].Count)
            {
                InnerIndex = -1;
                OnUpperIndexExceeded();
                return null;
            }

            CurrentIndex = InnerIndex;
            return this[InnerIndex];
        }
        
        /// <summary>
        /// Return previous element in the internal list
        /// If the new index is less than 0 , it will go to the end of the list
        /// and continue to provide data
        /// </summary>
        /// <returns></returns>
        public List<T> PreviousUnbounded()
        {
            InnerIndex--;
            if (InnerIndex < 0)
            {
                InnerIndex = MegaList[0].Count - 1;
                OnLowerIndexExceeded();
            }

            CurrentIndex = InnerIndex;
            return this[InnerIndex];
        }

        /// <summary>
        /// Reconstruct the mega list from another mega list
        /// </summary>
        /// <param name="newValue">The new mega list to assign</param>
        public void Reconstruct(List<List<T>> newValue)
        {
            MegaList = newValue;
            InnerIndex = -1;
            CurrentIndex = InnerIndex;
        }

  

        /// <summary>
        /// Number of list within the mega list
        /// </summary>
        public int NumLists
        {
            get { return MegaList.Count; }
            set
            {
                if (value == MegaList.Count) return;

                MegaList.Clear();
                for (int i = 0; i < value; i++)
                {
                    MegaList.Add(new List<T>());
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="numList">Number of lists within the mega list</param>
        public RecyclableMegaList(int numList = 1)
        {
            InnerIndex = -1;
            MegaList = new List<List<T>>();
            NumLists = numList;
        }

        public List<T> this[int index]
        {
            get
            {
                var output = new List<T>();
                foreach (var list in MegaList)
                {
                    output.Add(list[index]);
                }

                return output;
            }
            set
            {
                if (index >= Count || index < 0) throw new IndexOutOfRangeException();
                if (value.Count != NumLists)
                    throw new ArgumentException("The count of the added item should equal to NumLists");

                int count = value.Count;
                for (int i = 0; i < count; i++)
                {
                    MegaList[i].Insert(index, value[i]);
                }
            }
        }

        public IEnumerator<List<T>> GetEnumerator()
        {
          
            return MegaList.AsRows().GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(List<T> item)
        {
            if (item.Count != MegaList.Count)
                throw new ArgumentException("The count of the added item should equal to NumLists");

            for (int i = 0; i < item.Count; i++)
            {
                MegaList[i].Add(item[i]);
            }
        }

        public void Clear()
        {
            MegaList.Clear();
            InnerIndex = -1;
            CurrentIndex = InnerIndex;
        }

        public bool Contains(List<T> item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(List<T>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(List<T> item)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Count of elements in individual list
        /// </summary>
        public int Count => MegaList.Count == 0 ? 0 : MegaList[0].Count;

        public bool IsReadOnly
        {
            get { return false; }
        }

        protected virtual void OnCurrentIndexChanged(int obj)
        {
            CurrentIndexChanged?.Invoke(obj);
        }

        protected virtual void OnUpperIndexExceeded()
        {
            UpperIndexExceeded?.Invoke();
        }

        protected virtual void OnLowerIndexExceeded()
        {
            LowerIndexExceeded?.Invoke();
        }
    }
}