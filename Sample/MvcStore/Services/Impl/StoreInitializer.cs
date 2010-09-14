using System;
using System.Collections.Generic;
using System.Linq;
using MvcStore.Models;
using NCommon.Extensions;
using NHibernate;

namespace MvcStore.Services.Impl
{
    public class StoreInitializer : IStoreInitializer
    {
        IEnumerable<Category> _categories;

        public void Initialize(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                InitializeCategories(session);
                InitializeProducts(session);
                session.Flush();
            }
        }

        void InitializeCategories(ISession session)
        {
            var categories = new HashSet<Category>
            {
                new Category {Name = "Backpacks", Description = "Backpacks"},
                new Category {Name = "Bikes", Description = "Bikes"},
                new Category {Name = "Boots", Description = "Boots"},
                new Category {Name = "Hats & Helmets", Description = "Hats & Helmets"},
                new Category {Name = "Hiking Gear", Description = "Hiking Gear"},
                new Category {Name = "Sunglasses", Description = "Sunglasses"}
            };

            categories.ForEach(category => session.Save(category));
            _categories = categories;
        }

        void InitializeProducts(ISession session)
        {
            var products = new HashSet<Product>
            {
                new Product{Name = "Hiking Backpack", ProductCode = "Backpack1_1", Category = _categories.First(x => x.Name == "Backpacks"), Price = 50},
                new Product{Name = "Wide-base Backpack", ProductCode = "Backpack2_1", Category = _categories.First(x => x.Name == "Backpacks"), Price = 50},
                new Product{Name = "Short Backpack", ProductCode = "Backpack3_1", Category = _categories.First(x => x.Name == "Backpacks"), Price = 40},
                new Product{Name = "Mountaineering Backpack", ProductCode = "Backpack4_1", Category = _categories.First(x => x.Name == "Backpacks"), Price = 130},
                new Product{Name = "Sprint 500 Bike", ProductCode = "Bike1_1", Category = _categories.First(x => x.Name == "Bikes"), Price = 460},
                new Product{Name = "Escape 3.0 Bike", ProductCode = "Bike2_1", Category = _categories.First(x => x.Name == "Bikes"), Price = 680},
                new Product{Name = "Scoop Cruiser", ProductCode = "Bike3_1", Category = _categories.First(x => x.Name == "Bikes"), Price = 380},
                new Product{Name = "Sierra Leather Hiking Boots", ProductCode = "Boots1_1", Category = _categories.First(x => x.Name == "Boots"), Price = 90},
                new Product{Name = "Rainier Leather Hiking Boots", ProductCode = "Boots2_1", Category = _categories.First(x => x.Name == "Boots"), Price = 110},
                new Product{Name = "Cascade Fur-Lined Hiking Boots", ProductCode = "Boots3_1", Category = _categories.First(x => x.Name == "Boots"), Price = 130},
                new Product{Name = "Adirondak Fur-Lined Hiking Boots", ProductCode = "Boots4_1", Category = _categories.First(x => x.Name == "Boots"), Price = 60},
                new Product{Name = "Olympic Hiking Boots", ProductCode = "Boots5_1", Category = _categories.First(x => x.Name == "Boots"), Price = 90},
                new Product{Name = "Weathered Lether Baseball Cap", ProductCode = "Hat1_1", Category = _categories.First(x => x.Name == "Hats & Helmets"), Price = 13},
                new Product{Name = "Colorful Straw hat", ProductCode = "Hat2_1", Category = _categories.First(x => x.Name == "Hats & Helmets"), Price = 10},
                new Product{Name = "Summertime Straw Hat", ProductCode = "Hat3_1", Category = _categories.First(x => x.Name == "Hats & Helmets"), Price = 23},
                new Product{Name = "Bicycle Safety Helmet", ProductCode = "Helmet1_1", Category = _categories.First(x => x.Name == "Hats & Helmets"), Price = 80},
                new Product{Name = "Fusion Helmet", ProductCode = "Helmet2_1", Category = _categories.First(x => x.Name == "Hats & Helmets"), Price = 150},
                new Product{Name = "Fire Helmet", ProductCode = "Helmet3_1", Category = _categories.First(x => x.Name == "Hats & Helmets"), Price = 125},
                new Product{Name = "Bicycle Safety Helmet", ProductCode = "Helmet1_1", Category = _categories.First(x => x.Name == "Hats & Helmets"), Price = 80},
                new Product{Name = "Sentinel Locking Carbiner", ProductCode = "Carbiner1_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 16},
                new Product{Name = "Guardian Locking Carbiner", ProductCode = "Carbiner2_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 6},
                new Product{Name = "Trailhead Locking Carbiner", ProductCode = "Carbiner3_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 80},
                new Product{Name = "Traiguide Compass", ProductCode = "Compass1_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 30},
                new Product{Name = "Northstar Compass", ProductCode = "Compass2_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 18},
                new Product{Name = "Sundial Compass", ProductCode = "Compass3_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 12},
                new Product{Name = "Polar Start Compass", ProductCode = "Compass4_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 15},
                new Product{Name = "Compass Necklace", ProductCode = "Compass5_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 16},
                new Product{Name = "Battery Operated Flashlight", ProductCode = "Flashlight1_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 8},
                new Product{Name = "Heavy-Duty Flashlight", ProductCode = "Flashlight2_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 13},
                new Product{Name = "Retro Flashlight", ProductCode = "Flashlight3_1", Category = _categories.First(x => x.Name == "Hiking Gear"), Price = 24},
                new Product{Name = "Northwind Traders Arizona Sunglasses", ProductCode = "Sunglasses1_1", Category = _categories.First(x => x.Name == "Sunglasses"), Price = 35},
                new Product{Name = "Northwind Traders Eclipse Sunglasses", ProductCode = "Sunglasses2_1", Category = _categories.First(x => x.Name == "Sunglasses"), Price = 55},
            };
            products.ForEach(product => session.Save(product));
        }
    }
}