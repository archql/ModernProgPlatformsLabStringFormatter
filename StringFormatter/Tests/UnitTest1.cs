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
    }
}