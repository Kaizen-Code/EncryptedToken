using EncryptedToken.Service.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedToken.Service.Repositories
{
    public class UserRoleRepository
    {
        internal UserRoleRepository()
        {

        }
        public UserRole FirstOrDefault(int id)
        {
            using (var data = new SecureContext())
            {
                return data.UserRoles.FirstOrDefault(c => c.Id == id);
            }
        }
        public List<UserRole> GetAll()
        {
            using (var data = new SecureContext())
            {
                var list = data.UserRoles.ToList();
                return list;
            }
        }
        public List<UserRole> GetAllowedUserRoles(int permissionId)
        {
            using (var data = new SecureContext())
            {
                var queryRoleIds = data.RolePermissionMaps.Where(c => c.PermissionId == permissionId).Select(c => c.RoleId);
                var list = data.UserRoles.Where(c => queryRoleIds.Contains(c.Id)).ToList();
                return list;
            }
        }
        public List<UserRole> GetDeniedUserRoles(int permissionId)
        {
            using (var data = new SecureContext())
            {
                var queryRoleIds = data.RolePermissionMaps.Where(c => c.PermissionId == permissionId).Select(c => c.RoleId);
                var list = data.UserRoles.Where(c => !queryRoleIds.Contains(c.Id)).ToList();
                return list;
            }
        }

        public bool AssignPermissions(int roleId, int[] permissionList)
        {
            var list = new List<RolePermissionMap>();
            foreach (var item in permissionList)
            {
                list.Add(new RolePermissionMap() { PermissionId = item, RoleId = roleId });
            }
            using (var data = new SecureContext())
            {
                data.RolePermissionMaps.AddRange(list);
                data.SaveChanges();
            }
            return true;
        }
        public bool DeniedPermissions(int roleId, int[] permissionIds)
        {
            using (var data = new SecureContext())
            {
                var range = data.RolePermissionMaps.Where(c => c.RoleId == roleId && permissionIds.Contains(c.PermissionId));
                data.RolePermissionMaps.RemoveRange(range);
                data.SaveChanges();
            }
            return true;
        }

        #region
        public bool Delete(int roleId)
        {
            using (var data = new SecureContext())
            {
                if (data.RolePermissionMaps.FirstOrDefault(c => c.RoleId == roleId) == null)
                {
                    var model = data.UserRoles.FirstOrDefault(c => c.Id == roleId);
                    data.UserRoles.Remove(model);
                    data.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public UserRole AddIfNotAdded(UserRole model)
        {
            using (var context = new SecureContext())
            {
                var dataModel  = context.UserRoles.FirstOrDefault(c => c.RoleTitle == model.RoleTitle);
                if (dataModel == null)
                {
                    dataModel = context.UserRoles.Add(model);
                    context.SaveChanges();
                }
                return dataModel;
            }
        }
        public UserRole AddIfNotAdded(string roleName)
        {
            using (var context = new SecureContext())
            {
                var dataModel = context.UserRoles.FirstOrDefault(c => c.RoleTitle == roleName);
                if (dataModel == null)
                {
                    dataModel = context.UserRoles.Add(new UserRole() { RoleTitle = roleName });
                    context.SaveChanges();
                }
                return dataModel;
            }
        }
        public void AddIfNotAdded(IEnumerable<string> roleNames)
        {
            using (var context = new SecureContext())
            {
                roleNames = roleNames.Except(context.UserRoles.Select(c=> c.RoleTitle).AsEnumerable());
                IList<UserRole> list = new List<UserRole>();
                foreach (var item in roleNames)
                {
                    list.Add(new UserRole() { RoleTitle = item });
                }
                context.UserRoles.AddRange(list);
                context.SaveChanges();
            }
        }
        #endregion
    }
}
