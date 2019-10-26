using System;
using System.Collections.Generic;
using System.Linq;
using ImageDebugger.Core.ViewModels;
using ImageDebugger.Core.ViewModels.HalconWindowViewModel;
using NUnit.Framework;

namespace ImageDebugger.Core.Tests
{
    [TestFixture]
    public class RecyclableMegaListTests
    {
        #region TestsOfTestTools

        [Test]
        public void ListValueEqual_NotEqual_ReturnFalse()
        {
            Assert.IsFalse(ListValueEqual(FirstRow, SecondRow));
        }

        #endregion

        #region Add

        [Test]
        public void Add_Always_CountIncrementsBy1()
        {
            var megaList = MakeRecyclableMegaList();
            var oldCount = megaList.Count;
            
            megaList.Add(new List<int>(){100, 101});
            var newCount = megaList.Count;
            
            Assert.AreEqual(oldCount + 1, newCount);
        }
        
        [Test]
        public void Add_WhenCountOfInputNotEqualToNumLists_ThrowsArgumentException()
        {
            var megaList = MakeRecyclableMegaList();

            Assert.Catch<ArgumentException>(() => { megaList.Add(new List<int>() { }); });
        }

        #endregion


        #region Indexer

        [Test]
        public void IndexerSet_IndexOutOfRange_ThrowsIndexOutOfRangeException()
        {
            var megaList = MakeRecyclableMegaList();
            
            Assert.Catch<IndexOutOfRangeException>(() => { megaList[megaList.Count] = new List<int>(){100, 200}; });

        }
        
        [Test]
        public void IndexerSet_InputCountNotEqualToNumLists_ThrowsArgumentException()
        {
            var megaList = MakeRecyclableMegaList();
            
            Assert.Catch<ArgumentException>(() => { megaList[megaList.Count - 1] = new List<int>(){}; });

        }
        
        [Test]
        public void IndexerGet_Always_ReturnProperElement()
        {
            var megaList = MakeRecyclableMegaList();

            var actual = megaList[1];

            var expected = SecondRow;
            
            Assert.IsTrue(ListValueEqual(actual, expected));
        }

        #endregion

        #region DefaultConstructor

        [Test]
        public void DefaultConstructor_Always_SetCurrentIndexToMinusOne()
        {
            var megaList = new RecyclableMegaList<int>();

            var actual = megaList.CurrentIndex;

            var expected = -1;
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void DefaultConstructor_Always_SetCountToZero()
        {
            var megaList = new RecyclableMegaList<int>();

            var actual = megaList.Count;

            var expected = 0;
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Clear

        [Test]
        public void Reconstruct_Always_SetCurrentIndexToMinusOne()
        {
            var megaList = MakeRecyclableMegaList();
            
            megaList.Reconstruct(new List<List<int>>());
            var actual = megaList.CurrentIndex;

            var expected = -1;
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public void Clear_Always_SetCountToZero()
        {
            var megaList = MakeRecyclableMegaList();
            
            megaList.Clear();
            var actual = megaList.Count;

            var expected = 0;
            
            Assert.AreEqual(expected, actual);

        }

        #endregion

        
        
        #region JumpTo

        [Test]
        public void JumpTo_IndexOutOfRange_ThrowsIndexOutOfRangeException()
        {
            var megaList = MakeRecyclableMegaList();

            Assert.Catch<IndexOutOfRangeException>(() => { megaList.JumpTo(megaList.Count); });
        }
        
        [Test]
        public void JumpTo_IndexValid_CurrentIndexSetProperly()
        {
            var megaList = MakeRecyclableMegaList();

            megaList.JumpTo(1);

            Assert.AreEqual(1, megaList.CurrentIndex);
        }
        
        [Test]
        public void JumpTo_IndexValid_ReturnProperElement()
        {
            var megaList = MakeRecyclableMegaList();

            var actual = megaList.JumpTo(1);

            var expected = SecondRow;
            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        

        #endregion
        
         
        

        #region NextUnbounded

        [Test]
        public void NextUnbounded_UponConstruction_Provide0IndexElement()
        {
            var megaList = MakeRecyclableMegaList();

            var actual = megaList.NextUnbounded();
            var expected = FirstRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        
        [Test]
        public void NextUnbounded_IndexNoLessThanZeroLessThanCountMinusOne_ProvideNextElement()
        {
            var megaList = MakeRecyclableMegaList();
            megaList.JumpTo(0);

            var actual = megaList.NextUnbounded();
            var expected = SecondRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        

        [Test]
        public void NextUnbounded_ExceedIndex_ProvideNextElement()
        {
            var megaList = MakeRecyclableMegaList();
            megaList.JumpTo(megaList.Count-1);

            var actual = megaList.NextUnbounded();
            var expected = FirstRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        #endregion


        #region NextBounded

        [Test]
        public void NextBounded_UponConstruction_Provide0IndexElement()
        {
            var megaList = MakeRecyclableMegaList();

            var actual = megaList.NextBounded();
            var expected = FirstRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        
        [Test]
        public void NextBounded_IndexNoLessThanZeroLessThanCountMinusOne_ProvideNextElement()
        {
            var megaList = MakeRecyclableMegaList();
            megaList.JumpTo(0);

            var actual = megaList.NextBounded();
            var expected = SecondRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        
        
        [Test]
        public void NextBounded_ExceedIndex_ProvideNull()
        {
            var megaList = MakeRecyclableMegaList();
            megaList.JumpTo(megaList.Count-1);

            var actual = megaList.NextBounded();
            
            Assert.IsNull(actual);
        }
        

        #endregion

        #region PreviousUnbounded

        [Test]
        public void PreviousUnbound_UponConstruction_ProvideLastElement()
        {
            var megaList = MakeRecyclableMegaList();

            var actual = megaList.PreviousUnbounded();
            var expected = LastRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        
        [Test]
        public void PreviousUnbound_IndexGreaterThanZero_ProvidePreviousElement()
        {
            var megaList = MakeRecyclableMegaList();

            megaList.JumpTo(1);
            var actual = megaList.PreviousUnbounded();
            var expected = FirstRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }
        
        [Test]
        public void PreviousUnbound_IndexGoesBelowZero_ProvideLastElement()
        {
            var megaList = MakeRecyclableMegaList();

            megaList.JumpTo(0);
            var actual = megaList.PreviousUnbounded();
            var expected = LastRow;

            Assert.IsTrue(ListValueEqual(actual, expected));
        }

        #endregion
        





        #region Factory

        private static RecyclableMegaList<int> MakeRecyclableMegaList()
        {
            return new RecyclableMegaList<int>(2)
            {
                FirstRow, SecondRow, LastRow
            };
        }

        private static bool ListValueEqual(List<int> actual, List<int> expected)
        {
            return actual.Zip(expected, (a, b) => a == b).All(ele => ele);
        }

        private static List<int> FirstRow => new List<int>() {1, 2};
        
        private static List<int> SecondRow => new List<int>() {3, 4};
        
        private static List<int> LastRow => new List<int>() {5, 6};
        

        #endregion
    }
}