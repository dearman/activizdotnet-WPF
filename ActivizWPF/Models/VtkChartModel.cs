using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivizWPF.Models
{
    public abstract class VtkChartModel : VtkGraphModel, IVtkChartModel
    {
        public bool IsLegendVisible { get; set; }
        public IList<VtkScatterPlotStyle> PlotStyles { get; set; }

        public override void New()
        {
            this.PlotStyles = new List<VtkScatterPlotStyle>();
            this.IsLegendVisible = false;
        }
    }
}
