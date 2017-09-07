using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ActivizWPF.Commands;
using ActivizWPF.Framework.Services;
using ActivizWPF.Models;
using ActivizWPF.Windows;
using ActivizWPF.Python.Numpy;
using IronPython.Runtime;
using Kitware.VTK;
using Microsoft.Practices.Unity;
using Point = System.Windows.Point;

namespace ActivizWPF.ViewModels
{
    public class VtkChartXyViewModel : VtkChartViewModel
    {
        #region Constants

        private const string DEFAULT_X_ARRAY = "X_array";
        private const string DEFAULT_Y_ARRAY = "DATA";
        private const int PIXEL_RADIUS = 10;

        #endregion

        #region Fields

        private FloatingTextBox _textBox;
        private vtkAxis _lAxis;
        private readonly double?[] _lAxisValues = { null, null };
        private vtkAxis _bAxis;
        private readonly double?[] _bAxisValues = { null, null };
        private vtkAxis _rAxis;
        private readonly double?[] _rAxisValues = { null, null };
        private vtkAxis _tAxis;
        private readonly double?[] _tAxisValues = { null, null };
        private vtkAxis _currentAxis;

        private int numPlots = 0;

        #endregion

        #region Properties

        public IList<IVtkPlotItemModel> Plots
        {
            get
            {
                VtkChartXyModel model = this.Model as VtkChartXyModel;

                return model == null ? null : model.Plots;
            }
        }

        public bool IsAutoAxesOn
        {
            get
            {
                vtkChartXY chart = this._chartItem as vtkChartXY;

                return chart != null && chart.GetAutoAxes();
            }

            set
            {
                vtkChartXY chart = this._chartItem as vtkChartXY;
                if (chart == null)
                    return;

                chart.SetAutoAxes(value);

                OnPropertyChanged();
                this.Render();
            }
        }

        public bool DrawAxesAtOrigin
        {
            get
            {
                vtkChartXY chart = this._chartItem as vtkChartXY;

                return chart != null && chart.GetDrawAxesAtOrigin();
            }
            set
            {
                vtkChartXY chart = this._chartItem as vtkChartXY;
                if (chart == null)
                    return;

                chart.SetDrawAxesAtOrigin(value);

                OnPropertyChanged();
                this.Render();
            }
        }

        public double LeftAxisMinimum
        {
            get { return _lAxis.GetMinimum(); }
            set
            {
                _lAxis.SetMinimum(value);
                _lAxisValues[0] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        public double LeftAxisMaximum
        {
            get { return _lAxis.GetMaximum(); }
            set
            {
                _lAxis.SetMaximum(value);
                _lAxisValues[1] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        public double BottomAxisMinimum
        {
            get { return _bAxis.GetMinimum(); }
            set
            {
                _bAxis.SetMinimum(value);
                _bAxisValues[0] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        public double BottomAxisMaximum
        {
            get { return _bAxis.GetMaximum(); }
            set
            {
                _bAxis.SetMaximum(value);
                _bAxisValues[1] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        public double RightAxisMinimum
        {
            get { return _rAxis.GetMinimum(); }
            set
            {
                _rAxis.SetMinimum(value);
                _rAxisValues[0] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        public double RightAxisMaximum
        {
            get { return _rAxis.GetMinimum(); }
            set
            {
                _rAxis.SetMinimum(value);
                _rAxisValues[1] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        public double TopAxisMinimum
        {
            get { return _tAxis.GetMinimum(); }
            set
            {
                _tAxis.SetMinimum(value);
                _tAxisValues[0] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        public double TopAxisMaximum
        {
            get { return _tAxis.GetMaximum(); }
            set
            {
                _tAxis.SetMaximum(value);
                _tAxisValues[1] = value;

                OnPropertyChanged();
                this.Render();
            }
        }

        #endregion

        #region Constructors

        public VtkChartXyViewModel()
        {
            Create();
        }

        public VtkChartXyViewModel(IUnityContainer container, IGuiThreadDispatchService dispatchService = null, IEventAggregator events = null)
            : base(container, dispatchService, events)
        {
            Create();
        }

        protected virtual void Create()
        {
            _chartItem = new vtkChartXY();

            _textBox = new FloatingTextBox();

            vtkChartXY chart = (vtkChartXY) _chartItem;

            _lAxis = chart.GetAxis((int)vtkAxis.Location.LEFT);
            vtkTextProperty titleProps = _lAxis.GetTitleProperties();
            titleProps.SetOpacity(0.0);

            _bAxis = chart.GetAxis((int)vtkAxis.Location.BOTTOM);
            titleProps = _bAxis.GetTitleProperties();
            titleProps.SetOpacity(0.0);

            _rAxis = chart.GetAxis((int)vtkAxis.Location.RIGHT);
            titleProps = _rAxis.GetTitleProperties();
            titleProps.SetOpacity(0.0);

            _tAxis = chart.GetAxis((int)vtkAxis.Location.TOP);
            titleProps = _tAxis.GetTitleProperties();
            titleProps.SetOpacity(0.0);
        }

        #endregion

        #region ViewModelBase

        protected override void Initialize()
        {
            base.Initialize();

            IVtkChartXyModel model = this.Model as IVtkChartXyModel;
            vtkChartXY chart = this._chartItem as vtkChartXY;

            if (model != null && chart != null)
            {
            }
        }

        protected override void RegisterCommandBindings()
        {
            base.RegisterCommandBindings();

            this.CommandBindings.Add(new CommandBinding(GraphCommands.AddMarker, OnAddMarker));
        }

        private void OnAddMarker(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            this.Log.Info("Add Marker...");
        }

        #endregion
        
        #region Public Methods

        public virtual void ClearPlots()
        {
            vtkChartXY chart = this._chartItem as vtkChartXY;
            if (chart == null)
                return;

            chart.ClearPlots();

            VtkChartXyModel model = this.Model as VtkChartXyModel;
            if (model == null)
                return;

            foreach (IVtkPlotItemModel plotItemModel in model.Plots)
            {
                plotItemModel.PropertyChanged -= OnPlotItemChanged;
            }

            model.Plots.Clear();

            numPlots = 0;

            this.Render();
        }

        /// <summary>
        /// Plots python arrays.
        /// </summary>
        /// <param name="context"> The context. This is automatically populated from python. </param>
        /// <param name="array"> The data array. Will only accept 1D and 2D arrays. 2D arrays use the first array as the x-array. </param>
        /// <param name="names"> The names array. This is not necessary. </param>
        /// <param name="type"> The type of plot. Accepts derived classes of VtkPlotPointsModel. This is not necessary. </param>
        public virtual void Plot(CodeContext context, ndarray array, IList<string> names = null, Type type = null)
        {
            // Plotting must happen on the Main Thread, as one cannot use the container to resolve array items for thread-safety
            this.RunTaskSynchronously(async () => this.PlotInternal(context, array, names, type));
        }

        /// <summary>
        /// Sets plot corner.
        /// The bottom left corner, 0, uses the left and bottom axes.
        /// The bottom right corner, 1, uses the right and bottom axes.
        /// The top right corner, 2, uses the right and top axes.
        /// The top left corner, 3, uses the left and top axes.
        /// </summary>
        /// <param name="plotModel"> The plot model. </param>
        /// <param name="corner"> The corner, accepts 0 through 3. </param>
        public virtual void SetPlotCorner(VtkPlotPointsModel plotModel, int corner)
        {
            vtkPlotPoints plot = plotModel.PlotItem as vtkPlotPoints;
            vtkChartXY chart = this._chartItem as vtkChartXY;

            if (plot == null || chart == null)
                return;

            chart.SetPlotCorner(plot, corner);

            this.Render();
        }

        #endregion

        #region Protected Methods

        protected virtual void PlotInternal(CodeContext context, ndarray array, IList<string> names = null, Type type = null)
        {
            VtkChartXyModel model = this.Model as VtkChartXyModel;
            if (model == null)
                return;

            int ndim = array.ndim;

            if (type == null || !type.IsSubclassOf(typeof (VtkPlotPointsModel)))
            {
                type = typeof (VtkPlotPointsModel);
            }

            ConstructorInfo cInfo = type.GetConstructor(new Type[] { });
            if (cInfo == null)
                return;

            switch (ndim)
            {
                case 1:
                {
                    // Treat array as y-values
                    VtkPlotPointsModel plot = cInfo.Invoke(new object[] {}) as VtkPlotPointsModel;
                    if (plot == null)
                        return;

                    int len = (int) array.Dims[0];

                    ndarray linspace = Math.General.LinSpace(context, len, 0.0, 1.0);

                    vtkDoubleArray arrX = NumpyVtk.NumpyToVtk(linspace) as vtkDoubleArray;
                    vtkDoubleArray arrY = NumpyVtk.NumpyToVtk(array) as vtkDoubleArray;

                    if (arrX == null || arrY == null)
                    {
                        this.Log.Error("Plot(): Could not cast ndarrays to vtkDoubleArrays");
                        return;
                    }

                    numPlots++;
                    arrX.SetName(DEFAULT_X_ARRAY + '_' + numPlots);

                    if (names != null && !String.IsNullOrEmpty(names[0]))
                    {
                        arrY.SetName(names[0]);
                    }
                    else
                    {
                        numPlots++;
                        arrY.SetName(DEFAULT_Y_ARRAY + '_' + numPlots);
                    }

                    this.SetXArray(arrX, plot);
                    this.SetYArray(arrY, plot);

                    model.Plots.Add(plot);
                }
                break;

                case 2:
                {
                    // Treat 1st array as x-values, rest as y-values
                    // No need to check for dimensional conformity, ndarray won't allow asymmetric arrays to be created
                    ndarray arrayX = array[0] as ndarray;
                    if (arrayX == null)
                    {
                        this.Log.Error("Plot(): Could not cast X-data to ndarray");
                        return;
                    }

                    vtkDoubleArray arrX = NumpyVtk.NumpyToVtk(arrayX) as vtkDoubleArray;
                    if (arrX == null)
                    {
                        this.Log.Error("Plot(): Could not cast X-ndarray to vtkDoubleArray");
                        return;
                    }

                    if (names != null && !String.IsNullOrEmpty(names[0]))
                    {
                        arrX.SetName(names[0]);
                    }
                    else
                    {
                        numPlots++;
                        arrX.SetName(DEFAULT_X_ARRAY + '_' + numPlots);
                    }

                    int numArrays = (int) array.Dims[0];

                    for (int i = 1; i < numArrays; i++)
                    {
                        VtkPlotPointsModel plot = cInfo.Invoke(new object[] { }) as VtkPlotPointsModel;
                        if (plot == null)
                            return;

                        ndarray arrayY = array[i] as ndarray;
                        if (arrayY == null)
                        {
                            this.Log.Error("Plot(): Could not cast Y-data to ndarray");
                            return;
                        }

                        vtkDoubleArray arrY = NumpyVtk.NumpyToVtk(arrayY) as vtkDoubleArray;
                        if (arrY == null)
                        {
                            this.Log.Error("Plot(): Could not cast Y-ndarray to vtkDoubleArray");
                            return;
                        }

                        if (names != null && names.Count > i && !String.IsNullOrEmpty(names[i]))
                        {
                            arrY.SetName(names[i]);
                        }
                        else
                        {
                            numPlots++;
                            arrY.SetName(DEFAULT_Y_ARRAY + '_' + numPlots);
                        }

                        this.SetXArray(arrX, plot);
                        this.SetYArray(arrY, plot);

                        model.Plots.Add(plot);
                    }
                }
                break;

                default:
                    this.Log.Error("Can only plot 1D and 2D arrays");
                    break;
            }

            vtkChartXY chart = this._chartItem as vtkChartXY;
            if (chart == null)
                return;

            foreach (IVtkPlotItemModel plotItemModel in model.Plots)
            {
                plotItemModel.PropertyChanged += OnPlotItemChanged;
                vtkPlot plot = plotItemModel.PlotItem as vtkPlot;
                if (plot != null)
                {
                    chart.AddPlot(plot);
                }
            }

            this.RenderInternal();
        }

        protected virtual void SetXArray(vtkDoubleArray array, VtkPlotPointsModel plot)
        {
            VtkChartXyModel model = this.Model as VtkChartXyModel;
            if (model == null)
                return;

            vtkTable table = plot.Table;

            if (table.GetNumberOfColumns() <= 1)
            {
                table.Initialize();
                table.AddColumn(array);
            }
            else
            {
                vtkDoubleArray arrY = table.GetColumn(1) as vtkDoubleArray;
                if (arrY == null)
                {
                    this.Log.Error("SetXArray(): Could not cast table data to vtkDoubleArray");
                    return;
                }

                table.Initialize();
                table.AddColumn(array);
                table.AddColumn(arrY);

                vtkPlotPoints vtkPlot = plot.PlotItem as vtkPlotPoints;
                if (vtkPlot == null)
                {
                    this.Log.Error("SetXArray(): Could not cast plot item to vtkPlotPoints");
                    return;
                }

                vtkPlot.SetInputData(table, 0, 1);
            }
        }

        protected virtual void SetYArray(vtkDoubleArray array, VtkPlotPointsModel plot)
        {
            VtkChartXyModel model = this.Model as VtkChartXyModel;
            if (model == null)
                return;

            vtkTable table = plot.Table;

            if (table.GetNumberOfColumns() == 0)
            {
                this.Log.Error("SetYArray(): No XArray data");
                return;
            }

            if (table.GetNumberOfColumns() == 1)
            {
                table.AddColumn(array);
            }
            else
            {
                table.RemoveColumn(1);
                table.AddColumn(array);
            }

            vtkPlotPoints vtkPlot = plot.PlotItem as vtkPlotPoints;
            if (vtkPlot == null)
            {
                this.Log.Error("SetYArray(): Could not cast plot item to vtkPlotPoints");
                return;
            }

            vtkPlot.SetInputData(table, 0, 1);
        }

        protected double[] ConvertPixelPositionToGraphValues(int x, int y)
        {
            vtkChartXY chart = this._chartItem as vtkChartXY;
            if (chart == null)
                return null;

            int lowerLeftX = chart.GetPoint1()[0];
            int lowerLeftY = chart.GetPoint1()[1];

            int upperRightX = chart.GetPoint2()[0];
            int upperRightY = chart.GetPoint2()[1];

            if (x < lowerLeftX || x > upperRightX || y < lowerLeftY || y > upperRightY)
                return null;

            int xPosPixels = x - lowerLeftX;
            int yPosPixels = y - lowerLeftY;

            int xTotPixels = upperRightX - lowerLeftX;
            int yTotPixels = upperRightY - lowerLeftY;

            double xBotTotValue = _bAxis.GetMaximum() - _bAxis.GetMinimum();
            double yLeftTotValue = _lAxis.GetMaximum() - _lAxis.GetMinimum();
            double xTopTotValue = _tAxis.GetMaximum() - _tAxis.GetMinimum();
            double yRightTotValue = _rAxis.GetMaximum() - _rAxis.GetMinimum();

            double xBotPosValue = xBotTotValue*xPosPixels/xTotPixels + _bAxis.GetMinimum();
            double yLeftPosValue = yLeftTotValue*yPosPixels/yTotPixels + _lAxis.GetMinimum();
            double xTopPosValue = xTopTotValue*xPosPixels/xTotPixels + _tAxis.GetMinimum();
            double yRightPosValue = yRightTotValue*yPosPixels/yTotPixels + _rAxis.GetMinimum();

            return new[] {xBotPosValue, yLeftPosValue, xTopPosValue, yRightPosValue};
        }

        #endregion

        #region Private Methods

        private bool CheckLeftAxis(int x, int y)
        {
            float bottomX = _lAxis.GetPoint1()[0]; //x-position of bottom point
            float bottomY = _lAxis.GetPoint1()[1]; //y-position of bottom point

            float topX = _lAxis.GetPoint2()[0]; //x-position of top point
            float topY = _lAxis.GetPoint2()[1]; //y-position of top point

            if (x < bottomX && y - bottomY < PIXEL_RADIUS && y - bottomY > -PIXEL_RADIUS)
            {
                _currentAxis = _lAxis;
                _textBox.DataEntered += ModifyMinimumValue;
                SetupTextBox();

                return true;
            }
            else if (x < topX && y - topY < PIXEL_RADIUS && y - topY > -PIXEL_RADIUS)
            {
                _currentAxis = _lAxis;
                _textBox.DataEntered += ModifyMaximumValue;
                SetupTextBox();

                return true;
            }

            return false;
        }

        private bool CheckBottomAxis(int x, int y)
        {
            float leftX = _bAxis.GetPoint1()[0]; //x-position of left point
            float leftY = _bAxis.GetPoint1()[1]; //y-position of left point

            float rightX = _bAxis.GetPoint2()[0]; //x-position of right point
            float rightY = _bAxis.GetPoint2()[1]; //y-position of right point

            if (x - leftX < PIXEL_RADIUS && x - leftX > -PIXEL_RADIUS && y < leftY)
            {
                _currentAxis = _bAxis;
                _textBox.DataEntered += ModifyMinimumValue;
                SetupTextBox();

                return true;
            }
            else if (x - rightX < PIXEL_RADIUS && x - rightX > -PIXEL_RADIUS && y < rightY)
            {
                _currentAxis = _bAxis;
                _textBox.DataEntered += ModifyMaximumValue;
                SetupTextBox();

                return true;
            }

            return false;
        }

        private bool CheckRightAxis(int x, int y)
        {
            float bottomX = _rAxis.GetPoint1()[0]; //x-position of bottom point
            float bottomY = _rAxis.GetPoint1()[1]; //y-position of bottom point

            float topX = _rAxis.GetPoint2()[0]; //x-position of top point
            float topY = _rAxis.GetPoint2()[1]; //y-position of top point

            if (x > bottomX && y - bottomY < PIXEL_RADIUS && y - bottomY > -PIXEL_RADIUS)
            {
                _currentAxis = _rAxis;
                _textBox.DataEntered += ModifyMinimumValue;
                SetupTextBox();

                return true;
            }
            else if (x > topX && y - topY < PIXEL_RADIUS && y - topY > -PIXEL_RADIUS)
            {
                _currentAxis = _rAxis;
                _textBox.DataEntered += ModifyMaximumValue;
                SetupTextBox();

                return true;
            }

            return false;
        }

        private bool CheckTopAxis(int x, int y)
        {
            float leftX = _tAxis.GetPoint1()[0]; //x-position of left point
            float leftY = _tAxis.GetPoint1()[1]; //y-position of left point

            float rightX = _tAxis.GetPoint2()[0]; //x-position of right point
            float rightY = _tAxis.GetPoint2()[1]; //y-position of right point

            if (x - leftX < PIXEL_RADIUS && x - leftX > -PIXEL_RADIUS && y > leftY)
            {
                _currentAxis = _tAxis;
                _textBox.DataEntered += ModifyMinimumValue;
                SetupTextBox();

                return true;
            }
            else if (x - rightX < PIXEL_RADIUS && x - rightX > -PIXEL_RADIUS && y > rightY)
            {
                _currentAxis = _tAxis;
                _textBox.DataEntered += ModifyMaximumValue;
                SetupTextBox();

                return true;
            }

            return false;
        }

        private void SetupTextBox()
        {
            Point mousePosition = MousePoint.GetCursorPosition();

            _textBox.Left = mousePosition.X - 5;
            _textBox.Top = mousePosition.Y - 10;
            _textBox.Show();
        }

        #endregion

        #region Events

        protected override void OnMouseLeftClick(vtkObject sender, vtkObjectEventArgs e)
        {
            int[] pos = _interactor.GetEventPosition();
            int x = pos[0];
            int y = pos[1];

            double[] positionValues = ConvertPixelPositionToGraphValues(x, y);

            if (positionValues != null)
            {
                this.Log.Info("Mouse Clicked At: bottom x: {0:F2}, left y: {1:F2}, top x: {2:F2}, right y: {3:F2}",
                    positionValues[0], positionValues[1], positionValues[2], positionValues[3]);
            }

            if (CheckLeftAxis(x, y))
                return;
            if (CheckBottomAxis(x, y))
                return;
            if (CheckRightAxis(x, y))
                return;
            if (CheckTopAxis(x, y))
                return;
        }

        protected override void OnGraphWindowLoaded(object sender, RoutedEventArgs e)
        {
            base.OnGraphWindowLoaded(sender, e);

            const double padding = 0.1;

            double leftAxisPadding = padding*(this.LeftAxisMaximum - this.LeftAxisMinimum);
            double bottomAxisPadding = padding*(this.BottomAxisMaximum - this.BottomAxisMinimum);
            double rightAxisPadding = padding*(this.RightAxisMaximum - this.RightAxisMinimum);
            double topAxisPadding = padding*(this.TopAxisMaximum - this.TopAxisMinimum);

            if (_lAxisValues[0].HasValue)
                this.LeftAxisMinimum = _lAxisValues[0].Value;
            else
                this.LeftAxisMinimum = this.LeftAxisMinimum - leftAxisPadding;

            if (_lAxisValues[1].HasValue)
                this.LeftAxisMaximum = _lAxisValues[1].Value;
            else
                this.LeftAxisMaximum = this.LeftAxisMaximum + leftAxisPadding;

            if (_bAxisValues[0].HasValue)
                this.BottomAxisMinimum = _bAxisValues[0].Value;
            else
                this.BottomAxisMinimum = this.BottomAxisMinimum - bottomAxisPadding;

            if (_bAxisValues[1].HasValue)
                this.BottomAxisMaximum = _bAxisValues[1].Value;
            else
                this.BottomAxisMaximum = this.BottomAxisMaximum + bottomAxisPadding;

            if (_rAxisValues[0].HasValue)
                this.RightAxisMinimum = _rAxisValues[0].Value;
            else
                this.RightAxisMinimum = this.RightAxisMinimum - rightAxisPadding;

            if (_rAxisValues[1].HasValue)
                this.RightAxisMaximum = _rAxisValues[1].Value;
            else
                this.RightAxisMaximum = this.RightAxisMaximum + rightAxisPadding;

            if (_tAxisValues[0].HasValue)
                this.TopAxisMinimum = _tAxisValues[0].Value;
            else
                this.TopAxisMinimum = this.TopAxisMinimum - topAxisPadding;

            if (_tAxisValues[1].HasValue)
                this.TopAxisMaximum = _tAxisValues[1].Value;
            else
                this.TopAxisMaximum = this.TopAxisMaximum + topAxisPadding;
        }

        private void ModifyMinimumValue(object sender, EventArgs eventArgs)
        {
            double min;
            if (Double.TryParse(_textBox.TextBox.Text, out min))
            {
                _currentAxis.SetMinimum(min);
                _currentAxis = null;
                this.Render();
            }
            else if (String.IsNullOrEmpty(_textBox.TextBox.Text))
            {
                // Do nothing
            }
            else
            {
                this.Log.Error("Improperly formatted min value.");
            }

            _textBox.TextBox.Text = "";
            _textBox.DataEntered -= ModifyMinimumValue;
        }

        private void ModifyMaximumValue(object sender, EventArgs eventArgs)
        {
            double max;
            if (Double.TryParse(_textBox.TextBox.Text, out max))
            {
                _currentAxis.SetMaximum(max);
                _currentAxis = null;
                this.Render();
            }
            else if (String.IsNullOrEmpty(_textBox.TextBox.Text))
            {
                // Do nothing
            }
            else
            {
                this.Log.Error("Improperly formatted min value.");
            }

            _textBox.TextBox.Text = "";
            _textBox.DataEntered -= ModifyMaximumValue;
        }

        private void OnPlotItemChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.Render();
        }

        #endregion
    }
}
