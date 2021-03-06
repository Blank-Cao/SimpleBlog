﻿using System;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scottxu.Blog.Models;
using Scottxu.Blog.Models.Units;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Scottxu.Blog.Controllers
{
    public class FileController : BaseController
    {
        IHostingEnvironment HostingEnvironment { get; }

        public FileController(BlogSystemContext context, IOptions<SiteOptions> options,
            IHostingEnvironment hostingEnvironment)
            : base(context, options) => HostingEnvironment = hostingEnvironment;

        // GET: /File/{articleGuid}/{uploadedFileGuid}
        public IActionResult Get(Guid articleGuid, Guid uploadedFileGuid)
        {
            var diskFile = DataBaseContext.DiskFiles
                .Include(o => o.UploadedFile)
                .FirstOrDefault(q => q.ArticleGuid == articleGuid &&
                                     q.UploadedFileGuid == uploadedFileGuid);
            if (diskFile == null) return new NotFoundResult();
            var uploadUnit = new UploadUnit(DataBaseContext, HostingEnvironment);
            var fileInfo = uploadUnit.GetFileInfo(diskFile.UploadedFile, false);
            if (!fileInfo.Exists) return new NotFoundResult();
            Response.Headers["Content-Disposition"] =
                $"{(Request.Query["download"].Any() ? "attachment;" : string.Empty)}filename*={Encoding.Default.BodyName}''{HttpUtility.UrlEncode(diskFile.Name)}";
            return File(fileInfo.OpenRead(), diskFile.MIME);
        }
    }
}