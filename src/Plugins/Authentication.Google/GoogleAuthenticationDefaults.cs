﻿
namespace Authentication.Google
{
    /// <summary>
    /// Default values used by the Google authentication middleware
    /// </summary>
    public static class GoogleAuthenticationDefaults
    {
        /// <summary>
        /// System name of the external authentication method
        /// </summary>
        public const string ProviderSystemName = "ExternalAuth.Google";

        public const string ConfigurationUrl = "../GoogleAuthentication/Configure";
    }
}
