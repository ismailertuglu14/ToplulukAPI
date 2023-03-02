using System;
using Topluluk.Shared.Enums;

namespace Topluluk.Shared.Dtos
{
    public class PagingResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public static PagingResponse<T> Success(T data, ResponseStatus statusCode, int PageNumber, int PageSize)
        {
            return new PagingResponse<T> { Data = data, StatusCode = statusCode, IsSuccess = true, PageNumber = PageNumber, PageSize = PageSize };
        }

    }
}

