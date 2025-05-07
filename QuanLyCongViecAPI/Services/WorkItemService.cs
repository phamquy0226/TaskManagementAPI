using Microsoft.Extensions.Configuration;
using QuanLyCongViecAPI.Helpers;
using QuanLyCongViecAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCongViecAPI.Services
{
    public class WorkItemService
    {
        private readonly DatabaseHelper _databaseHelper;

        public WorkItemService(IConfiguration configuration)
        {
            _databaseHelper = new DatabaseHelper(configuration);
        }

        public ResponseModel GetWorkItemList(string userName = null, string assignerName = null, DateTime? startDateFrom = null, DateTime? startDateTo = null, DateTime? endDateFrom = null, DateTime? endDateTo = null, int? status = null, int? departmentID = null, string searchTaskName = null, int? priority = null, bool? isPinned = null)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserName", SqlDbType.NVarChar, 100) { Value = userName ?? (object)DBNull.Value },
                new SqlParameter("@AssignerName", SqlDbType.NVarChar, 100) { Value = assignerName ?? (object)DBNull.Value },
                new SqlParameter("@StartDateFrom", SqlDbType.DateTime) { Value = startDateFrom ?? (object)DBNull.Value },
                new SqlParameter("@StartDateTo", SqlDbType.DateTime) { Value = startDateTo ?? (object)DBNull.Value },
                new SqlParameter("@EndDateFrom", SqlDbType.DateTime) { Value = endDateFrom ?? (object)DBNull.Value },
                new SqlParameter("@EndDateTo", SqlDbType.DateTime) { Value = endDateTo ?? (object)DBNull.Value },
                new SqlParameter("@Status", SqlDbType.Int) { Value = status ?? (object)DBNull.Value },
                new SqlParameter("@DepartmentID", SqlDbType.Int) { Value = departmentID ?? (object)DBNull.Value },
                new SqlParameter("@SearchTaskName", SqlDbType.NVarChar, 255) { Value = searchTaskName ?? (object)DBNull.Value },
                new SqlParameter("@Priority", SqlDbType.Int) { Value = priority ?? (object)DBNull.Value },
                new SqlParameter("@IsPinned", SqlDbType.Bit) { Value = isPinned ?? (object)DBNull.Value }
            };

            Tuple<bool, string, DataTable> result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_GetWorkItemList", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true, Data = _databaseHelper.MapDataTableToList<WorkItem>(result.Item3) };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 }; // Or a specific error code
            }
        }

        public ResponseModel CreateWorkItem(WorkItemCreateModel model)
        {
            string departmentIDs = model.DepartmentIDs != null ? string.Join(",", model.DepartmentIDs) : null;
            string userIDs = model.UserIDs != null ? string.Join(",", model.UserIDs) : null;

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@TaskName", SqlDbType.NVarChar, 255) { Value = model.TaskName },
        new SqlParameter("@Status", SqlDbType.Int) { Value = model.Status },
        new SqlParameter("@Progress", SqlDbType.Decimal) { Value = model.Progress },
        new SqlParameter("@TaskType", SqlDbType.NVarChar, 50) { Value = (object?)model.TaskType ?? DBNull.Value },
        new SqlParameter("@IsPinned", SqlDbType.Bit) { Value = model.IsPinned },
        new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = (object?)model.StartDate ?? DBNull.Value },
        new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = (object?)model.EndDate ?? DBNull.Value },
        new SqlParameter("@AssignerID", SqlDbType.Int) { Value = model.AssignerID },
        new SqlParameter("@Priority", SqlDbType.Int) { Value = model.Priority },
        new SqlParameter("@DepartmentIDs", SqlDbType.VarChar, -1) { Value = (object?)departmentIDs ?? DBNull.Value },
        new SqlParameter("@UserIDs", SqlDbType.VarChar, -1) { Value = (object?)userIDs ?? DBNull.Value }
            };

            // Gọi hàm đọc kết quả từ bảng #Status
            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_CreateWorkItem", parameters);

            if (result.Item1)
            {
                int? newId = null;
                if (result.Item3 != null && result.Item3.Rows.Count > 0)
                {
                    var idObj = result.Item3.Rows[0]["WorkItemID"];
                    newId = idObj != DBNull.Value ? Convert.ToInt32(idObj) : (int?)null;
                }

                return new ResponseModel { Success = true, Data = newId };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }


        public ResponseModel UpdateWorkItem(WorkItemUpdateModel model)
        {
            // Kiểm tra để đảm bảo WorkItemID không bị thay đổi (dù model luôn cần có WorkItemID)
            if (model.WorkItemID <= 0)
            {
                return new ResponseModel { Success = false, Message = "Invalid WorkItemID", ErrorCode = -1 };
            }

            string departmentIDs = model.DepartmentIDs != null ? string.Join(",", model.DepartmentIDs) : null;
            string userIDs = model.UserIDs != null ? string.Join(",", model.UserIDs) : null;

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@WorkItemID", SqlDbType.Int) { Value = model.WorkItemID },
        new SqlParameter("@TaskName", SqlDbType.NVarChar, 255) { Value = model.TaskName },
        new SqlParameter("@Status", SqlDbType.Int) { Value = model.Status },
        new SqlParameter("@Progress", SqlDbType.Decimal) { Value = model.Progress },
        new SqlParameter("@TaskType", SqlDbType.NVarChar, 50) { Value = (object?)model.TaskType ?? DBNull.Value },
        new SqlParameter("@IsPinned", SqlDbType.Bit) { Value = model.IsPinned },
        new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = (object?)model.StartDate ?? DBNull.Value },
        new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = (object?)model.EndDate ?? DBNull.Value },
        new SqlParameter("@AssignerID", SqlDbType.Int) { Value = model.AssignerID },
        new SqlParameter("@Priority", SqlDbType.Int) { Value = model.Priority },
        new SqlParameter("@DepartmentIDs", SqlDbType.VarChar, -1) { Value = (object?)departmentIDs ?? DBNull.Value },
        new SqlParameter("@UserIDs", SqlDbType.VarChar, -1) { Value = (object?)userIDs ?? DBNull.Value }
            };

            // Gọi stored procedure để cập nhật dữ liệu
            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_UpdateWorkItem", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

        public ResponseModel GetWorkItemById(int workItemId)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@WorkItemID", SqlDbType.Int) { Value = workItemId }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_GetWorkItemByID", parameters);

            if (result.Item1 && result.Item3.Rows.Count > 0)
            {
                var workItem = _databaseHelper.MapDataTableToList<WorkItem>(result.Item3).FirstOrDefault();
                return new ResponseModel { Success = true, Data = workItem };
            }
            else if (result.Item1 && result.Item3.Rows.Count == 0)
            {
                return new ResponseModel { Success = false, Message = "WorkItem not found", ErrorCode = 404 };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

    }
}