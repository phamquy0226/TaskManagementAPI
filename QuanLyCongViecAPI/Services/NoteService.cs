using Microsoft.Extensions.Configuration;
using QuanLyCongViecAPI.Helpers;
using QuanLyCongViecAPI.Models;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace QuanLyCongViecAPI.Services
{
    public class NoteService
    {
        private readonly DatabaseHelper _databaseHelper;

        public NoteService(IConfiguration configuration)
        {
            _databaseHelper = new DatabaseHelper(configuration);
        }

        public ResponseModel GetNotesByWorkItem(int workItemID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@WorkItemID", SqlDbType.Int) { Value = workItemID }
            };

            Tuple<bool, string, DataTable> result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_GetNotesByWorkItem", parameters);

            if (result.Item1)
            {
                if (result.Item3 != null && result.Item3.Rows.Count > 0)
                {
                    
                    foreach (DataRow row in result.Item3.Rows)
                    {
                       
                        Console.WriteLine($"NoteID: {row["NoteID"]}, WorkItemID: {row["WorkItemID"]}, Content: {row["Content"]}");
                    }

                    var notes = _databaseHelper.MapDataTableToList<Note>(result.Item3);
                    return new ResponseModel { Success = true, Data = notes };
                }
                else
                {
                    return new ResponseModel { Success = false, Message = "No notes found for the specified WorkItem.", ErrorCode = -1 };
                }
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }



        public ResponseModel AddNote(NoteCreateModel model)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@WorkItemID", SqlDbType.Int) { Value = model.WorkItemID },
        new SqlParameter("@Content", SqlDbType.NVarChar, -1) { Value = model.Content },
        new SqlParameter("@DateCreate", SqlDbType.DateTime) { Value = model.DateCreate } 
            };

            Tuple<bool, string, int?> result = _databaseHelper.ExecuteNonQueryStoredProcedureWithStatus("sp_AddNote", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true, Data = result.Item3 };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }
    

        public ResponseModel UpdateNote(NoteUpdateModel model)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NoteID", SqlDbType.Int) { Value = model.NoteID },
                new SqlParameter("@Content", SqlDbType.NVarChar, -1) { Value = model.Content }
            };

            Tuple<bool, string, DataTable> result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_UpdateNote", parameters);

            if (result.Item1)
            {
                return new ResponseModel { Success = true };
            }
            else
            {
                return new ResponseModel { Success = false, Message = result.Item2, ErrorCode = -1 };
            }
        }

        public ResponseModel DeleteNote(int noteID)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NoteID", SqlDbType.Int) { Value = noteID }
            };

            Tuple<bool, string, DataTable> result = _databaseHelper.ExecuteStoredProcedureWithStatus("sp_DeleteNote", parameters);

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