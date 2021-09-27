using ChatCase.Business.Configuration.Common;
using ChatCase.Business.Interfaces.Configuration;
using ChatCase.Core.Caching;
using ChatCase.Core.Configuration.Settings;
using ChatCase.Core.Domain.Configuration;
using ChatCase.Core.Infrastructure;
using ChatCase.Repository.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ChatCase.Business.Services.Configuration
{
    /// <summary>
    /// Setting service
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Fields
        private readonly IRepository<Setting> _settingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        #endregion

        #region Ctor
        public SettingService(IRepository<Setting> settingRepository,
            IStaticCacheManager staticCacheManager)
        {
            _settingRepository = settingRepository;
            _staticCacheManager = staticCacheManager;
        }
        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IDictionary<string, IList<Setting>>> GetAllSettingsDictionaryAsync()
        {
            return await _staticCacheManager.GetAsync(SystemConfigurationDefaults.SettingsAllAsDictionaryCacheKey, async () =>
            {
                var settings = await GetListAsync();

                var dictionary = new Dictionary<string, IList<Setting>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new Setting
                    {
                        Name = s.Name,
                        Value = s.Value
                    };
                    if (!dictionary.ContainsKey(resourceName))
                        dictionary.Add(resourceName, new List<Setting>
                        {
                            settingForCaching
                        });
                    else
                        dictionary[resourceName].Add(settingForCaching);
                }

                return dictionary;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets setting by a key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = await GetAllSettingsDictionaryAsync();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault();

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Gets all setting
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IList<Setting>> GetListAsync()
        {
            var settings = await _settingRepository.GetListAsync();
            return settings;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public virtual async Task<T> LoadSettingAsync<T>() where T : ISettings, new()
        {
            return (T)await LoadSettingAsync(typeof(T));
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual async Task<ISettings> LoadSettingAsync(Type type)
        {
            var settings = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;

                var setting = await GetSettingByKeyAsync<string>(key);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns></returns>
        public virtual async Task ClearCacheAsync()
        {
            await _staticCacheManager.RemoveByPrefixAsync(EntityCacheDefaults<Setting>.Prefix);
        }

        #endregion
    }
}
