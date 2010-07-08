using System;
using System.Linq;
using MvcStoreModels;
using NUnit.Framework;

namespace MvcStore.Tests
{
    [TestFixture]
    public class ShoppingCartTests
    {
        [Test]
        public void Adding_a_null_product_throws_an_argument_null_exception()
        {
            var cart = ShoppingCart.Create(Guid.NewGuid());
            Assert.Throws<ArgumentNullException>(() => cart.AddToCart((Product)null));
        }

        [Test]
        public void Adding_a_new_product_adds_a_shopping_cart_item_to_the_cart()
        {
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var product = new Product
            {
                Code = "prodA",
                Name = "Product A"
            };
            cart.AddToCart(product);
            Assert.That(cart.Items.Any(x => x.ProductId == product.Id));
        }

        [Test]
        public void Adding_an_existing_product_increments_the_quantity_of_the_shopping_cart_item()
        {
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var product = new Product
            {
                Code = "prodA",
                Name = "Product A"
            };
            cart.AddToCart(product);
            cart.AddToCart(product);
            Assert.That(cart.Items.Count(x => x.ProductId == product.Id), Is.EqualTo(1));
            Assert.That(cart.Items.Where(x => x.ProductId == product.Id).Sum(x => x.Quantity), Is.EqualTo(2));
        }

        [Test]
        public void Removing_an_item_that_doesnt_exist_throws_no_error()
        {
            var cart = ShoppingCart.Create(Guid.NewGuid());
            Assert.DoesNotThrow(() => cart.RemoveFromCart(Guid.NewGuid()));
        }

        [Test]
        public void Can_remove_an_item_from_the_cart()
        {
            var cart = ShoppingCart.Create(Guid.NewGuid());
            cart.AddToCart(new Product
            {
                Code = "ABC",
                Name = "ProductA",
                Price = 10
            });

            cart.RemoveFromCart(cart.Items.First().Id);
            Assert.That(cart.Items.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Can_set_the_quantity_of_an_item()
        {
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var product = new Product
            {
                Code = "ABC",
                Name = "ProductA",
                Price = 10
            };
            cart.AddToCart(product, product);
            cart.SetQuantity(cart.Items.First().Id, 1);
            Assert.That(cart.Items.First().Quantity, Is.EqualTo(1));
        }

        [Test]
        public void Setting_the_quantity_of_an_item_to_zero_removes_the_item_from_the_cart()
        {
            var cart = ShoppingCart.Create(Guid.NewGuid());
            var product = new Product
            {
                Code = "ABC",
                Name = "ProductA",
                Price = 10
            };
            cart.AddToCart(product, product);
            cart.SetQuantity(cart.Items.First().Id, 0);
            Assert.That(cart.Items.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Total_returns_sum_total_of_all_items_in_cart()
        {
            var productA = new Product {Code = "A", Name = "ProductA", Price = 10};
            var productB = new Product {Code = "B", Name = "ProductB", Price = 5};
            var productC = new Product {Code = "C", Name = "ProductC", Price = 1};
            var cart = ShoppingCart.Create(Guid.NewGuid());
            cart.AddToCart(
                productA, 
                productA, 
                productB, 
                productC);

            Assert.That(cart.Total, Is.EqualTo(26));
        }

        [Test]
        public void Clearning_cart_removes_all_items_from_the_cart()
        {
            var productA = new Product { Code = "A", Name = "ProductA", Price = 10 };
            var productB = new Product { Code = "B", Name = "ProductB", Price = 5 };
            var productC = new Product { Code = "C", Name = "ProductC", Price = 1 };
            var cart = ShoppingCart.Create(Guid.NewGuid());
            cart.AddToCart(
                productA,
                productA,
                productB,
                productC);
            cart.ClearCart();
            Assert.False(cart.Items.Any());
        }
    }
}