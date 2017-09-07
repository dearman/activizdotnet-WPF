using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivizWPF.Framework.Models;
using ActivizWPF.Python.Numpy;
using NationalInstruments;

namespace ActivizWPF.Models
{
    public class VtkChartXyModel : VtkChartModel, IVtkChartXyModel
    {
        #region IModel

        public override string Name
        {
            get { return "VTK ChartXY Document"; }
        }

        // {CC4A17FB-8ADA-408B-A9F7-E21A0BC53499}
        private static readonly Guid Guid = new Guid("CC4A17FB-8ADA-408B-A9F7-E21A0BC53499");

        public override Guid Id
        {
            get { return Guid; }
        }

        public override IList<Type> Types
        {
            get
            {
                return ConvertableTypes.Create(typeof(ndarray),
                    typeof(double[]),
                    typeof(ComplexDouble[]),
                    typeof(double),
                    typeof(ComplexDouble));
            }
        }

        public override void FromObject(object obj)
        {
            throw new NotImplementedException();
        }

        public override void New()
        {
            base.New();
            _plotItems = new List<IVtkPlotItemModel>();
        }

        #endregion

        #region ISerializableModel

        public override void Open(Uri uri)
        {
            throw new NotImplementedException();
        }

        public override void Save(Uri uri)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
