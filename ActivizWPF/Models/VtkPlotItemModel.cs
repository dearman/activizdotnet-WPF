using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace ActivizWPF.Models
{
    public abstract class VtkPlotItemModel : IVtkPlotItemModel
    {
        #region Fields

        protected vtkContextItem _plotItem;

        #endregion

        #region IModel

        public abstract string Name { get; }
        public abstract Guid Id { get; }
        public abstract IList<Type> Types { get; }

        public abstract void FromObject(object obj);
        public abstract void New();

        #endregion

        #region IVtkPlotItemModel

        /// <summary> Gets the plot item. Please do not use this from python. </summary>
        /// <value> The plot item. </value>
        public vtkContextItem PlotItem { get { return _plotItem; } }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
