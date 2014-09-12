using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ImageService
{
    public partial class LoadImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.AllKeys.Contains("id"))
            {
                int imageId = int.Parse(Request.QueryString["id"]);
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ImageDB"].ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("sp_ReadImage", connection) { CommandType = CommandType.StoredProcedure };
                    command.Parameters.AddWithValue("@ImageId", imageId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        byte[] data = (byte[])reader["ImageContent"];
                        Response.Clear();
                        Response.ContentType = "image/png";
                        Response.BinaryWrite(data);
                    }

                    connection.Close();
                }
            }
        }
    }
}