﻿using Grand.Domain.Data;

namespace Grand.Infrastructure.Data
{
    public class MongoDBDataProvider : IDataProvider
    {
        #region Methods

        /// <summary>
        /// Initialize database
        /// </summary>
        public virtual void InitDatabase()
        {
            DataSettingsHelper.InitConnectionString();
        }

        #endregion

        public string ConnectionString => DataSettingsHelper.ConnectionString();

    }
}
