using System;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using EF.StoreProcedureHelper.utility;

namespace EF.StoreProcedureHelper
{
    public abstract class BaseProcedureProfile : IProcedureProfile
    {

        protected SqlParameter[] Parameters;
        protected int MaxResultSet;

        public abstract string Name { get; }
        public virtual int ResultSet => MaxResultSet;
        public abstract bool HasInput { get; }
        public abstract Type InputDataType { get; }

        public void SetResultSetCount(int resultSets)
        {
            MaxResultSet = resultSets;
        }

        public SqlParameter[] GetParameters()
        {
            return Parameters;
        }

        public virtual void FillParameters(IProcedureInputConvertible input)
        {
            if (!HasInput || Parameters.Length == 0) return;

            var type = input.GetType();
            if (type != InputDataType) return;

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var parameterName = $"@{property.Name.FirstCharToLower()}";
                var parameter = Parameters.FirstOrDefault(a => a.ParameterName == parameterName);
                if (parameter == null) continue;

                if (property.PropertyType.IsClass && property.PropertyType == typeof(string))
                {
                    var val = property.GetValue(input);
                    parameter.Value = val ?? string.Empty;
                }
                else if (property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                    parameter.Value = property.GetValue(input);
                }
                else if (property.PropertyType.IsEnum)
                {
                    parameter.Value = Convert.ToInt32(property.GetValue(input));
                }
                else if (property.PropertyType.IsGenericType)
                {
                    var values = property.GetValue(input) as IEnumerable;
                    var dataTable = values.ToDataTable();
                    parameter.Value = dataTable;
                }
            }
        }

        public virtual string GetParameterNames()
        {
            var names = Parameters.Select(a => a.ParameterName).ToArray();
            return string.Join(",", names);
        }

        public void Dispose()
        {
        }
    }
}
