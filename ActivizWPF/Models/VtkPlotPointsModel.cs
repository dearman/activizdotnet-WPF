using System;
using System.Collections.Generic;
using System.Drawing;
using Kitware.VTK;

namespace ActivizWPF.Models
{
    public class VtkPlotPointsModel : VtkPlotModel, IVtkPlotPointsModel
    {
        #region Fields

        protected vtkTable _table;

        #endregion

        #region Properties

        public Color Color
        {
            set
            {
                vtkPlotPoints plotPoints = this.PlotItem as vtkPlotPoints;
                if (plotPoints == null)
                    return;

                plotPoints.SetColor(value.R, value.G, value.B, value.A);
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get
            {
                vtkPlotPoints plotPoints = this.PlotItem as vtkPlotPoints;
                if (plotPoints == null)
                    return false;

                return plotPoints.GetVisible();
            }
            set
            {
                vtkPlotPoints plotPoints = this.PlotItem as vtkPlotPoints;
                if (plotPoints == null)
                    return;

                plotPoints.SetVisible(value);
                OnPropertyChanged();
            }
        }

        public vtkPlotPoints.CIRCLE_WrapperEnum PointStyle
        {
            get
            {
                vtkPlotPoints plotPoints = this.PlotItem as vtkPlotPoints;
                if (plotPoints == null)
                    return 0;

                return (vtkPlotPoints.CIRCLE_WrapperEnum) plotPoints.GetMarkerStyle();
            }
            set
            {
                vtkPlotPoints plotPoints = this.PlotItem as vtkPlotPoints;
                if (plotPoints == null)
                    return;

                plotPoints.SetMarkerStyle((int)value);
                OnPropertyChanged();
            }
        }
        
        public float Width
        {
            get
            {
                vtkPlotPoints plot = _plotItem as vtkPlotPoints;
                return plot == null ? 0.0f : plot.GetWidth();
            }
            set
            {
                vtkPlotPoints plot = _plotItem as vtkPlotPoints;
                if (plot == null)
                    return;

                plot.SetWidth(value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public VtkPlotPointsModel()
        {
            Create();
            _table = new vtkTable();
        }

        protected virtual void Create()
        {
            _plotItem = new vtkPlotPoints();
        }

        #endregion

        #region IModel

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override Guid Id
        {
            get { throw new NotImplementedException(); }
        }

        public override IList<Type> Types
        {
            get { throw new NotImplementedException(); }
        }

        public override void FromObject(object obj)
        {
            throw new NotImplementedException();
        }

        public override void New()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVtkPlotPointsModel

        public vtkTable Table { get { return _table; } }

        #endregion
    }
}
