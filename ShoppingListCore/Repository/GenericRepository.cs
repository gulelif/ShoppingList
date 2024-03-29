﻿
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using ShoppingListCore.Abstract;
using ShoppingListCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ShoppingListCore.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        public void Delete(T t)
        {
            using var c = new ShoppingDbContext();
            c.Remove(t);
            c.SaveChanges();
        }

        public T GetByID(int id)
        {
            using var c = new ShoppingDbContext();
            return c.Set<T>()
                    .Find(id);
        }


        public List<T> GetList()
        {//tüm listeyi getir
            using var c = new ShoppingDbContext();
            return c.Set<T>().ToList();
        }
        public  List<T> GetAll(Expression<Func<T, bool>>? filter =null, params Expression<Func<T, object>>[] expressionList) 
        {
            //Tüm listeyi getirirken şarta bak ve diğer tablolarla birleştir
            using var c = new ShoppingDbContext();
            var query = c.Set<T>().AsQueryable();
            foreach (var expression in expressionList)
            {
                query = query.Include(expression);
            }
            if (filter != null)
                query = query.Where(filter);
            var list=  query.ToList();
            return list;
        }
        public List<T> GetListByFilter(Expression<Func<T, bool>> filter)
        {
            //tüm listeyi şarta göre getir
            using var c = new ShoppingDbContext();
            return c.Set<T>().Where(filter).ToList();
        }
        public void Insert(T t)
        {
            using var c = new ShoppingDbContext();
            c.Add(t);
            c.SaveChanges();
        }
        public void Update(T t)
        {
            using var c = new ShoppingDbContext();
            c.Update(t);
            c.SaveChanges();
        }

        public T GetByFilter(Expression<Func<T, bool>> filter)
        {
            //şarta göre tek satırı getir
            using var c = new ShoppingDbContext();
            return c.Set<T>()
                    .Where(filter).FirstOrDefault();
        }
    }
}
