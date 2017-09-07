using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ActivizWPF.Framework.Document;
using ActivizWPF.Framework.Services;
using ActivizWPF.Models;
using ActivizWPF.Python.Numpy;
using IronPython.Runtime;
using Kitware.VTK;
using Microsoft.Practices.Unity;

namespace ActivizWPF.ViewModels
{
    public abstract class VtkChartViewModel : VtkGraphViewModel
    {
        #region Constructors

        protected VtkChartViewModel() : base()
        {
            
        }

        protected VtkChartViewModel(IUnityContainer container, IGuiThreadDispatchService dispatchService = null, IEventAggregator events = null)
            : base(container, dispatchService, events)
        {

        }

        #endregion

        #region ViewModelBase

        protected override void Initialize()
        {
            base.Initialize();

            IVtkChartModel doc = this.Model as IVtkChartModel;
            vtkChart chart = this._chartItem as vtkChart;

            if (doc != null && chart != null)
            {
                chart.SetShowLegend(doc.IsLegendVisible);
            }
        }

        #endregion

        #region Properties

        public bool IsLegendVisible
        {
            get
            {
                vtkChart chart = this._chartItem as vtkChart;

                if (chart != null)
                {
                    return chart.GetShowLegend();
                }

                return false;
            }
            set
            {
                vtkChart chart = this._chartItem as vtkChart;

                if (chart != null)
                {
                    chart.SetShowLegend(value);
                }

                this.Render();
            }
        }

        #endregion

        #region Public Functions

        public virtual int AddPlot(VtkPlotModel plotModel)
        {
            vtkChart chart = this._chartItem as vtkChart;
            vtkPlot plot = plotModel.PlotItem as vtkPlot;

            if (chart == null || plot == null)
                return -1;

            int index = chart.AddPlot(plot);
            if (index == -1)
            {
                this.Log.Error("Could not add plot");
            }
            return index;
        }

        #endregion
    }
}
