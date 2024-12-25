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
        /// <summary>
        /// Executes a stored procedure with no output
        /// </summary>
        /// <param name="profile">stored procedure's profile</param>
        /// <param name="input">object which contains input parameters' values</param>
        /// <exception cref="Exception">exception</exception>
        public void CallProcedure(IProcedureProfile profile, IProcedureInputConvertible input)
        {
            try
            {
                profile.FillParameters(input);
                if (profile.ResultSet == 0)
                    CallVoidProcedure($"{profile.Name} {profile.GetParameterNames()}", profile.GetParameters());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Executes a stored procedure which returns a single set of records
        /// </summary>
        /// <typeparam name="T">type of output</typeparam>
        /// <param name="profile">stored procedure's profile</param>
        /// <param name="input">object which contains input parameters' values</param>
        /// <param name="resetResultSet">reset number of result sets to default</param>
        /// <returns>List of records</returns>
        /// <exception cref="Exception">exception</exception>
        public IList<T> CallProcedure<T>(IProcedureProfile profile, IProcedureInputConvertible input, bool resetResultSet = false)
        {
            try
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
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Executes a stored procedure which returns double sets of records
        /// </summary>
        /// <typeparam name="T">type of first result set</typeparam>
        /// <typeparam name="TM">type of second result set</typeparam>
        /// <param name="profile">stored procedure's profile</param>
        /// <param name="input">object which contains input parameters' values</param>
        /// <param name="timeoutMin">Minutes to wait for execution </param>
        /// <param name="resetResultSet">reset number of result sets to default</param>
        /// <returns>two lists of records</returns>
        /// <exception cref="Exception">exception</exception>
        public Tuple<List<T>, List<TM>> CallProcedure<T, TM>(IProcedureProfile profile, IProcedureInputConvertible input, int timeoutMin,
          bool resetResultSet = false)
        {
            try
            {
                profile.FillParameters(input);

                if (resetResultSet)
                    profile.SetResultSetCount(2);

                return profile.ResultSet == 2
                  ? GetDoubleRowSet<T, TM>(profile.Name, timeoutMin, profile.GetParameters())
                  : new Tuple<List<T>, List<TM>>(new List<T>(), new List<TM>());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
        
        /// <summary>
        /// Executes a stored procedure which returns triple sets of records
        /// </summary>
        /// <typeparam name="T">type of first result set</typeparam>
        /// <typeparam name="TM">type of second result set</typeparam>
        /// <typeparam name="TN">type of third result set</typeparam>
        /// <param name="profile">stored procedure's profile</param>
        /// <param name="input">object which contains input parameters' values</param>
        /// <param name="timeoutMin">Minutes to wait for execution </param>
        /// <param name="resetResultSet">reset number of result sets to default</param>
        /// <returns>Three lists of records</returns>
        /// <exception cref="Exception">exception</exception>
        public Tuple<List<T>, List<TM>, List<TN>> CallProcedure<T, TM, TN>(IProcedureProfile profile,
          IProcedureInputConvertible input, int timeoutMin, bool resetResultSet = false)
        {
            try
            {
                profile.FillParameters(input);

                if (resetResultSet)
                    profile.SetResultSetCount(3);

                return profile.ResultSet == 3
                  ? GetTripleRowSet<T, TM, TN>(profile.Name, timeoutMin, profile.GetParameters())
                  : new Tuple<List<T>, List<TM>, List<TN>>(new List<T>(), new List<TM>(), new List<TN>());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Executes a stored procedure with no output asynchronously
        /// </summary>
        /// <param name="profile">stored procedure's profile</param>
        /// <param name="input">object which contains input parameters' values</param>
        /// <exception cref="Exception">exception</exception>
        public async Task CallProcedureAsync(IProcedureProfile profile, IProcedureInputConvertible input)
        {
            try
            {
                profile.FillParameters(input);
                if (profile.ResultSet == 0)
                    await CallVoidProcedureAsync($"{profile.Name} {profile.GetParameterNames()}", profile.GetParameters());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Executes a stored procedure with no output asynchronously
        /// </summary>
        /// <param name="profile">stored procedure's profile</param>
        /// <param name="input">object which contains input parameters' values</param>
        /// <param name="timeoutMin">Minutes to wait for execution </param>
        /// <returns></returns>
        /// <exception cref="Exception">exception</exception>
        public async Task CallProcedureAsync(IProcedureProfile profile, IProcedureInputConvertible input, int timeoutMin)
        {
            try
            {
                profile.FillParameters(input);
                if (profile.ResultSet == 0)
                    await CallVoidProcedureAsync($"{profile.Name}", timeoutMin, profile.GetParameters());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Executes a stored procedure which returns a single set of records asynchronously
        /// </summary>
        /// <typeparam name="T">type of output</typeparam>
        /// <param name="profile">stored procedure's profile</param>
        /// <param name="input">object which contains input parameters' values</param>
        /// <returns>List of records</returns>
        /// <exception cref="Exception">exception</exception>
        public async Task<IList<T>> CallProcedureAsync<T>(IProcedureProfile profile, IProcedureInputConvertible input)
        {
            try
            {
                profile.FillParameters(input);
                return profile.ResultSet == 1
                  ? await GetRowsAsync<T>($"{profile.Name} {profile.GetParameterNames()}", profile.GetParameters())
                  : new List<T>();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
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

        public Tuple<List<T>, List<TM>> GetDoubleRowSet<T, TM>(string spName, int timeoutMin, params object[] parameters)
        {

            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                cmd.CommandTimeout = timeoutMin * 60;
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

        public Tuple<List<T>, List<TM>, List<TN>> GetTripleRowSet<T, TM, TN>(string spName, int timeoutMin, params object[] parameters)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                cmd.CommandTimeout = timeoutMin * 60;

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

        public async Task CallVoidProcedureAsync(string spName, int timeoutMin, params object[] parameters)
        {
            using (var cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
                cmd.CommandTimeout = timeoutMin * 60;

                if (Database.Connection.State != ConnectionState.Open)
                    await Database.Connection.OpenAsync();

                await cmd.ExecuteNonQueryAsync();
            }
        }


    }
}
