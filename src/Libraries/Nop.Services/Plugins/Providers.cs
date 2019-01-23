using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Plugins
{
    public partial class Providers<TPlugin> : IProviders<TPlugin> where TPlugin : class, IPlugin
    {
        private readonly IPluginService _pluginService;

        private readonly List<PluginDescriptor> _descriptors;

        IList<TPlugin> _allProviders;
        Dictionary<string, TPlugin> _providerBySysName;

        #region MyRegion

        /// <summary>
        /// Check whether to load the plugin based on the customer passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="customer">Customer</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByCustomer(PluginDescriptor pluginDescriptor, Customer customer)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            if (customer == null || !pluginDescriptor.LimitedToCustomerRoles.Any())
                return true;

            var customerRoleIds = customer.CustomerRoles.Where(role => role.Active).Select(role => role.Id);

            return pluginDescriptor.LimitedToCustomerRoles.Intersect(customerRoleIds).Any();
        }

        /// <summary>
        /// Check whether to load the plugin based on the store identifier passed
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Result of check</returns>
        protected virtual bool FilterByStore(PluginDescriptor pluginDescriptor, int storeId)
        {
            if (pluginDescriptor == null)
                throw new ArgumentNullException(nameof(pluginDescriptor));

            //no validation required
            if (storeId == 0)
                return true;

            if (!pluginDescriptor.LimitedToStores.Any())
                return true;

            return pluginDescriptor.LimitedToStores.Contains(storeId);
        }


        #endregion


        public Providers(IPluginService pluginService)
        {
            this._pluginService = pluginService;

            _providerBySysName = new Dictionary<string, TPlugin>();

            _descriptors = _pluginService.GetPluginDescriptors<TPlugin>().ToList();

        }

        /// <summary>
        /// Returns all providers
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>List of providers</returns>
        public virtual IList<TPlugin> LoadAllProviders(Customer customer = null, int storeId = 0)
        {
            if (_allProviders == null)
            {
                var pluginDescriptors = _descriptors.Where(descriptor =>
                    FilterByCustomer(descriptor, customer) &&
                    FilterByStore(descriptor, storeId));
                _allProviders = pluginDescriptors.Select(descriptor => descriptor.Instance<TPlugin>()).ToList();
                return _allProviders;
            }
            else
            {
                return _allProviders;
            }
        }

        /// <summary>
        /// Returns provider by system name
        /// </summary>
        /// <param name="systemNames">System name</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <returns>Provider</returns>
        public virtual TPlugin LoadProviderBySystemName(string systemName, Customer customer = null)
        {
            if (!_providerBySysName.ContainsKey(systemName))
            {
                var pluginDescriptors = _descriptors.Where(descriptor =>
                        FilterByCustomer(descriptor, customer)
                    );
                var pluginDescriptor = pluginDescriptors.FirstOrDefault(descriptor => descriptor.SystemName.Equals(
                        systemName,
                        StringComparison.InvariantCultureIgnoreCase)
                    );

                _providerBySysName.Add(systemName, pluginDescriptor?.Instance<TPlugin>());

                return pluginDescriptor?.Instance<TPlugin>();
            }
            else
            {
                return _providerBySysName[systemName];
            }


        }

        /// <summary>
        /// Returns all active providers
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>List of providers</returns>
        public virtual TPlugin LoadActiveProvider(string systemName, Customer customer = null, int storeId = 0)
        {
            var plugin = LoadProviderBySystemName(systemName, customer: customer) ??
                 LoadAllProviders(customer: customer, storeId: storeId).FirstOrDefault();

            return plugin;

        }

        /// <summary>
        /// Returns active providers by system name
        /// </summary>
        /// <param name="systemNames">System names</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>List of providers</returns>
        public virtual IList<TPlugin> LoadActiveProviders(List<string> systemNames, Customer customer = null, int storeId = 0)
        {
            return LoadAllProviders(customer: customer, storeId: storeId)
                .Where(provider => systemNames
                    .Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase)).ToList();
        }
    }

}
