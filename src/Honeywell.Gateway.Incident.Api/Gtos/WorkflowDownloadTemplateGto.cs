using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowDownloadTemplateGto: ExecuteResult
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public WorkflowDownloadTemplateGto()
        {
        }
        public WorkflowDownloadTemplateGto(ExecuteStatus result, string filename, byte[] bytes)
        {
            Status = result;
            FileName = filename;
            FileBytes = bytes;
        }
    }
}
