

namespace Honeywell.Gateway.Incident.Api.WorkflowDesign.DownloadTemplate
{
    public class WorkflowTemplateGto
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public WorkflowTemplateGto()
        {
        }
        public WorkflowTemplateGto(string filename, byte[] bytes)
        {
            FileName = filename;
            FileBytes = bytes;
        }

        public WorkflowTemplateGto(byte[] bytes)
        {
            FileBytes = bytes;
        }
    }
}
