using System;
using HttpServer;
using NUnit.Framework;

namespace HttpServer_Test
{
	[TestFixture]
	public class CheckTests
	{
		[Test]
		public void TestNotEmpty()
		{
            Check.NotEmpty("string", "error");
            Check.NotEmpty("another string", "another error");

            // Even though the argument to check is non empty an exception is expected,
            // if this was not the case the missing error message would only be discovered when the check fails (possibly in production)
            Assert.That(() => Check.NotEmpty("something not empty", null), Throws.TypeOf<ArgumentException>());

            Assert.That(() => Check.NotEmpty("", "error"), Throws.TypeOf<ArgumentException>());
            Assert.That(() => Check.NotEmpty("", "bad parameter"), Throws.TypeOf<ArgumentException>());
            Assert.That(() => Check.NotEmpty("", null), Throws.TypeOf<ArgumentException>());

            Assert.That(() => Check.NotEmpty(null, "null"), Throws.TypeOf<ArgumentException>());
            Assert.That(() => Check.NotEmpty(null, "cannot be null"), Throws.TypeOf<ArgumentException>());
            Assert.That(() => Check.NotEmpty(null, null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void testRequire()
        {
            Object someObject = "some string";

            Check.Require(someObject, "parameter");
            Check.Require(someObject, "error message");
            Assert.That(() => Check.Require(someObject, null), Throws.TypeOf<ArgumentException>());

            Assert.That(() => Check.Require(null, "null"), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => Check.Require(null, "cannot be null"), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => Check.Require(null, null), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void TestMin()
        {
            Check.Min(0, 100, "value");
            Check.Min(-100, 0, "value");
            Check.Min(int.MaxValue, int.MaxValue, "value");
            Assert.That(() => Check.Min(0, 100, null), Throws.TypeOf<ArgumentException>());

            Assert.That(() => Check.Min(100, 0, "value"), Throws.TypeOf<ArgumentException>());
            Assert.That(() => Check.Min(0, -100, "value"), Throws.TypeOf<ArgumentException>());
            Assert.That(() => Check.Min(int.MaxValue, int.MinValue, "value"), Throws.TypeOf<ArgumentException>());
        }
	}
}
