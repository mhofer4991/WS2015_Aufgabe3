//-----------------------------------------------------------------------
// <copyright file="CanvasWindow.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a window, which can be drawn on a canvas surface.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// This class represents a window, which can be drawn on a canvas surface.
    /// </summary>
    public class CanvasWindow : BasicWindow
    {
        /// <summary> The tooltip of the window. </summary>
        private ToolTip toolTip;

        /// <summary> The color, which will be used to identify the cluster. </summary>
        private Color clusterColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasWindow"/> class.
        /// </summary>
        /// <param name="representedWindow">The window, which will be represented by this class.</param>
        public CanvasWindow(IWindow representedWindow) : base(representedWindow.GetID(), representedWindow.GetUniqueID())
        {
            this.RepresentedWindow = representedWindow;

            this.clusterColor = Color.FromArgb(255, 100, 100, 100);
        }

        /// <summary>
        /// Delegate for event OnMouseUp.
        /// </summary>
        /// <param name="sender">The sender, which has fired the event.</param>
        public delegate void MouseUp(object sender);

        /// <summary>
        /// Delegate for event OnToolTipOpened.
        /// </summary>
        /// <param name="sender">The sender, which has fired the event.</param>
        public delegate void ToolTipOpened(object sender);

        /// <summary>
        /// Gets called when the user clicks on the window.
        /// </summary>
        public event MouseUp OnMouseUp;

        /// <summary>
        /// Gets called when the user wants to see the tooltip of the window.
        /// </summary>
        public event ToolTipOpened OnToolTipOpened;

        /// <summary> Gets the window, which is represented by this class. </summary>
        /// <value> The window, which is represented by this class. </value>
        public IWindow RepresentedWindow { get; private set; }

        /// <summary> Gets the position of the window on canvas surface. </summary>
        /// <value> The position of the window on canvas surface. </value>
        public Point Position { get; private set; }

        /// <summary> Gets the size of the window on canvas surface. </summary>
        /// <value> The size of the window on canvas surface. </value>
        public Size Size { get; private set; }

        /// <summary> Gets a value indicating whether the window is currently selected or not. </summary>
        /// <value> A value indicating whether the window is currently selected or not. </value>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// Sets the color, which is used to identify the cluster.
        /// </summary>
        /// <param name="color">The color of the cluster.</param>
        public void SetClusterColor(Color color)
        {
            this.clusterColor = color;
        }

        /// <summary>
        /// Sets the position of the window.
        /// </summary>
        /// <param name="position">The new position.</param>
        public void SetPosition(Point position)
        {
            this.Position = position;
        }

        /// <summary>
        /// Sets the size of the window.
        /// </summary>
        /// <param name="size">The new size.</param>
        public void SetSize(Size size)
        {
            this.Size = size;
        }

        /// <summary>
        /// Gets the position and size of this window plus all it's sub windows, which is represented as a rectangle.
        /// </summary>
        /// <returns>A rectangle representing the total position and size.</returns>
        public Rect GetTotalRect()
        {
            Rect total = new Rect(this.Position, this.Size);

            foreach (CanvasWindow child in this.GetChildren())
            {
                Rect temp = child.GetTotalRect();

                if (temp.X < total.X)
                {
                    total.Width += total.X - temp.X;
                    total.X = temp.X;
                }

                if (temp.X + temp.Width > total.X + total.Width)
                {
                    total.Width = temp.X + temp.Width - total.X;
                }

                if (temp.Y + temp.Height > total.Y + total.Height)
                {
                    total.Height = temp.Y + temp.Height - total.Y;
                }
            }

            return total;
        }

        /// <summary>
        /// Sets the content of the tooltip.
        /// </summary>
        /// <param name="content">The content.</param>
        public void SetToolTipContent(object content)
        {
            this.toolTip.Dispatcher.BeginInvoke(new Action(() =>
            {
                List<WindowMessageVM> messages = (List<WindowMessageVM>)content;                
                StackPanel panel = new StackPanel();

                foreach (WindowMessageVM message in messages)
                {
                    panel.Children.Add(new TextBlock() { Text = message.DisplayText });
                }

                this.toolTip.Content = panel;
            }));
        }

        /// <summary>
        /// Decides if the window is selected or not.
        /// </summary>
        /// <param name="isSelected">Indicates whether the window is selected or not.</param>
        public void Select(bool isSelected)
        {
            this.IsSelected = isSelected;
        }

        /// <summary>
        /// Moves the window plus it's sub windows.
        /// </summary>
        /// <param name="moveX">Amount of pixels in X - direction.</param>
        /// <param name="moveY">Amount of pixels in Y - direction.</param>
        public void Move(double moveX, double moveY)
        {
            this.Position = new Point(this.Position.X + moveX, this.Position.Y + moveY);

            foreach (CanvasWindow child in this.GetChildren())
            {
                child.Move(moveX, moveY);
            }
        }

        /// <summary>
        /// Draws the window on the given canvas.
        /// </summary>
        /// <param name="canvas">The given canvas, where the window will be drawn.</param>
        /// <param name="animated">Indicates whether the drawing should be animated or not.</param>
        public void Draw(Canvas canvas, bool animated)
        {
            this.Draw(canvas, animated, this.Position);
        }

        /// <summary>
        /// Draws the window on the given canvas.
        /// </summary>
        /// <param name="canvas">The given canvas, where the window will be drawn.</param>
        /// <param name="animated">Indicates whether the drawing should be animated or not.</param>
        /// <param name="animationStart">Position of the window, where the animation should start.</param>
        private void Draw(Canvas canvas, bool animated, Point animationStart)
        {
            // Draw grid for rectangle and text
            Grid grid = new Grid();
            grid.Width = this.Size.Width;
            grid.Height = this.Size.Height;

            // Draw rectangle in specific color
            Rectangle rect = new Rectangle();
            rect.Width = this.Size.Width;
            rect.Height = this.Size.Height;

            rect.Stroke = new SolidColorBrush(this.clusterColor);
            rect.StrokeThickness = 2;

            // Depending on whether it is selected or not we use a specific color for the filling
            if (!this.IsSelected)
            {
                rect.Fill = new SolidColorBrush(Color.FromArgb(255, 248, 248, 216));
            }
            else
            {
                rect.Fill = new SolidColorBrush(Color.FromArgb(255, 220, 220, 200));
            }

            // Draw the text
            TextBlock text = new TextBlock();
            text.Text = this.GetID().ToString();
            text.Width = this.Size.Width;
            text.Height = this.Size.Height;
            text.TextAlignment = TextAlignment.Center;
            text.FontSize = this.Size.Height / 3;

            // Add all childs to the canvas
            grid.Children.Add(rect);
            grid.Children.Add(text);

            canvas.Children.Add(grid);

            // Create a tooltip
            ToolTip tooltip = new ToolTip();
            grid.ToolTip = tooltip;

            this.toolTip = tooltip;

            grid.ToolTipOpening += this.Grid_ToolTipOpening;
            grid.MouseUp += this.Rect_MouseUp;

            // Set the position of the grid. This can be animated by fading in and moving.
            if (animated)
            {
                Canvas.SetTop(grid, animationStart.Y);
                Canvas.SetLeft(grid, animationStart.X);

                grid.Opacity = 0;
                grid.BeginAnimation(Grid.OpacityProperty, new DoubleAnimation(1, TimeSpan.FromSeconds(0.375)));

                grid.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(this.Position.Y, TimeSpan.FromSeconds(0.375)));
                grid.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(this.Position.X, TimeSpan.FromSeconds(0.375)));
            }
            else
            {
                Canvas.SetTop(grid, this.Position.Y);
                Canvas.SetLeft(grid, this.Position.X);
            }

            int index = 1;

            foreach (CanvasWindow child in this.GetChildren())
            {
                // Draw line to each child node
                Line line = new Line();

                line.Stroke = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
                line.StrokeThickness = 1;

                // Set the position of the line. This can be animated by changing the size and position of the line.
                if (animated)
                {
                    line.X1 = animationStart.X + ((this.Size.Width / (this.GetChildren().Count + 1)) * index);
                    line.Y1 = animationStart.Y + this.Size.Height;

                    line.X2 = this.Position.X + ((this.Size.Width / (this.GetChildren().Count + 1)) * index);
                    line.Y2 = this.Position.Y + this.Size.Height;

                    line.BeginAnimation(
                        Line.X1Property,
                        new DoubleAnimation(this.Position.X + ((this.Size.Width / (this.GetChildren().Count + 1)) * index), TimeSpan.FromSeconds(0.375)));

                    line.BeginAnimation(Line.Y1Property, new DoubleAnimation(this.Position.Y + this.Size.Height, TimeSpan.FromSeconds(0.375)));

                    line.BeginAnimation(Line.X2Property, new DoubleAnimation(child.Position.X + (child.Size.Width / 2), TimeSpan.FromSeconds(0.375)));
                    line.BeginAnimation(Line.Y2Property, new DoubleAnimation(child.Position.Y, TimeSpan.FromSeconds(0.375)));
                }
                else
                {
                    line.X1 = this.Position.X + ((this.Size.Width / (this.GetChildren().Count + 1)) * index);
                    line.Y1 = this.Position.Y + this.Size.Height;

                    line.X2 = child.Position.X + (child.Size.Width / 2);
                    line.Y2 = child.Position.Y;
                }

                canvas.Children.Add(line);

                child.Draw(canvas, animated, this.Position);

                index++;
            }
        }

        /// <summary>
        /// Gets called when the user wants to see the content of the tooltip.
        /// </summary>
        /// <param name="sender">The sender, which called the event.</param>
        /// <param name="e">Arguments passed by this event.</param>
        private void Grid_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (this.OnToolTipOpened != null)
            {
                this.OnToolTipOpened(this);
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the window.
        /// </summary>
        /// <param name="sender">The sender, which called the event.</param>
        /// <param name="e">Arguments passed by this event.</param>
        private void Rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.OnMouseUp != null)
            {
                this.OnMouseUp(this);
            }
        }
    }
}
