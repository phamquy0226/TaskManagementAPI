using Microsoft.Extensions.Configuration;
using QuanLyCongViecAPI.Helpers;
using QuanLyCongViecAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCongViecAPI.Services
{
    public class DepartmentService
    {
        private readonly DatabaseHelper _databaseHelper;

        // Constructor để inject DatabaseHelper
        public DepartmentService(IConfiguration configuration)
        {
            _databaseHelper = new DatabaseHelper(configuration);
        }

        // Lấy danh sách department
        public ResponseModel GetDepartments()
        {
            SqlParameter[] parameters = new SqlParameter[] { };
            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_GetDepartments", parameters);

            if (result.Item1)
            {
                var departments = _databaseHelper.MapDataTableToList<Department>(result.Item3);
                return new ResponseModel { Success = true, Data = departments, ErrorCode = -1000 };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1101 };
            }
        }

        //test jenkins 222
        public ResponseModel CreateDepartment(DepartmentCreateModel model)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 255) { Value = model.DepartmentName }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_InsertDepartment", parameters);

            if (result.Item1)
            {
                int? newId = null;
                if (result.Item3 != null && result.Item3.Rows.Count > 0)
                {
                    var idObj = result.Item3.Rows[0]["DepartmentID"];
                    newId = idObj != DBNull.Value ? Convert.ToInt32(idObj) : (int?)null;
                }

                return new ResponseModel { Success = true, Data = newId };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

        
        public ResponseModel UpdateDepartment(DepartmentUpdateModel model)
        {
           
            if (model.DepartmentID <= 0)
            {
                return new ResponseModel { Success = false, Message = "Invalid DepartmentID", ErrorCode = -1 };
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = model.DepartmentID },
                new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 255) { Value = model.DepartmentName }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_UpdateDepartment", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

        
        public ResponseModel DeleteDepartment(int departmentID)
        {
            if (departmentID <= 0)
            {
                return new ResponseModel { Success = false, Message = "Invalid DepartmentID", ErrorCode = -1 };
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_DeleteDepartment", parameters);

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
