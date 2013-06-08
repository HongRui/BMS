using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Configuration;
using System.Data.Entity.Validation;
namespace OUDAL
{
    public class OUContext : DbContext
    {
        public OUContext()
            : base(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString)
        { }

        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentUser> DepartmentUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleUser> RoleUsers { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<RoleFunction> RoleFunctions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<DictionaryItem> DictionaryItems { get; set; }
        public DbSet<AccessLog> AccessLogs { get; set; }
        public DbSet<SysCode> SysCode { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<AssetCatalog> AssetCatalogs { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialIn> MaterialIns { get; set; }
        public DbSet<MaterialInItem> MaterialInItems { get; set; }
        public DbSet<MaterialOut> MaterialOuts { get; set; }
        public DbSet<MaterialOutItem> MaterialOutItems { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetUser> AssetUsers { get; set; }
        public DbSet<AssetChangeLog> AssetChangeLogs { get; set; }
        //public DbSet<AssetExtInfo> AssetExtInfo { get; set; }
        public DbSet<AssetMaintain> AssetMaintains { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Building>Buildings{ get; set; }
        public DbSet<Room>Rooms{ get; set; }
        public DbSet<Client>Clients{ get; set; }
        public DbSet<RentContract>RentContracts{ get; set; }
        public DbSet<RentContractPrice>RentContractPrices{ get; set; }
        public DbSet<RentContractRoom> RentContractRooms { get; set; }
        public DbSet<RentBill>RentBills{ get; set; }
        public DbSet<RentIncome>RentIncomes{ get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DepartmentUser>().HasKey(p => new { p.UserId, p.DepartmentId });
            modelBuilder.Entity<RoleUser>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<RoleFunction>().HasKey(p => new { p.RoleId, p.FunctionId });
            base.OnModelCreating(modelBuilder);
        }
    }
    public class ContextInitializer : DropCreateDatabaseIfModelChanges<OUContext>//DropCreateDatabaseAlways<OUContext>// 
    {
        protected override void Seed(OUContext context)
        {
            base.Seed(context);
            Seeding.Seed(context);
        }
    }
    public class ContextDropDBInitializer : DropCreateDatabaseAlways<OUContext>// 
    {
        protected override void Seed(OUContext context)
        {
            base.Seed(context);
            Seeding.Seed(context);
        }
    }
    internal class Seeding
    {
        public static void Seed(OUContext context)
        {
            try
            {
                List<Function> funlist = new List<Function>
                                             {
                                                 new Function {Name = "系统管理", ParentName = "-", Sort = 100},
                                                 new Function {Name = "部门管理", ParentName = "系统管理", Sort = 0},
                                                 new Function {Name = "用户管理", ParentName = "系统管理", Sort = 0},
                                                 new Function {Name = "角色管理", ParentName = "系统管理", Sort = 0},

                                             };
                funlist.ForEach(o => context.Functions.Add(o));
                context.SaveChanges();
                List<Role> roles = new List<Role>
                                       {
                                           new Role {Name = "系统管理员"}
                                       };
                roles.ForEach(o =>
                                  {
                                      context.Roles.Add(o);
                                      context.SaveChanges();
                                      funlist.ForEach(
                                          p =>
                                          context.RoleFunctions.Add(new RoleFunction { RoleId = o.Id, FunctionId = p.Id }));
                                      context.SaveChanges();
                                  });




                List<Department> departments = new List<Department>
                                                   {
                                                       new Department {Name = "集团", PId = 0, DepartmentType = ""},
                                                       //1
                                                       new Department {Name = "上海绿地商业(集团)有限公司", PId = 1, DepartmentType = "集团部门"},
                                                       //2  
                                                       new Department {Name = "上海卢湾绿地商业管理有限公司", PId = 1, DepartmentType = "商管公司"},
                                                       //3  
                                                       new Department {Name = "北京绿地京城商业管理有限公司", PId = 1, DepartmentType = "商管公司"}
                                                       //4
                                                   };
                departments.ForEach(o => context.Departments.Add(o));
                context.SaveChanges();
                List<SystemUser> users = new List<SystemUser>
                                             {
                                                 new SystemUser
                                                     {Name = "Admin", Password = "11", State = 0, WorkNO = ""}
                                             };
                users.ForEach(o =>
                                  {
                                      o.Save(context);
                                      context.RoleUsers.Add(new RoleUser { UserId = o.Id, RoleId = roles[0].Id });
                                      context.DepartmentUsers.Add(new DepartmentUser
                                                                      {
                                                                          UserId = o.Id,
                                                                          DepartmentId = departments[0].Id
                                                                      });

                                  }
                    );

                context.SaveChanges();
                List<AssetCatalog> catalogs = new List<AssetCatalog>
                                                  {
                                                      new AssetCatalog {Name = "物资设备种类"},
                                                      new AssetCatalog {Name = "低值易耗品", CatalogType = "低值易耗品", PId = 1},
                                                      new AssetCatalog {Name = "固定资产", CatalogType = "固定资产", PId = 1}
                                                  };
                catalogs.ForEach(o =>
                                     {
                                         context.AssetCatalogs.Add(o);
                                     });
                //List<Company> companies = new List<Company>
                //                              {
                //                                  new Company {Name = "卢湾商管"},
                //                                  new Company {Name = "北京商管"}
                //                              };
                //companies.ForEach(o =>
                //                      {
                //                          context.Companies.Add(o);
                //                      }
                //    );
                new List<Material>
                    {
                        new Material{ CatalogId = 1,MallId = 3,Name = "A",Remark = "",Spec = "1mm",DepartmentId = 3},
                        new Material{CatalogId = 2,MallId = 3,Name = "B",Remark = "",Spec = "1cm*2cm*3cm",DepartmentId = 3},
                         new Material{CatalogId = 2,MallId = 3,Name = "C",Remark = "",Spec = "",DepartmentId = 3}
                    }.ForEach(o => context.Materials.Add(o));
                new List<MaterialIn>
                    {
                        new MaterialIn{InDate = new DateTime(2012,1,1),MallId = 3,PersonId = 1,Code="1"},
                        new MaterialIn{InDate = new DateTime(2012,2,1),MallId = 3,PersonId = 1,Code="2"},
                        new MaterialIn{InDate = new DateTime(2012,3,1),MallId = 3,PersonId = 1,Code="3"},
                    }.ForEach(o => context.MaterialIns.Add(o));
                new List<MaterialInItem>
                    {
                        new MaterialInItem{InId = 1,MaterialId = 1,Num = 1.0m,UnitPrice = 1.1m},
                        new MaterialInItem{InId = 1,MaterialId = 2,Num = 2.0m,UnitPrice = 2m},
                        new MaterialInItem{InId = 1,MaterialId = 3,Num = 3.0m,UnitPrice = 3.1m},
                        new MaterialInItem{InId = 2,MaterialId = 1,Num = 1.0m,UnitPrice = 1.1m},
                        new MaterialInItem{InId = 2,MaterialId = 2,Num = 2.0m,UnitPrice = 2m},
                        new MaterialInItem{InId = 2,MaterialId = 3,Num = 3.0m,UnitPrice = 3.1m},
                        new MaterialInItem{InId = 3,MaterialId = 1,Num = 1.0m,UnitPrice = 1.1m},
                        new MaterialInItem{InId = 3,MaterialId = 2,Num = 2.0m,UnitPrice = 2m},
                        new MaterialInItem{InId = 3,MaterialId = 3,Num = 3.0m,UnitPrice = 3.1m},
                    }.ForEach(o => context.MaterialInItems.Add(o));
                context.SaveChanges();
                context.Companies.Add(new Company {Name = "众裕"});
                context.Companies.Add(new Company { Name = "商业集团" });
                context.Buildings.Add(new Building{CompanyId=1,Address="address some where",Name="1贝多芬"});
                context.Buildings.Add(new Building{CompanyId=1,Address="",Name="2宁夏路"});
                context.Rooms.Add(new Room {BuildingId = 1, Dim = 100, RentDim = 90, Name = "301"});
                context.Clients.Add(new Client {Name = "ABC", FullName = "Abb company"});
                context.Clients.Add(new Client {Name = "ABD", FullName = "abd company"});
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {

            }

        }
    }
}