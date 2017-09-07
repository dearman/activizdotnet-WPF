using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace ActivizWPF.Models
{
    public class VtkPlotLineModel : VtkPlotPointsModel, IVtkPlotLineModel
    {
        #region Constructors

        public VtkPlotLineModel() : base()
        {
            
        }

        protected override void Create()
        {
            _plotItem = new vtkPlotLine();
        }

        #endregion
    }
}
