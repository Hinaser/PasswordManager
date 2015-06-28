#region Notice
/*
 * Copyright 2015 Yunoske
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

namespace PasswordManager
{
    /// <summary>
    /// Implementation for validation password complexity
    /// </summary>
    class PasswordComplexityValidator : PasswordComplexityValidatorBase
    {
        /// <summary>
        /// Add complexity validation method one by one
        /// </summary>
        protected override void OnInitialize()
        {
            base.AddAdjusterMethod(this.CheckPasswordCharsAreAllDifferent);
            base.AddAdjusterMethod(this.CheckSameCharsInARow);
            base.AddAdjusterMethod(this.CheckUsingSameKindOfCharsInARow);
            base.AddAdjusterMethod(this.CheckNotUsingSameKindOfCharsInARow);
            base.AddAdjusterMethod(this.CheckUsingDangerouslyTypicalWord);
        }

        /// <summary>
        /// Check whether all characters are different from each other.
        /// Length bonus: * 1.2
        /// </summary>
        /// <param name="password"></param>
        /// <param name="charLength"></param>
        /// <returns></returns>
        protected double CheckPasswordCharsAreAllDifferent(string password, double charLength)
        {
            double returnValue = charLength;
            char[] passwordChars = password.ToCharArray();

            HashSet<char> hash = new HashSet<char>();
            bool isAllDifferent = true;
            foreach (char c in passwordChars)
            {
                if (!hash.Add(c))
                {
                    isAllDifferent = false;
                    break;
                }
            }
            if (isAllDifferent)
            {
                // Add strength adjustment report
                base.Report(strings.General_NewPassword_Strength_TypeGood, strings.General_NewPassword_Strength_AllDifferent, Color.Green);
                // Modify adjust value
                returnValue *= 1.2;
            }

            return returnValue;
        }

        /// <summary>
        /// Check whether using the same character in a row.
        /// Length penalty: -0.5/count
        /// </summary>
        /// <param name="password"></param>
        /// <param name="charLength"></param>
        /// <returns></returns>
        protected double CheckSameCharsInARow(string password, double charLength)
        {
            double returnValue = charLength;
            char[] passwordChars = password.ToCharArray();

            for (int i = 1; i < passwordChars.Length; i++)
            {
                if (passwordChars[i - 1] == password[i])
                {
                    // Add strength adjustment report
                    base.Report(strings.General_NewPassword_Strength_TypeBad, strings.General_NewPassword_Strength_SameCharInARow, Color.Red);
                    // Modify adjust value
                    returnValue -= 0.5;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Check whether using the same kind of 5 characters in a row.
        /// Length penalty: : -0.2
        /// </summary>
        /// <param name="password"></param>
        /// <param name="charLength"></param>
        /// <returns></returns>
        protected double CheckUsingSameKindOfCharsInARow(string password, double charLength)
        {
            double returnValue = charLength;
            char[] passwordChars = password.ToCharArray();

            // When password length is less than 5, do nothing
            if (passwordChars.Length < 5)
            {
                return returnValue;
            }

            for (int i = 0; i < passwordChars.Length - 4; i++)
            {
                PasswordTextClass[] pswdClass = new PasswordTextClass[5]
                    {
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+0, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+1, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+2, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+3, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+4, 1))
                    };

                bool isTheSameClassInARow = true;
                for (int j = 1; j < pswdClass.Length; j++)
                {
                    isTheSameClassInARow = isTheSameClassInARow && (pswdClass[j] == pswdClass[0]);
                    if (!isTheSameClassInARow) break;
                }
                if (isTheSameClassInARow)
                {
                    // Add strength adjustment report
                    base.Report(strings.General_NewPassword_Strength_TypeBad, String.Format(strings.General_NewPassword_Strength_SameClassInARow, 5), Color.Red);
                    // Modify adjust value
                    returnValue -= 0.2;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Check whether not using the same kind of 4 characters in a row.
        /// Length bonus: *1.15
        /// </summary>
        /// <param name="password"></param>
        /// <param name="charLength"></param>
        /// <returns></returns>
        protected double CheckNotUsingSameKindOfCharsInARow(string password, double charLength)
        {
            double returnValue = charLength;
            char[] passwordChars = password.ToCharArray();

            // When password length is less than 4, do nothing
            if (passwordChars.Length < 4)
            {
                return returnValue;
            }

            bool areAllCharFrameNeverUsingTheSameClass = true;
            for (int i = 0; i < passwordChars.Length - 3; i++)
            {
                PasswordTextClass[] pswdClass = new PasswordTextClass[4]
                    {
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+0, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+1, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+2, 1)),
                        FormCreatePassword.GetPasswordClass(new String(passwordChars, i+3, 1))
                    };

                bool isTheSameClassInARow = true;
                for (int j = 1; j < pswdClass.Length; j++)
                {
                    isTheSameClassInARow = isTheSameClassInARow && (pswdClass[j] == pswdClass[0]);
                    if (!isTheSameClassInARow) break;
                }
                areAllCharFrameNeverUsingTheSameClass = areAllCharFrameNeverUsingTheSameClass && !isTheSameClassInARow;
            }

            if (areAllCharFrameNeverUsingTheSameClass)
            {
                // Add strength adjustment report
                this.Report(strings.General_NewPassword_Strength_TypeGood, String.Format(strings.General_NewPassword_Strength_SameClassNotInARow, 4), Color.Green);
                // Modify adjust value
                returnValue *= 1.15;
            }

            return returnValue;
        }

        /// <summary>
        /// Check whether password contains a text which is very easy to expect.
        /// Length penalty: =0
        /// </summary>
        /// <param name="password"></param>
        /// <param name="charLength"></param>
        /// <returns></returns>
        protected double CheckUsingDangerouslyTypicalWord(string password, double charLength)
        {
            foreach (string word in LocalConfig.DangerousPasswordList)
            {
                if (password == word)
                {
                    this.Report(strings.General_NewPassword_Strength_TypeCritical, strings.General_NewPassword_Strength_DangerousPassword, Color.Red);
                    return 0;
                }
            }

            return charLength;
        }
    }
}
