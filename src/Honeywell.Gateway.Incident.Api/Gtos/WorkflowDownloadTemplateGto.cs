using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowDownloadTemplateGto
    {
        public bool Result { get; set; }
        public string FileName { get; set; }
        //public Stream FileStream { get; set; }
        public byte[] FileBytes { get; set; }
        public WorkflowDownloadTemplateGto()
        {
        }
        public WorkflowDownloadTemplateGto(bool result, string filename, byte[] bytes)
        {
            Result = result;
            FileName = filename;
            FileBytes = bytes;
        }
    }
}
