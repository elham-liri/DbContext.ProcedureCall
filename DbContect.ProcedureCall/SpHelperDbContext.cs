using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EF.StoreProcedureHelper
{
    public class SpHelperDbContext : DbContext
    {
        public void CallProcedure(IProcedureProfile profile, IProcedureInputConvertible input)
        {
            profile.FillParameters(input);
            if (profile.ResultSet == 0)
                CallVoidProcedure($"{profile.Name} {profile.GetParameterNames()}", profile.GetParameters());
        }

        public IList<T> CallProcedure<T>(IProcedureProfile profile, IProcedureInputConvertible input, bool resetResultSet = false)
        {
            profile.FillParameters(input);

            if (resetResultSet)
                profile.SetResultSetCount(1);

            var result = profile.ResultSet == 1
              ? GetRows<T>($"{profile.Name} {profile.GetParameterNames()}", profile.GetParameters())
              : new List<T>();

            profile.Dispose();
            return result;
        }

        public Tuple<List<T>, List<TM>> CallProcedure<T, TM>(IProcedureProfile profile, IProcedureInputConvertible input, int timeOutMin,
          bool resetResultSet = false)
        {
            profile.FillParameters(input);

            if (resetResultSet)
                profile.SetResultSetCount(2);

            return profile.ResultSet == 2
              ? GetDoubleRowSet<T, TM>(profile.Name, timeOutMin, profile.GetParameters())
              : new Tuple<List<T>, List<TM>>(new List<T>(), new List<TM>());
        }

        public Tuple<List<T>, List<TM>, List<TN>> CallProcedure<T, TM, TN>(IProcedureProfile profile,
          IProcedureInputConvertible input, int timeOutMin, bool resetResultSet = false)
        {
            profile.FillParameters(input);

            if (resetResultSet)
                profile.SetResultSetCount(3);

            return profile.ResultSet == 3
              ? GetTripleRowSet<T, TM, TN>(profile.Name, timeOutMin, profile.GetParameters())
              : new Tuple<List<T>, List<TM>, List<TN>>(new List<T>(), new List<TM>(), new List<TN>());
        }

        public async Task CallProcedureAsync(IProcedureProfile profile, IProcedureInputConvertible input)
        {
            profile.FillParameters(input);
            if (profile.ResultSet == 0)
                await CallVoidProcedureAsync($"{profile.Name} {profile.GetParameterNames()}", profile.GetParameters());
        }

        public async Task CallProcedureAsync(IProcedureProfile profile, IProcedureInputConvertible input, int timeOutMin)
        {
            profile.FillParameters(input);
            if (profile.ResultSet == 0)
                await CallVoidProcedureAsync($"{profile.Name}", timeOutMin, profile.GetParameters());
        }

        public async Task<IList<T>> CallProcedureAsync<T>(IProcedureProfile profile, IProcedureInputConvertible input)
        {
            profile.FillParameters(input);
            return profile.ResultSet == 1
              ? await GetRowsAsync<T>($"{profile.Name} {profile.GetParameterNames()}", profile.GetParameters())
              : new List<T>();
        }


        public void CallVoidProcedure(string sqlQuery, params object[] parameters)
        {
            Database.ExecuteSqlCommand(sqlQuery, parameters);
        }

        public IList<T> GetRows<T>(string sql, params object[] parameters)
        {
            return Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public async Task<IList<T>> GetRowsAsync<T>(string sql, params object[] parameters)
        {
            return await Database.SqlQuery<T>(sql, parameters).ToListAsync();
        }

        public Tuple<List<T>, List<TM>> GetDoubleRowSet<T, TM>(string spName, int timeOutMin, params object[] parameters)
        {

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                cmd.CommandTimeout = timeOutMin * 60;
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    var payment = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader: reader).ToList();

                    reader.NextResult();

                    var performances = ((IObjectContextAdapter)this).ObjectContext.Translate<TM>(reader: reader).ToList();
                    Database.Connection.Close();
                    return new Tuple<List<T>, List<TM>>(payment, performances);
                }
            }

        }

        public Tuple<List<T>, List<TM>, List<TN>> GetTripleRowSet<T, TM, TN>(string spName, int timeOutMin, params object[] parameters)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                cmd.CommandTimeout = timeOutMin * 60;

                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    var firstSet = ((IObjectContextAdapter)this).ObjectContext.Translate<T>(reader: reader).ToList();

                    reader.NextResult();

                    var secondSet = ((IObjectContextAdapter)this).ObjectContext.Translate<TM>(reader: reader).ToList();

                    reader.NextResult();

                    var thirdSet = ((IObjectContextAdapter)this).ObjectContext.Translate<TN>(reader: reader).ToList();

                    return new Tuple<List<T>, List<TM>, List<TN>>(firstSet, secondSet, thirdSet);
                }
            }
        }

        public async Task CallVoidProcedureAsync(string sqlQuery, params object[] parameters)
        {
            await Database.ExecuteSqlCommandAsync(sqlQuery, parameters);
        }

        public async Task CallVoidProcedureAsync(string spName, int timeOutMin, params object[] parameters)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                cmd.CommandTimeout = timeOutMin * 60; 

                if (Database.Connection.State != ConnectionState.Open)
                    await Database.Connection.OpenAsync();

                await cmd.ExecuteNonQueryAsync();
            }
        }


    }
}
