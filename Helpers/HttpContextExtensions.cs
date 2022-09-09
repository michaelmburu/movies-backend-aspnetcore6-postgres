using System;
using Microsoft.EntityFrameworkCore;

namespace Movies_API.Helpers
{
    public static class HttpContextExtensions
    {
       public async static Task InsertParametersPaginationFromHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if(httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }

            double count = await queryable.CountAsync();

            //Pass to client via header in response the total amount of records from our db
            httpContext.Response.Headers.Add("totalamountofrecords", count.ToString());


        }
    }
}

