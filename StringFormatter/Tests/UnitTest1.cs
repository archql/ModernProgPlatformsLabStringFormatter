using NUnit.Framework;

using lab5StringFormatter.Core;
using System;
using System.Collections.Generic;

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

            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(A)), Is.EqualTo(1));
            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(B)), Is.EqualTo(0));

            res = StringFormatter.Shared.Format("{{a}} = {a}; {{b}} = {b}", a);
            Assert.That(res, Is.EqualTo("{a} = 10; {b} = 22"));

            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(A)), Is.EqualTo(2));
            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(B)), Is.EqualTo(0));

            res = StringFormatter.Shared.Format("{{b.a}} = {a}", b);
            Assert.That(res, Is.EqualTo("{b.a} = {{a} = 10; {b} = 22}"));

            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(A)), Is.EqualTo(2));
            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(B)), Is.EqualTo(1));

            Assert.Throws<ArgumentNullException>(() =>
            {
                res = StringFormatter.Shared.Format("{a}", null);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format("{aa}", a);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format("{a{}}", a);
            });
            Assert.Throws<ArgumentException>(() =>
            {
                res = StringFormatter.Shared.Format("{a{a}a}", a);
            });

            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(A)), Is.EqualTo(2));
            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(B)), Is.EqualTo(1));
        }

        [Test]
        public void TestArrayAccess()
        {
            C c = new C();

            var res = StringFormatter.Shared.Format("{{c.a[0]}} = {a[0]}", c);
            Assert.That(res, Is.EqualTo("{c.a[0]} = 1"));
            res = StringFormatter.Shared.Format("c.a[1] = {a[1]}", c);
            Assert.That(res, Is.EqualTo("c.a[1] = 2"));
            res = StringFormatter.Shared.Format("c.a[2] = {a[2]}", c);
            Assert.That(res, Is.EqualTo("c.a[2] = 3"));

            res = StringFormatter.Shared.Format("c.a = {a}", c);
            Assert.That(res, Is.EqualTo("c.a = System.Int32[]"));

            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(C)), Is.EqualTo(4));

            // check cache hit
            res = StringFormatter.Shared.Format("c.a[2] = {a[2]}", c);
            Assert.That(res, Is.EqualTo("c.a[2] = 3"));

            Assert.That(StringFormatter.Shared.getChacheCountForType(typeof(C)), Is.EqualTo(4));
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

    class C
    {
        public int[] a = {1, 2, 3 };
    }
}