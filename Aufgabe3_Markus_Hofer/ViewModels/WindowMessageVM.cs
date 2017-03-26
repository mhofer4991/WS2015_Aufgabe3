//-----------------------------------------------------------------------
// <copyright file="WindowMessageVM.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a view model, which will be used to display messages in a listbox.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a view model, which will be used to display messages in a list box.
    /// </summary>
    [Serializable]
    public class WindowMessageVM
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowMessageVM"/> class.
        /// </summary>
        /// <param name="message">The message which will be displayed.</param>
        public WindowMessageVM(WindowMessage message)
        {
            this.Message = message;
        }

        /// <summary> Gets the displayed message. </summary>
        /// <value> The displayed message. </value>
        public WindowMessage Message { get; private set; }

        /// <summary>
        /// Gets the text which will be displayed in the list box.
        /// </summary>
        /// <value> The displayed text. </value>
        public string DisplayText
        {
            get
            {
                if (this.Message.Code == WindowMessageCode.Text_message)
                {
                    return (string)this.Message.Content;
                }

                return this.Message.ID.ToString();
            }
        }
    }
}
