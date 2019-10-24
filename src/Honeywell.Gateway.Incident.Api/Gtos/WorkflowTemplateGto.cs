
namespace Honeywell.Gateway.Incident.Api.Gtos
{
    public class WorkflowTemplateGto: ExecuteResult
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public WorkflowTemplateGto()
        {
            Status = ExecuteStatus.Error;
        }
        public WorkflowTemplateGto(ExecuteStatus result, string filename, byte[] bytes)
        {
            Status = result;
            FileName = filename;
            FileBytes = bytes;
        }

        public WorkflowTemplateGto(ExecuteStatus result, byte[] bytes)
        {
            Status = result;
            FileBytes = bytes;
        }
    }
}
