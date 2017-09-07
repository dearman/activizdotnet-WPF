using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace ActivizWPF.Models
{
    public interface IVtkPlotPointsModel : IVtkPlotModel
    {
        vtkTable Table { get; }
    }
}
