using MyEvernote.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{

    public class RepositoryBase
    {
        protected static DatabaseContext context;
        protected static object _lockSync = new object();

        protected RepositoryBase()
        {
            CreateContext();
        }

        //Aynı Database context üzerinden ilişkili tablolara veri ekleme için çözüm 
        //Singleton Pattern Design
        //Yalnız 1 kere DatabaseContext olusturulup diger bütün classlar generic classa gittiginde aynı context üzerinden işlemleri yapacak
        //Singleton Pattern Yaklasımı , asagıdaki metot static olmak zorunda yoksa singleton olmaz

        private static void CreateContext()
        {
            if (context == null)
            {
                lock (_lockSync)
                {
                    if (context == null)
                    {
                        context = new DatabaseContext();
                    }
                }

            }
        }
    }
}
