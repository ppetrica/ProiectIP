﻿using System.Collections.Generic;
using core;

/// <summary>
/// This module is a middleware (proxy) beetween database and highlevel module view.
/// </summary>
namespace database
{
    public interface IDatabase
    {
        /// <summary>
        /// Insert the list of countryInfo to the database, transferring raw data to IDbManager
        /// </summary>
        /// <param name="countryInfoList">List of Country Info to be inserted in the database</param>
        void InsertCountryData(IReadOnlyList<CountryInfo> countryInfoList);

        /// <returns>The most recent Date of the data from the database</returns>
        Date GetTheMostRecentDateOfData();
    }
}