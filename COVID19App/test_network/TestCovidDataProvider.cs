﻿/*************************************************************************
 *                                                                        *
 *  File:        TestCovidDataProvider.cs                                 *
 *  Copyright:   (c) 2020, Moisii Marin                                   *
 *  E-mail:      marin.moisii@student.tuiasi.ro                           *
 *  Description: This module is responsible for testing the               *
 *  ConvidDateProvider.                                                   *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using Network;
using Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace TestNetwork
{
    [TestClass]
    public class TestCovidDataProvider
    {
        [TestMethod]
        public void TestDataProvider()
        {
            Assert.ThrowsException<System.Net.WebException>(TestTimeout);
            Assert.ThrowsException<System.ArgumentException>(TestNegativeTimeout);
         
            TestRequest();
            TestParsedCovidInfo();
        }

        private void TestTimeout()
        {
            var dataProvider = new CovidDataProvider(-1) {Timeout = 100};

            Assert.AreEqual(100, dataProvider.Timeout);

            IReadOnlyList<CountryInfo> list = dataProvider.GetCountryData();
        }

        private void TestRequest()
        {
            IReadOnlyList<CountryInfo> list = (new CovidDataProvider()).GetCountryData();
            Assert.AreNotEqual(0, list.Count);
        }

        private void TestNegativeTimeout()
        {
            var dataProvider = new CovidDataProvider();
            dataProvider.Timeout = -100;
        }

        private void TestParsedCovidInfo()
        {
            IReadOnlyList<CountryInfo> countryInfoList = (new CovidDataProvider()).GetCountryData();

            Assert.AreEqual<DayInfo>(new DayInfo(new Date(2020, 3, 30), 170, 4, 2), countryInfoList[0].DaysInfo[68]);
            Assert.AreEqual("Afghanistan", countryInfoList[0].Name);

            Assert.AreEqual<DayInfo>(new DayInfo(new Date(2020, 3, 29), 745, 19, 72), countryInfoList[6].DaysInfo[67]);
            Assert.AreEqual("Argentina", countryInfoList[6].Name);

            Assert.AreEqual<DayInfo>(new DayInfo(new Date(2020, 3, 30), 11899, 513, 1527), countryInfoList[16].DaysInfo[68]);
            Assert.AreEqual("Belgium", countryInfoList[16].Name);
        }
    }
}
