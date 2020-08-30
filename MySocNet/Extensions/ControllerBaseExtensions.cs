using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MySocNet.Input;
using MySocNet.InputData;
using MySocNet.Models;
using MySocNet.Response;
using MySocNet.Services;
using MySocNet.Services.Interfaces;
using Newtonsoft.Json;
using NLog.Fluent;
using DapperSqlite.Models;
using DapperSqlite.Services;
using DeviceDetectorNET;
using MySocNet.Exceptions;

namespace MySocNet.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static string GetConfirmLink(this ControllerBase controller, User user)
        {
            return controller.Url.ActionLink
                (action: "ConfirmEmail",
                 controller: "Login",
                new { user.ActiveKey.Key },
                controller.Request.Scheme);
        }
    }
}
