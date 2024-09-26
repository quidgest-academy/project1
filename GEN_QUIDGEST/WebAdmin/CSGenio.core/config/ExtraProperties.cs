using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CSGenio.config
{
    /// <summary>
    /// Handle specific project properties that are specific to this system. 
    /// They will be visible in the more properties area of Webadmin
    /// </summary>
    public static class ExtraProperties
    {
        private static Dictionary<string, string> initialProperties = new Dictionary<string, string>()
        {
            // Specify key-value pairs with the following notation: { "key", "value" },
            // Properties with empty/null value, will NOT be added,
            // but are displayed in the form dropdown for selection.
            // Notice: these properties cannot be deleted on WebAdmin,
            // because they will be recreated from the MANWIN.
// USE /[MANUAL PRO INITPROPERTIES]/
        };


        /// <summary>
        /// Add initial properties that should have a default value to an existing property list
        /// </summary>
        public static void AddMissingValues(SerializableDictionary<string, string>  propertyList)
        {
            foreach (var entry in initialProperties)
            {
                // Do not add properties with empty value
                if (String.IsNullOrEmpty(entry.Value))
                    continue;
                // Do not add already existing properties
                if (propertyList.ContainsKey(entry.Key))
                    continue;

                propertyList.Add(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Returns a list of initial properties that don't have null values
        /// </summary>        
        public static SerializableDictionary<string, string> GetInitialValues()
        {
            var result = new SerializableDictionary<string, string>();
            foreach (var entry in initialProperties)
            {
                // Do not add properties with empty value
                if (String.IsNullOrEmpty(entry.Value))
                    continue;

                result.Add(entry.Key, entry.Value);
            }
            return result;
        }

        public static List<string> GetInitialKeys()
        {
            return initialProperties.Keys.ToList();
        }

        public static bool HasDefaultValue(string key)
        {
            return initialProperties.ContainsKey(key) && !String.IsNullOrEmpty(initialProperties[key]);
        }


    }
}
