#region Notice
/*
 * Author: Yunoske
 * Create Date: June 10, 2015
 * Description :
 * 
 */
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace PasswordManager
{
    /// <summary>
    /// Delegate for output adjustment result to specified location with header emphasized by designated color
    /// </summary>
    /// <param name="header">Summary of adjustment result</param>
    /// <param name="text">Adjustment detail</param>
    /// <param name="headerColor">Header color</param>
    public delegate void ReportComplexityResult(RichTextBox rich, string header, string text, Color headerColor);

    /// <summary>
    /// Adjuster method which validate password text and adjusted character length and returns adjusted character length
    /// </summary>
    /// <param name="password">Password text</param>
    /// <param name="charLength">Password text length (might be adjusted already)</param>
    /// <returns>Adjusted password text length</returns>
    public delegate double ComplexityAdjuster(string password, double charLength);

    /// <summary>
    /// A class for validate password complexity and generate adjusted password character length for later password strength validation.
    /// In this context, password complexity and password strength are different.
    /// Password complexity(defined by the ammount between original password length and its adjusted value) is used to calculate password strength.
    /// </summary>
    public abstract class PasswordComplexityValidatorBase
    {
        /// <summary>
        /// Adjuster must be added in Initialize() method in derived class
        /// </summary>
        private ComplexityAdjuster AdjusterMethods = null;

        /// <summary>
        /// Complexity validation result will be reported through this delegate. When this field is null, then no report will be performed.
        /// Derived class cannot access this field directly. Derived class must call Report method in order to report complexity validation result.
        /// </summary>
        private ReportComplexityResult ReportMethod = null;

        /// <summary>
        /// Target rich textbox to report password strength.
        /// </summary>
        private RichTextBox Rich = null;

        /// <summary>
        /// This class calls Initialize() method. After Initialize() method is called,
        /// if AdjusterMethods is empty, then the constructer throws an Exception.
        /// </summary>
        public PasswordComplexityValidatorBase()
        {
            this.OnInitialize();

            if (this.AdjusterMethods == null)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Exposed method which is cosidered to be called by outside of this class.
        /// This class shall validate password and output the result through ReportComplexityResult delegate and returns adjusted password length
        /// </summary>
        /// <param name="password">Password text</param>
        /// <param name="charLength">Password length (might be adjusted or not)</param>
        /// <param name="reportAction">Reporting method for password complexity validation result</param>
        /// <returns>Adjusted password length</returns>
        public double GetAdjustedCharLength(RichTextBox rich, string password, double charLength, ReportComplexityResult reportAction)
        {
            // If password text is empty, then do nothing
            if (String.IsNullOrEmpty(password))
            {
                return 0;
            }

            // Set ReportMethod field in order for adjuster methods to be able to use reporting funtion
            this.ReportMethod = reportAction;
            this.Rich = rich;

            double returnValue = charLength;

            try
            {
                // Execute delegate in order
                foreach (ComplexityAdjuster d in AdjusterMethods.GetInvocationList())
                {
                    returnValue = d.Invoke(password, returnValue);
                }
            }
            catch (Exception e)
            {
                // Report exception throught reporting delegate
                this.Report(String.Empty, e.Message, Color.Transparent);
                this.Report(String.Empty, e.StackTrace, Color.Transparent);

                // Reset all delegate
                this.Initialize();

                return 0;
            }

            return returnValue;
        }

        /// <summary>
        /// Call ReportMethod delegate if it is not null. If it is null, do nothing.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="text"></param>
        /// <param name="headerColor"></param>
        protected void Report(string header, string text, Color headerColor)
        {
            if (this.ReportMethod == null)
            {
                return;
            }

            this.ReportMethod(this.Rich, header, text, headerColor);
        }

        /// <summary>
        /// Add adjustment method to base delegete field
        /// </summary>
        /// <param name="adj"></param>
        protected void AddAdjusterMethod(ComplexityAdjuster adj)
        {
            if (adj == null)
            {
                throw new ArgumentNullException();
            }

            this.AdjusterMethods += adj;
        }

        /// <summary>
        /// Reset all adjustment delegate at once
        /// </summary>
        public void Initialize()
        {
            this.AdjusterMethods = null;
            this.OnInitialize();
        }

        /// <summary>
        /// A method which is called in base constructor. Derived class must define this method to initialize AdjusterMethods delegate field.
        /// Derived class must implement custom complexity adjuster methods and
        /// must register those method to AdjusterMethods base field throught this Initialize method.
        /// </summary>
        protected abstract void OnInitialize();
    }


    /// <summary>
    /// Classification for validating password strength
    /// </summary>
    public enum PasswordTextClass
    {
        Unknown,
        UseLowercaseOnly,
        UseUppercaseOnly,
        UseNumberOnly,
        UseSymbolOnly,
        UseLowerUpper,
        UseLowerNumber,
        UseLowerSymbol,
        UseUpperNumber,
        UseUpperSymbol,
        UseNumberSymbol,
        UseLowerUpperNumber,
        UseLowerUpperSymbol,
        UseLowerNumberSymbol,
        UseUpperNumberSymbol,
        UseLowerUpperNumberSymbol
    }
}
