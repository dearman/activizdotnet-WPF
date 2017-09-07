using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivizWPF.Framework.Models;
using ActivizWPF.Python.Numpy;
using Kitware.VTK;
using NationalInstruments;

namespace ActivizWPF.Models
{
    public abstract class VtkGraphModel : IVtkGraphModel
    {
        #region Fields

        protected IList<IVtkPlotItemModel> _plotItems;

        #endregion

        #region IModel

        public abstract string Name { get; }
        public abstract Guid Id { get; }
        public abstract IList<Type> Types { get; }

        public abstract void FromObject(object obj);
        public abstract void New();

        #endregion

        #region ISerializableModel

        public abstract void Open(Uri uri);
        public abstract void Save(Uri uri);

        #endregion

        #region IVtkGraphModel

        public IList<IVtkPlotItemModel> Plots
        {
            get { return _plotItems; }
        }

        #endregion
    }
}
