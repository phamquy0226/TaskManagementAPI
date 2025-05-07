using Microsoft.Extensions.Configuration;
using QuanLyCongViecAPI.Helpers;
using QuanLyCongViecAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCongViecAPI.Services
{
    public class UserService
    {
        private readonly DatabaseHelper _databaseHelper;

        public UserService(IConfiguration configuration)
        {
            _databaseHelper = new DatabaseHelper(configuration);
        }

        // Lấy danh sách user
        public ResponseModel GetUsers()
        {
            Tuple<bool, string, DataTable> result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_GetUsers");

            if (result.Item1)
            {
                return new ResponseModel { Success = true, Data = _databaseHelper.MapDataTableToList<User>(result.Item3) };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

        // Thêm mới user
        public ResponseModel InsertUser(string userName)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", SqlDbType.NVarChar, 100) { Value = userName }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_InsertUser", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

        // Sửa thông tin user
        public ResponseModel UpdateUser(int userID, string userName)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SqlDbType.Int) { Value = userID },
                new SqlParameter("@UserName", SqlDbType.NVarChar, 100) { Value = userName }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_UpdateUser", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

        // Xoá user
        public ResponseModel DeleteUser(int userID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SqlDbType.Int) { Value = userID }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_DeleteUser", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }
    }
}
