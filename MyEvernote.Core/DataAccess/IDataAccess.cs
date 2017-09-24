using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Core.DataAccess
{
    public interface IDataAccess<T>
    {
        List<T> List();

        //Entity ile istediğim kritere göre select atan LinQ...
        List<T> List(Expression<Func<T, bool>> where);

        //Entity ile tek bir tip döndüren liste..
        T Find(Expression<Func<T, bool>> where);

        //Entity ile hangi class gelirse gelsin ona göre calıs ve ekleme yap..
        int Insert(T obj);

        //Entity ile hangi class gelirse gelsin ona göre calıs ve güncelle , entity de aslında update yok data üzerinde degisiklik mantıgı vardır...
        int Update(T obj);

        //Entity ile hangi class gelirse gelsin ona göre calıs ve sil..
        int Delete(T obj);

        IQueryable<T> ListQueryable();

        //Entity ile degisiklikleri kaydet metodu..
        int Save();

    }
}
