using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedToken.Service.StateService
{
    public interface IStateService
    {
        void Save<T>(string key,T model) where T : class;
        T Get<T>(string key) where T : class;
    }
}
