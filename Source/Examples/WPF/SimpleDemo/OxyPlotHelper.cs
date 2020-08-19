using OxyPlot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SimpleDemo
{
    public class OxyPlotHelper
    {

        public static bool GetIsChecked(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCheckedProperty);
        }

        public static void SetIsChecked(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCheckedProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollToEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached("IsChecked", typeof(bool), typeof(OxyPlotHelper), new PropertyMetadata(IsCheckedPropertyChanged));

        private static void IsCheckedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null && d is MenuItem)
            {
                var mi = d as MenuItem;

                if (mi.Tag != null && mi.Tag is PlotModel)
                {
                    var plotModel = mi.Tag as PlotModel;
                    plotModel.InvalidatePlot(false);
                    plotModel.ResetAllAxes();
                }

            }
        }
    }
}
