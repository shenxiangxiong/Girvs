﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Girvs.WebFrameWork
{
    public class GirvsBusinessErrorResult : StatusCodeResult
    {
        private readonly string _message;
        private const int DefaultStatusCode = 568;
        public GirvsBusinessErrorResult(string message) : base(DefaultStatusCode)
        {
            this._message = message;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            await base.ExecuteResultAsync(context);
            await context.HttpContext.Response.WriteAsync(_message);
        }
    }
}