using InvictaInternalAPI.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace InvictaInternalAPI.Services
{
    public class eCommerceActionSupport
    {
        public static void eCommerceAction(long fulfillmentId, int action, string prefix, IConfiguration _configuration)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:DefaultConnectionInvicta"];
                var procName = "InvictaAUX.dbo.eCommerceActionCancel";
                if (prefix.Equals("TCO"))
                {
                    procName = "Merlin.dbo.eCommerceActionCancel";
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(procName, connection);
                    //SqlCommand cmd = new SqlCommand("Merlin.dbo.PortalProcTest", connection);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@id", fulfillmentId));
                    cmd.Parameters.Add(new SqlParameter("@action", action));
                    cmd.Parameters.Add(new SqlParameter("@operator", "SYSTEM"));
                    cmd.Parameters.Add(new SqlParameter("@reportedReason", 1));
                    Console.WriteLine("Action:" + action);
                    Console.WriteLine("fulfillmentId:" + fulfillmentId);
                    if (action == 11)
                    {
                        cmd.Parameters.Add(new SqlParameter("@optValue", 2734));
                        Console.WriteLine("@optValue " + 2734);
                    }
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        // iterate through results, printing each to console
                        while (rdr.Read())
                        {
                            Console.WriteLine(rdr.ToString());
                        }
                    }
                }

            }
            catch (Exception e)
            {

                throw new BusinessException("Data inconsistent" + e);
            }
            

        }
    }
}
