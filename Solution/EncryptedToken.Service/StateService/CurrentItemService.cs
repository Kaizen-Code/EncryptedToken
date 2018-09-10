using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EncryptedToken.Service.StateService
{
    class CurrentItemService : IStateService
    {
        
        public T Get<T>(string key) where T : class
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Items[key] as T;
            return null;
        }

        public void Save<T>(string key, T model) where T : class
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items.Add(key, model);
        }
    }
}
