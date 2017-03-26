//-----------------------------------------------------------------------
// <copyright file="WindowVM.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a view model, which will be used to display information about a window.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a view model, which will be used to display information about a window.
    /// </summary>
    public class WindowVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowVM"/> class.
        /// </summary>
        public WindowVM()
        {
        }

        /// <summary>
        /// Gets called when a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the window which will be represented by this view model.
        /// </summary>
        /// <value> The window which will be represented by this view model. </value>
        public IWindow Window { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the window can be displayed or not.
        /// </summary>
        /// <value> A value indicating whether the window can be displayed or not. </value>
        public bool IsAvailable
        {
            get
            {
                return this.Window != null;
            }
        }

        /// <summary>
        /// Gets the ID of the window.
        /// </summary>
        /// <value> The ID of the window. </value>
        public int ID
        {
            get
            {
                if (this.Window == null)
                {
                    return 0;
                }

                return this.Window.GetID();
            }
        }

        /// <summary>
        /// Sets the window of the view model.
        /// </summary>
        /// <param name="window">The window which will be displayed.</param>
        public void SetWindow(IWindow window)
        {
            this.Window = window;

            this.PropertyChanged(this, new PropertyChangedEventArgs("IsAvailable"));
            this.PropertyChanged(this, new PropertyChangedEventArgs("ID"));
        }
    }
}
