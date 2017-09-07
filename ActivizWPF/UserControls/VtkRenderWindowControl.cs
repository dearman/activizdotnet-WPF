using System;
using System.Runtime.InteropServices;
using System.Windows;
using Kitware.VTK;
using NLog;

using ActivizWPF.Framework.Native;

namespace ActivizWPF.UserControls
{
    public class VtkRenderWindowControl : HwndWrapper
    {
        #region Fields

        protected vtkRenderer _renderer;
        protected vtkRenderWindow _renderWindow;
        protected vtkRenderWindowInteractor _renderWindowInteractor;
        protected bool _isInteractorAttached;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the window class.
        /// </summary>
        ///
        /// <value>
        /// The window class.
        /// </value>
        ///
        /// <seealso cref="P:ActivizWPF.Framework.Native.HwndWrapper.WindowClass"/>
        public override string WindowClass { get { return "VtkRenderWindowClass"; } } 

        /// <summary>
        /// This property gives you access to the vtkRenderWindow that
        /// fills the client area.
        /// </summary>
        public vtkRenderWindow RenderWindow
        {
            get
            {
                return _renderWindow;
            }
        }

        /// <summary>
        /// Gets the default interactor for this RenderWindow.
        /// </summary>
        ///
        /// <value>
        /// The interactor, null otherwise.
        /// </value>
        public vtkRenderWindowInteractor Interactor
        {
            get
            {
                if (_isInteractorAttached)
                {
                    return _renderWindowInteractor;
                }

                return null;
            }
        }

        #endregion

        #region Constructor

        public VtkRenderWindowControl()
        {
        }

        #endregion

        #region HwndWrapper

        /// <summary>
        /// Executes the destroy action.
        /// </summary>
        ///
        /// <seealso cref="M:ActivizWPF.Framework.Native.HwndWrapper.OnDestroy()"/>
        protected override void OnDestroy()
        {
            
        }

        /// <summary>
        /// Executes the initialize action.
        /// </summary>
        ///
        /// <param name="hWnd">The HWND we present to when rendering.</param>
        ///
        /// <seealso cref="M:ActivizWPF.Framework.Native.HwndWrapper.OnInitialize(IntPtr)"/>
        protected override void OnInitialize(IntPtr hWnd)
        {
            try
            {
                vtkLogoWidget vtkLogoWidget = new vtkLogoWidget();

                _renderer = vtkRenderer.New();
                _renderWindow = vtkRenderWindow.New();

                _renderWindowInteractor = vtkRenderWindowInteractor.New();

                vtkInteractorStyleSwitch interactorStyleSwitch = _renderWindowInteractor.GetInteractorStyle() as vtkInteractorStyleSwitch;

                if (null != interactorStyleSwitch)
                    interactorStyleSwitch.SetCurrentStyleToTrackballCamera();

                _renderWindow.SetParentId(hWnd);
                _renderWindow.AddRenderer(_renderer);

                AttachInteractor();

                vtkLogoWidget.Dispose();
            }
            catch (Exception ex)
            {
                log.ErrorException("OnInitialize()", ex);
            }
        }

        /// <summary>
        /// Occurs when the Control gets focus.
        /// </summary>
        ///
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
        ///
        /// <seealso cref="M:System.Windows.FrameworkElement.OnGotFocus(RoutedEventArgs)"/>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            try
            {
                if (_renderWindow != null)
                {
                    IntPtr hWnd = _renderWindow.GetGenericWindowId();

                    if (IntPtr.Zero != hWnd)
                    {
                        Win32.SetFocus(hWnd);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorException("OnGotFocus()", ex);
            }

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Executes the resize action.
        /// </summary>
        ///
        /// <param name="sizeInfo">Information describing the size.</param>
        ///
        /// <seealso cref="M:ActivizWPF.Framework.Native.HwndWrapper.OnResize(SizeChangedInfo)"/>
        protected override void OnResize(SizeChangedInfo sizeInfo)
        {
            try
            {
                SyncRenderWindowSize();
                vtkGenericRenderWindowInteractor windowInteractor = _renderWindowInteractor as vtkGenericRenderWindowInteractor;

                if (null != windowInteractor)
                    windowInteractor.ConfigureEvent();
            }
            catch (Exception ex)
            {
                log.ErrorException("OnResize()", ex);
            }
        }

        /// <summary>
        /// Renders this VtkRenderWindowControl.
        /// </summary>
        ///
        /// <seealso cref="M:ActivizWPF.Framework.Native.HwndWrapper.Render()"/>
        protected override void Render()
        {
            try
            {
                if (_renderWindow != null)
                {
                    this.SyncRenderWindowSize();

                    if (_renderWindow.GetInteractor() != _renderWindowInteractor)
                    {
                        AttachInteractor();
                        _renderWindow.Render();
                    }

                    _renderWindow.Render();
                }
            }
            catch (Exception e)
            {
                log.ErrorException("Render()", e);
            }

        }

        /// <summary>
        /// Releases the unmanaged resources used by the ActivizWPF.UserControls.VtkRenderWindowControl
        /// and optionally releases the managed resources.
        /// </summary>
        ///
        /// <param name="disposing">Set to true if called from an explicit disposer and false otherwise.</param>
        ///
        /// <seealso cref="M:ActivizWPF.Framework.Native.HwndWrapper.Dispose(bool)"/>
        protected override void Dispose(bool disposing)
        {
            if (_renderer != null)
                _renderer.SetRenderWindow(null);

            if (_renderWindowInteractor != null)
            {
                _renderWindowInteractor.Dispose();
                _renderWindowInteractor = null;
            }

            if (_renderWindow != null)
            {
                _renderWindow.Dispose();
                _renderWindow = null;
            }

            if (_renderer != null)
            {
                _renderer.Dispose();
                _renderer = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called to set the vtkRenderWindow size according to this control's
        ///             Size property.
        /// 
        /// </summary>
        private void SyncRenderWindowSize()
        {
            if (_renderWindow == null)
                return;

            try
            {
                // Get the current width and height of the control
                int width = (int)ActualWidth;
                int height = (int)ActualHeight;

                _renderWindow.SetSize(width, height);
            }
            catch (Exception e)
            {
                log.ErrorException("SyncRenderWindowSize()",e);
            }
        }

        /// <summary>
        /// Attach the internal Interactor to the RenderWindow
        /// </summary>
        protected void AttachInteractor()
        {
            if (_isInteractorAttached)
                return;

            try
            {
                _renderWindow.SetInteractor(_renderWindowInteractor);
                _isInteractorAttached = true;
            }
            catch (Exception e)
            {
                log.ErrorException("AttachInteractor()", e);
            }
        }


        #endregion

    }
}
