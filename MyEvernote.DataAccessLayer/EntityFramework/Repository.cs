using MyEvernote.Common;
using MyEvernote.Core.DataAccess;

using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace MyEvernote.DataAccessLayer.EntityFramework
{
    // repository bir generic classdır, yani , T ye kategoriler,yorumlar vs vs gelebilir ona göre aksiyon alır, where ile gelen değerin class olacagını kısıtladım
    public class Repository<T> : RepositoryBase, IDataAccess<T> where T : class
    {  
        private DbSet<T> _objectSet;

        //Kurucu Metot constractur, yeni bir nesne new ledigimiz zaman burası calısıp ilgili dbseti bulucak..
        public Repository()
        {
            _objectSet = context.Set<T>();
        }

        //Entity ile hangi class gelirse gelsin ona göre calıs ve listele..
        public List<T> List()
        {
            return _objectSet.ToList();
        }

        public IQueryable<T> ListQueryable()
        {
            return _objectSet.AsQueryable<T>();
        }

        //Entity ile istediğim kritere göre select atan LinQ...
        public List<T> List(Expression <Func<T,bool>> where)
        {
            return _objectSet.Where(where).ToList();
        }

        //Entity ile tek bir tip döndüren liste..
        public T Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.FirstOrDefault(where);
        }

        //Entity ile hangi class gelirse gelsin ona göre calıs ve ekleme yap..
        public int Insert(T obj)
        {
            _objectSet.Add(obj);

            if(obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime now = DateTime.Now;

                o.CreatedOn = now;
                o.ModifiedOn = now;
                o.ModifiedUsername = App.Common.GetUserName(); // TODO: ileride burayı ayarlayacagız , işlem yapan kişiyi yazacagız.          
            }

            return Save();
        }

        //Entity ile hangi class gelirse gelsin ona göre calıs ve güncelle , entity de aslında update yok data üzerinde degisiklik mantıgı vardır...
        public int Update(T obj)
        {
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                o.ModifiedOn = DateTime.Now; ;
                o.ModifiedUsername = App.Common.GetUserName(); // TODO: ileride burayı ayarlayacagız , işlem yapan kişiyi yazacagız.          
            }

            return Save();
        }

        //Entity ile hangi class gelirse gelsin ona göre calıs ve sil..
        public int Delete(T obj)
        {
            _objectSet.Remove(obj);

            return Save();
        }

        //Entity ile degisiklikleri kaydet metodu..
        public int Save()
        {
            return context.SaveChanges();
        }

    }
}
