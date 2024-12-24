using System;
using System.Data.SqlClient;

namespace DbContext.ProcedureCall
{
    public interface IProcedureProfile
    {
        string Name { get; }
        int ResultSet { get; }
        bool HasInput { get; }
        Type InputDataType { get; }
        SqlParameter[] GetParameters();
        void SetResultSetCount(int resultSets);
        void FillParameters(IProcedureInputConvertible input);
        string GetParameterNames();
    }
}
