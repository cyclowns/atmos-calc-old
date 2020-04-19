using System;
using System.Collections.Generic;
using System.Text;

namespace AtmosLibrary
{
    /// <summary>
    /// Throws on any generic atmos exception
    /// </summary>
    public class AtmosException : Exception
    {
        /// <summary>
        /// Exception base constructor
        /// </summary>
        public AtmosException()
            :base()
        {
        }
        /// <summary>
        /// Exception constructor with message
        /// </summary>
        /// <param name="message">Exception description</param>
        public AtmosException(String message)
            : base()
        {
        }
        /// <summary>
        /// Exception constructor with message and exception cause
        /// </summary>
        /// <param name="message">Exception description</param>
        /// <param name="innerexception">Inner cause of exception</param>
        public AtmosException(String message, Exception innerexception)
            : base()
        {
        }
        /// <summary>
        /// Thrown when a GasMixture is created with volume = zero, which causes DivideByZero errors
        /// </summary>
        public class VolumeZeroException : Exception
        {
            /// <summary>
            /// Exception base constructor
            /// </summary>
            public VolumeZeroException()
                : base()
            {
            }
            /// <summary>
            /// Exception constructor with message
            /// </summary>
            /// <param name="message">Exception description</param>
            public VolumeZeroException(String message)
                : base(message)
            {
            }
            /// <summary>
            /// Exception constructor with message and exception cause
            /// </summary>
            /// <param name="message">Exception description</param>
            /// <param name="innerexception">Inner cause of exception</param>
            public VolumeZeroException(String message, Exception innerexception)
                : base(message, innerexception)
            {
            }
        }
        /// <summary>
        /// Thrown when AddTank fails as the gas percents do not add up to 100%
        /// </summary>
        public class GasPercentException : Exception
        {
            /// <summary>
            /// Exception base constructor
            /// </summary>
            public GasPercentException()
                : base()
            {
            }
            /// <summary>
            /// Exception constructor with message
            /// </summary>
            /// <param name="message">Exception description</param>
            public GasPercentException(String message)
                : base(message)
            {
            }
            /// <summary>
            /// Exception constructor with message and exception cause
            /// </summary>
            /// <param name="message">Exception description</param>
            /// <param name="innerexception">Inner cause of exception</param>
            public GasPercentException(String message, Exception innerexception)
                : base(message, innerexception)
            {
            }
        }
        /// <summary>
        /// Thrown when a given temperature/mole quantity is less than zero
        /// </summary>
        public class NegativeException : Exception
        {
            /// <summary>
            /// Exception base constructor
            /// </summary>
            public NegativeException()
                : base()
            {
            }
            /// <summary>
            /// Exception constructor with message
            /// </summary>
            /// <param name="message">Exception description</param>
            public NegativeException(String message)
                : base(message)
            {
            }
            /// <summary>
            /// Exception constructor with message and exception cause
            /// </summary>
            /// <param name="message">Exception description</param>
            /// <param name="innerexception">Inner cause of exception</param>
            public NegativeException(String message, Exception innerexception)
                : base(message, innerexception)
            {
            }
        }
        /// <summary>
        /// Thrown when a error in Calculate happens
        /// </summary>
        public class CalculateResultsException : Exception
        {
            /// <summary>
            /// Exception base constructor
            /// </summary>
            public CalculateResultsException()
                : base()
            {
            }
            /// <summary>
            /// Exception constructor with message
            /// </summary>
            /// <param name="message">Exception description</param>
            public CalculateResultsException(String message)
                : base(message)
            {
            }
            /// <summary>
            /// Exception constructor with message and exception cause
            /// </summary>
            /// <param name="message">Exception description</param>
            /// <param name="innerexception">Inner cause of exception</param>
            public CalculateResultsException(String message, Exception innerexception)
                : base(message, innerexception)
            {
            }
        }
    }
}
