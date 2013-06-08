using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using OUDAL;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to drop Database");
            Console.Read();
            System.Data.Entity.Database.SetInitializer<OUContext>(new ContextDropDBInitializer());
            //System.Data.Entity.Database.SetInitializer<OUContext>(new ContextInitializer());
            OUContext db = new OUContext();
            var query = (from o in db.Materials select o).ToList();
           
            Console.WriteLine("Press any key to Exit");
            Console.Read();
        }
    }
}
