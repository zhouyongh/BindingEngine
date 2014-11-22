using System;
using System.Collections.Generic;

namespace BindingEngine.Test.Helpers
{
    public class TestView
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public int ValueInt { get; set; }
        public float ValueFloat { get; set; }
        public double ValueDouble { get; set; }
        public int EventCount { get; set; }


        public IList<string> CollectionNames { get; set; }
        public IList<TestViewModel2> TestViewModelCollections { get; set; }

        public bool Enable { get; set; }
        public static event TestViewEventHandler StaticTestViewEvent;

        public event TestViewEventHandler TestViewEvent;

        public void RaiseTestViewEvent()
        {
            TestViewEventHandler handler = TestViewEvent;
            if(handler != null)
                handler(this, new TestViewEventArgs());
        }

        public static void RaiseStaticTestViewEvent()
        {
            TestViewEventHandler handler = StaticTestViewEvent;
            if(handler != null)
                handler(new object(), new TestViewEventArgs());
        }

        public int SetValueInt(int value)
        {
            ValueInt = value;
            RaiseTestViewEvent();
            return ValueInt;
        }

        public void OnEventOccured(object sender, EventArgs args)
        {
            EventCount++;
        }
    }

    public class TestViewEventArgs : EventArgs
    {
    }

    public delegate void TestViewEventHandler(object sender, TestViewEventArgs e);
}