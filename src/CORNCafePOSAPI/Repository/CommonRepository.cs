using CORNCafePOSAPICommon;
using CORNCafePOSAPI.Common;
using CORNCafePOSAPI.Model;
using CORNCafePOSAPI.Model.Request;
using CORNCafePOSAPI.Model.Response;
using System.Data;
using System.Dynamic;

namespace CORNCafePOSAPI.Repository
{
    public class CommonRepository : BaseRepository
    {
        public CommonRepository(int userId, string connectionString) : base(userId, connectionString)
        {
        }

        
        public async Task<BaseResponse> GetClientInfo(string pin)
        {
            try
            {
                await CreateDBCommand(CommandType.StoredProcedure, DBConstant.PROC_GET_CLIENT_CONNECTION);

                DbCommand!.Parameters.Add("@ClientPIN", SqlDbType.NVarChar).Value = pin;

                var ds = await FillDataSet();

                if (ds != null)
                {
                    var data = ds.Tables[0].ToObject<ClientInfo>();

                    SetSuccessResponse(data);
                }
                else
                {
                    UsrResponse.Reason = E_ResponseReason.INVALID_PIN;
                }

                return UsrResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeDBObjects();
            }
        }
        public async Task<BaseResponse> Login(CommonRequest request)
        {
            try
            {
                await CreateDBCommand(CommandType.StoredProcedure, DBConstant.PROC_AUTHENTICATE_CORAPI);

                DbCommand!.Parameters.Add("@LOGIN_ID", SqlDbType.NVarChar).Value = request.Username;
                DbCommand.Parameters.Add("@PASSWORD", SqlDbType.NVarChar).Value = request.Password;

                var ds = await FillDataSet();

                if (ds != null && Convert.ToInt64(ds.Tables[0].Rows[0][0]) > 0)
                {
                    var userId = Convert.ToInt64(ds.Tables[0].Rows[0][0]);
                    var userInfo = new ExpandoObject();
                    dynamic data = new ExpandoObject();

                    await ResetDBCommand(CommandType.StoredProcedure, DBConstant.PROC_AUTHENTICATE_USER_ID_CORAPI);

                    DbCommand!.Parameters.Add("@UserID", SqlDbType.Int).Value = userId;

                    ds = await FillDataSet();

                    if (ds != null)
                    {
                        userInfo = ds.Tables[0].ToDynamic()[0];
                    }

                    data.UserId = userId;
                    data.UserInfo = userInfo;

                    SetSuccessResponse(data);
                }
                else
                {
                    UsrResponse.Reason = E_ResponseReason.INVALID_USERNAME_PASSWORD;
                }

                return UsrResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeDBObjects();
            }
        }
        public async Task<BaseResponse> ExecSP(CommonRequest request)
        {
            try
            {
                await CreateDBCommand(CommandType.StoredProcedure, request.SpName!);

                if (request.Parameters != null)
                {
                    foreach (var param in request.Parameters)
                    {
                        var paramName = "@" + param.Key;
                        var paramValue = Convert.ToString(param.Value);

                        DbCommand!.Parameters.Add(paramName, SqlDbType.NVarChar).Value = paramValue;
                    }
                }

                if (!DbCommand!.Parameters.Contains("@UserId"))
                {
                    DbCommand.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                }

                var ds = await FillDataSet();
                dynamic data = new ExpandoObject();

                data.Rows = new List<dynamic>();
                data.TotalLength = 0;

                if (ds != null)
                {
                    if (ds.Tables.Count == 1)
                    {
                        var rows = ds.Tables[0].ToDynamic();

                        data.Rows = rows;
                        data.TotalLength = rows.Count;
                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            data.Rows.Add(ds.Tables[i].ToDynamic());
                        }

                        data.TotalLength = ds.Tables.Count;
                    }
                }

                SetSuccessResponse(data);

                return UsrResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeDBObjects();
            }
        }
        public async Task<BaseResponse> ExecSPOpen(CommonRequest request)
        {
            try
            {
                await CreateDBCommand(CommandType.StoredProcedure, request.SpName!);

                if (request.Parameters != null)
                {
                    foreach (var param in request.Parameters)
                    {
                        var paramName = "@" + param.Key;
                        var paramValue = Convert.ToString(param.Value);

                        DbCommand!.Parameters.Add(paramName, SqlDbType.NVarChar).Value = paramValue;
                    }
                }

                var ds = await FillDataSet();
                dynamic data = new ExpandoObject();

                data.Rows = new List<dynamic>();
                data.TotalLength = 0;

                if (ds != null)
                {
                    if (ds.Tables.Count == 1)
                    {
                        var rows = ds.Tables[0].ToDynamic();

                        data.Rows = rows;
                        data.TotalLength = rows.Count;
                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            data.Rows.Add(ds.Tables[i].ToDynamic());
                        }

                        data.TotalLength = ds.Tables.Count;
                    }
                }

                SetSuccessResponse(data);

                return UsrResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeDBObjects();
            }
        }
        public async Task<BaseResponse> ExecSPOpenParam(string SPName,string MyParameterList)
        {
            try
            {
                await CreateDBCommand(CommandType.StoredProcedure, SPName);
                
                if (MyParameterList != null)
                {
                    if(MyParameterList != "")
                    {
                        if(MyParameterList.Length > 0)
                        {
                            string[] paramList = MyParameterList.Split(',');
                            foreach (string paramlst in paramList)
                            {                                
                                string[] actualparam = paramlst.Split(":");                                
                                try
                                {
                                    var paramName = "@" + actualparam[0];
                                    var paramValue = Convert.ToString(actualparam[1]);
                                    DbCommand!.Parameters.Add(paramName, SqlDbType.NVarChar).Value = paramValue;
                                }
                                catch(Exception e)
                                {

                                }
                            }
                        }
                    }
                }

                var ds = await FillDataSet();
                dynamic data = new ExpandoObject();

                data.Rows = new List<dynamic>();
                data.TotalLength = 0;

                if (ds != null)
                {
                    if (ds.Tables.Count == 1)
                    {
                        var rows = ds.Tables[0].ToDynamic();

                        data.Rows = rows;
                        data.TotalLength = rows.Count;
                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            data.Rows.Add(ds.Tables[i].ToDynamic());
                        }

                        data.TotalLength = ds.Tables.Count;
                    }
                }

                SetSuccessResponse(data);

                return UsrResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeDBObjects();
            }
        }

        public async Task<BaseResponse> UpdateCustomerOTPCode(OTPCodeRequest request,string OTPCode)
        {
            try
            {
                await CreateDBCommand(CommandType.StoredProcedure, "WEBUpdateCustomerOTPCode");                
                try
                {
                    DbCommand!.Parameters.Add("@CustomerPhoneNo", SqlDbType.VarChar).Value = request.CustomerPhoneNo;
                    DbCommand!.Parameters.Add("@OTPCode", SqlDbType.VarChar).Value = OTPCode;                                        
                }
                catch(Exception e)
                {
                }
                var ds = await FillDataSet();
                dynamic data = new ExpandoObject();
                data.Rows = new List<dynamic>();
                data.TotalLength = 0;
                if (ds != null)
                {
                    if (ds.Tables.Count == 1)
                    {
                        var rows = ds.Tables[0].ToDynamic();
                        data.Rows = rows;
                        data.TotalLength = rows.Count;
                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            data.Rows.Add(ds.Tables[i].ToDynamic());
                        }
                        data.TotalLength = ds.Tables.Count;
                    }
                }
                SetSuccessResponse(data);
                return UsrResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeDBObjects();
            }
        }

        public async Task<BaseResponse> GetCustomerByOTPCode(OTPCodeRequest request)
        {
            try
            {
                await CreateDBCommand(CommandType.StoredProcedure, "WEBVerifyCustomerOTPCode");                
                try
                {
                    DbCommand!.Parameters.Add("@CONTACT_NUMBER", SqlDbType.VarChar).Value = request.CustomerPhoneNo;
                    DbCommand!.Parameters.Add("@OTPCode", SqlDbType.VarChar).Value = request.OTPCode;                                        
                }
                catch(Exception e)
                {
                }
                var ds = await FillDataSet();
                dynamic data = new ExpandoObject();
                data.Rows = new List<dynamic>();
                data.TotalLength = 0;
                if (ds != null)
                {
                    if (ds.Tables.Count == 1)
                    {
                        var rows = ds.Tables[0].ToDynamic();
                        data.Rows = rows;
                        data.TotalLength = rows.Count;
                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            data.Rows.Add(ds.Tables[i].ToDynamic());
                        }
                        data.TotalLength = ds.Tables.Count;
                    }
                }
                SetSuccessResponse(data);
                return UsrResponse;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DisposeDBObjects();
            }
        }                
    }
}