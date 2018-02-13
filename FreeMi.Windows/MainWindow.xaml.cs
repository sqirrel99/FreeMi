using FreeMi.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FreeMi.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Inner classes

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly IntPtr _handle;
            public OldWindow(IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of MainWindows class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
            {
                if (DataContext is MainViewModel)
                {
                    var vm = (MainViewModel)DataContext;
                    vm.Win32Window = GetIWin32Window(this);
                }
            };
        }

        #endregion

        #region Properties

        private Point StartDragPosition { get; set; }

        #endregion

        #region Methods

        private System.Windows.Forms.IWin32Window GetIWin32Window(Visual visual)
        {
            return new OldWindow(((HwndSource)PresentationSource.FromVisual(visual)).Handle);
        }

        /// <summary>
        /// Occurs directly after Close is called, and can be handled to cancel window closure
        /// </summary>
        /// <param name="e">a CancelEventArgs that contains the event data</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (FreeMi.Core.Application.RunningMode != Core.RunningMode.Service)
            {
                e.Cancel = true;
                Hide();
            }
            base.OnClosing(e);
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        /// <summary>
        /// Select the TreeViewItem on right click
        /// </summary>
        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem == null)
            {
                if (treeView.SelectedItem is ITreeViewItemViewModel)
                {
                    ((ITreeViewItemViewModel)treeView.SelectedItem).IsSelected = false;
                }
            }
            else
            {
                treeViewItem.IsSelected = true;
            }
        }

        #region Drag & Drop

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);
            if (treeViewItem == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                new DropAdorner(treeViewItem, AdornerLayer.GetAdornerLayer(treeViewItem), getMousePosition(e, treeViewItem));
            }
        }

        private Position getMousePosition(DragEventArgs e, TreeViewItem treeViewItem)
        {
            double height = 0;
            if (treeViewItem != null && VisualTreeHelper.GetChildrenCount(treeViewItem) > 0)
            {
                var grid = VisualTreeHelper.GetChild(treeViewItem, 0) as Grid;
                if (grid != null)
                {
                    height = grid.RowDefinitions[0].ActualHeight;
                }
            }

            var y = e.GetPosition(treeViewItem).Y;
            if (treeViewItem.DataContext is ITreeViewItemViewModel && ((ITreeViewItemViewModel)treeViewItem.DataContext).CanAddSubItems)
            {
                height = height / 4;
                return y < height ? Position.Top : y > height * 3 ? Position.Bottom : Position.Middle;
            }
            else
            {
                return y < height / 2 ? Position.Top : Position.Bottom;
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);
            if (treeViewItem == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                foreach (var adorner in AdornerLayer.GetAdornerLayer(treeViewItem).GetAdorners(treeViewItem))
                {
                    if (adorner is DropAdorner)
                    {
                        ((DropAdorner)adorner).MousePosition = getMousePosition(e, treeViewItem);
                        adorner.InvalidateVisual();
                        break;
                    }
                }
            }
        }

        private void DisposeAdorner(TreeViewItem treeViewItem)
        {
            if (treeViewItem != null)
            {
                foreach (var adorner in AdornerLayer.GetAdornerLayer(treeViewItem).GetAdorners(treeViewItem))
                {
                    if (adorner is DropAdorner)
                    {
                        ((DropAdorner)adorner).Dispose();
                        break;
                    }
                }
            }
        }

        private void treeView_DragLeave(object sender, DragEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);
            if (treeViewItem == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
            else
            {
                DisposeAdorner(treeViewItem);
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                var position = getMousePosition(e, treeViewItem);
                DisposeAdorner(treeViewItem);
                if (treeView.SelectedItem == treeViewItem.DataContext)
                {
                    return;
                }

                var viewModel = (ITreeViewItemViewModel)treeView.SelectedItem;
                var oldCollection = viewModel.ParentCollection;
                oldCollection.Remove(viewModel);

                var destinationViewModel = (ITreeViewItemViewModel)treeViewItem.DataContext;
                var folderViewModel = destinationViewModel as FolderViewModel;
                var newCollection = (folderViewModel != null && position == Position.Middle) ? folderViewModel.Children : destinationViewModel.ParentCollection;
                if (position == Position.Middle)
                {
                    newCollection.Add(viewModel);
                }
                else
                {
                    var index = newCollection.IndexOf(destinationViewModel);
                    newCollection.Insert(position == Position.Top ? index : index + 1, viewModel);
                }
                if (folderViewModel != null)
                {
                    folderViewModel.IsExpanded = true;
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void treeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && treeView.SelectedItem != null)
            {
                Point position = e.GetPosition((IInputElement)e.Source);
                if (Math.Abs(position.X - StartDragPosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - StartDragPosition.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    var treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);
                    if (treeViewItem != null)
                    {
                        DragDrop.DoDragDrop(treeViewItem, new DataObject(treeView.SelectedItem), DragDropEffects.Move);
                    }
                }
            }
        }

        private void treeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartDragPosition = e.GetPosition((IInputElement)e.Source);
        }

        #endregion

        #endregion
    }
}
