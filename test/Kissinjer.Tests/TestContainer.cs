namespace Kissinjer.Tests {
	using NUnit.Framework;

	[TestFixture]
	public class TestContainer {
		public interface IThing {
			void Method();
		}

		public interface IBar {
			void Method();
		}

		public class Thing : IThing {
			public void Method() {
				
			}
		}

		public class Thing2 : IThing {
			public Thing2(IBar bar) {

			}

			public void Method() {

			}
		}

		public class Bar : IBar {
			public void Method() {
				
			}
		}

		[Test]
		public void TestDefaultConstructor() {
			IContainer container = new Container()
				.Bind<IThing, Thing>(); ;

			IThing thing = container.Get<IThing>();

			Assert.That(thing, Is.Not.Null);
			Assert.That(thing, Is.InstanceOf<IThing>());
		}

		[Test]
		public void TestSingleParameterConstructorNotResolved() {
			IContainer container = new Container()
				.Bind<IThing, Thing2>();

			Assert.Throws<BindingNotResolvedException>(() => container.Build());
		}

		[Test]
		public void TestSingleParameterConstructor() {
			IContainer container = new Container()
				.Bind<IThing, Thing2>()
				.Bind<IBar, Bar>();

			IThing thing = container.Get<IThing>();

			Assert.That(thing, Is.Not.Null);
			Assert.That(thing, Is.InstanceOf<IThing>());
		}
	}
}
