using System;
using System.Linq.Expressions;
using BindingEngine.Test.Helpers;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void Test_Extensions_Expression()
        {
            var expressionClass = new ExpressionClass();
            Assert.IsNull(expressionClass.Expression.GetName());

            expressionClass.SetProperty<TestViewModel>(i => i.Age);
            Assert.AreEqual("Age", expressionClass.Expression.GetName());

            expressionClass.SetProperty<TestViewModel>(i => i.TestViewModel2.TestViewModel3.IntValues);
            Assert.AreEqual("TestViewModel2.TestViewModel3.IntValues", expressionClass.Expression.GetName());

            expressionClass.SetProperty<TestViewModel>(i => i.TestViewModel2.TestViewModel3.IntValues[2]);
            Assert.AreEqual("TestViewModel2.TestViewModel3.IntValues[2]", expressionClass.Expression.GetName());
        }

        [TestMethod]
        public void Test_Extensions_String()
        {
            string testString = null;
            string s = testString.FirstRight(@"\");
            Assert.IsNull(s);
            s = testString.Right(@"\");
            Assert.IsNull(s);
            s = testString.LastLeft(@"\");
            Assert.IsNull(s);

            testString = "yohan";
            s = testString.FirstRight(@"\");
            Assert.AreEqual(s, string.Empty);
            s = testString.Right(@"\");
            Assert.AreEqual(s, string.Empty);
            s = testString.LastLeft(@"\");
            Assert.AreEqual(s, string.Empty);

            testString = @"yohan\";
            s = testString.FirstRight(@"\");
            Assert.AreEqual(s, string.Empty);
            s = testString.Right(@"\");
            Assert.AreEqual(s, string.Empty);


            testString = @"C:\Program Files (x86)\Microsoft\Exchange\Web Services\io.dll";

            string firstRight = testString.FirstRight(@"\");
            Assert.AreEqual(@"Program Files (x86)\Microsoft\Exchange\Web Services\io.dll", firstRight);

            string right = testString.Right(@"\");
            Assert.AreEqual(@"io.dll", right);

            string left = testString.LastLeft(@"\");
            Assert.AreEqual(@"C:\Program Files (x86)\Microsoft\Exchange\Web Services", left);
        }

        [TestMethod]
        public void Test_Extensions_Object()
        {
            const int value = 1;

            object result = value.ConvertTo(typeof(int));
            Assert.AreEqual(result, value);

            object result1 = value.ConvertTo(typeof(string));
            Assert.AreEqual(result1, "1");

            object result2 = value.ConvertTo(typeof(int?));
            Assert.AreEqual(result2, value);

            const string stringValue = "1";
            object result3 = stringValue.ConvertTo(typeof(int));
            Assert.AreEqual(1, result3);

            object result4 = stringValue.ConvertTo(null);
            Assert.AreEqual(stringValue, result4);
        }

        [TestMethod]
        public void Test_Extensions_Type()
        {
            Type type = null;
            object value = type.DefaultValue();
            Assert.IsNull(value);

            object intValue = typeof(int).DefaultValue();
            Assert.AreEqual(0, intValue);

            object stringValue = typeof(string).DefaultValue();
            Assert.AreEqual(null, stringValue);
        }

        private class ExpressionClass
        {
            public Expression Expression { get; private set; }

            public void SetProperty<TSource>(Expression<Func<TSource, object>> sourceProperty)
            {
                Expression = sourceProperty;
            }
        }
    }
}