namespace Kissinjer {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Kissinjer.Bindings;
	using Kissinjer.Scopes;

	public class Container : IContainer {
		private static readonly Type __type = typeof(Container);
		private static readonly MethodInfo __createTypeBindingBuilderMethod = __type.GetMethod(nameof(CreateTypeBindingBuilder), BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly MethodInfo __createFuncBindingBuilderMethod = __type.GetMethod(nameof(CreateFuncBindingBuilder), BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly MethodInfo __createConstantBindingBuilderMethod = __type.GetMethod(nameof(CreateConstantBindingBuilder), BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly MethodInfo __makeSpecificBindingBuilderMethod = __type.GetMethod(nameof(MakeSpecificBindingBuilder), BindingFlags.Instance | BindingFlags.NonPublic);

		private readonly Dictionary<Type, IScopeProvider> _scopeProvidersByType = new Dictionary<Type, IScopeProvider>();
		private readonly Dictionary<Type, HashSet<IBindingBuilder>> _buildersByType = new Dictionary<Type, HashSet<IBindingBuilder>>();
		private readonly Dictionary<Type, HashSet<IGenericBindingBuilder>> _genericBuildersByType = new Dictionary<Type, HashSet<IGenericBindingBuilder>>();

		private readonly HashSet<IBindingBuilder> _currentlyBuilding = new HashSet<IBindingBuilder>();
		private readonly Dictionary<IBindingBuilder, IBinding> _bindingsByBuilder = new Dictionary<IBindingBuilder, IBinding>();

		private readonly Dictionary<Type, HashSet<IBinding>> _bindings = new Dictionary<Type, HashSet<IBinding>>();

		public Container() {
			WithScopeProvider(new SingletonScopeProvider());
		}

		public IContainer WithBindingBuilder(IBindingBuilder builder) {
			if (builder == null) {
				throw new ArgumentNullException(nameof(builder));
			}

			if (!_buildersByType.TryGetValue(builder.ServiceType, out var bindingsList)) {
				_buildersByType[builder.ServiceType] = bindingsList = new HashSet<IBindingBuilder>();
			}

			bindingsList.Add(builder);

			return this;
		}

		public IContainer WithGenericBindingBuilder(IGenericBindingBuilder genericBuilder) {
			if (genericBuilder == null) {
				throw new ArgumentNullException(nameof(genericBuilder));
			}

			if (!_genericBuildersByType.TryGetValue(genericBuilder.ServiceType, out var bindingsSet)) {
				_genericBuildersByType[genericBuilder.ServiceType] = bindingsSet = new HashSet<IGenericBindingBuilder>();
			}

			bindingsSet.Add(genericBuilder);

			return this;
		}

		public IContainer WithScopeProvider(IScopeProvider scopeProvider) {
			if (scopeProvider == null) {
				throw new ArgumentNullException(nameof(scopeProvider));
			}

			_scopeProvidersByType.Add(scopeProvider.GetType(), scopeProvider);

			return this;
		}

		public IContainer Bind(Type serviceType, Type implementationType, Action<IBindingBuilder> composeAction = null) {
			TypeInfo serviceTypeInfo = serviceType.GetTypeInfo();
			TypeInfo implementationTypeInfo = implementationType.GetTypeInfo();

			if (serviceTypeInfo.IsGenericTypeDefinition) {
				IGenericBindingBuilder genericBuilder = new GenericTypeBindingBuilder(this, serviceType, implementationType);

				composeAction?.Invoke(genericBuilder);

				return WithGenericBindingBuilder(genericBuilder);
			} else {
				IBindingBuilder builder = (IBindingBuilder)__createTypeBindingBuilderMethod
					.MakeGenericMethod(serviceType, implementationType)
					.Invoke(this, Array.Empty<object>());

				composeAction?.Invoke(builder);

				return WithBindingBuilder(builder);
			}
		}

		private IBindingBuilder<TService> CreateTypeBindingBuilder<TService, TImplementation>() {
			return new TypeBindingBuilder<TService, TImplementation>(this);
		}

		public IContainer Bind(Type serviceType, Func<object> createFunc, Action<IBindingBuilder> composeAction = null) {
			TypeInfo serviceTypeInfo = serviceType.GetTypeInfo();

			if (serviceTypeInfo.IsGenericTypeDefinition) {
				IGenericBindingBuilder genericBuilder = new GenericFuncBindingBuilder(this, serviceType, createFunc);

				composeAction?.Invoke(genericBuilder);

				return WithGenericBindingBuilder(genericBuilder);
			} else {
				IBindingBuilder builder = (IBindingBuilder)__createFuncBindingBuilderMethod
					.MakeGenericMethod(serviceType)
					.Invoke(this, new object[] { createFunc });

				composeAction?.Invoke(builder);

				return WithBindingBuilder(builder);
			}
		}

		private IBindingBuilder<TService> CreateFuncBindingBuilder<TService>(Func<TService> createFunc) {
			return new FuncBindingBuilder<TService>(this, createFunc);
		}

		public IContainer Bind(Type serviceType, object value, Action<IBindingBuilder> composeAction = null) {
			TypeInfo serviceTypeInfo = serviceType.GetTypeInfo();

			if (serviceTypeInfo.IsGenericTypeDefinition) {
				IGenericBindingBuilder genericBuilder = new GenericConstantBindingBuilder(this, serviceType, value);

				composeAction?.Invoke(genericBuilder);

				return WithGenericBindingBuilder(genericBuilder);
			} else {
				IBindingBuilder builder = (IBindingBuilder)__createConstantBindingBuilderMethod
					.MakeGenericMethod(serviceType)
					.Invoke(this, new object[] { value });

				composeAction?.Invoke(builder);

				return WithBindingBuilder(builder);
			}
		}

		private IBindingBuilder<TService> CreateConstantBindingBuilder<TService>(TService value) {
			return new ConstantBindingBuilder<TService>(this, value);
		}

		internal bool TryGetBindingBuilders(Type serviceType, out IEnumerable<IBindingBuilder> builders) {
			if (!_buildersByType.TryGetValue(serviceType, out var builderSet)) {
				TypeInfo serviceTypeInfo = serviceType.GetTypeInfo();

				if (!serviceTypeInfo.IsGenericType) {
					builders = null;

					return false;
				}

				return TryGetSpecificBindingBuilders(serviceType, serviceTypeInfo, out builders);
			}

			builders = builderSet;

			return true;
		}

		private bool TryGetSpecificBindingBuilders(Type serviceType, TypeInfo serviceTypeInfo, out IEnumerable<IBindingBuilder> builders) {
			Type genericServiceType = serviceTypeInfo.GetGenericTypeDefinition();

			if (!_genericBuildersByType.TryGetValue(genericServiceType, out var builderSet)) {
				builders = null;

				return false;
			}
			
			MethodInfo makeSpecificBindingBuilderMethod = __makeSpecificBindingBuilderMethod
				.MakeGenericMethod(serviceType);

			HashSet<IBindingBuilder> specificBuilderSet = new HashSet<IBindingBuilder>();

			foreach (IGenericBindingBuilder builder in builderSet) {
				IBindingBuilder specificBuilder = (IBindingBuilder)makeSpecificBindingBuilderMethod.Invoke(this, new object[] { builder });

				specificBuilderSet.Add(specificBuilder);
			}

			_buildersByType.Add(serviceType, specificBuilderSet);

			builders = specificBuilderSet;

			return true;
		}

		private IBindingBuilder<TSpecificService> MakeSpecificBindingBuilder<TSpecificService>(IGenericBindingBuilder genericBuilder) {
			return genericBuilder.MakeSpecific<TSpecificService>();
		}

		internal TScopeProvider GetScopeProvider<TScopeProvider>()
			where TScopeProvider : IScopeProvider {

			if (!_scopeProvidersByType.TryGetValue(typeof(TScopeProvider), out var scopeProvider)) {
				throw new ScopeProviderNotFoundException();
			}

			return (TScopeProvider)scopeProvider;
		}

		public IContainer Build() {
			foreach (IBindingBuilder builder in _buildersByType.SelectMany(l => l.Value)) {
				GetOrBuildBinding(builder);
			}

			return this;
		}

		internal IBinding GetOrBuildBinding(IBindingBuilder builder) {
			if (_currentlyBuilding.Contains(builder)) {
				throw new BindingRecursionException();
			}

			if (!_bindingsByBuilder.TryGetValue(builder, out var binding)) {
				_currentlyBuilding.Add(builder);

				_bindingsByBuilder[builder] = binding = builder.Build();

				if (!_bindings.TryGetValue(binding.ServiceType, out var bindingsSet)) {
					_bindings[binding.ServiceType] = bindingsSet = new HashSet<IBinding>();
				}

				bindingsSet.Add(binding);

				_currentlyBuilding.Remove(builder);
			}

			return binding;
		}

		public virtual object Get(Type serviceType) {
			if (!_bindings.TryGetValue(serviceType, out var bindingsSet)) {
				if(!TryGetBindingBuilders(serviceType, out var builders)) {
					throw new BindingNotFoundException();
				}

				foreach(IBindingBuilder builder in builders) {
					GetOrBuildBinding(builder);
				}

				if(!_bindings.TryGetValue(serviceType, out bindingsSet)) {
					throw new BindingNotFoundException();
				}
			}

			if (bindingsSet.Count > 1) {
				throw new BindingNotUniqueException();
			}

			return bindingsSet.First().Invoke();
		}

		public TService Get<TService>() {
			return (TService)Get(typeof(TService));
		}

		public virtual bool TryGet(Type serviceType, out object service) {
			if (!_bindings.TryGetValue(serviceType, out var bindingsList)) {
				service = null;

				return false;
			}

			service = bindingsList.First().Invoke();

			return true;
		}

		public bool TryGet<TService>(out TService service) {
			if (TryGet(typeof(TService), out var rawService)) {
				service = (TService)rawService;

				return true;
			}

			service = default(TService);

			return false;
		}

		public virtual IEnumerable GetAll(Type serviceType) {
			if (!_bindings.TryGetValue(serviceType, out var bindingList)) {
				yield break;
			}

			foreach (IBinding binding in bindingList) {
				yield return binding.Invoke();
			}
		}

		public IEnumerable<TService> GetAll<TService>() {
			foreach (object service in GetAll(typeof(TService))) {
				yield return (TService)service;
			}
		}
	}
}
