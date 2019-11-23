using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WindowsExplorer
{
    public class ListViewDragSelectAdorner : Adorner
    {
        private Canvas selectRectangleCanvas;
        private Rectangle selectRectangle;
        private Point startPoint;
        private Point endPoint;
        public Point StartPoint
        {
            get => this.startPoint;
            set
            {
                this.startPoint = value;
                this.UpdateRectangle();
            }
        }
        public Point EndPoint
        {
            get => this.endPoint;
            set
            {
                this.endPoint = value;
                this.UpdateRectangle();
            }
        }

        public ListViewDragSelectAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.selectRectangleCanvas = new Canvas
            {
                Height = double.NaN,
                Width = double.NaN,
            };
            this.selectRectangle = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromRgb(170, 204, 238)) { Opacity = 0.3 },
                Stroke = new SolidColorBrush(Color.FromRgb(0,120, 215)),
            };
            this.selectRectangleCanvas.Children.Add(this.selectRectangle);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.selectRectangleCanvas.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.selectRectangleCanvas;
        }

        protected override int VisualChildrenCount => 1;

        private void UpdateRectangle()
        {
            Point topLeft = new Point(Math.Min(this.StartPoint.X, this.EndPoint.X), Math.Min(this.StartPoint.Y, this.endPoint.Y));
            Size size = new Size(Math.Abs(this.StartPoint.X - this.EndPoint.X), Math.Abs(this.StartPoint.Y - this.EndPoint.Y));

            Canvas.SetLeft(this.selectRectangle, topLeft.X);
            Canvas.SetTop(this.selectRectangle, topLeft.Y);
            this.selectRectangle.Width = size.Width;
            this.selectRectangle.Height = size.Height;
            AdornerLayer.GetAdornerLayer(this.AdornedElement).Update();
        }
}
}
