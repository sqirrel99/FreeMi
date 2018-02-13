using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FreeMi.Core
{
    /// <summary>
    /// Power management
    /// </summary>
    static class PowerManagement
    {
        #region NativeMethods

        private static class NativeMethods
        {
            #region Enums

            [Flags]
            public enum EXECUTION_STATE : uint
            {
                ES_SYSTEM_REQUIRED = 0x00000001,
                ES_DISPLAY_REQUIRED = 0x00000002,
                // Legacy flag, should not be used.
                // ES_USER_PRESENT   = 0x00000004,
                ES_CONTINUOUS = 0x80000000,
            }

            #endregion

            #region SetThreadExecutionState Methods

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the idle timer
        /// </summary>
        public static void ResetIdleTimer()
        {
            if (!Application.IsMono)
            {
                try
                {
                    NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Prevents the system from entering sleep
        /// </summary>
        public static void PreventSystemStandBy()
        {
            if (!Application.IsMono)
            {
                try
                {
                    NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                        NativeMethods.EXECUTION_STATE.ES_CONTINUOUS);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Allow the system from entering sleep
        /// </summary>
        public static void AllowSystemStandBy()
        {
            if (!Application.IsMono)
            {
                try
                {
                    NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS);
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion
    }
}
