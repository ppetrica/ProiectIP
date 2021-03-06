﻿/*************************************************************************
 *                                                                        *
 *  File:        GlobalView.cs                                            *
 *  Copyright:   (c) 2020, Moisii Marin                                   *
 *  E-mail:      marin.moisii@student.tuiasi.ro                           *
 *  Description: This class represents the global data View from the      *
 *  application                                                           *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using Core;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Drawing;


namespace View
{
    /// <summary>
    /// Class responsible for creating a View with 
    /// statistics about COVID-19 effects on global level, 
    /// using the country info list.
    /// </summary>
    public class GlobalView : IView
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info">The information about COVID19 for each country.</param>
        public GlobalView(IReadOnlyList<CountryInfoEx> info)
        {
            _countries = info;
    
            DateTime startDate = _countries[0].DaysInfo[0].Date.ToDateTime();
            NumberOfDays = _countries[0].DaysInfo.Count;

            long[] confirmedEurope = new long[NumberOfDays];
            long[] confirmedAmerica = new long[NumberOfDays];
            long[] confirmedAfrica = new long[NumberOfDays];
            long[] confirmedAsia = new long[NumberOfDays];
            long[] confirmedOceania = new long[NumberOfDays];
            long[] confirmedOthers = new long[NumberOfDays];

            long globalConfirmed = 0;
            long globalDeaths = 0;
            long globalRecovered = 0;
            long globalPopulation = 0;

            long mostConfirmed = -1;
            string mostConfirmedCountry = string.Empty;

            long mostDeaths = -1;
            string mostDeathsCountry = string.Empty;

            long mostRecovered = -1;
            string mostRecoveredCountry = string.Empty;

            foreach (CountryInfoEx country in _countries)
            {
                foreach (DayInfo day in country.DaysInfo)
                {
                    int index = (day.Date.ToDateTime() - startDate).Days;
                    if (index < 0) // skip info from days that are before the chosen start date 
                        continue;
                    switch (country.Continent)
                    {
                        case "Europe":
                            confirmedEurope[index] += day.Confirmed;
                            break;
                        case "Asia":
                            confirmedAsia[index] += day.Confirmed;
                            break;
                        case "Africa":
                            confirmedAfrica[index] += day.Confirmed;
                            break;
                        case "Americas":
                            confirmedAmerica[index] += day.Confirmed;
                            break;
                        case "Oceania":
                            confirmedOceania[index] += day.Confirmed;
                            break;
                        case "Others":
                            confirmedOthers[index] += day.Confirmed;
                            break;
                    }
                }

                globalPopulation += country.Population;
                globalConfirmed += country.Confirmed;
                globalDeaths += country.Deaths;
                globalRecovered += country.Recovered;

                if (country.Confirmed > mostConfirmed)
                {
                    mostConfirmedCountry = country.Name;
                    mostConfirmed = country.Confirmed;
                }

                if (country.Deaths > mostDeaths)
                {
                    mostDeathsCountry = country.Name;
                    mostDeaths = country.Deaths;
                }

                if (country.Recovered > mostRecovered)
                {
                    mostRecoveredCountry = country.Name;
                    mostRecovered = country.Recovered;
                }
            }

            // build the chart with numbers of infected people on every region
            _cartesianChartRegions.Series = new SeriesCollection
            {
                new StackedAreaSeries
                {
                    Title = "Europe",
                    Values = GetRegionChartValues(confirmedEurope, startDate),
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "America",
                    Values = GetRegionChartValues(confirmedAmerica, startDate),
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Asia",
                    Values = GetRegionChartValues(confirmedAsia, startDate),
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Africa",
                    Values = GetRegionChartValues(confirmedAfrica, startDate),
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Oceania",
                    Values = GetRegionChartValues(confirmedOceania, startDate),
                    LineSmoothness = 0
                },
                new StackedAreaSeries
                {
                    Title = "Antarctica",
                    Values = GetRegionChartValues(confirmedOthers, startDate),
                    LineSmoothness = 0,
                    Fill = new SolidColorBrush(Colors.Green)
                },
            };

            _cartesianChartRegions.AxisX.Add(new Axis
            {
                LabelFormatter = value => new DateTime((long)value).ToString("dd-MMM")
            });
            _cartesianChartRegions.AxisY.Add(new Axis
            {
                Title = "Confirmed infected people", 
                LabelFormatter = value => ((long)value).ToString("N0")
            });

            _cartesianChartRegions.LegendLocation = LegendLocation.Right;

            // build the solid gauge for confirmed infected people rate
            _solidGaugeInfectioRate.From = 0;
            _solidGaugeInfectioRate.To = 100;
            _solidGaugeInfectioRate.Value = 100.0 * globalConfirmed / globalPopulation;
            _solidGaugeInfectioRate.Base.LabelsVisibility = Visibility.Hidden;
            _solidGaugeInfectioRate.LabelFormatter = value => $"  {value:F2}%\ninfected";
            _solidGaugeInfectioRate.Base.GaugeActiveFill = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.DarkOrange, 0),
                    new GradientStop(Colors.Gold, 0.5),
                    new GradientStop(Colors.Yellow, 1)
                }
            };

            // build the solid gauge for death rate
            _solidGaugeDeathRate.From = 0;
            _solidGaugeDeathRate.To = 100;
            _solidGaugeDeathRate.Value = 100.0 * globalDeaths / globalConfirmed;
            _solidGaugeDeathRate.Base.LabelsVisibility = Visibility.Hidden;
            _solidGaugeDeathRate.LabelFormatter = value => $" {value:F1}%\ndead";
            _solidGaugeDeathRate.Base.GaugeActiveFill = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.Firebrick, 0),
                    new GradientStop(Colors.Crimson, 0.5),
                    new GradientStop(Colors.Red, 1)
                }
            };

            // build the solid gauge for recovery rate
            _solidGaugeRecoveryRate.From = 0;
            _solidGaugeRecoveryRate.To = 100;
            _solidGaugeRecoveryRate.Value = 100.0 * globalRecovered / globalConfirmed;
            _solidGaugeRecoveryRate.Base.LabelsVisibility = Visibility.Hidden;
            _solidGaugeRecoveryRate.LabelFormatter = value => $"    {value:F1}%\nrecovered";
            _solidGaugeRecoveryRate.Base.GaugeActiveFill = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.DarkGreen, 0),
                    new GradientStop(Colors.MediumSeaGreen, 0.5),
                    new GradientStop(Colors.Chartreuse, 1)
                }
            };

            // buid the page layout as a table layout
            _layoutPanel.Dock = DockStyle.Fill;
            _layoutPanel.RowCount = 3;
            _layoutPanel.ColumnCount = 3;

            _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            _layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10));

            _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            _layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));


            // row 0 is empty, it exits for alignment

            // row 1
            _layoutPanel.Controls.Add(_cartesianChartRegions, 0, 1);
            _layoutPanel.SetColumnSpan(_cartesianChartRegions, 3);
            _cartesianChartRegions.Dock = DockStyle.Fill;

            // row 2 
            _layoutPanel.Controls.Add(_solidGaugeInfectioRate, 0, 2);
            _solidGaugeInfectioRate.Anchor = AnchorStyles.None;
            
            _layoutPanel.Controls.Add(_solidGaugeDeathRate, 1, 2);
            _solidGaugeDeathRate.Anchor = AnchorStyles.None;

            _layoutPanel.Controls.Add(_solidGaugeRecoveryRate, 2, 2);
            _solidGaugeRecoveryRate.Anchor = AnchorStyles.None;

            // row 3
            Font font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            _layoutPanel.Controls.Add(new Label
            {
                Text = $"Most confirmed: {mostConfirmedCountry} ({mostConfirmed:N0})",
                AutoSize = true,
                Font = font,
                Anchor = AnchorStyles.None
            }, 0, 3);

            _layoutPanel.Controls.Add(new Label
            {
                Text = $"Most dead: {mostDeathsCountry} ({mostDeaths:N0})",
                AutoSize = true,
                Font = font,
                Anchor = AnchorStyles.None
            }, 1, 3);

            _layoutPanel.Controls.Add(new Label
            {
                Text = $"Most recovered: {mostRecoveredCountry} ({mostRecovered:N0})",
                AutoSize = true,
                Font = font,
                Anchor = AnchorStyles.None
            }, 2, 3);

            _tabPage.Padding = new Padding(30);
            _tabPage.Controls.Add(_layoutPanel);
        }

        /// <summary>
        /// Returns the generated tab page control to be added in a form
        /// </summary>
        /// <returns>Generated tab page</returns>
        public TabPage GetPage()
        {
            return _tabPage;
        }

        private ChartValues<DateTimePoint> GetRegionChartValues(long[] region, DateTime startDate)
        {
            ChartValues<DateTimePoint> values = new ChartValues<DateTimePoint>();

            for (int i = 0; i < region.Length; ++i)
                values.Add(new DateTimePoint(startDate.AddDays(i), region[i]));
            
            return values;
        }


        readonly int NumberOfDays; // infection rate evolution is followed on last {numberOfDays} days

        private IReadOnlyList<CountryInfoEx> _countries;
        
        private LiveCharts.WinForms.CartesianChart _cartesianChartRegions = new LiveCharts.WinForms.CartesianChart();
        
        private LiveCharts.WinForms.SolidGauge _solidGaugeInfectioRate = new LiveCharts.WinForms.SolidGauge();
        private LiveCharts.WinForms.SolidGauge _solidGaugeDeathRate = new LiveCharts.WinForms.SolidGauge();
        private LiveCharts.WinForms.SolidGauge _solidGaugeRecoveryRate = new LiveCharts.WinForms.SolidGauge();

        private TableLayoutPanel _layoutPanel = new TableLayoutPanel();
        
        private TabPage _tabPage = new TabPage("Global statistics");
    }
}
