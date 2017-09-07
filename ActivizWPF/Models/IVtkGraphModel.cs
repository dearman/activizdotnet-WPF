using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ActivizWPF.Framework.Models;
using ActivizWPF.Python.Numpy;

using Kitware.VTK;

namespace ActivizWPF.Models
{
    
    public interface IVtkGraphModel : ISerializableModel
    {
        IList<IVtkPlotItemModel> Plots { get; }
    }
}
