using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace ActivizWPF.Models
{
    public class VtkScatterPlotStyle
    {
        public Color Color { get; set; }
        public string XName { get; set; }
        public string YName { get; set; }
        public MarkerStyleEnum Marker { get; set; }
        public double LineWidth { get; set; }
        public PlotStyleEnum PlotStyle { get; set; }

        public VtkScatterPlotStyle()
        {
            this.XName = "X Axis";
            this.YName = "Y Axis";
            
            this.Marker = MarkerStyleEnum.CIRCLE;
            this.PlotStyle = PlotStyleEnum.POINTS;
            this.LineWidth = 1.0;
            this.Color = Color.Black;

        }

        public enum PlotStyleEnum
        {
            LINE,
            POINTS,
            BAR,
            STACKED,
            BAG,
            FUNCTIONALBAG,
        }

        public enum MarkerStyleEnum
        {
            NONE,
            CROSS,
            PLUS,
            SQUARE,
            CIRCLE,
            DIAMOND,
        }
    }
}
