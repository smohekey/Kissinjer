namespace Kissinjer {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Kissinjer.Bindings;
	using Kissinjer.Scopes;

	public class ContainerBuilder {
		private static readonly Type __type = typeof(ContainerBuilder);
		private static readonly MethodInfo __makeSpecificBindingBuilderMethod = __type.GetMethod(nameof(MakeSpecificBindingBuilder));

		private readonly Dictionary<Type, IScopeProvider> _scopeProvidersByType = new Dictionary<Type, IScopeProvider>();
		private readonly Dictionary<Type, HashSet<IBindingBuilder>> _buildersByType = new Dictionary<Type, HashSet<IBindingBuilder>>();
		private readonly Dictionary<Type, HashSet<IGenericBindingBuilder>> _genericBuildersByType = new Dictionary<Type, HashSet<IGenericBindingBuilder>>();

		private readonly HashSet<IBindingBuilder> _currentlyBuilding = new HashSet<IBindingBuilder>();
		private readonly Dictionary<IBindingBuilder, IBinding> _bindings = new Dictionary<IBindingBuilder, IBinding>();

		public ContainerBuilder() {
			WithScopeProvider(new SingletonScopeProvider());
		}

		public ContainerBuilder WithBindingBuilder(IBindingBuilder bindingBuilder) {
			if (bindingBuilder == null) {
				throw new ArgumentNullException(nameof(bindingBuilder));
			}

			if (!_buildersByType.TryGetValue(bindingBuilder.ServiceType, out var bindingsList)) {
				_buildersByType[bindingBuilder.ServiceType] = bindingsList = new HashSet<IBindingBuilder>();
			}

			bindingsList.Add(bindingBuilder);

			return this;
		}

		public ContainerBuilder WithGenericBindingBuilder(IGenericBindingBuilder genericBindingBuilder) {
			if (genericBindingBuilder == null) {
				throw new ArgumentNullException(nameof(genericBindingBuilder));
			}

			if (!_genericBuildersByType.TryGetValue(genericBindingBuilder.ServiceType, out var bindingsSet)) {
				_genericBuildersByType[genericBindingBuilder.ServiceType] = bindingsSet = new HashSet<IGenericBindingBuilder>();
			}

			bindingsSet.Add(genericBindingBuilder);

			return this;
		}

		internal bool TryGetBindingBuilders(Type serviceType, out IEnumerable<IBindingBuilder> bindingBuilders) {
			if (!_buildersByType.TryGetValue(serviceType, out var builderSet)) {
				TypeInfo serviceTypeInfo = serviceType.GetTypeInfo();

				if (!serviceTypeInfo.IsGenericType) {
					bindingBuilders = null;

					return false;
				}

				return TryGetSpecificBindingBuilders(serviceType, serviceTypeInfo, out bindingBuilders);
			}

			bindingBuilders = builderSet;

			return true;
		}

		private bool TryGetSpecificBindingBuilders(Type serviceType, TypeInfo serviceTypeInfo, out IEnumerable<IBindingBuilder> bindingBuilders) {
			Type genericServiceType = serviceTypeInfo.GetGenericTypeDefinition();

			if (!_genericBuildersByType.TryGetValue(genericServiceType, out var builderSet)) {
				bindingBuilders = null;

				return false;
			}

			MethodInfo makeSpecificBindingBuilderMethod = __makeSpecificBindingBuilderMethod
				.MakeGenericMethod(genericServiceType, serviceType);

			HashSet<IBindingBuilder> specificBuilderSet = new HashSet<IBindingBuilder>();

			foreach (IGenericBindingBuilder builder in builderSet) {
				IBindingBuilder specificBuilder = (IBindingBuilder)makeSpecificBindingBuilderMethod.Invoke(this, new object[] { builder });

				specificBuilderSet.Add(specificBuilder);
			}

			_buildersByType.Add(serviceType, specificBuilderSet);

			bindingBuilders = specificBuilderSet;

			return true;
		}

		private IBindingBuilder<TSpecificService> MakeSpecificBindingBuilder<TService, TSpecificService>(IGenericBindingBuilder<TService> genericBindingBuilder)
			where TSpecificService : TService {

			return genericBindingBuilder.MakeSpecific<TSpecificService>();
		}

		/*public IBindingBuilder GetBindingBuilder<TService>() {
			Type serviceType = typeof(TService);

			if (!_buildersByType.TryGetValue(serviceType, out var bindingsSet)) {
				TypeInfo serviceTypeInfo = serviceType.GetTypeInfo();

				if (!serviceTypeInfo.IsGenericType) {
					throw new BindingNotFoundException();
				}

				return GetSpecificBindingBuilder(serviceType, serviceTypeInfo);
			}

			if (bindingsSet.Count > 1) {
				throw new BindingNotUniqueException();
			}

			return bindingsSet.First();
		}

		private IBindingBuilder GetSpecificBindingBuilder(Type serviceType, TypeInfo serviceTypeInfo) {
			Type genericServiceType = serviceTypeInfo.GetGenericTypeDefinition();

			if (!_genericBuildersByType.TryGetValue(genericServiceType, out var bindingsSet)) {
				throw new BindingNotFoundException();
			}

			if (bindingsSet.Count > 1) {
				throw new BindingNotUniqueException();
			}

			return (IBindingBuilder)__makeSpecificBindingBuilderMethod
				.MakeGenericMethod(genericServiceType, serviceType)
				.Invoke(this, new object[] { bindingsSet.First() });
		}*/

		public ContainerBuilder WithScopeProvider(IScopeProvider scopeProvider) {
			if (scopeProvider == null) {
				throw new ArgumentNullException(nameof(scopeProvider));
			}

			_scopeProvidersByType.Add(scopeProvider.GetType(), scopeProvider);

			return this;
		}

		internal TScopeProvider GetScopeProvider<TScopeProvider>()
			where TScopeProvider : IScopeProvider {

			if (!_scopeProvidersByType.TryGetValue(typeof(TScopeProvider), out var scopeProvider)) {
				throw new ScopeProviderNotFoundException();
			}

			return (TScopeProvider)scopeProvider;
		}

		public IContainer Build() {
			return CreateContainer(BuildBindings());
		}

		protected virtual IContainer CreateContainer(IReadOnlyDictionary<Type, IReadOnlyCollection<IBinding>> bindings) {
			return new Container(bindings);
		}

		private IReadOnlyDictionary<Type, IReadOnlyCollection<IBinding>> BuildBindings() {
			Dictionary<Type, List<IBinding>> bindings = new Dictionary<Type, List<IBinding>>();

			foreach (IBindingBuilder builder in _buildersByType.SelectMany(l => l.Value)) {
				if(!_bindings.ContainsKey(builder)) {
					_bindings[builder] = builder.Build();
				}
			}

			return _bindings.Values.GroupBy(binding => binding.ServiceType).ToDictionary(
				grouping => grouping.Key,
				grouping => (IReadOnlyCollection<IBinding>)new List<IBinding>(grouping).AsReadOnly()
			);
		}

		internal IBinding GetOrBuildBinding(IBindingBuilder bindingBuilder) {
			if(_currentlyBuilding.Contains(bindingBuilder)) {
				throw new BindingRecursionException();
			}

			if(!_bindings.TryGetValue(bindingBuilder, out var binding)) {
				_currentlyBuilding.Add(bindingBuilder);

				_bindings[bindingBuilder] = binding = bindingBuilder.Build();
			}

			return binding;
		}
	}
}