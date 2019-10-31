using System;

namespace CustomApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var httpHelper = new HttpHelper();
                var incidentAdapter = new IncidentAdapter(httpHelper);
                incidentAdapter.GetWorkflowDesignIds();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.Read();
            }
        }
    }
}
