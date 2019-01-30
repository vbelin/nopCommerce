using Nop.Core.Domain.Customers;
using System.Collections.Generic;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents an provider manager
    /// </summary>
    /// <typeparam name="TPlugin">Type of plugin</typeparam>
    public interface IProviderManager<TPlugin> where TPlugin : class, IProvider
    {
        /// <summary>
        /// Returns all providers
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>List of providers</returns>
        IList<TPlugin> LoadAllProviders(Customer customer = null, int storeId = 0);

        /// <summary>
        /// Returns provider by system name
        /// </summary>
        /// <param name="systemNames">System name</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <returns>Provider</returns>
        TPlugin LoadProviderBySystemName(string systemName, Customer customer = null);

        /// <summary>
        /// Returns active provider
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Active provider</returns>
        TPlugin LoadActiveProvider(string systemName, Customer customer = null, int storeId = 0);

        /// <summary>
        /// Returns active providers
        /// </summary>
        /// <param name="systemNames">System names</param>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>List of active providers</returns>
        IList<TPlugin> LoadActiveProviders(List<string> systemNames, Customer customer = null, int storeId = 0);
    }
}
