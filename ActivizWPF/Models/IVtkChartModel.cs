using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivizWPF.Python.Numpy;

namespace ActivizWPF.Models
{
    public interface IVtkChartModel : IVtkGraphModel
    {
        // Chart Data
        bool IsLegendVisible { get; set; }

        // Plot Data
        IList<VtkScatterPlotStyle> PlotStyles { get; set; }
    }
}
