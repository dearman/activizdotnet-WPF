using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Caliburn.Micro;
using ActivizWPF.Framework.Document;
using ActivizWPF.Framework.Models;
using ActivizWPF.Framework.Native;
using ActivizWPF.Framework.Services;
using ActivizWPF.Framework.ViewModels;
using ActivizWPF.Models;
using ActivizWPF.UserControls;
using ActivizWPF.Python.Numpy;
using IronPython.Runtime;
using Kitware.VTK;
using Microsoft.Practices.Unity;

namespace ActivizWPF.ViewModels
{
    public abstract class VtkGraphViewModel : DocumentItemViewModel
    {
        #region Fields

        private readonly VtkGraphWindowControl _graphWindow;
        protected vtkContextItem _chartItem;
        protected vtkRenderWindowInteractor _interactor;
        protected vtkRenderer _graphLayerRenderer;
        protected vtkRenderer _markerLayerRenderer;

        #endregion

        #region Properties

        public VtkGraphWindowControl GraphWindow
        {
            get { return _graphWindow; }
        }

        /// <summary> Sets the color of the background. Note: alpha value is ignored. </summary>
        public Color BackgroundColor
        {
            set
            {
                _graphLayerRenderer.SetBackground((float)value.R / 255, (float)value.G / 255, (float)value.B / 255);
                this.Render();
            }
        }

        #endregion

        #region Constructors

        protected VtkGraphViewModel()
        {

        }

        protected VtkGraphViewModel(IUnityContainer container, IGuiThreadDispatchService dispatchService = null, IEventAggregator events = null) : base(container, dispatchService, events)
        {
            _graphWindow = new VtkGraphWindowControl();

            _markerLayerRenderer = vtkRenderer.New();
            _markerLayerRenderer.SetInteractive(0);
            _markerLayerRenderer.SetLayer(1);

            this.Initialize();
        }

        #endregion

        #region ViewModelBase

        protected override void Initialize()
        {
            base.Initialize();

            _graphWindow.Loaded += OnGraphWindowLoaded;

            this.Title = "Untitled Graph";
        }

        protected override void RegisterCommandBindings()
        {
            base.RegisterCommandBindings();
        }

        #endregion

        #region IDocumentItem

        public override string FileName { get; set; }

        public override bool IsReadOnly { get; set; }

        public override bool IsDirty { get; set; }

        public override bool New(DocumentItemType typeDescription = null)
        {
            base.New(typeDescription);

            try
            {
                
               
            }
            catch (Exception e)
            {
                this.Log.ErrorException("New()", e);
                return false;
            }

            return true;
        }

        public override bool Open(Uri path, DocumentItemType typeDescription = null)
        {
            base.Open(path, typeDescription);

            try
            {
                
                
            }
            catch (Exception e)
            {
                this.Log.ErrorException("Open()", e);
                return false;
            }

            return true;
        }

        public override bool Open(IModel model, DocumentItemType typeDescription = null)
        {
            base.Open(model, typeDescription);

            try
            {
                
                
            }
            catch (Exception e)
            {
                this.Log.ErrorException("Open()", e);
                return false;
            }

            return true;
        }

        #endregion

        #region Public Methods

        public void Render()
        {
            this.Execute(this.RenderInternal);
        }

        #endregion

        #region Protected Methods

        protected void RenderInternal()
        {
            //Set up the camera
            if (_graphWindow.RenderWindow != null)
            {
                try
                {
                    _graphLayerRenderer.ResetCamera();
                    _graphWindow.RenderWindow.Render();
                }
                catch (Exception e)
                {
                    this.Log.ErrorException("Render()", e);
                }
            }
        }

        #endregion

        //vtkPoints points = vtkPoints.New();
        //points.InsertNextPoint(0.0, 0.0, 0.0);
        //points.InsertNextPoint(200.0, 0.0, 0.0);
        //points.InsertNextPoint(200.0, 200.0, 0.0);
        //points.InsertNextPoint(0.0, 200.0, 0.0);
        //points.InsertNextPoint(0.0, 0.0, 0.0);

        //vtkPolygon polygon = vtkPolygon.New();
        //polygon.GetPointIds().SetNumberOfIds(5);
        //for (int i = 0; i < 5; i++)
        //{
        //    polygon.GetPointIds().SetId(i, i);
        //}

        public vtkActor2D AddMarkerLine(double[] startCoords, double[] endCoords, Color color)
        {
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(startCoords[0], startCoords[1], 0.0);
            points.InsertNextPoint(endCoords[0], endCoords[1], 0.0);

            vtkPolyLine line = vtkPolyLine.New();
            line.GetPointIds().SetNumberOfIds(2);
            for (int i = 0; i < 2; i++)
            {
                line.GetPointIds().SetId(i, i);
            }

            vtkCellArray cellArray = vtkCellArray.New();
            cellArray.InsertNextCell(line);

            vtkPolyData polyData = vtkPolyData.New();
            polyData.SetPoints(points);
            polyData.SetLines(cellArray);

            vtkPolyDataMapper2D mapper = vtkPolyDataMapper2D.New();
            mapper.SetInputData(polyData);

            vtkActor2D actor = vtkActor2D.New();
            actor.SetMapper(mapper);

            actor.GetProperty().SetOpacity((float)color.A / 255);
            actor.GetProperty().SetColor((float)color.R / 255, (float)color.G / 255, (float)color.B / 255);

            _markerLayerRenderer.AddActor(actor);

            return actor;
        }

        #region Events

        /// <summary>
        /// Called on the graph document loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnGraphWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //Setup the variables and the background
            _graphLayerRenderer = _graphWindow.RenderWindow.GetRenderers().GetFirstRenderer();

            _graphLayerRenderer.SetBackground(1.0, 1.0, 1.0);

            _graphWindow.RenderWindow.SetMultiSamples(0);
            _graphWindow.RenderWindow.SetNumberOfLayers(2);

            _graphWindow.RenderWindow.AddRenderer(_markerLayerRenderer);

            if (_chartItem != null)
            {
                _graphWindow.ContextView.GetScene().AddItem(_chartItem);
            }

            //AddMarkerLine(new[] { 10.0, 10.0 }, new[] { 200.0, 0.0 }, Color.Red);
            //AddMarkerLine(new[] { 10.0, 10.0 }, new[] { 200.0, 200.0 }, Color.Blue);
            //AddMarkerLine(new[] { 10.0, 10.0 }, new[] { 0.0, 200.0 }, Color.Yellow);

            _interactor = _graphWindow.Interactor;
            vtkRenderWindowInteractor iren = _graphWindow.RenderWindow.GetInteractor();

            iren.LeftButtonPressEvt += OnMouseLeftClick;
            
            this.RenderInternal();
        }

        /// <summary>
        /// Called on left-clicking with the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void OnMouseLeftClick(vtkObject sender, vtkObjectEventArgs e);

        #endregion
    }
}
