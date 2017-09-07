using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivizWPF.Framework.Models;
using Kitware.VTK;

namespace ActivizWPF.Models
{
    public interface IVtkPlotItemModel : IModel, INotifyPropertyChanged
    {
        vtkContextItem PlotItem { get; }
    }
}
