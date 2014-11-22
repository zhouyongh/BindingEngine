using System;
using System.ComponentModel;
using System.Reflection;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    using BindingEngine.Test.Annotations;

    [TestClass]
    public class DynamicEngineTests
    {
        [TestMethod]
        public void Test_DynamicEngine_CreateInstance()
        {
            DynamicEngine.ClearCache();
            var te = DynamicEngine.CreateInstance<TestEmit>();
            Assert.IsNotNull(te);

            te = DynamicEngine.CreateInstance<TestEmit>(2);
            Assert.IsNotNull(te);
            Assert.AreEqual(te.Age, 2);

            DynamicEngine.SetBindingManager(new ReflectManager());

            te = DynamicEngine.CreateInstance(typeof(TestEmit), 2) as TestEmit;
            Assert.IsNotNull(te);
            Assert.AreEqual(te.Age, 2);
        }

        [TestMethod]
        public void Test_DynamicEngine_Field()
        {
            var te = DynamicEngine.CreateInstance<TestEmit>();
            Assert.IsNull(te.Name);

            DynamicEngine.SetField(te, "Age", 1);
            Assert.AreEqual(1, DynamicEngine.GetField(te, "Age"));
            Assert.AreEqual(1, DynamicEngine.GetField(te, typeof(TestEmit).GetField("Age")));

            Exception exception = null;
            try
            {
                DynamicEngine.GetField(te, "Age2");
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            exception = null;
            try
            {
                DynamicEngine.SetField(te, "Age3", 8);
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void Test_DynamicEngine_Property()
        {
            var te = DynamicEngine.CreateInstance<TestEmit>();
            Assert.IsNull(te.Name);

            const string name = "yohan";

            DynamicEngine.SetProperty(te, "Name", name);
            Assert.AreEqual(name, DynamicEngine.GetProperty(te, "Name"));

            DynamicEngine.SetIndexProperty(te, 0, 2);
            Assert.AreEqual(2, DynamicEngine.GetIndexProperty(te, 0));

            Exception exception = null;
            try
            {
                DynamicEngine.GetProperty(te, "Name9");
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            exception = null;
            try
            {
                DynamicEngine.SetProperty(te, "Name9", 4);
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void Test_DynamicEngine_Method()
        {
            var te = DynamicEngine.CreateInstance<TestEmit>();
            Assert.IsNull(te.Name);

            DynamicEngine.InvokeMethod(te, "SetAge", new object[] { 9 });
            Assert.AreEqual(9, te.Age);

            var addVlue = (int)DynamicEngine.InvokeMethod(te, "Add", new object[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            Assert.AreEqual(10, addVlue);

            string propertyName = null;
            te.PropertyChanged += (sender, args) => { propertyName = args.PropertyName; };

            DynamicEngine.RaiseEvent(te, "PropertyChanged", new PropertyChangedEventArgs("Age"));
            Assert.AreEqual(propertyName, "Age");

            Exception exception = null;
            try
            {
                DynamicEngine.InvokeMethod(te, "GetAgeEx", null);
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            exception = null;
            try
            {
                DynamicEngine.RaiseEvent(te, "PropertyChanged22", new PropertyChangedEventArgs("Age"));
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            DynamicEngine.RegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension));
            DynamicEngine.RegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension));
            DynamicEngine.RegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension2));

            MethodInfo methodInfo = DynamicEngine.GetMethodInfo(te.GetType(), "GetAgeEx", null);
            Assert.IsNotNull(methodInfo);

            var age = (int)DynamicEngine.InvokeMethod(te, "GetAgeEx", null);
            Assert.AreEqual(9, age);

            DynamicEngine.UnRegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension));
            DynamicEngine.UnRegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension2));

            exception = null;
            try
            {
                DynamicEngine.InvokeMethod(te, "GetAgeEx", null);
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            DynamicEngine.RegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension2));
            age = (int)DynamicEngine.InvokeMethod(te, "GetAgeEx2", null);
            Assert.AreEqual(9, age);

            TestEmit.AgeEx = 0;
            object age1 = DynamicEngine.InvokeMethod(typeof(TestEmit), "AddAgeEx", null);
            Assert.AreEqual(1, age1);

            age1 = DynamicEngine.InvokeMethod(typeof(TestEmit), "SetAgeEx", new object[] { 5 });
            Assert.AreEqual(5, age1);

            const string name = "yohan";
            var parameters = new object[] { name, 1 };
            DynamicEngine.InvokeMethod(te, "ChangeProperty", parameters);
            Assert.AreEqual("yohan1", parameters[0]);

            parameters = new object[] { name };
            DynamicEngine.InvokeMethod(te, "ChangeProperty", parameters);
            Assert.AreEqual("yohan", parameters[0]);
        }
    }

    internal class TestEmit : INotifyPropertyChanged
    {
        public static int AgeEx;

        public int Age;

        private string name2;

        private string name3;

        public TestEmit()
        {
            ValueList = new int[2];
        }

        public TestEmit(int age)
            : this()
        {
            Age = age;
        }

        public int this[int index]
        {
            get { return ValueList[index]; }
            set { ValueList[index] = value; }
        }

        public string Name { get; set; }

        public string Name2
        {
            set
            {
                this.name2 = value;
            }
        }

        public string Name3
        {
            get
            {
                return this.name3;
            }
        }
        private int[] ValueList { get; set; }

        public static int AddAgeEx()
        {
            AgeEx++;
            return AgeEx;
        }

        public static int SetAgeEx(int age)
        {
            AgeEx = age;
            return AgeEx;
        }

        public void ChangeProperty(ref string name, int index)
        {
            name = name + index;
        }

        public void ChangeProperty(out string name)
        {
            name = "yohan";
        }

        public int Add(int a, int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9)
        {
            return a + a1 + a2 + a3 + a4 + a5 + a6 + a7 + a8 + a9;
        }

        public void SetAge(int age)
        {
            Age = age;
        }

        public int GetAge()
        {
            return Age;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    internal static class TestEmitExtension
    {
        public static int GetAgeEx(this TestEmit testEmit)
        {
            return testEmit.Age;
        }
    }

    internal static class TestEmitExtension2
    {
        public static int GetAgeEx2(this TestEmit testEmit)
        {
            return testEmit.Age;
        }
    }
}