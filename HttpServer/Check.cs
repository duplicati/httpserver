using System;

namespace HttpServer
{
	/// <summary>
	/// Utility class for precondition checks.
	/// </summary>
	public static class Check
	{
		/// <summary>
		/// Check whether a parameter is non empty.
		/// </summary>
		/// <param name="value">Parameter value</param>
		/// <param name="parameterOrErrorMessage">Parameter name, or error description.</param>
		/// <exception cref="ArgumentException">value is empty.</exception>
        public static void NotEmpty(string value, string parameterOrErrorMessage)
		{
            if (string.IsNullOrEmpty(parameterOrErrorMessage))
                Check.NotEmpty(parameterOrErrorMessage, "parameterOrErrorMessage");

			if (!string.IsNullOrEmpty(value))
				return;

            bool isParameterName = parameterOrErrorMessage.IndexOf(' ') == -1;
			if (isParameterName)
                throw new ArgumentException(String.Format("'{0}' cannot be empty.", parameterOrErrorMessage), parameterOrErrorMessage);
            else
                throw new ArgumentException(parameterOrErrorMessage);
		}

		/// <summary>
		/// Checks whether a parameter is non null.
		/// </summary>
		/// <param name="value">Parameter value</param>
		/// <param name="parameterOrErrorMessage">Parameter name, or error description.</param>
		/// <exception cref="ArgumentNullException">value is null.</exception>
		public static void Require(object value, string parameterOrErrorMessage)
		{
            Check.NotEmpty(parameterOrErrorMessage, "parameterOrErrorMessage");

			if (value != null)
				return;

            bool isParameterName = parameterOrErrorMessage.IndexOf(' ') == -1;
            if (isParameterName)
                throw new ArgumentNullException(parameterOrErrorMessage, String.Format("'{0}' cannot be null.", parameterOrErrorMessage));
            else
			    throw new ArgumentNullException(null, parameterOrErrorMessage);
			
		}

		/// <summary>
		/// Checks whether an integer parameter is at least a certain value.
		/// </summary>
		/// <param name="minValue">Minimum value</param>
		/// <param name="value">Parameter value</param>
		/// <param name="parameterOrErrorMessage">Parameter name, or error description.</param>
		/// <exception cref="ArgumentException">value is smaller than minValue.</exception>
		public static void Min(int minValue, int value, string parameterOrErrorMessage)
		{
            Check.NotEmpty(parameterOrErrorMessage, "parameterOrErrorMessage");

            if (minValue <= value)
				return;

            bool isParameterName = parameterOrErrorMessage.IndexOf(' ') == -1;
            if (isParameterName)
				throw new ArgumentException(String.Format("'{0}' must be at least {1}.", parameterOrErrorMessage, minValue));
            else
			    throw new ArgumentException(parameterOrErrorMessage);

		}
	}
}
