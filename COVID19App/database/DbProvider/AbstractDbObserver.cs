﻿using System.Collections.Generic;
using core;

namespace database.DbProvider
{
    public abstract class AbstractDbObserver
    {
        /// <summary>
        /// Insert the list of countryInfo to the database, transferring raw data to IDbManager
        /// </summary>
        /// <param name="countryInfoList">List of Country Info to be inserted in the database</param>
        public abstract void InsertCountryData(IReadOnlyList<CountryInfo> countryInfoList);

        /// <returns>The most recent Date of the data from the database</returns>
        public abstract Date GetTheMostRecentDateOfData();
    }
}