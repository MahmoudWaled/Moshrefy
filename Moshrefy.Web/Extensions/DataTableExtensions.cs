using Microsoft.AspNetCore.Http;
using Moshrefy.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moshrefy.Web.Extensions
{
    public static class DataTableExtensions
    {
        public static DataTableRequest GetDataTableRequest(this HttpRequest request)
        {
            var draw = request.Form["draw"].FirstOrDefault();
            var start = request.Form["start"].FirstOrDefault();
            var length = request.Form["length"].FirstOrDefault();
            var searchValue = request.Form["search[value]"].FirstOrDefault();
            
            // Basic extraction of order[0] - typically sufficient for single column sorting
            var orderColumnIndexVal = request.Form["order[0][column]"].FirstOrDefault();
            int orderColumnIndex = 0;
            if (!string.IsNullOrEmpty(orderColumnIndexVal))
                int.TryParse(orderColumnIndexVal, out orderColumnIndex);

            var sortColumnName = request.Form[$"columns[{orderColumnIndex}][name]"].FirstOrDefault();
            var sortDirection = request.Form["order[0][dir]"].FirstOrDefault();

            // Custom Filters
            var filterDeleted = request.Form["filterDeleted"].FirstOrDefault();
            var activeFilter = request.Form["activeFilter"].FirstOrDefault();
            var filterRole = request.Form["filterRole"].FirstOrDefault();
            var filterStatus = request.Form["filterStatus"].FirstOrDefault();
            var filterAcademicYear = request.Form["filterAcademicYear"].FirstOrDefault();
            int? academicYearId = null;
            if (!string.IsNullOrEmpty(filterAcademicYear) && int.TryParse(filterAcademicYear, out int parsedYearId))
            {
                academicYearId = parsedYearId;
            }
            
            // Advanced Search
            var centerName = request.Form["centerName"].FirstOrDefault();
            var email = request.Form["email"].FirstOrDefault();
            var createdByName = request.Form["createdByName"].FirstOrDefault();
            var adminName = request.Form["adminName"].FirstOrDefault();

            var dtRequest = new DataTableRequest
            {
                Draw = !string.IsNullOrEmpty(draw) ? Convert.ToInt32(draw) : 0,
                Start = !string.IsNullOrEmpty(start) ? Convert.ToInt32(start) : 0,
                Length = !string.IsNullOrEmpty(length) ? Convert.ToInt32(length) : 10,
                Search = new Search { Value = searchValue },
                Order = new List<Order> 
                { 
                    new Order 
                    { 
                        Column = orderColumnIndex, 
                        Dir = sortDirection 
                    } 
                },
                Columns = new List<Column>(), // Can populate if needed, but usually just need name of sorted col
                
                FilterDeleted = filterDeleted,
                ActiveFilter = activeFilter,
                FilterRole = filterRole,
                FilterStatus = filterStatus,
                AcademicYearId = academicYearId,
                
                CenterName = centerName,
                Email = email,
                CreatedByName = createdByName,
                AdminName = adminName
            };

            // Populate the specific column name into the Columns list so SortColumnName property works
            for (int i = 0; i <= orderColumnIndex; i++)
            {
                dtRequest.Columns.Add(new Column());
            }
            dtRequest.Columns[orderColumnIndex].Name = sortColumnName;

            return dtRequest;
        }
    }
}
