using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FreeMi.Windows
{
    /// <summary>
    /// Handles the visual indication of the drop point
    /// </summary>
    internal class DropAdorner : Adorner, IDisposable
    {
        #region Fields

        private readonly AdornerLayer _adornerLayer;
        private static readonly Pen _pen;
        private static readonly Pen _dashPen;
        private static readonly PathGeometry _triangle;

        #endregion

        #region Constructors

        static DropAdorner()
        {
            _pen = new Pen(Brushes.Gray, 2);
            _pen.Freeze();

            _dashPen = new Pen(Brushes.LightGray, 2);
            _dashPen.DashStyle = DashStyles.Dash;
            _dashPen.Freeze();

            const int triangleSize = 5;
            var firstLine = new LineSegment(new Point(0, -triangleSize), false);
            firstLine.Freeze();
            var secondLine = new LineSegment(new Point(0, triangleSize), false);
            secondLine.Freeze();

            var figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            _triangle = new PathGeometry();
            _triangle.Figures.Add(figure);
            _triangle.Freeze();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="adornerLayer">The adorner layer.</param>
        /// <param name="mousePosition">mouse position</param>
        public DropAdorner(UIElement adornedElement, AdornerLayer adornerLayer, Position mousePosition)
            : base(adornedElement)
        {
            _adornerLayer = adornerLayer;
            _adornerLayer.Add(this);
            MousePosition = mousePosition;
            IsHitTestVisible = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the mouse position
        /// </summary>
        public Position MousePosition { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var tvItem = AdornedElement as TreeViewItem;
            if (tvItem == null)
            {
                return;
            }

            var position = MousePosition;
            var itemRect = Rect.Empty;
            if (VisualTreeHelper.GetChildrenCount(tvItem) > 0)
            {
                var grid = VisualTreeHelper.GetChild(tvItem, 0) as Grid;
                if (grid != null)
                {
                    var descendant = VisualTreeHelper.GetDescendantBounds(tvItem);
                    itemRect = new Rect(tvItem.TranslatePoint(new Point(), tvItem),
                        new Size(tvItem.RenderSize.Width, grid.RowDefinitions[0].ActualHeight));
                }
            }
            if (itemRect.IsEmpty)
            {
                return;
            }

            switch (position)
            {
                case Position.Middle:
                    drawingContext.DrawRoundedRectangle(null, _dashPen, itemRect, 2, 2);
                    break;
                default:
                    if (position == Position.Bottom)
                    {
                        itemRect.Y += itemRect.Height;
                    }

                    Point point1, point2;
                    double rotation = 0;
                    point1 = new Point(itemRect.X, itemRect.Y);
                    point2 = new Point(itemRect.Right, itemRect.Y);

                    drawingContext.DrawLine(_pen, point1, point2);
                    DrawTriangle(drawingContext, point1, rotation);
                    DrawTriangle(drawingContext, point2, 180 + rotation);
                    break;
            }
        }

        private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(rotation));

            drawingContext.DrawGeometry(_pen.Brush, null, _triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Free managed resources
                _adornerLayer.Remove(this);
            }
        }

        #endregion
    }
}
