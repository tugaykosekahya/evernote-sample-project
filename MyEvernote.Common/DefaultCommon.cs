using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Common
{
    public class DefaultCommon : ICommon
    {
        public string GetCurrentUserName()
        {
            throw new NotImplementedException();
        }

        public string GetUserName()
        {
            return "System";
        }

    }

}
