using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBHelper.Connection;

namespace Topluluk.Services.ChatAPI.Data.Settings
{
    public class ChatAPIDbSettings : IDbConfiguration
    {
        public string ConnectionString { get { return "mongodb+srv://ismail:ismail@cluster0.psznbcu.mongodb.net/?retryWrites=true&w=majority"; } }
        public string DatabaseName { get { return "Topluluk"; } }
    }
}
