namespace Kissinjer {
	using System;
	using System.Collections;

	public class NestedContainer : Container {
		private readonly IContainer _parent;

		public NestedContainer(IContainer parent) {
			_parent = parent;
		}

		public override object Get(Type serviceType) {
			if(!base.TryGet(serviceType, out var service)) {
				service = _parent.Get(serviceType);
			}

			return service;
		}

		public override bool TryGet(Type serviceType, out object service) {
			if(!base.TryGet(serviceType, out service)) {
				return _parent.TryGet(serviceType, out service);
			}

			return true;
		}

		public override IEnumerable GetAll(Type serviceType) {
			foreach(object service in base.GetAll(serviceType)) {
				yield return service;
			}

			foreach(object service in _parent.GetAll(serviceType)) {
				yield return service;
			}
		}
	}
}
