using EncryptedToken.Service.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedToken.Service.Repositories
{
    public class ConfigRepository
    {
        public void AddIfNotAdded(Config config)
        {
            Config dbModel;
            using (var context = new SecureContext())
            {
                dbModel = context.Configs.FirstOrDefault(c => c.ItemKey == config.ItemKey);
                if (dbModel == null)
                {
                    config.Id = context.Configs.Select(c => c.Id).DefaultIfEmpty(0).Max() + 1;
                    dbModel = context.Configs.Add(config);
                    context.SaveChanges();
                }
            }
        }
        public void AddIfNotAdded(params Config[] configs)
        {
            using (var context = new SecureContext())
            {
                var dbList = context.Configs.ToList();
                var max = dbList.Select(c => c.Id).DefaultIfEmpty(0).Max();
                var IsAdded = false;
                if (configs != null)
                    foreach (var item in configs)
                    {
                        if (dbList.FirstOrDefault(c => c.ItemKey == item.ItemKey) == null)
                        {
                            item.Id = ++max;
                            IsAdded = true;
                            context.Configs.Add(item);
                        }
                    }
                if (IsAdded)
                    context.SaveChanges();
            }
        }
        public Config FirstOrDefault(Expression<Func<Config, bool>> predicate)
        {
            using (var context = new SecureContext())
            {
                return context.Configs.FirstOrDefault(predicate);
            }
        }
    }
}
