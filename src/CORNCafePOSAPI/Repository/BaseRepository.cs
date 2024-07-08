using CORNCafePOSAPI.Common;
using CORNCafePOSAPICommon;
using CORNCafePOSAPI.Model.Response;
using System.Data;
using System.Data.SqlClient;

namespace CORNCafePOSAPI.Repository
{
    public class BaseRepository
    {
        protected SqlCommand? DbCommand;
        protected BaseResponse UsrResponse;
        protected int UserId;

        protected string ConnectionString;

        public BaseRepository(int userId, string connectionString)
        {
            UsrResponse = new BaseResponse();
            UserId = userId;
            ConnectionString = connectionString;
        }

        public async Task<DataSet?> FillDataSet()
        {
            var dbDataSet = new DataSet();
            var dbDataAdapter = new SqlDataAdapter(DbCommand);
            var status = false;

            await Task.Run(() => dbDataAdapter.Fill(dbDataSet));

            for (int i = 0; i < dbDataSet.Tables.Count; i++)
            {
                var table = dbDataSet.Tables[i];

                if (table.Rows.Count > 0)
                {
                    status = true;

                    break;
                }
            }

            if (status)
            {
                return dbDataSet;
            }
            else
            {
                UsrResponse.Reason = E_ResponseReason.DATA_NOT_FOUND;

                return null;
            }
        }
        public async Task<bool> CreateDBCommand(CommandType dbCommandType, string dbCommandText)
        {
            var dbConnection = new SqlConnection
            {
                ConnectionString = string.IsNullOrEmpty(ConnectionString) ? Cache.DBConnectionString : ConnectionString
            };

            await dbConnection.OpenAsync();

            DbCommand = new SqlCommand
            {
                Connection = dbConnection,
                CommandType = dbCommandType,
                CommandText = dbCommandText,
                CommandTimeout = 600
            };

            return true;
        }
        public async Task<bool> ResetDBCommand(CommandType dbCommandType, string dbCommandText)
        {
            if (DbCommand == null || DbCommand.Connection == null || DbCommand.Connection.State != ConnectionState.Open)
            {
                return await CreateDBCommand(dbCommandType, dbCommandText);
            }

            DbCommand!.CommandType = dbCommandType;
            DbCommand.CommandText = dbCommandText;

            DbCommand.Parameters.Clear();

            return true;
        }

        public void SetSuccessResponse(object data)
        {
            UsrResponse.Data = data;
            UsrResponse.Reason = E_ResponseReason.SUCCESS;
        }
        public void SetSuccessResponse(object data, E_ResponseReason reason)
        {
            UsrResponse.Data = data;
            UsrResponse.Reason = E_ResponseReason.SUCCESS;
            UsrResponse.Message = reason.ToDescription();
        }
        public void DisposeDBObjects()
        {
            if (DbCommand != null)
            {
                DbCommand.Cancel();

                if (DbCommand.Connection != null && DbCommand.Connection.State == ConnectionState.Open)
                {
                    DbCommand.Connection.Close();
                }

                DbCommand = null;
            }
        }
    }
}
