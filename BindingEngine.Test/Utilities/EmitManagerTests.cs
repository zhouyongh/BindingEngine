using System;
using System.ComponentModel;
using System.Reflection;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    [TestClass]
    public class EmitManagerTests
    {
        [TestMethod]
        public void Test_EmitManager_CreateInstance()
        {
            var manager = new EmitManager();
            manager.ClearCache();
            var te = manager.CreateInstance<TestEmit>();
            Assert.IsNotNull(te);

            te = manager.CreateInstance<TestEmit>(2);
            Assert.IsNotNull(te);
            Assert.AreEqual(te.Age, 2);

            te = manager.CreateInstance(typeof(TestEmit)) as TestEmit;
            Assert.IsNotNull(te);

            te = manager.CreateInstance(typeof(TestEmit), 2) as TestEmit;
            Assert.IsNotNull(te);
        }

        [TestMethod]
        public void Test_EmitManager_Field()
        {
            var manager = new EmitManager();

            var te = manager.CreateInstance<TestEmit>();
            Assert.IsNull(te.Name);

            manager.SetField(te, "Age", 1);
            Assert.AreEqual(1, manager.GetField(te, "Age"));
            Assert.AreEqual(1, manager.GetField(te, typeof(TestEmit).GetField("Age")));

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
        public void Test_EmitManager_Property()
        {
            var manager = new EmitManager();

            var te = manager.CreateInstance<TestEmit>();
            Assert.IsNull(te.Name);

            const string name = "yohan";

            manager.SetProperty(te, "Name", name);
            Assert.AreEqual(name, manager.GetProperty(te, "Name"));

            manager.SetIndexProperty(te, 0, 2);
            Assert.AreEqual(2, manager.GetIndexProperty(te, 0));

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

            manager.SetIndexProperty(te, 0, 2);
            Assert.AreEqual(2, manager.GetIndexProperty(te, 0));
        }

        [TestMethod]
        public void Test_EmitManager_Method()
        {
            var manager = new EmitManager();

            var te = manager.CreateInstance<TestEmit>();
            Assert.IsNull(te.Name);

            manager.InvokeMethod(te, "SetAge", new object[] { 9 });
            Assert.AreEqual(9, te.Age);

            var age1 = (int)manager.InvokeMethod(te, "GetAge", null);
            Assert.AreEqual(9, age1);

            string propertyName = null;
            te.PropertyChanged += (sender, args) => { propertyName = args.PropertyName; };

            var methodInfo = manager.GetMethodInfo(te.GetType(), "OnPropertyChanged", new object[] { "parameter" });
            Assert.IsNotNull(methodInfo);

            manager.RaiseEvent(te, "PropertyChanged", new PropertyChangedEventArgs("Age"));
            Assert.AreEqual(propertyName, "Age");

            Exception exception = null;
            try
            {
                manager.InvokeMethod(te, "GetAgeEx", null);
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            exception = null;
            try
            {
                manager.RaiseEvent(te, "NoEvent", null);
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }

            Assert.IsNotNull(exception);

            manager.RegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension));

            methodInfo = manager.GetMethodInfo(te.GetType(), "GetAgeEx", null);
            Assert.IsNotNull(methodInfo);

            var age = (int)manager.InvokeMethod(te, "GetAgeEx", null);
            Assert.AreEqual(9, age);

            manager.UnRegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension));

            exception = null;
            try
            {
                manager.InvokeMethod(te, "GetAgeEx", null);
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            te.Name2 = "2";
            exception = null;
            try
            {
                DynamicEngine.RaiseEvent(te, "name2", new PropertyChangedEventArgs("Age"));
            }
            catch (MissingMemberException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            manager.RegisterExtensionType(typeof(TestEmit), typeof(TestEmitExtension2));
            age = (int)manager.InvokeMethod(te, "GetAgeEx2", null);
            Assert.AreEqual(9, age);

            TestEmit.AgeEx = 0;
            object age2 = manager.InvokeMethod(typeof(TestEmit), "AddAgeEx", null);
            Assert.AreEqual(1, age2);

            age2 = manager.InvokeMethod(te, manager.GetMethodInfo(typeof(TestEmit), "AddAgeEx", null), null);
            Assert.AreEqual(2, age2);

            age2 = manager.InvokeMethod(typeof(TestEmit), "SetAgeEx", new object[] { 5 });
            Assert.AreEqual(5, age2);

            age2 = manager.InvokeMethod(te, "SetAgeEx", new object[] { 7 });
            Assert.AreEqual(7, age2);

            const string name = "yohan";
            var parameters = new object[] { name, 1 };

            manager.InvokeMethod(te, "ChangeProperty", parameters);
            Assert.AreEqual("yohan1", parameters[0]);

            parameters = new object[] { name };
            manager.InvokeMethod(te, "ChangeProperty", parameters);
            Assert.AreEqual("yohan", parameters[0]);
        }
    }
}