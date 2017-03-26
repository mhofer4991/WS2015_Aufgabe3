//-----------------------------------------------------------------------
// <copyright file="IWindowRenderer.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Classes, which implement this interface, are able to render windows.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Classes, which implement this interface, are able to render windows.
    /// </summary>
    public interface IWindowRenderer
    {
        /// <summary>
        /// Sets the window which will be rendered.
        /// </summary>
        /// <param name="window">The window, which will be rendered.</param>
        void SetWindow(IWindow window);

        /// <summary>
        /// Tells the renderer that the window has changed.
        /// </summary>
        void Update();

        /// <summary>
        /// Renders the window.
        /// </summary>
        void Render();
    }
}
