// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotView.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents a control that displays a <see cref="PlotModel" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;


    /// <summary>
    /// Represents a control that displays a <see cref="PlotModel" />.
    /// </summary>
    [TemplatePart(Name = PartGrid, Type = typeof(Grid))]
    public class PlotView : PlotBase
    { 

        /// <summary>
        /// Identifies the <see cref="Controller"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(IPlotController), typeof(PlotView));

        /// <summary>
        /// Identifies the <see cref="Model"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(PlotModel), typeof(PlotView), new PropertyMetadata(null, ModelChanged));

        /// <summary>
        /// The model lock.
        /// </summary>
        private readonly object modelLock = new object();

        /// <summary>
        /// The current model (synchronized with the <see cref="Model" /> property, but can be accessed from all threads.
        /// </summary>
        private PlotModel currentModel;

        /// <summary>
        /// The default plot controller.
        /// </summary>
        private IPlotController defaultController;

        /// <summary>
        /// Initializes static members of the <see cref="PlotView" /> class.
        /// </summary>
        static PlotView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlotView), new FrameworkPropertyMetadata(typeof(PlotView)));
            PaddingProperty.OverrideMetadata(typeof(PlotView), new FrameworkPropertyMetadata(new Thickness(8), AppearanceChanged));
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public PlotModel Model
        {
            get
            {
                return (PlotModel)this.GetValue(ModelProperty);
            }

            set
            {
                this.SetValue(ModelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Plot controller.
        /// </summary>
        /// <value>The Plot controller.</value>
        public IPlotController Controller
        {
            get
            {
                return (IPlotController)this.GetValue(ControllerProperty);
            }

            set
            {
                this.SetValue(ControllerProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual model.
        /// </summary>
        /// <value>The actual model.</value>
        public override PlotModel ActualModel
        {
            get
            {
                return this.currentModel;
            }
        }

        /// <summary>
        /// Gets the actual PlotView controller.
        /// </summary>
        /// <value>The actual PlotView controller.</value>
        public override IPlotController ActualController
        {
            get
            {
                return this.Controller ?? (this.defaultController ?? (this.defaultController = new PlotController()));
            }
        }

        /// <summary>
        /// Called when the visual appearance is changed.
        /// </summary>
        protected void OnAppearanceChanged()
        {
            this.InvalidatePlot(false);
        }

        /// <summary>
        /// Called when the visual appearance is changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void AppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlotView)d).OnAppearanceChanged();
        }

        /// <summary>
        /// Called when the model is changed.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlotView)d).OnModelChanged();
        }

        /// <summary>
        /// Called when the model is changed.
        /// </summary>
        private void OnModelChanged()
        {
            lock (this.modelLock)
            {
                if (this.currentModel != null)
                {
                    this.Model.Axes.CollectionChanged -= Model_AxesChanged;
                    this.Model.Annotations.CollectionChanged -= Model_AnnotationsChanged;
                    this.Model.Series.CollectionChanged -= Model_SeriesChanged;
                    ((IPlotModel)this.currentModel).AttachPlotView(null);
                    this.currentModel = null;
                }

                if (this.Model != null)
                {

                    this.SynchronizeProperties();
                    this.Model.Axes.CollectionChanged += Model_AxesChanged;
                    this.Model.Annotations.CollectionChanged += Model_AnnotationsChanged;
                    this.Model.Series.CollectionChanged += Model_SeriesChanged;
                    ((IPlotModel)this.Model).AttachPlotView(this);
                    this.currentModel = this.Model;
                }
            }

            this.InvalidatePlot();
        }

        #region BaseProperty
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UseStylePropertyProperty = DependencyProperty.Register(
            "UseStyleProperty",
            typeof(bool),
            typeof(PlotView),
            new PropertyMetadata(true));


        /// <summary>
        /// 
        /// </summary>
        public bool UseStyleProperty
        {
            get
            {
                return (bool)this.GetValue(UseStylePropertyProperty);
            }
            set
            {
                this.SetValue(UseStylePropertyProperty, value);
            }
        }

        ///// <summary>
        ///// Identifies the <see cref="Title"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        //    "Title", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        ///// <summary>
        ///// Gets or sets the title.
        ///// </summary>
        ///// <value>The title.</value>
        //public string Title
        //{
        //    get
        //    {
        //        return (string)this.GetValue(TitleProperty);
        //    }
        //    set
        //    {
        //        this.SetValue(TitleProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Identifies the <see cref="Subtitle"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        //    "Subtitle", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        ///// <summary>
        ///// Gets or sets the subtitle.
        ///// </summary>
        ///// <value>The subtitle.</value>
        //public string Subtitle
        //{
        //    get
        //    {
        //        return (string)this.GetValue(SubtitleProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(SubtitleProperty, value);
        //    }
        //}
        ///// <summary>
        ///// Identifies the <see cref="LegendTitle"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty LegendTitleProperty = DependencyProperty.Register(
        //    "LegendTitle", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        ///// <summary>
        ///// Gets or sets the legend title.
        ///// </summary>
        //public string LegendTitle
        //{
        //    get
        //    {
        //        return (string)this.GetValue(LegendTitleProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(LegendTitleProperty, value);
        //    }
        //}
        ///// <summary>
        ///// Identifies the <see cref="TitleToolTip"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty TitleToolTipProperty = DependencyProperty.Register(
        //    "TitleToolTip", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        ///// <summary>
        ///// Gets or sets the title tool tip.
        ///// </summary>
        ///// <value>The title tool tip.</value>
        //public string TitleToolTip
        //{
        //    get
        //    {
        //        return (string)this.GetValue(TitleToolTipProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(TitleToolTipProperty, value);
        //    }
        //}







        /// <summary>
        /// Identifies the <see cref="Culture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(
            "Culture", typeof(CultureInfo), typeof(PlotView), new UIPropertyMetadata(null, AppearanceChanged));
        /// <summary>
        /// Gets or sets Culture.
        /// </summary>
        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)this.GetValue(CultureProperty);
            }

            set
            {
                this.SetValue(CultureProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="IsLegendVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLegendVisibleProperty =
            DependencyProperty.Register(
                "IsLegendVisible", typeof(bool), typeof(PlotView), new PropertyMetadata(true, AppearanceChanged));

        /// <summary>
        /// Gets or sets a value indicating whether IsLegendVisible.
        /// </summary>
        public bool IsLegendVisible
        {
            get
            {
                return (bool)this.GetValue(IsLegendVisibleProperty);
            }

            set
            {
                this.SetValue(IsLegendVisibleProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty =
            DependencyProperty.Register(
                "LegendBackground", typeof(Color), typeof(PlotView), new PropertyMetadata(MoreColors.Undefined, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendBackground.
        /// </summary>
        public Color LegendBackground
        {
            get
            {
                return (Color)this.GetValue(LegendBackgroundProperty);
            }

            set
            {
                this.SetValue(LegendBackgroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendBorder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBorderProperty = DependencyProperty.Register(
            "LegendBorder", typeof(Color), typeof(PlotView), new PropertyMetadata(MoreColors.Undefined, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendBorder.
        /// </summary>
        public Color LegendBorder
        {
            get
            {
                return (Color)this.GetValue(LegendBorderProperty);
            }

            set
            {
                this.SetValue(LegendBorderProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBorderThicknessProperty =
            DependencyProperty.Register(
                "LegendBorderThickness", typeof(double), typeof(PlotView), new PropertyMetadata(1.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendBorderThickness.
        /// </summary>
        public double LegendBorderThickness
        {
            get
            {
                return (double)this.GetValue(LegendBorderThicknessProperty);
            }

            set
            {
                this.SetValue(LegendBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="LegendFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontProperty = DependencyProperty.Register(
            "LegendFont", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendFont.
        /// </summary>
        public string LegendFont
        {
            get
            {
                return (string)this.GetValue(LegendFontProperty);
            }

            set
            {
                this.SetValue(LegendFontProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontSizeProperty = DependencyProperty.Register(
            "LegendFontSize", typeof(double), typeof(PlotView), new PropertyMetadata(12.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendFontSize.
        /// </summary>
        public double LegendFontSize
        {
            get
            {
                return (double)this.GetValue(LegendFontSizeProperty);
            }

            set
            {
                this.SetValue(LegendFontSizeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontWeightProperty =
            DependencyProperty.Register(
                "LegendFontWeight",
                typeof(FontWeight),
                typeof(PlotView),
                new PropertyMetadata(FontWeights.Normal, AppearanceChanged));

        /// <summary>
        /// Gets or sets LegendFontWeight.
        /// </summary>
        public FontWeight LegendFontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(LegendFontWeightProperty);
            }

            set
            {
                this.SetValue(LegendFontWeightProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendItemAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemAlignmentProperty =
            DependencyProperty.Register(
                "LegendItemAlignment",
                typeof(HorizontalAlignment),
                typeof(PlotView),
                new PropertyMetadata(HorizontalAlignment.Left, AppearanceChanged));

        /// <summary>
        /// Gets or sets LegendItemAlignment.
        /// </summary>
        public HorizontalAlignment LegendItemAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(LegendItemAlignmentProperty);
            }

            set
            {
                this.SetValue(LegendItemAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendItemOrder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemOrderProperty =
            DependencyProperty.Register(
                "LegendItemOrder",
                typeof(LegendItemOrder),
                typeof(PlotView),
                new PropertyMetadata(LegendItemOrder.Normal, AppearanceChanged));

        /// <summary>
        /// Gets or sets LegendItemOrder.
        /// </summary>
        public LegendItemOrder LegendItemOrder
        {
            get
            {
                return (LegendItemOrder)this.GetValue(LegendItemOrderProperty);
            }

            set
            {
                this.SetValue(LegendItemOrderProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendItemSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendItemSpacingProperty =
            DependencyProperty.Register(
                "LegendItemSpacing", typeof(double), typeof(PlotView), new PropertyMetadata(24.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets the horizontal spacing between legend items when the orientation is horizontal.
        /// </summary>
        /// <value>The horizontal distance between items in device independent units.</value>
        public double LegendItemSpacing
        {
            get
            {
                return (double)this.GetValue(LegendItemSpacingProperty);
            }

            set
            {
                this.SetValue(LegendItemSpacingProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendLineSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendLineSpacingProperty =
            DependencyProperty.Register(
                "LegendLineSpacing", typeof(double), typeof(PlotView), new PropertyMetadata(0d, AppearanceChanged));
        /// <summary>
        /// Gets or sets the vertical spacing between legend items.
        /// </summary>
        /// <value>The spacing in device independent units.</value>
        public double LegendLineSpacing
        {
            get
            {
                return (double)this.GetValue(LegendLineSpacingProperty);
            }

            set
            {
                this.SetValue(LegendLineSpacingProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendMarginProperty = DependencyProperty.Register(
            "LegendMargin", typeof(double), typeof(PlotView), new PropertyMetadata(8.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendMargin.
        /// </summary>
        public double LegendMargin
        {
            get
            {
                return (double)this.GetValue(LegendMarginProperty);
            }

            set
            {
                this.SetValue(LegendMarginProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendMaxHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendMaxHeightProperty =
            DependencyProperty.Register("LegendMaxHeight", typeof(double), typeof(PlotView), new UIPropertyMetadata(double.NaN, AppearanceChanged));
        /// <summary>
        /// Gets or sets the max height of the legend.
        /// </summary>
        /// <value>The max width of the legends.</value>
        public double LegendMaxHeight
        {
            get
            {
                return (double)this.GetValue(LegendMaxHeightProperty);
            }

            set
            {
                this.SetValue(LegendMaxHeightProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendMaxWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendMaxWidthProperty =
            DependencyProperty.Register("LegendMaxWidth", typeof(double), typeof(PlotView), new UIPropertyMetadata(double.NaN, AppearanceChanged));
        /// <summary>
        /// Gets or sets the max width of the legend.
        /// </summary>
        /// <value>The max width of the legends.</value>
        public double LegendMaxWidth
        {
            get
            {
                return (double)this.GetValue(LegendMaxWidthProperty);
            }

            set
            {
                this.SetValue(LegendMaxWidthProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendOrientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendOrientationProperty =
            DependencyProperty.Register(
                "LegendOrientation",
                typeof(LegendOrientation),
                typeof(PlotView),
                new PropertyMetadata(LegendOrientation.Vertical, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendOrientation.
        /// </summary>
        public LegendOrientation LegendOrientation
        {
            get
            {
                return (LegendOrientation)this.GetValue(LegendOrientationProperty);
            }

            set
            {
                this.SetValue(LegendOrientationProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendColumnSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendColumnSpacingProperty =
            DependencyProperty.Register("LegendColumnSpacing", typeof(double), typeof(PlotView), new UIPropertyMetadata(8.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets the spacing between columns of legend items (only for vertical orientation).
        /// </summary>
        /// <value>The spacing in device independent units.</value>
        public double LegendColumnSpacing
        {
            get
            {
                return (double)this.GetValue(LegendColumnSpacingProperty);
            }

            set
            {
                this.SetValue(LegendColumnSpacingProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendPadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendPaddingProperty = DependencyProperty.Register(
            "LegendPadding", typeof(double), typeof(PlotView), new PropertyMetadata(8.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets the legend padding.
        /// </summary>
        public double LegendPadding
        {
            get
            {
                return (double)this.GetValue(LegendPaddingProperty);
            }

            set
            {
                this.SetValue(LegendPaddingProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendPlacementProperty =
            DependencyProperty.Register(
                "LegendPlacement",
                typeof(LegendPlacement),
                typeof(PlotView),
                new PropertyMetadata(LegendPlacement.Inside, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendPlacement.
        /// </summary>
        public LegendPlacement LegendPlacement
        {
            get
            {
                return (LegendPlacement)this.GetValue(LegendPlacementProperty);
            }

            set
            {
                this.SetValue(LegendPlacementProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendPositionProperty = DependencyProperty.Register(
            "LegendPosition",
            typeof(LegendPosition),
            typeof(PlotView),
            new PropertyMetadata(LegendPosition.RightTop, AppearanceChanged));
        /// <summary>
        /// Gets or sets the legend position.
        /// </summary>
        /// <value>The legend position.</value>
        public LegendPosition LegendPosition
        {
            get
            {
                return (LegendPosition)this.GetValue(LegendPositionProperty);
            }

            set
            {
                this.SetValue(LegendPositionProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendSymbolLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSymbolLengthProperty =
            DependencyProperty.Register(
                "LegendSymbolLength", typeof(double), typeof(PlotView), new PropertyMetadata(16.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendSymbolLength.
        /// </summary>
        public double LegendSymbolLength
        {
            get
            {
                return (double)this.GetValue(LegendSymbolLengthProperty);
            }

            set
            {
                this.SetValue(LegendSymbolLengthProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendSymbolMargin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSymbolMarginProperty =
            DependencyProperty.Register(
                "LegendSymbolMargin", typeof(double), typeof(PlotView), new PropertyMetadata(4.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendSymbolMargin.
        /// </summary>
        public double LegendSymbolMargin
        {
            get
            {
                return (double)this.GetValue(LegendSymbolMarginProperty);
            }

            set
            {
                this.SetValue(LegendSymbolMarginProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendSymbolPlacement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSymbolPlacementProperty =
            DependencyProperty.Register(
                "LegendSymbolPlacement",
                typeof(LegendSymbolPlacement),
                typeof(PlotView),
                new PropertyMetadata(LegendSymbolPlacement.Left, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendSymbolPlacement.
        /// </summary>
        public LegendSymbolPlacement LegendSymbolPlacement
        {
            get
            {
                return (LegendSymbolPlacement)this.GetValue(LegendSymbolPlacementProperty);
            }

            set
            {
                this.SetValue(LegendSymbolPlacementProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SelectionColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register(
            "SelectionColor", typeof(Color), typeof(PlotView), new PropertyMetadata(Colors.Yellow, AppearanceChanged));
        /// <summary>
        /// Gets or sets the color of selected elements.
        /// </summary>
        public Color SelectionColor
        {
            get
            {
                return (Color)this.GetValue(SelectionColorProperty);
            }

            set
            {
                this.SetValue(SelectionColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="RenderingDecorator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RenderingDecoratorProperty = DependencyProperty.Register(
            "RenderingDecorator", typeof(Func<IRenderContext, IRenderContext>), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        /// <summary>
        /// Gets or sets a rendering decorator.
        /// </summary>
        public Func<IRenderContext, IRenderContext> RenderingDecorator
        {
            get
            {
                return (Func<IRenderContext, IRenderContext>)this.GetValue(RenderingDecoratorProperty);
            }

            set
            {
                this.SetValue(RenderingDecoratorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SubtitleFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleFontProperty = DependencyProperty.Register(
            "SubtitleFont", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        /// <summary>
        /// Gets or sets the font of the subtitles.
        /// </summary>
        public string SubtitleFont
        {
            get
            {
                return (string)GetValue(SubtitleFontProperty);
            }

            set
            {
                SetValue(SubtitleFontProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="TitleColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleColorProperty = DependencyProperty.Register(
            "TitleColor", typeof(Color), typeof(PlotView), new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));
        /// <summary>
        /// Gets or sets the color of the titles.
        /// </summary>
        public Color TitleColor
        {
            get
            {
                return (Color)GetValue(TitleColorProperty);
            }

            set
            {
                SetValue(TitleColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SubtitleColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleColorProperty = DependencyProperty.Register(
            "SubtitleColor", typeof(Color), typeof(PlotView), new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));
        /// <summary>
        /// Gets or sets the color of the subtitles.
        /// </summary>
        public Color SubtitleColor
        {
            get
            {
                return (Color)GetValue(SubtitleColorProperty);
            }

            set
            {
                SetValue(SubtitleColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="DefaultFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultFontProperty = DependencyProperty.Register(
            "DefaultFont", typeof(string), typeof(PlotView), new PropertyMetadata("Segoe UI", AppearanceChanged));
        /// <summary>
        /// Gets or sets the default font.
        /// </summary>
        public string DefaultFont
        {
            get
            {
                return (string)GetValue(DefaultFontProperty);
            }

            set
            {
                SetValue(DefaultFontProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="DefaultFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultFontSizeProperty = DependencyProperty.Register(
            "DefaultFontSize", typeof(double), typeof(PlotView), new PropertyMetadata(12d, AppearanceChanged));
        /// <summary>
        /// Gets or sets the default font size.
        /// </summary>
        public double DefaultFontSize
        {
            get
            {
                return (double)GetValue(DefaultFontSizeProperty);
            }

            set
            {
                SetValue(DefaultFontSizeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="DefaultColors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultColorsProperty = DependencyProperty.Register(
            "DefaultColors",
            typeof(IList<Color>),
            typeof(PlotView),
            new PropertyMetadata(
                new[]
            {
                Color.FromRgb(0x4E, 0x9A, 0x06),
                    Color.FromRgb(0xC8, 0x8D, 0x00),
                    Color.FromRgb(0xCC, 0x00, 0x00),
                    Color.FromRgb(0x20, 0x4A, 0x87),
                    Colors.Red,
                    Colors.Orange,
                    Colors.Yellow,
                    Colors.Green,
                    Colors.Blue,
                    Colors.Indigo,
                    Colors.Violet
            },
                    AppearanceChanged));
        /// <summary>
        /// Gets or sets the default colors.
        /// </summary>
        public IList<Color> DefaultColors
        {
            get
            {
                return (IList<Color>)this.GetValue(DefaultColorsProperty);
            }

            set
            {
                this.SetValue(DefaultColorsProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="AxisTierDistance"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisTierDistanceProperty = DependencyProperty.Register(
            "AxisTierDistance", typeof(double), typeof(PlotView), new PropertyMetadata(4d, AppearanceChanged));
        /// <summary>
        /// Gets or sets the axis tier distance.
        /// </summary>
        public double AxisTierDistance
        {
            get
            {
                return (double)this.GetValue(AxisTierDistanceProperty);
            }

            set
            {
                this.SetValue(AxisTierDistanceProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendTextColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTextColorProperty = DependencyProperty.Register(
            "LegendTextColor", typeof(Color), typeof(PlotView), new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));

        /// <summary>
        /// Gets or sets the text color of the legends.
        /// </summary>
        public Color LegendTextColor
        {
            get
            {
                return (Color)this.GetValue(LegendTextColorProperty);
            }

            set
            {
                this.SetValue(LegendTextColorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="LegendTextColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleColorProperty = DependencyProperty.Register(
            "LegendTitleColor", typeof(Color), typeof(PlotView), new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));
        /// <summary>
        /// Gets or sets color of the legend title.
        /// </summary>
        public Color LegendTitleColor
        {
            get
            {
                return (Color)this.GetValue(LegendTitleColorProperty);
            }

            set
            {
                this.SetValue(LegendTitleColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendTitleFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleFontProperty =
            DependencyProperty.Register(
                "LegendTitleFont", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        /// <summary>
        /// Gets or sets LegendTitleFont.
        /// </summary>
        public string LegendTitleFont
        {
            get
            {
                return (string)this.GetValue(LegendTitleFontProperty);
            }

            set
            {
                this.SetValue(LegendTitleFontProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendTitleFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleFontSizeProperty =
            DependencyProperty.Register(
                "LegendTitleFontSize", typeof(double), typeof(PlotView), new PropertyMetadata(12.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets the font size of the legend titles.
        /// </summary>
        public double LegendTitleFontSize
        {
            get
            {
                return (double)this.GetValue(LegendTitleFontSizeProperty);
            }

            set
            {
                this.SetValue(LegendTitleFontSizeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="LegendTitleFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleFontWeightProperty =
            DependencyProperty.Register(
                "LegendTitleFontWeight",
                typeof(FontWeight),
                typeof(PlotView),
                new PropertyMetadata(FontWeights.Bold, AppearanceChanged));
        /// <summary>
        /// Gets or sets the font weight of the legend titles.
        /// </summary>
        public FontWeight LegendTitleFontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(LegendTitleFontWeightProperty);
            }

            set
            {
                this.SetValue(LegendTitleFontWeightProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="PlotAreaBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaBackgroundProperty =
            DependencyProperty.Register(
                "PlotAreaBackground",
                typeof(Brush),
                typeof(PlotView),
                new PropertyMetadata(null, AppearanceChanged));
        /// <summary>
        /// Gets or sets the background brush of the Plot area.
        /// </summary>
        /// <value>The brush.</value>
        public Brush PlotAreaBackground
        {
            get
            {
                return (Brush)this.GetValue(PlotAreaBackgroundProperty);
            }

            set
            {
                this.SetValue(PlotAreaBackgroundProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="PlotAreaBorderColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaBorderColorProperty =
            DependencyProperty.Register(
                "PlotAreaBorderColor",
                typeof(Color),
                typeof(PlotView),
                new PropertyMetadata(Colors.Black, AppearanceChanged));
        /// <summary>
        /// Gets or sets the color of the Plot area border.
        /// </summary>
        /// <value>The color of the Plot area border.</value>
        public Color PlotAreaBorderColor
        {
            get
            {
                return (Color)this.GetValue(PlotAreaBorderColorProperty);
            }

            set
            {
                this.SetValue(PlotAreaBorderColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="PlotAreaBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotAreaBorderThicknessProperty =
            DependencyProperty.Register(
                "PlotAreaBorderThickness", typeof(Thickness), typeof(PlotView), new PropertyMetadata(new Thickness(1.0), AppearanceChanged));
        /// <summary>
        /// Gets or sets the thickness of the Plot area border.
        /// </summary>
        /// <value>The thickness of the Plot area border.</value>
        public Thickness PlotAreaBorderThickness
        {
            get
            {
                return (Thickness)this.GetValue(PlotAreaBorderThicknessProperty);
            }

            set
            {
                this.SetValue(PlotAreaBorderThicknessProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="PlotMargins"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotMarginsProperty = DependencyProperty.Register(
            "PlotMargins",
            typeof(Thickness),
            typeof(PlotView),
            new PropertyMetadata(new Thickness(double.NaN), AppearanceChanged));
        /// <summary>
        /// Gets or sets the Plot margins.
        /// </summary>
        /// <value>The Plot margins.</value>
        public Thickness PlotMargins
        {
            get
            {
                return (Thickness)this.GetValue(PlotMarginsProperty);
            }

            set
            {
                this.SetValue(PlotMarginsProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="PlotType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotTypeProperty = DependencyProperty.Register(
            "PlotType", typeof(PlotType), typeof(PlotView), new PropertyMetadata(PlotType.XY, AppearanceChanged));
        /// <summary>
        /// Gets or sets PlotType.
        /// </summary>
        public PlotType PlotType
        {
            get
            {
                return (PlotType)this.GetValue(PlotTypeProperty);
            }

            set
            {
                this.SetValue(PlotTypeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SubtitleFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleFontSizeProperty =
            DependencyProperty.Register(
                "SubtitleFontSize", typeof(double), typeof(PlotView), new PropertyMetadata(14.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets the font size of the subtitle.
        /// </summary>
        public double SubtitleFontSize
        {
            get
            {
                return (double)this.GetValue(SubtitleFontSizeProperty);
            }

            set
            {
                this.SetValue(SubtitleFontSizeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="SubtitleFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SubtitleFontWeightProperty =
            DependencyProperty.Register(
                "SubtitleFontWeight",
                typeof(FontWeight),
                typeof(PlotView),
                new PropertyMetadata(FontWeights.Normal, AppearanceChanged));
        /// <summary>
        /// Gets or sets the font weight of the subtitle.
        /// </summary>
        public FontWeight SubtitleFontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(SubtitleFontWeightProperty);
            }

            set
            {
                this.SetValue(SubtitleFontWeightProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TextColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
            "TextColor", typeof(Color), typeof(PlotView), new PropertyMetadata(Colors.Black, AppearanceChanged));
        /// <summary>
        /// Gets or sets text color.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return (Color)this.GetValue(TextColorProperty);
            }

            set
            {
                this.SetValue(TextColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="TitleHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleAlignmentProperty =
            DependencyProperty.Register("TitleHorizontalAlignment", typeof(TitleHorizontalAlignment), typeof(PlotView), new PropertyMetadata(TitleHorizontalAlignment.CenteredWithinPlotArea, AppearanceChanged));
        /// <summary>
        /// Gets or sets the horizontal alignment of the title and subtitle.
        /// </summary>
        /// <value>
        /// The alignment.
        /// </value>
        public TitleHorizontalAlignment TitleHorizontalAlignment
        {
            get
            {
                return (TitleHorizontalAlignment)this.GetValue(TitleAlignmentProperty);
            }

            set
            {
                this.SetValue(TitleAlignmentProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFont"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontProperty = DependencyProperty.Register(
            "TitleFont", typeof(string), typeof(PlotView), new PropertyMetadata(null, AppearanceChanged));
        /// <summary>
        /// Gets or sets font of the title.
        /// </summary>
        public string TitleFont
        {
            get
            {
                return (string)this.GetValue(TitleFontProperty);
            }

            set
            {
                this.SetValue(TitleFontProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(
            "TitleFontSize", typeof(double), typeof(PlotView), new PropertyMetadata(18.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets font size of the title.
        /// </summary>
        public double TitleFontSize
        {
            get
            {
                return (double)this.GetValue(TitleFontSizeProperty);
            }

            set
            {
                this.SetValue(TitleFontSizeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontWeightProperty =
            DependencyProperty.Register(
                "TitleFontWeight",
                typeof(FontWeight),
                typeof(PlotView),
                new PropertyMetadata(FontWeights.Bold, AppearanceChanged));
        /// <summary>
        /// Gets or sets font weight of the title.
        /// </summary>
        public FontWeight TitleFontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(TitleFontWeightProperty);
            }

            set
            {
                this.SetValue(TitleFontWeightProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="TitlePadding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitlePaddingProperty = DependencyProperty.Register(
            "TitlePadding", typeof(double), typeof(PlotView), new PropertyMetadata(6.0, AppearanceChanged));
        /// <summary>
        /// Gets or sets padding around the title.
        /// </summary>
        public double TitlePadding
        {
            get
            {
                return (double)this.GetValue(TitlePaddingProperty);
            }

            set
            {
                this.SetValue(TitlePaddingProperty, value);
            }
        }





        #endregion

        #region AxeProperty
        /// <summary>
        /// Identifies the <see cref="AxislineColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxislineColorProperty = DependencyProperty.Register(
            "AxislineColor", typeof(Color), typeof(PlotView), new UIPropertyMetadata(Colors.Black, AppearanceChanged));
        /// <summary>
        /// 
        /// </summary>
        public Color AxislineColor
        {
            get
            {
                return (Color)this.GetValue(AxislineColorProperty);
            }

            set
            {
                this.SetValue(AxislineColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="AxislineStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxislineStyleProperty = DependencyProperty.Register(
            "AxislineStyle", typeof(LineStyle), typeof(PlotView), new UIPropertyMetadata(LineStyle.None, AppearanceChanged));
        /// <summary>
        /// 
        /// </summary>
        public LineStyle AxislineStyle
        {
            get
            {
                return (LineStyle)this.GetValue(AxislineStyleProperty);
            }

            set
            {
                this.SetValue(AxislineStyleProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="AxisTextColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisTextColorProperty = DependencyProperty.Register(
            "AxisTextColor", typeof(Color), typeof(PlotView), new PropertyMetadata(MoreColors.Automatic, AppearanceChanged));
        
        
        
        /// <summary>
        /// 
        /// </summary>
        public Color AxisTextColor
        {
            get
            {
                return (Color)this.GetValue(AxisTextColorProperty);
            }

            set
            {
                this.SetValue(AxisTextColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="AxisTicklineColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisTicklineColorProperty = DependencyProperty.Register(
            "AxisTicklineColor", typeof(Color), typeof(PlotView), new PropertyMetadata(Colors.Black, AppearanceChanged));
       
        
        /// <summary>
        /// 
        /// </summary>
        public Color AxisTicklineColor
        {
            get
            {
                return (Color)this.GetValue(AxisTicklineColorProperty);
            }

            set
            {
                this.SetValue(AxisTicklineColorProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="AxisTitleColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisTitleColorProperty = DependencyProperty.Register(
            "AxisTitleColor", typeof(Color), typeof(PlotView), new UIPropertyMetadata(OxyColors.Automatic.ToColor(), AppearanceChanged));
        
        
        /// <summary>
        /// 
        /// </summary>
        public Color AxisTitleColor
        {
            get
            {
                return (Color)this.GetValue(AxisTitleColorProperty);
            }

            set
            {
                this.SetValue(AxisTitleColorProperty, value);
            }
        }

        #endregion
        /// <summary>
        /// Synchronize properties in the internal Plot model
        /// </summary>
        private void SynchronizeProperties()
        {
            if (UseStyleProperty)
            {
                var m = this.Model;

                //m.Title = this.Title;
                //m.Subtitle = this.Subtitle;
                //m.LegendTitle = this.LegendTitle;
                //m.TitleToolTip = this.TitleToolTip;




                m.PlotType = this.PlotType;

                m.PlotMargins = this.PlotMargins.ToOxyThickness();
                m.Padding = this.Padding.ToOxyThickness();
                m.TitlePadding = this.TitlePadding;

                m.Culture = this.Culture;

                m.DefaultColors = this.DefaultColors.Select(c => c.ToOxyColor()).ToArray();
                m.DefaultFont = this.DefaultFont;
                m.DefaultFontSize = this.DefaultFontSize;


                m.TitleColor = this.TitleColor.ToOxyColor();
                m.TitleFont = this.TitleFont;
                m.TitleFontSize = this.TitleFontSize;
                m.TitleFontWeight = this.TitleFontWeight.ToOpenTypeWeight();



                m.SubtitleColor = this.SubtitleColor.ToOxyColor();
                m.SubtitleFont = this.SubtitleFont;
                m.SubtitleFontSize = this.SubtitleFontSize;
                m.SubtitleFontWeight = this.SubtitleFontWeight.ToOpenTypeWeight();

                m.TextColor = this.TextColor.ToOxyColor();
                m.SelectionColor = this.SelectionColor.ToOxyColor();

                m.RenderingDecorator = this.RenderingDecorator;

                m.AxisTierDistance = this.AxisTierDistance;

                m.IsLegendVisible = this.IsLegendVisible;
                m.LegendTextColor = this.LegendTextColor.ToOxyColor();

                m.LegendTitleColor = this.LegendTitleColor.ToOxyColor();
                m.LegendTitleFont = this.LegendTitleFont;
                m.LegendTitleFontSize = this.LegendTitleFontSize;
                m.LegendTitleFontWeight = this.LegendTitleFontWeight.ToOpenTypeWeight();
                m.LegendFont = this.LegendFont;
                m.LegendFontSize = this.LegendFontSize;
                m.LegendFontWeight = this.LegendFontWeight.ToOpenTypeWeight();
                m.LegendSymbolLength = this.LegendSymbolLength;
                m.LegendSymbolMargin = this.LegendSymbolMargin;
                m.LegendPadding = this.LegendPadding;
                m.LegendColumnSpacing = this.LegendColumnSpacing;
                m.LegendItemSpacing = this.LegendItemSpacing;
                m.LegendLineSpacing = this.LegendLineSpacing;
                m.LegendMargin = this.LegendMargin;
                m.LegendMaxHeight = this.LegendMaxHeight;
                m.LegendMaxWidth = this.LegendMaxWidth;

                m.LegendBackground = this.LegendBackground.ToOxyColor();
                m.LegendBorder = this.LegendBorder.ToOxyColor();
                m.LegendBorderThickness = this.LegendBorderThickness;

                m.LegendPlacement = this.LegendPlacement;
                m.LegendPosition = this.LegendPosition;
                m.LegendOrientation = this.LegendOrientation;
                m.LegendItemOrder = this.LegendItemOrder;
                m.LegendItemAlignment = this.LegendItemAlignment.ToHorizontalAlignment();
                m.LegendSymbolPlacement = this.LegendSymbolPlacement;

                m.PlotAreaBackground = this.PlotAreaBackground.ToOxyColor();
                m.PlotAreaBorderColor = this.PlotAreaBorderColor.ToOxyColor();
                m.PlotAreaBorderThickness = this.PlotAreaBorderThickness.ToOxyThickness();

                foreach (var item in m.Axes)
                {
                    item.AxislineColor = this.AxislineColor.ToOxyColor();
                    item.AxislineStyle = this.AxislineStyle;
                    item.TextColor = this.AxisTextColor.ToOxyColor();
                    item.TicklineColor = this.AxisTicklineColor.ToOxyColor();
                    item.TitleColor = this.AxisTitleColor.ToOxyColor();
                }

            }

        }

        private void Model_SeriesChanged(object sender, ElementCollectionChangedEventArgs<OxyPlot.Series.Series> e)
        {
            if (UseStyleProperty)
            {
            }
        }

        private void Model_AnnotationsChanged(object sender, ElementCollectionChangedEventArgs<Annotations.Annotation> e)
        {
            if (UseStyleProperty)
            {
            }
        }

        private void Model_AxesChanged(object sender, ElementCollectionChangedEventArgs<Axes.Axis> e)
        {
            if (UseStyleProperty)
            {
                foreach (var item in e?.AddedItems)
                {
                    item.AxislineColor = this.AxislineColor.ToOxyColor();
                    item.AxislineStyle = this.AxislineStyle;
                    item.TextColor = this.AxisTextColor.ToOxyColor();
                    item.TicklineColor = this.AxisTicklineColor.ToOxyColor();
                    item.TitleColor = this.AxisTitleColor.ToOxyColor();
                }
            }
        }











    }
}
