// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents the view-model for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleDemo
{
    using OxyPlot;
    using OxyPlot.Series;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the view-model for the main window.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel { Title = "Simple example", Subtitle = "using OxyPlot" };

 
     //  OxyColors.Red





            for (int i = 0; i < 20; i++)
            {
                var line = new LineSeries() {Title="Line"+i,Color=OxyColors.Red };
                line.Points.Add(new DataPoint(0+i, 0+i));
                line.Points.Add(new DataPoint(10 + i, 18 + i));
                line.Points.Add(new DataPoint(20 + i, 12 + i));
                line.Points.Add(new DataPoint(30 + i, 8 + i));
                line.Points.Add(new DataPoint(40 + i, 15 + i));
                tmp.Series.Add(line);
            }

          

            // Axes are created automatically if they are not defined

            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            this.Model = tmp;
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel Model { get; private set; }
    }
}
