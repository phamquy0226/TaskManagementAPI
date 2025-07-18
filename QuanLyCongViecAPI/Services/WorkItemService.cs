﻿using Microsoft.Extensions.Configuration;
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

        public ResponseModel GetWorkItemList(string userName = null, string assignerName = null,
    DateTime? startDateFrom = null, DateTime? startDateTo = null,
    DateTime? endDateFrom = null, DateTime? endDateTo = null,
    int? status = null, int? departmentID = null,
    string searchTaskName = null, int? priority = null, bool? isPinned = null,
    int pageNumber = 1, int pageSize = 20)
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
        new SqlParameter("@IsPinned", SqlDbType.Bit) { Value = isPinned ?? (object)DBNull.Value },
        new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber },
        new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize }
            };

            // Gọi stored và nhận về DataSet
            DataSet ds = _databaseHelper.ExecuteStoredProcedureWithDataSet("sp_GetWorkItemList", parameters);

            if (ds.Tables.Count >= 2)
            {
                // Table 0: trạng thái
                var statusTable = ds.Tables[0];
                bool success = Convert.ToBoolean(statusTable.Rows[0]["Success"]);
                string errorMessage = statusTable.Rows[0]["ErrorMessage"]?.ToString();
                int? errorCode = statusTable.Rows[0]["ErrorCode"] as int?;
                int totalCount = Convert.ToInt32(statusTable.Rows[0]["TotalCount"]);

                // Table 1: dữ liệu WorkItem
                var dataTable = ds.Tables[1];
                var workItems = _databaseHelper.MapDataTableToList<WorkItem>(dataTable);

                return new ResponseModel
                {
                    Success = success,
                    Message = errorMessage,
                    ErrorCode = errorCode,
                    TotalCount = totalCount,
                    Data = workItems
                };
            }

            // Trường hợp lỗi bất thường
            return new ResponseModel
            {
                Success = false,
                Message = "No data returned.",
                ErrorCode = -99
            };
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
        new SqlParameter("@UserIDs", SqlDbType.VarChar, -1) { Value = (object?)userIDs ?? DBNull.Value },
        new SqlParameter("@DateCreate", SqlDbType.DateTime) { Value = DateTime.Now }
            };

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
        new SqlParameter("@UserIDs", SqlDbType.VarChar, -1) { Value = (object?)userIDs ?? DBNull.Value },
        new SqlParameter("@DateCreate", SqlDbType.DateTime) { Value = (object?)model.DateCreate ?? DBNull.Value }
            };

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

        public ResponseModel DeleteWorkItem(int workItemId)
        {
            // Kiểm tra nếu WorkItemID không hợp lệ
            if (workItemId <= 0)
            {
                return new ResponseModel { Success = false, Message = "Invalid WorkItemID", ErrorCode = -1 };
            }

            // Thực hiện gọi stored procedure xóa WorkItem
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@WorkItemID", SqlDbType.Int) { Value = workItemId }
            };

            var result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_DeleteWorkItem", parameters);

            // Kiểm tra kết quả và trả về Response tương ứng
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