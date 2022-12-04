using NUnit.Framework;

using lab5StringFormatter.Core;
using System;

namespace lab5StringFormatter.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNoObjectGiven()
        {
            var res = StringFormatter.Shared.Format("", null);
            Assert.That(res, Is.EqualTo(""));

            res = StringFormatter.Shared.Format("a{{}}a", null);
            Assert.That(res, Is.EqualTo("a{}a"));

            res = StringFormatter.Shared.Format("a{{{{{{{{ }}}} }}}}a", null);
            Assert.That(res, Is.EqualTo("a{{{{ }} }}a"));

            res = StringFormatter.Shared.Format("a {{ bb }} {} {}", null);
            Assert.That(res, Is.EqualTo("a { bb }  "));

            Assert.Throws<ArgumentException>(() =>
            {
                var res = StringFormatter.Shared.Format(" {}{ ", null);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format(" {}{", null);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format(" {}} ", null);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format(" {}}", null);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format(" {}}{", null);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format(" {}}{ ", null);
            });
        }

        [Test]
        public void TestSimpleObjects()
        {
            A a = new A();
            B b = new B();

            var res = StringFormatter.Shared.Format("a{a}a", a);
            Assert.That(res, Is.EqualTo("a10a"));

            res = StringFormatter.Shared.Format("{{a}} = {a}; {{b}} = {b}", a);
            Assert.That(res, Is.EqualTo("{a} = 10; {b} = 22"));

            res = StringFormatter.Shared.Format("{{b.a}} = {a}", b);
            Assert.That(res, Is.EqualTo("{b.a} = {{a} = 10; {b} = 22}"));

            Assert.Throws<ArgumentNullException>(() =>
            {
                res = StringFormatter.Shared.Format("{a}", null);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format("{aa}", a);
            });
        }

    }

    class A
    {
        public A()
        {

        }

        public int a = 10;
        public string b = "22";

        public override string ToString()
        {
            return StringFormatter.Shared.Format("{{{{a}} = {a}; {{b}} = {b}}}", this);
        }
    }

    class B
    {
        public B()
        {

        }

        public A a = new A();
    }
}