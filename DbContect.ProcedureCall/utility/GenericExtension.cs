using System;
using System.Collections;
using System.Data;
using System.Linq;

namespace EF.StoreProcedureHelper.utility
{
    internal static class GenericExtension
    {
        public static DataTable ToDataTable(this IEnumerable self)
        {
            var type = self.GetType().GetGenericArguments()[0];
            var properties = type.GetProperties();// typeof(T).GetProperties();

            var dataTable = new DataTable();
            foreach (var info in properties)
                dataTable.Columns.Add(info.Name.FirstCharToLower(), Nullable.GetUnderlyingType(info.PropertyType)
                                                                    ?? info.PropertyType);

            foreach (var entity in self)
                dataTable.Rows.Add(properties.Select(p => p.GetValue(entity)).ToArray());

            return dataTable;
        }
    }
}
