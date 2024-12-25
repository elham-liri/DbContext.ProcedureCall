using System;
using System.Data.SqlClient;

namespace EF.StoreProcedureHelper
{
    /// <summary>
    /// Use to build a profile for a stored procedure
    /// </summary>
    public interface IProcedureProfile : IDisposable
    {
        /// <summary>
        /// Stored procedure's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Number of result sets that stored procedure will return by default
        /// </summary>
        int ResultSet { get; }

        /// <summary>
        /// Determine if stored procedure has any input 
        /// </summary>
        bool HasInput { get; }

        /// <summary>
        /// Type of stored procedure's input ; Should be a class
        /// </summary>
        Type InputDataType { get; }
        
        /// <summary>
        /// List of stored procedure parameters
        /// </summary>
        /// <returns></returns>
        SqlParameter[] GetParameters();

        /// <summary>
        /// Set a new value for ResultSet to be used instead of default number of result sets
        /// </summary>
        /// <param name="resultSets">Number of result sets</param>
        void SetResultSetCount(int resultSets);

        /// <summary>
        /// Set the value of store procedure's parameters using an object 
        /// </summary>
        /// <param name="input">object which holds the input values for parameters</param>
        void FillParameters(IProcedureInputConvertible input);

        /// <summary>
        /// Returns the list of parameters 
        /// </summary>
        /// <returns>list of parameters separated by comma</returns>
        string GetParameterNames();
    }
}
