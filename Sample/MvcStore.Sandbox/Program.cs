using System;
using System.Configuration;
using MvcStoreModels;
using NCommon.Extensions;

namespace MvcStore.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var context =
                new MvcStoreDataContext("name=MvcStoreEntities");

            context.Categories.ForEach(category => Console.WriteLine(category.Name));
        }
    }
}
