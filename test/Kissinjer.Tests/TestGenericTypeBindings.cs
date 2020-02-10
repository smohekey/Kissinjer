namespace Kissinjer.Tests {
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	class TestGenericTypeBindings {
		[Test]
		public void BindGeneric() {
			IContainer container = new Container()
				.Bind(typeof(IList<>), typeof(List<>));

			IList<int> list = container.Get<IList<int>>();

			Assert.That(list, Is.Not.Null);
			Assert.That(list, Is.InstanceOf<List<int>>());
		}
	}
}
