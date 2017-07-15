namespace Kissinjer.Tests {
	using System.Collections.Generic;
	using NUnit.Framework;

	[TestFixture]
	class TestSingletonScope {
		[Test]
		public void SameObjectReturned() {
			IContainer container = new Container()
				.Bind<IList<int>, List<int>>(_ => _.InSingletonScope());

			IList<int> list1 = container.Get<IList<int>>();
			IList<int> list2 = container.Get<IList<int>>();

			Assert.AreEqual(list1, list2);
		}
	}
}
