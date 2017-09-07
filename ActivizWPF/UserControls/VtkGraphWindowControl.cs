using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace ActivizWPF.UserControls
{
    public class VtkGraphWindowControl : VtkRenderWindowControl, INotifyPropertyChanged
    {
        protected vtkContextView _contextView;
        private bool _contextMenuShow;

        public vtkContextView ContextView
        {
            get { return _contextView; }
        }

        public VtkGraphWindowControl()
        {
            _contextView = new vtkContextView();
        }

        protected override void OnInitialize(IntPtr hWnd)
        {
            try
            {
                _renderer = _contextView.GetRenderer();
                _renderWindow = _contextView.GetRenderWindow();

                _renderWindowInteractor = vtkRenderWindowInteractor.New();

                //vtkInteractorStyleSwitch interactorStyleSwitch = _renderWindowInteractor.GetInteractorStyle() as vtkInteractorStyleSwitch;

                //if (null != interactorStyleSwitch)
                //    interactorStyleSwitch.SetCurrentStyleToTrackballCamera();

                _renderWindow.SetParentId(hWnd);

                AttachInteractor();
            }
            catch (Exception ex)
            {
                log.ErrorException("OnInitialize()", ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
