using EncryptedToken.Service.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EncryptedToken.Service.Repositories
{
    public class PermissionRepository
    {
        internal PermissionRepository()
        {

        }
        
        public IList<Permission> GetAll()
        {
            using (var data = new SecureContext())
            {
                return data.Permissions.ToList();
            }
        }
        public Permission FirstOrDefault(int id)
        {
            using (var data = new SecureContext())
            {
                return data.Permissions.FirstOrDefault(c=> c.Id ==  id);
            }
        }

        public List<Permission> GetAllowedPermissions(int userRoleId)
        {
            using (var data = new SecureContext())
            {
                var maps = data.RolePermissionMaps.Where<RolePermissionMap>(c => c.RoleId == userRoleId).Select(c => c.PermissionId);
                var list = data.Permissions.Where<Permission>(c => maps.Contains(c.Id)).ToList();
                return list;
            }

        }
        public List<Permission> GetDeniedPermissions(int userRoleId)
        {
            using (var data = new SecureContext())
            {
                var maps = data.RolePermissionMaps.Where<RolePermissionMap>(c => c.RoleId == userRoleId).Select(c => c.PermissionId);
                var list = data.Permissions.Where<Permission>(c => !maps.Contains(c.Id)).ToList();
                return list;
            }
        }
        public Permission Update(Permission model)
        {
            Permission obj;
            using (var data = new SecureContext())
            {
                obj = data.Permissions.FirstOrDefault(c => c.Id == model.Id);
                if (obj != null)
                {
                    obj.Caption = model.Caption;
                    obj.Description = model.Description;
                    data.SaveChanges();
                }
            }
            return obj;
        }

        public void RegisterPermissioins<T>() where T : struct
        {
            var type = typeof(T);
            if(!typeof(T).IsEnum)
            {
                throw new ArgumentException();
            }
            List<Permission> list;
            using (var data = new SecureContext())
            {
                list = data.Permissions.ToList();
                foreach (var item in Enum.GetValues(type))
                {
                    var id = (int)item;
                    var model = list.FirstOrDefault(c => c.Id ==  id);
                    if(model == null)
                    {
                        model = new Permission()
                        {
                            Id = id
                        };
                        model.Name = model.Caption = model.Description = item.ToString();
                        data.Permissions.Add(model);
                    }
                    else if(model.Name != item.ToString())
                    {
                        model.Name = item.ToString();
                        data.Permissions.Attach(model);
                        data.Entry<Permission>(model).State = System.Data.Entity.EntityState.Modified;
                    }
                }// close foreach loop
                data.SaveChanges();
            }
           

        }

        
        public bool AssignUserRoles(int permissionId, int[] roleIds)
        {
            var list = new List<RolePermissionMap>();
            foreach (var item in roleIds)
            {
                list.Add(new RolePermissionMap() { PermissionId = permissionId, RoleId = item });
            }
            using (var data = new SecureContext())
            {
                data.RolePermissionMaps.AddRange(list);
                data.SaveChanges();
            }
            return true;
        }
        public bool DismissUserRoles(int permissionId, int[] roleIds)
        {
            using (var data = new SecureContext())
            {
                var range = data.RolePermissionMaps.Where(c => c.PermissionId == permissionId && roleIds.Contains(c.RoleId));
                data.RolePermissionMaps.RemoveRange(range);
                data.SaveChanges();
            }
            return true;
        }



        internal bool Delete(int permissionId)
        {
            using (var data = new SecureContext())
            {
                if (data.RolePermissionMaps.FirstOrDefault(c => c.PermissionId == permissionId) == null)
                {
                    var model = data.Permissions.FirstOrDefault(c => c.Id == permissionId);
                    data.Permissions.Remove(model);
                    data.SaveChanges();
                    return true;
                }
                return false;
            }
        }


    }


}

   
