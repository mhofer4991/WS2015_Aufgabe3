//-----------------------------------------------------------------------
// <copyright file="NetworkView.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class can be used to draw a network of windows.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// This class can be used to draw a network of windows.
    /// </summary>
    public class NetworkView : IWindowRenderer
    {
        /// <summary> Gets the default width of a window. </summary>
        public const int DefaultWindowWidth = 80;

        /// <summary> Gets the default height of a window. </summary>
        public const int DefaultWindowHeight = 50;

        /// <summary> Gets the default horizontal gap size of a window. </summary>
        public const double DefaultHorizontalGapSize = 20;

        /// <summary> Gets the default vertical gap size of a window. </summary>
        public const double DefaultVerticalGapSize = 40;

        /// <summary> The current horizontal gap size of a window. </summary>
        private double horitzontalGapSize;

        /// <summary> The current vertical gap size of a window. </summary>
        private double verticalGapSize;

        /// <summary> The current size of a window. </summary>
        private Size windowSize;

        /// <summary> The current root of the generated draw tree. </summary>
        private CanvasWindow drawRoot;

        /// <summary> The window, which will be used to generate the draw structure. </summary>
        private IWindow window;
        
        /// <summary> The canvas where the windows will be drawn. </summary>
        private Canvas drawCanvas;

        /// <summary> Indicates whether the updated windows have been rendered or not. </summary>
        private bool renderedUpdate;

        /// <summary> The window, which is currently selected. </summary>
        private CanvasWindow currentlySelectedWindow;

        /// <summary> The window, which currently shows the tooltip. </summary>
        private CanvasWindow currentlyToolTippedWindow;

        /// <summary> This list contains all different cluster identifiers. </summary>
        private List<object> listOfClusterIdentifiers;

        /// <summary> This dictionary assigns a color to each cluster identifier. </summary>
        private Dictionary<object, Color> clusterColorizationRules;

        /// <summary> Is needed to generate random colors for the cluster identifier. </summary>
        private Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkView"/> class.
        /// </summary>
        /// <param name="drawCanvas">The canvas where the windows will be drawn.</param>
        public NetworkView(Canvas drawCanvas)
        {
            this.drawCanvas = drawCanvas;

            this.SetWindowSize(new Size(NetworkView.DefaultWindowWidth, NetworkView.DefaultWindowHeight));
            this.horitzontalGapSize = NetworkView.DefaultHorizontalGapSize;
            this.verticalGapSize = NetworkView.DefaultVerticalGapSize;

            this.random = new Random();

            this.listOfClusterIdentifiers = new List<object>();
            this.clusterColorizationRules = new Dictionary<object, Color>();            
        }

        /// <summary>
        /// Delegate for event OnWindowSelected.
        /// </summary>
        /// <param name="window">The window, which is selected.</param>
        public delegate void WindowSelected(IWindow window);

        /// <summary>
        /// Delegate for event OnWindowToolTipOpened.
        /// </summary>
        /// <param name="window">The window, whose tooltip content is shown.</param>
        public delegate void WindowToolTipOpened(IWindow window);

        /// <summary>
        /// Gets called when a window of the network gets selected.
        /// </summary>
        public event WindowSelected OnWindowSelected;

        /// <summary>
        /// Gets called when a window of the network shows it's tooltip content.
        /// </summary>
        public event WindowToolTipOpened OnWindowToolTipOpened;

        /// <summary>
        /// Sets the window which will be rendered.
        /// </summary>
        /// <param name="window">The window, which will be rendered.</param>
        public void SetWindow(IWindow window)
        {
            this.window = window;
        }

        /// <summary>
        /// Sets the size of a window, which will be drawn.
        /// </summary>
        /// <param name="size">The size of a window.</param>
        public void SetWindowSize(Size size)
        {
            // Update needs to be called.
            this.windowSize = size;
        }

        /// <summary>
        /// Updates the canvas by generating a new draw structure from the current window.
        /// </summary>
        public void Update()
        {
            this.GenerateDrawStructure(this.window);

            this.renderedUpdate = false;
        }

        /// <summary>
        /// Draws the windows on a canvas surface.
        /// </summary>
        public void Render()
        {
            Rect totalSize = this.drawRoot.GetTotalRect();

            ScaleTransform scaletransform = new ScaleTransform();
            scaletransform.ScaleX = this.drawCanvas.ActualWidth / (totalSize.Width + (this.horitzontalGapSize * 2));
            scaletransform.ScaleY = scaletransform.ScaleX;
            
            if (scaletransform.ScaleX > 1)
            {
                scaletransform.ScaleX = 1;
                scaletransform.ScaleY = 1;
            }

            // If the network is larger than the window size, we have to zoom out.
            if ((totalSize.Height + (this.horitzontalGapSize * 2)) * scaletransform.ScaleY > this.drawCanvas.ActualHeight)
            {
                scaletransform.ScaleY = this.drawCanvas.ActualHeight / (totalSize.Height + (this.horitzontalGapSize * 2));
                scaletransform.ScaleX = scaletransform.ScaleY;
            }

            // Center the network
            this.drawRoot.Move(
                (((this.drawCanvas.ActualWidth - (totalSize.Width * scaletransform.ScaleX)) / 2) - (totalSize.X * scaletransform.ScaleX)) / scaletransform.ScaleX, 
                0);

            this.drawRoot.Move(
                0, 
                (((this.drawCanvas.ActualHeight - (totalSize.Height * scaletransform.ScaleY)) / 2) - (totalSize.Y * scaletransform.ScaleY)) / scaletransform.ScaleY);

            // Draw the network
            this.drawCanvas.Children.Clear();

            this.drawCanvas.RenderTransform = scaletransform;

            this.drawRoot.Draw(this.drawCanvas, !this.renderedUpdate);

            this.renderedUpdate = true;
        }

        /// <summary>
        /// Sets the tooltip content of a window.
        /// </summary>
        /// <param name="content">The content for the tooltip.</param>
        public void SetToolTipContent(object content)
        {
            if (this.currentlyToolTippedWindow != null)
            {
                this.currentlyToolTippedWindow.SetToolTipContent(content);
            }
        }

        /// <summary>
        /// Generates a draw structure, which can be used to draw the windows on the canvas.
        /// </summary>
        /// <param name="window">The window, which will be drawn.</param>
        private void GenerateDrawStructure(IWindow window)
        {
            // Look for the root...
            IWindow root = window;

            while (root.GetParent() != null)
            {
                root = root.GetParent();
            }

            this.listOfClusterIdentifiers.Clear();

            CanvasWindow temp = this.currentlySelectedWindow;

            this.drawRoot = this.GetDrawTree(root, null, new Point(0, 0));
            
            if (this.currentlySelectedWindow == null || temp == this.currentlySelectedWindow)
            {
                this.SelectWindow(this.drawRoot);
            }
            else if (this.currentlySelectedWindow.GetUniqueID().Equals(this.drawRoot.GetUniqueID()))
            {
                this.SelectWindow(this.drawRoot);
            }
        }

        /// <summary>
        /// Generates a tree of canvas window instances.
        /// </summary>
        /// <param name="start">The window, from which the tree will be generated.</param>
        /// <param name="parent">The parent window of the start window.</param>
        /// <param name="position">The position of the start window.</param>
        /// <returns>The root of the draw tree.</returns>
        private CanvasWindow GetDrawTree(IWindow start, CanvasWindow parent, Point position)
        {
            int childs = start.GetChildren().Count;

            // Calculate the position of the child element
            double diff = ((childs * this.windowSize.Width) + ((childs - 1) * this.horitzontalGapSize)) - this.windowSize.Width;

            Point startPosition = new Point(position.X - diff, position.Y + this.windowSize.Height + this.verticalGapSize);

            // Create root element
            CanvasWindow root = new CanvasWindow(start);
            root.SetParent(parent, false);
            root.SetPosition(position);
            root.SetSize(new Size(this.windowSize.Width, this.windowSize.Height));
            
            // Get all child elements
            for (int i = 0; i < start.GetChildren().Count; i++)
            {
                // Generate the childs from left to right, whereby the index determines the X - position.
                CanvasWindow newChild = this.GetDrawTree(start.GetChildren()[i], root, new Point(startPosition.X + ((this.windowSize.Width + this.horitzontalGapSize) * i), startPosition.Y));

                // The new child overlaps the rest of the tree.
                if (root.GetTotalRect().X + root.GetTotalRect().Width > newChild.GetTotalRect().X)
                {
                    // Shift it to the right.
                    newChild.Move(root.GetTotalRect().X + root.GetTotalRect().Width - newChild.GetTotalRect().X + this.horitzontalGapSize, 0);
                }

                this.CheckIfIsInCluster(newChild);

                if (this.currentlySelectedWindow != null)
                {
                    if (this.currentlySelectedWindow.GetUniqueID().Equals(newChild.GetUniqueID()))
                    {
                        this.SelectWindow(newChild);
                    }
                }

                root.AddChild(newChild, false);
            }

            root.OnMouseUp += this.Window_OnMouseUp;
            root.OnToolTipOpened += this.Window_OnToolTipOpened;

            this.CheckIfIsInCluster(root);

            // Center parent above the childs.
            if (root.GetChildren().Count > 0)
            {
                CanvasWindow first = (CanvasWindow)root.GetChildren()[0];
                CanvasWindow last = first;

                if (root.GetChildren().Count > 1)
                {
                    last = (CanvasWindow)root.GetChildren()[root.GetChildren().Count - 1];
                }

                double newPos = first.Position.X + ((last.Position.X + last.Size.Width - first.Position.X - root.Size.Width) / 2);

                root.SetPosition(new Point(newPos, root.Position.Y));
            }

            return root;
        }

        /// <summary>
        /// Checks if the given window is in a cluster and assign the color of the cluster to it.
        /// </summary>
        /// <param name="window">The given window.</param>
        private void CheckIfIsInCluster(CanvasWindow window)
        {
            IWindow representedWindow = window.RepresentedWindow;

            if (representedWindow.GetClusterIdentifier() != null)
            {
                // The list already contains this cluster identifier, so there are at least 2 windows with the same cluster identifier.
                if (this.listOfClusterIdentifiers.Contains(representedWindow.GetClusterIdentifier()))
                {
                    // Generate a new color for this cluster identifier.
                    if (!this.clusterColorizationRules.ContainsKey(representedWindow.GetClusterIdentifier()))
                    {
                        Color color = Color.FromRgb((byte)this.random.Next(0, 256), (byte)this.random.Next(0, 256), (byte)this.random.Next(0, 256));

                        this.clusterColorizationRules.Add(representedWindow.GetClusterIdentifier(), color);
                    }

                    // Assign it to the window.
                    window.SetClusterColor(this.clusterColorizationRules[representedWindow.GetClusterIdentifier()]);
                }
                else
                {
                    this.listOfClusterIdentifiers.Add(representedWindow.GetClusterIdentifier());
                }
            }
        }

        /// <summary>
        /// Gets called when the given window shows it's tooltip content.
        /// </summary>
        /// <param name="sender">The given window.</param>
        private void Window_OnToolTipOpened(object sender)
        {
            if (this.OnWindowToolTipOpened != null)
            {
                this.currentlyToolTippedWindow = (CanvasWindow)sender;
                this.OnWindowToolTipOpened(this.currentlyToolTippedWindow.RepresentedWindow);
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the given window.
        /// </summary>
        /// <param name="sender">The given window.</param>
        private void Window_OnMouseUp(object sender)
        {
            CanvasWindow vw = (CanvasWindow)sender;

            this.SelectWindow(vw);
        }

        /// <summary>
        /// Deselects the currently selected window and selects the given window.
        /// </summary>
        /// <param name="vw">The given window.</param>
        private void SelectWindow(CanvasWindow vw)
        {
            if (this.currentlySelectedWindow != null)
            {
                this.currentlySelectedWindow.Select(false);
            }

            this.currentlySelectedWindow = vw;
            this.currentlySelectedWindow.Select(true);

            this.Render();

            if (this.OnWindowSelected != null)
            {
                this.OnWindowSelected(this.currentlySelectedWindow.RepresentedWindow);
            }
        }
    }
}
