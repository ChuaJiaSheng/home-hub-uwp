using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;

namespace Project2_222257U
{
    public static class GlobalSettings
    {
        // Property to store and get the current language
        public static string CurrentLanguage
        {
            get
            {
                // Return the primary language override, defaulting to the system's language
                return ApplicationLanguages.PrimaryLanguageOverride ?? ApplicationLanguages.Languages[0];
            }
            set
            {
                // Set the language override for the app
                ApplicationLanguages.PrimaryLanguageOverride = value;
            }
        }
    }
}
